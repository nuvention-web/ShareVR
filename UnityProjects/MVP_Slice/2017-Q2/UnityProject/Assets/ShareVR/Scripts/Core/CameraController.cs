//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Contains methods that allow the spectator camera
//          to smoothly follow the player
// Version: 1.0
// Date: 4/5/2017
// Revision History:  v1.0 - Adam - Created class for SDK v1.0
// 
// Still to do in v1.0 - set game object references for vrGameObj,
//                       pcGameObj, camStatus & camModel (lines 27 - 30)
//                     - set game object references for targetCam
//                       and capCam (lines 41 & 43)
//                     - define GetPlayerTransform in RecordManager
//                       (line 74)
//=============================================================
using System.Collections;
using UnityEngine;
using ShareVR.Core;
using System;
using VRCapture;

namespace ShareVR.Core
{
	// Make sure a Camera component is attached
	[RequireComponent (typeof(Camera))]
	[RequireComponent (typeof(VRCapture.VRCapture))]
	[RequireComponent (typeof(VRCaptureVideo))]
	public class CameraController : MonoBehaviour
	{
		[NonSerializedAttribute]
		public bool isCapturing = false;
		[NonSerializedAttribute]
		public bool isCamModelEnabled = false;

		// Game object reference - to be set internally
		protected Material camStatusLight;
		protected GameObject camModelPrefab;
		protected Camera capCam;
		protected Transform playerTr;

		private VRCapture.VRCapture vrcap;
		private VRCaptureVideo vrcapVideo;
		private RecordManager recManager;
		private GameObject m_camModelInstance;

		// Smooth LookAt function related variables and parameters
		private const float damp = 2.0f;
		private const float camDist = 4.0f;
		private Vector3 targetDirection = new Vector3 (-0.9f, 0.5f, 1.1f);
		private Vector3 camPos;
		private Quaternion camRot;

		void Awake ()
		{
			// Initialize Reference
			InitializeRefernece ();
		}

		void Start ()
		{
			// Initialize Camera
			InitializeCamera ();
		}


		void LateUpdate ()
		{
			if (isCapturing || isCamModelEnabled) {
				SmoothLookAt (playerTr);
			}
		}

		private void InitializeRefernece ()
		{
			capCam = GetComponent <Camera> ();
			recManager = FindObjectOfType (typeof(RecordManager)) as RecordManager;
			playerTr = recManager.GetPlayerTransform ();

			// Reference VRCaptureVideo and set video spec
			vrcapVideo = GetComponent <VRCaptureVideo> ();
			vrcapVideo.frameSize = recManager.frameSize;
			vrcapVideo.targetFramerate = recManager.frameRate;
			vrcapVideo.encodeQuality = VRCaptureVideo.EncodeQualityType.High;
			vrcapVideo.antiAliasing = VRCaptureVideo.AntiAliasingType._1;
			vrcapVideo.formatType = VRCaptureVideo.FormatType.NORMAL;
			vrcapVideo.isDedicated = true;
			vrcapVideo.isEnabled = true;

			vrcap = GetComponent <VRCapture.VRCapture> ();
			vrcap.CaptureVideos = new []{ vrcapVideo };

			// Find main AudioListener in the scene and attach VRCaptureAudio component to it
			//var audioListener = FindObjectOfType (typeof(AudioListener)) as AudioListener;
			//vrcap.CaptureAudio = audioListener.gameObject.AddComponent <VRCaptureAudio> ();
			//Debug.Log (vrcap.CaptureAudio);

			if (camModelPrefab == null)
				camModelPrefab = Resources.Load ("Prefabs/CameraModel") as GameObject;
			camStatusLight = Resources.Load ("Materials/CamStatus") as Material;
		}

		private void InitializeCamera ()
		{
			capCam.depth = 99.0f;
			capCam.fieldOfView = 60.0f;
			capCam.farClipPlane = 500.0f;
			capCam.nearClipPlane = 0.8f;
		}

		// Purpose: Get current spectator recording status
		public bool GetCamModelStatus ()
		{
			return isCamModelEnabled;
		}

		// Purpose: Get current spectator Capture status
		public bool GetCaptureStatus ()
		{
			return isCapturing;
		}

		public Transform GetCameraTransform ()
		{
			return transform;
		}

		public Camera GetCaptureCamera ()
		{
			return capCam;
		}

		// Purpose: Toggle the camera model while in game
		public void ShowCameraModel (bool state, float scale = 1.0f)
		{
			// Create or Destroy Current Camera Model Instance
			if (m_camModelInstance == null) {
				if (state) {
					// Instantiate a new camera model
					if (camModelPrefab == null)
						camModelPrefab = Resources.Load ("Prefabs/CameraModel") as GameObject;
					m_camModelInstance = Instantiate (camModelPrefab);
					m_camModelInstance.transform.SetParent (transform);
					m_camModelInstance.transform.localPosition = Vector3.zero;
					m_camModelInstance.transform.localEulerAngles = Vector3.zero;
					m_camModelInstance.transform.localScale *= scale;
				} else {
					// Destroy current instance
					Destroy (m_camModelInstance);
					m_camModelInstance = null;
				}
			}

			// Update the toggle status of camera model
			isCamModelEnabled = state;
		}

		// Purpose: Start capture session
		public void StartCapture ()
		{
			if (isCapturing)
				return;
			isCapturing = true;

			if (recManager.showDebugMessage)
				Debug.Log ("ShareVR: Start Recording!");
			
			camStatusLight.color = new Color (0, 1, 0);
			vrcap.StartCapture ();
		}

		// Purpose: End capture session
		public void StopCapture ()
		{
			if (!isCapturing)
				return;
			isCapturing = false;

			if (recManager.showDebugMessage)
				Debug.Log ("Stop Recording!");
			
			camStatusLight.color = new Color (1, 0, 0);
			vrcap.StopCapture ();
		}

		// Purpose: Smoothly move and rotate the camera so that it will always follow and look at player
		private void SmoothLookAt (Transform target)
		{
			// Calculate target camera position
			camPos = target.position + (targetDirection.normalized * camDist);

			// Smoothly move camera to target position
			transform.position = Vector3.Lerp (transform.position, camPos, damp * Time.deltaTime);

			// Calculate target camera rotation
			camRot = Quaternion.LookRotation (target.position - transform.position);

			// Smoothly rotate camera to look at target
			transform.rotation = Quaternion.Slerp (transform.rotation, camRot, damp * Time.deltaTime);
		}
	}
}