//======= Copyright (c) ShareVR ============================================
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
        [Tooltip ("Reference your own spectator camera")]
        public Camera trackingCamera;
        [Tooltip ("Target hand for the hand held camera")]
        public Ctrler targetCtrler = Ctrler.leftHand;
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
        public FrameSizeType frameSize = FrameSizeType._1280x720;
        [Tooltip ("Specify recording video framerate")]
        public TargetFramerateType frameRate = TargetFramerateType._30;
        [Tooltip ("Specify video file save folder (leave it blank if want to use default: Documents/ShareVR/)")]
        public string saveFolder = null;
        [Tooltip ("Specify video file name (leave it blank if want to use default)")]
        public string saveFileName = null;

        [Tooltip ("Do you want to automatically upload video online?")]
        public bool uploadFileOnline = false;

        [HideInInspector]
        public bool isVRDevicePresent = false;
        [HideInInspector]
        internal S3Uploader s3Uploader;

        // Internal Scripts Reference
        private GameObject cameraRigPrefab;
        private GameObject playerAvatarPrefab;
        private GameObject cameraGameObj;
        private CameraController camCtrler;
        private AvatarController avatarCtrler;
        private LiveFeed liveFeed;
        private VRCapture vrCap;
        private VRCaptureVideo vrCapVideo;
        private bool isCapturing = false;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _Instance = this;
            MetaDataContainer.unityVersion = Application.unityVersion;
        }

        void Start()
        {
            InitializeReference();

            if (showDebugMessage)
                Debug.Log("Checking active VR instance...");

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

        internal static RecordManager _Instance;

        void InitializeReference()
        {
            if (cameraFollowMethod == CameraFollowMethod.CustomCamera)
            {
                cameraGameObj = trackingCamera.gameObject;
                CameraController.InitializeCamera(cameraGameObj.GetComponent<Camera>());
                showCameraPreview = false;

            }
            else if (cameraFollowMethod == CameraFollowMethod.HandHeldCamera)
            {
                // Instantiate a new capture camera
                cameraRigPrefab = Resources.Load("Prefabs/ShareVRCameraRig") as GameObject;
                cameraGameObj = Instantiate(cameraRigPrefab);
                cameraGameObj.AddComponent<HandHeldCameraController>();
                trackingCamera = cameraGameObj.GetComponent<Camera>();

                // Initialize Camera Model
                if (showCameraModel)
                    camCtrler.ShowCameraModel(true, cameraModelScale);
            }
            else
            {
                // Instantiate a new capture camera
                cameraRigPrefab = Resources.Load("Prefabs/ShareVRCameraRig") as GameObject;
                cameraGameObj = Instantiate(cameraRigPrefab);
                camCtrler = cameraGameObj.AddComponent<CameraController>();
                trackingCamera = cameraGameObj.GetComponent<Camera>();

                // Initialize Camera Model
                if (showCameraModel)
                    camCtrler.ShowCameraModel(true, cameraModelScale);
            }

            if (cameraGameObj == null)
                Debug.LogError("ShareVR: Please make sure you DO specify a spectator camera when using custom camera mode!");

            // Find or attach capture scripts
            vrCap = cameraGameObj.GetComponent<VRCapture>();
            vrCapVideo = cameraGameObj.GetComponent<VRCaptureVideo>();
            if (vrCap == null)
                vrCap = cameraGameObj.AddComponent<VRCapture>();
            if (vrCapVideo == null)
                vrCapVideo = cameraGameObj.AddComponent<VRCaptureVideo>();

            // Initialize Capture scripts
            // Reference VRCaptureVideo and set video spec
            vrCapVideo.frameSize = frameSize;
            vrCapVideo.targetFramerate = frameRate;
            vrCapVideo.encodeQuality = VRCaptureVideo.EncodeQualityType.High;
            vrCapVideo.antiAliasing = VRCaptureVideo.AntiAliasingType._1;
            vrCapVideo.formatType = VRCaptureVideo.FormatType.NORMAL;
            vrCapVideo.isDedicated = true;
            vrCapVideo.isEnabled = true;
            vrCap.CaptureVideos = new[] { vrCapVideo };
            // Find main AudioListener in the scene and attach VRCaptureAudio component to it
            var audioListener = FindObjectOfType (typeof(AudioListener)) as AudioListener;
            if (audioListener != null)
            {
                vrCap.CaptureAudio = audioListener.gameObject.AddComponent<VRCaptureAudio>();
                if (showDebugMessage)
                    Debug.Log("ShareVR: Found main AudioSource - " + vrCap.CaptureAudio);
            }
            else
            {
                // Maybe add AudioSource to the Main Player?
            }

            // Dont destory this on load
            DontDestroyOnLoad(cameraGameObj);
            if (showCameraPreview)
                CameraController.ShowCameraPreviewPanel(true);

            // Avatar Control
            playerAvatarPrefab = Resources.Load("Prefabs/GenericMale1") as GameObject;
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
            if (isCapturing)
                return;
            isCapturing = true;

            if (showDebugMessage)
                Debug.Log("ShareVR: Start Recording!");

            if (showCameraModel)
                camCtrler.EnableCaptureLED(true);

            vrCap.StartCapture();
        }

        public void StopRecording()
        {
            if (!isCapturing)
                return;
            isCapturing = false;

            if (showDebugMessage)
                Debug.Log("ShareVR: Stop Recording!");

            if (showCameraModel)
                camCtrler.EnableCaptureLED(false);

            vrCap.StopCapture();

            // Update ShareVR Log
            MetaDataContainer.UpdateMetaData(saveFileName);
        }

        public Camera GetCaptureCamera()
        {
            return trackingCamera;
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
