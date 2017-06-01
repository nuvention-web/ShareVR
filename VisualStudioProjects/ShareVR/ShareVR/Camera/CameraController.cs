//======= Copyright (c) ShareVR ===========================
//
// Purpose: Spectator Camera Controller
// Version: 0.4b
// Chen Chen
// 4/30/2017
//=========================================================
using System.Collections;
using UnityEngine;
using ShareVR.Core;
using System;
using ShareVR.Capture;

namespace ShareVR.Core
{
    [System.Serializable]
    public enum CameraFollowMethod
    {
        FixedSmooth,
        OrbitSmooth
    }

    // Make sure a Camera component is attached
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(VRCapture))]
    [RequireComponent(typeof(VRCaptureVideo))]
    public class CameraController : MonoBehaviour
    {
        [NonSerializedAttribute]
        public bool isCapturing = false;
        [NonSerializedAttribute]
        public bool isCamModelEnabled = false;

        // Game object reference - to be set internally
        protected GameObject camStatusLight;
        protected GameObject camModelPrefab;
        protected GameObject camPreviewPanelPrefab;
        protected Camera capCam;
        protected Transform playerTr;

        private VRCapture vrcap;
        private VRCaptureVideo vrcapVideo;
        private RecordManager recManager;
        private GameObject m_camModelInstance;
        private GameObject m_camPreviewPanelInstance;

        // Smooth LookAt function related variables and parameters
        private Vector3 camPos;
        private Quaternion camRot;

        private Vector3 m_orbitOffset;
        private Vector3 m_playerRefOffset;

        // Local component reference
        private Transform m_Transform;

        void Awake()
        {
            // Initialize Reference
            InitializeRefernece();
        }

        void Start()
        {
            // Initialize Camera
            InitializeCamera();
            m_playerRefOffset = playerTr.position;
            UpdateOrbitCameraParameters();
        }

        public void UpdateOrbitCameraParameters()
        {
            m_orbitOffset = m_playerRefOffset + new Vector3(0.0f, recManager.camHeight, recManager.camDistance);
        }

        void LateUpdate()
        {
            // Update Internal Parameters
            switch (recManager.cameraFollowMethod)
            {
                case CameraFollowMethod.FixedSmooth:
                    SmoothLookAt(playerTr);
                    break;
                case CameraFollowMethod.OrbitSmooth:
                    SmoothFollow(playerTr);
                    break;
            }
        }

        private void InitializeRefernece()
        {
            m_Transform = transform;

            capCam = GetComponent<Camera>();

            recManager = FindObjectOfType(typeof(RecordManager)) as RecordManager;
            playerTr = recManager.GetPlayerTransform();

            // Fall back reference
            if (playerTr == null)
            {
                Debug.LogWarning("ShareVR: Tracking target not specified, using main player instead");
                playerTr = UnityEngine.Object.FindObjectOfType<AudioListener>().transform;
            }

            // Reference VRCaptureVideo and set video spec
            vrcapVideo = GetComponent<VRCaptureVideo>();
            if (vrcapVideo == null)
                vrcapVideo = gameObject.AddComponent<VRCaptureVideo>();
            vrcapVideo.frameSize = recManager.frameSize;
            vrcapVideo.targetFramerate = recManager.frameRate;
            vrcapVideo.encodeQuality = VRCaptureVideo.EncodeQualityType.High;
            vrcapVideo.antiAliasing = VRCaptureVideo.AntiAliasingType._1;
            vrcapVideo.formatType = VRCaptureVideo.FormatType.NORMAL;
            vrcapVideo.isDedicated = true;
            vrcapVideo.isEnabled = true;

            vrcap = GetComponent<VRCapture>();
            if (vrcap == null)
                vrcap = gameObject.AddComponent<VRCapture>();
            vrcap.CaptureVideos = new[] { vrcapVideo };

            // Find main AudioListener in the scene and attach VRCaptureAudio component to it
            var audioListener = FindObjectOfType (typeof(AudioListener)) as AudioListener;
            if (audioListener != null)
            {
                vrcap.CaptureAudio = audioListener.gameObject.AddComponent<VRCaptureAudio>();
                if (recManager.showDebugMessage)
                    Debug.Log("ShareVR: Found main AudioSource - " + vrcap.CaptureAudio);
            }
            else
            {
                // Maybe add AudioSource to the Main Player?
            }

            if (camModelPrefab == null)
                camModelPrefab = Resources.Load("Prefabs/CameraModel") as GameObject;

            if (camPreviewPanelPrefab == null)
                camPreviewPanelPrefab = Resources.Load("Prefabs/CameraPreviewPanel") as GameObject;
        }

        private void InitializeCamera()
        {
            capCam.depth = 99.0f;
            capCam.fieldOfView = 60.0f;
            capCam.farClipPlane = 500.0f;
            capCam.nearClipPlane = 0.8f;

            // Remove ShareVRIgnoreCaptureOnly and add ShareVRIgnoreViewOnly in culling mask
            capCam.cullingMask &= ~( 1 << LayerMask.NameToLayer("ShareVRIgnoreCaptureOnly") );
            capCam.cullingMask |= ( 1 << LayerMask.NameToLayer("ShareVRIgnoreViewOnly") );

            // Remove ShareVRIgnoreViewOnly in culling mask
            Camera.main.cullingMask &= ~( 1 << LayerMask.NameToLayer("ShareVRIgnoreViewOnly") );
        }

        // Purpose: Get current spectator recording status
        public bool GetCamModelStatus()
        {
            return isCamModelEnabled;
        }

        // Purpose: Get current spectator Capture status
        public bool GetCaptureStatus()
        {
            return isCapturing;
        }

        public Transform GetCameraTransform()
        {
            return m_Transform;
        }

        public Camera GetCaptureCamera()
        {
            return capCam;
        }

        public void ShowCameraPreviewPanel( bool state )
        {
            if (state)
            {
                if (m_camPreviewPanelInstance == null)
                {
                    m_camPreviewPanelInstance = Instantiate(camPreviewPanelPrefab);
                    m_camPreviewPanelInstance.transform.SetParent(m_Transform);

                    m_camPreviewPanelInstance.transform.localPosition = new Vector3(0.75f, 0, 0.5f);
                    m_camPreviewPanelInstance.transform.localEulerAngles = new Vector3(90, 0, 0);
                    m_camPreviewPanelInstance.transform.localScale = new Vector3(0.07f, 1.0f, 0.05f) * recManager.cameraModelScale;

                    // Enable Preview
                    m_camPreviewPanelInstance.GetComponent<LiveFeed>().InitializeReference();
                    m_camPreviewPanelInstance.GetComponent<LiveFeed>().EnableLiveFeed(true);
                }
            }
            else
            {
                if (m_camPreviewPanelInstance != null)
                    Destroy(m_camPreviewPanelInstance);
                m_camPreviewPanelInstance = null;
            }
        }

        // Purpose: Toggle the camera model while in game
        public void ShowCameraModel( bool state, float scale = 1.0f )
        {
            // Create or Destroy Current Camera Model Instance
            if (state)
            {
                // Instantiate a new camera model
                if (m_camModelInstance == null)
                {
                    m_camModelInstance = Instantiate(camModelPrefab);
                    m_camModelInstance.transform.SetParent(m_Transform);
                    m_camModelInstance.transform.localPosition = Vector3.zero;
                    m_camModelInstance.transform.localEulerAngles = Vector3.zero;
                    m_camModelInstance.transform.localScale *= scale;

                    camStatusLight = m_camModelInstance.transform.Find("Status").gameObject;
                    camStatusLight.GetComponent<MeshRenderer>().material.color = Color.red;
                    if (camStatusLight != null)
                        camStatusLight.SetActive(false);
                }
            }
            else
            {
                // Destroy current instance
                if (m_camModelInstance != null)
                    Destroy(m_camModelInstance);
                m_camModelInstance = null;
            }

            // Update the toggle status of camera model
            isCamModelEnabled = state;
        }

        // Purpose: Start capture session
        public void StartCapture()
        {
            if (isCapturing)
                return;
            isCapturing = true;

            if (recManager.showDebugMessage)
                Debug.Log("ShareVR: Start Recording!");

            if (recManager.showCameraModel)
                StartCoroutine(BlinkCameraLight());

            vrcap.StartCapture();
        }

        // Purpose: End capture session
        public void StopCapture()
        {
            if (!isCapturing)
                return;
            isCapturing = false;

            if (recManager.showDebugMessage)
                Debug.Log("Stop Recording!");

            if (recManager.showCameraModel)
                StopCoroutine(BlinkCameraLight());

            vrcap.StopCapture(recManager.saveFileName);
        }

        // Purpose: Smoothly move and rotate the camera so that it will always follow and look at player
        private void SmoothLookAt( Transform target )
        {
            // Calculate target camera position
            camPos = target.position + new Vector3(0.0f, recManager.camHeight, recManager.camDistance);
            camPos = Quaternion.AngleAxis(recManager.camAngle, Vector3.up) * camPos;

            // Smoothly move camera to target position
            m_Transform.position = Vector3.Lerp(m_Transform.position, camPos, recManager.camMotionDamp * Time.deltaTime);

            // Calculate target camera rotation
            camRot = Quaternion.LookRotation(target.position - m_Transform.position);

            // Smoothly rotate camera to look at target
            m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, camRot, recManager.camMotionDamp * Time.deltaTime);
        }

        private void SmoothFollow( Transform target )
        {
            m_orbitOffset = Quaternion.AngleAxis(recManager.cameraOrbitSpeed * Time.deltaTime * 10.0f, Vector3.up) * m_orbitOffset;
            m_Transform.position = target.position + m_orbitOffset;
            m_Transform.LookAt(target.position);
        }

        IEnumerator BlinkCameraLight()
        {
            while (true)
            {
                if (isCapturing)
                {
                    yield return new WaitForSeconds(0.4f);
                    camStatusLight.SetActive(true);
                    yield return new WaitForSeconds(0.4f);
                    camStatusLight.SetActive(false);
                }
                else
                    yield break;
            }
        }
    }
}
