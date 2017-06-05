//======= Copyright (c) ShareVR ===========================
//
// Purpose: Spectator Camera Controller
// Version: 0.4c
// Chen Chen
// 4/30/2017
//=========================================================
using System.Collections;
using UnityEngine;
using UnityEngine.VR;
using System;
using ShareVR.Capture;

namespace ShareVR.Core
{
    [System.Serializable]
    public enum CameraFollowMethod
    {
        FixedSmooth,
        OrbitSmooth,
        //HandHeldCamera,
        //HandSelfieCamera,
        CustomCamera
    }

    // Make sure a Camera component is attached
    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(VRCapture))]
    [RequireComponent(typeof(VRCaptureVideo))]
    internal class CameraController : MonoBehaviour
    {
        [NonSerializedAttribute]
        public bool isCapturing = false;
        [NonSerializedAttribute]
        public bool isCamModelEnabled = false;

        // Game object reference - to be set internally
        protected GameObject camStatusLight;
        protected GameObject camModelPrefab;
        protected static GameObject camPreviewPanelPrefab;
        protected Camera capCam;
        protected Transform playerTr;

        private RecordManager recManager;
        private GameObject m_camModelInstance;
        private static GameObject m_camPreviewPanelInstance;

        // Smooth LookAt function related variables and parameters
        private Vector3 camPos;
        private Quaternion camRot;

        private Vector3 m_orbitOffset;
        private Vector3 m_playerRefOffset;

        // Local component reference
        private static Transform m_Transform;

        void Awake()
        {
            // Initialize Reference
            InitializeRefernece();
        }

        void Start()
        {
            // Initialize Camera
            InitializeCamera(capCam);
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

            if (camModelPrefab == null)
                camModelPrefab = Resources.Load("Prefabs/CameraModel") as GameObject;

            if (camPreviewPanelPrefab == null)
                camPreviewPanelPrefab = Resources.Load("Prefabs/CameraPreviewPanel") as GameObject;
        }

        public static void InitializeCamera( Camera cam )
        {
            cam.enabled = false;
            cam.targetDisplay = 8;
            cam.stereoTargetEye = StereoTargetEyeMask.None;
            cam.allowHDR = false;
            cam.allowMSAA = false;
            cam.depth = 99.0f;
            cam.fieldOfView = 60.0f;
            cam.farClipPlane = 400.0f;
            cam.nearClipPlane = 0.8f;
            cam.enabled = true;

            // Remove ShareVRIgnoreCaptureOnly and add ShareVRIgnoreViewOnly in culling mask
            cam.cullingMask &= ~( 1 << LayerMask.NameToLayer("ShareVRIgnoreCaptureOnly") );
            cam.cullingMask |= ( 1 << LayerMask.NameToLayer("ShareVRIgnoreViewOnly") );

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

        public static void ShowCameraPreviewPanel( bool state, Transform tr = null, float scale = 1.0f )
        {
            if (state)
            {
                if (m_camPreviewPanelInstance == null)
                {
                    m_camPreviewPanelInstance = Instantiate(Resources.Load("Prefabs/CameraPreviewPanel") as GameObject);
                    if (tr != null)
                        m_camPreviewPanelInstance.transform.SetParent(tr);
                    else
                        m_camPreviewPanelInstance.transform.SetParent(m_Transform);

                    m_camPreviewPanelInstance.transform.localPosition = new Vector3(0.75f, 0, 0.5f);
                    m_camPreviewPanelInstance.transform.localEulerAngles = new Vector3(90, 0, 0);
                    m_camPreviewPanelInstance.transform.localScale = new Vector3(0.07f, 1.0f, 0.05f) * scale;

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
        public void EnableCaptureLED( bool state )
        {
            if (state)
                StartCoroutine(BlinkCameraLight());
            else
                StopCoroutine(BlinkCameraLight());
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

    [RequireComponent(typeof(Camera))]
    [RequireComponent(typeof(VRCapture))]
    [RequireComponent(typeof(VRCaptureVideo))]
    internal class HandHeldCameraController : MonoBehaviour
    {
        private Camera capCam;
        private Ctrler m_targetCtrler = Ctrler.leftHand;
        [HideInInspector]
        public bool isSelfieMode = false;

        // Local Variables
        private Transform m_cameraTransform;
        private Vector3 handPosition;
        private Quaternion handRotation;
        [SerializeField]
        private Vector3 handTiltRotation = new Vector3(30.0f, 0.0f, 0.0f);
        [SerializeField]
        private Vector3 selfieModeRotation = new Vector3(-30.0f, 180.0f, 0.0f);
        [SerializeField]
        private Vector3 selfieModeOffset = new Vector3(0.0f, 0.0f, 0.0f);


        private void Start()
        {
            capCam = GetComponent<Camera>();
            m_cameraTransform = transform;
        }

        private void LateUpdate()
        {
            if (m_targetCtrler == Ctrler.leftHand)
                handPosition = InputTracking.GetLocalPosition(VRNode.LeftHand);
            else
                handPosition = InputTracking.GetLocalPosition(VRNode.RightHand);

            // Get hand rotation
            if (isSelfieMode)
            {
                if (m_targetCtrler == Ctrler.leftHand)
                    handRotation = InputTracking.GetLocalRotation(VRNode.LeftHand) * Quaternion.Euler(selfieModeRotation);
                else
                    handRotation = InputTracking.GetLocalRotation(VRNode.RightHand) * Quaternion.Euler(selfieModeRotation);
            }
            else
            {
                if (m_targetCtrler == Ctrler.leftHand)
                    handRotation = InputTracking.GetLocalRotation(VRNode.LeftHand) * Quaternion.Euler(handTiltRotation);
                else
                    handRotation = InputTracking.GetLocalRotation(VRNode.RightHand) * Quaternion.Euler(handTiltRotation);
            }

            // Update transform
            transform.SetPositionAndRotation(handPosition, handRotation);
        }

        public void SetTargetHand( Ctrler ctl )
        {
            m_targetCtrler = ctl;
        }

        public void EnableSelfieMode( bool b )
        {
            isSelfieMode = b;
        }
    }
}
