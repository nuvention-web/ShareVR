﻿//======= Copyright (c) ShareVR ============================================
//
// Purpose: Manage recording status and all capture related scripts
// Version: SDK v0.4b
// Chen Chen
// 4/30/2017
//==========================================================================

using System;
using UnityEngine;
using UnityEngine.VR;
using ShareVR.Capture;
using ShareVR.Utils;

namespace ShareVR.Core
{
    [RequireComponent(typeof(InputManager))]
    public class RecordManager : MonoBehaviour
    {
        [Tooltip ("Select the way you want the camera to move")]
        public CameraFollowMethod cameraFollowMethod = CameraFollowMethod.FixedSmooth;
        [Tooltip ("Camera orbit speed")]
        public float cameraOrbitSpeed = 1.0f;
        [Tooltip ("Camera height")]
        public float camHeight = 2.0f;
        [Tooltip ("Camera distance")]
        public float camDistance = 3.0f;
        [Tooltip ("Camera angle")]
        public float camAngle = 30.0f;
        [Tooltip ("Camera motion smooth factor")]
        public float camMotionDamp = 2.0f;
        [Tooltip ("Make the spectator camera visible")]
        public bool showCameraModel = true;
        [Tooltip ("Adjust camera model scale if it's too big or small")]
        public float cameraModelScale = 1.0f;
        [Tooltip ("Show a camera preview window?")]
        public bool showCameraPreview = true;

        [Tooltip ("Show a cute avatar for demo")]
        public bool showPlayerAvatar = false;
        [Tooltip ("Adjust avatar scale if it's too big or small")]
        public float avatarScale = 1.0f;
        [Tooltip ("Adjust avatar offset")]
        public Vector3 avatarOffset = new Vector3 (0, -1.5f, 0);

        [Tooltip ("Specify your preferred input method")]
        public ViveCtrlerMapping toggleRecordingInput = ViveCtrlerMapping.KeyboardOnly_Key_X;
        [Tooltip ("Do you want to use our voice command (beta) feature?")]
        public bool useVoiceCommand = true;

        [Tooltip ("Specify your main player gameobject")]
        public GameObject trackingTarget;
        [Tooltip ("Display debug message")]
        public bool showDebugMessage = true;

        [Tooltip ("Specify recording video resolution")]
        public VRCaptureVideo.FrameSizeType frameSize = VRCaptureVideo.FrameSizeType._1280x720;
        [Tooltip ("Specify recording video framerate")]
        public VRCaptureVideo.TargetFramerateType frameRate = VRCaptureVideo.TargetFramerateType._30;
        [Tooltip ("Specify video file save folder (leave it blank if want to use default: Documents/ShareVR/)")]
        public string saveFolder;
        [Tooltip ("Specify video file name (leave it blank if want to use default)")]
        public string saveFileName;

        [Tooltip ("Do you want to automatically upload video online?")]
        public bool uploadFileOnline = false;

        [HideInInspector]
        public bool isVRDevicePresent = false;
        [HideInInspector]
        public S3Uploader s3Uploader;

        // Internal Scripts Reference
        private GameObject cameraRigPrefab;
        private GameObject playerAvatarPrefab;
        private CameraController camCtrler;
        private AvatarController avatarCtrler;
        private LiveFeed liveFeed;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            InitializeReference();

            if (showDebugMessage)
                Debug.Log("Checking active SteamVR instance...");

            // Search for avtive VR Device every one second
            isVRDevicePresent = VRDevice.isPresent;
            if (showDebugMessage)
            {
                if (isVRDevicePresent)
                    Debug.Log("ShareVR: Found HMD - " + VRDevice.model);
                else
                    Debug.LogWarning("ShareVR: No HMD device found, switch to PC mode");
            }
        }

        void InitializeReference()
        {
            // Instantiate a new capture camera
            cameraRigPrefab = Resources.Load("Prefabs/ShareVRCameraRig") as GameObject;
            camCtrler = Instantiate(cameraRigPrefab).GetComponent<CameraController>();
            // Find or attach capture scripts
            VRCapture vrCap = camCtrler.gameObject.GetComponent <VRCapture> ();
            VRCaptureVideo vrCapVideo = camCtrler.gameObject.GetComponent <VRCaptureVideo> ();
            if (vrCap == null)
                vrCap = camCtrler.gameObject.AddComponent<VRCapture>();
            if (vrCapVideo == null)
                vrCapVideo = camCtrler.gameObject.AddComponent<VRCaptureVideo>();
            // Dont destory this on load
            DontDestroyOnLoad(camCtrler.gameObject);
            if (showCameraPreview)
                camCtrler.ShowCameraPreviewPanel(true);

            // Avatar Control
            playerAvatarPrefab = Resources.Load("Prefabs/August_Lowpoly") as GameObject;
            if (showPlayerAvatar)
            {
                avatarCtrler = Instantiate(playerAvatarPrefab).GetComponent<AvatarController>();
                DontDestroyOnLoad(avatarCtrler.gameObject);
                SetLayer(avatarCtrler.gameObject, LayerMask.NameToLayer("ShareVRIgnoreViewOnly"));
                avatarCtrler.EnableAvatar(showPlayerAvatar, avatarScale);

                avatarCtrler.UpdateAvatarOffset(avatarOffset);
            }

            // Initialize LiveFeed Object
            var livePlayGameObj = GameObject.Find ("LivePlayPlane");
            if (livePlayGameObj != null)
            {
                liveFeed = livePlayGameObj.GetComponent<LiveFeed>();
                DontDestroyOnLoad(liveFeed.gameObject);
                SetLayer(liveFeed.gameObject, LayerMask.NameToLayer("ShareVRIgnoreCaptureOnly"));
                //isUsingLiveFeed = true;
                liveFeed.InitializeReference();
                liveFeed.EnableLiveFeed(true);

                // Reference live play render texture
                vrCapVideo.copyTargetRT = liveFeed.livePlayRT;
            }

            // Initialize Camera Model
            if (showCameraModel)
                camCtrler.ShowCameraModel(true, cameraModelScale);

            // Attach WatsonService Object if using voice command feature
            if (useVoiceCommand)
                gameObject.AddComponent<WatsonService>();

            if (uploadFileOnline)
            {
                s3Uploader = gameObject.AddComponent<S3Uploader>();
                vrCap.SetS3Instance(s3Uploader);
            }

            // Update save folder
            if (saveFolder.Length > 3)
            {
                VRCaptureUtils.SaveFolder = saveFolder;
            }
        }

        void Update()
        {
            ProceedUserAction();
        }

        private void ProceedUserAction()
        {
            if (InputManager.userAct.startRec)
            {
                StartRecording();
                return;
            }
            if (InputManager.userAct.stopRec)
            {
                StopRecording();
                return;
            }
            if (InputManager.userAct.toggleAvatar)
            {
                avatarCtrler.EnableAvatar(!avatarCtrler.isAvatarEnabled);
                return;
            }
            if (InputManager.userAct.toggleCam)
            {
                camCtrler.ShowCameraModel(!camCtrler.isCamModelEnabled);
                return;
            }

            if (InputManager.userAct.toggleRec)
            {
                if (camCtrler.isCapturing)
                    StopRecording();
                else
                    StartRecording();
                return;
            }
        }

        public void StartRecording()
        {
            camCtrler.StartCapture();
        }

        public void StopRecording()
        {
            camCtrler.StopCapture();
        }

        public Transform GetPlayerTransform()
        {
            // Fall back reference
            if (trackingTarget == null)
            {
                Debug.Log("ShareVR: Tracking target not specified, using HMD instead");
                trackingTarget = GameObject.FindObjectOfType<AudioListener>().gameObject;
            }

            return trackingTarget.transform;
        }

        public void UpdateCameraParameters()
        {
            if (camCtrler != null)
                camCtrler.UpdateOrbitCameraParameters();
        }

        // Purpose: Recursively apply a given layer to a gameobject
        public static void SetLayer( GameObject parentGameObj, int layerID, bool applyChild = true )
        {
            parentGameObj.layer = layerID;
            if (applyChild)
            {
                foreach (Transform tr in parentGameObj.transform.GetComponentsInChildren<Transform>(true))
                {
                    tr.gameObject.layer = layerID;
                }
            }
        }
    }
}
