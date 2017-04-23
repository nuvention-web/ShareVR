//======= Copyright (c) ShareVR ===============
//
// Purpose: Contains methods that allow the spectator camera
//          to smoothly follow the player
// Version: 0.3
// Date: 4/23/2017
//=============================================================
using System.Collections;
using UnityEngine;
using ShareVR.Core;
using System;
using VRCapture;

namespace ShareVR.Core
{
	[System.Serializable]
	public enum CameraFollowMethod
	{
		FixedSmooth,
		OrbitSmooth
	}

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
		protected GameObject camStatusLight;
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
		private Color camIdleColor = Color.gray;

		private float turnSpeed = 1.0f;
		private Vector3 offset;

		void Awake ()
		{
			// Initialize Reference
			InitializeRefernece ();
		}

		void Start ()
		{
			// Initialize Camera
			InitializeCamera ();
			offset = playerTr.position + new Vector3 (0, 2, 3);
		}


		void LateUpdate ()
		{
			if (isCapturing || isCamModelEnabled) {
				switch (recManager.cameraFollowMethod) {
				case CameraFollowMethod.FixedSmooth:
					SmoothLookAt (playerTr);
					break;
				case CameraFollowMethod.OrbitSmooth:
					SmoothFollow (playerTr);
					break;
				}
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
			var audioListener = FindObjectOfType (typeof(AudioListener)) as AudioListener;
			if (audioListener != null) {
				vrcap.CaptureAudio = audioListener.gameObject.AddComponent <VRCaptureAudio> ();
				if (recManager.showDebugMessage)
					Debug.Log ("ShareVR: Found main AudioSource - " + vrcap.CaptureAudio);
			} else {
				// Maybe add AudioSource in the Main Player?
			}

			if (camModelPrefab == null)
				camModelPrefab = Resources.Load ("Prefabs/CameraModel") as GameObject;
		}

		private void InitializeCamera ()
		{
			capCam.depth = 99.0f;
			capCam.fieldOfView = 60.0f;
			capCam.farClipPlane = 500.0f;
			capCam.nearClipPlane = 0.8f;

			capCam.cullingMask &= ~(1 << LayerMask.NameToLayer ("IgnoreInCapture"));
			capCam.cullingMask |= (1 << LayerMask.NameToLayer ("IgnoreInView"));
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

					camStatusLight = m_camModelInstance.transform.FindChild ("Status").gameObject;
					camStatusLight.SetActive (false);
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

			StartCoroutine (BlinkCameraLight ());

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

			StopCoroutine (BlinkCameraLight ());

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

		private void SmoothFollow (Transform target)
		{
			offset = Quaternion.AngleAxis (turnSpeed * Time.deltaTime * 10.0f, Vector3.up) * offset;
			transform.position = target.position + offset; 
			transform.LookAt (target.position);
		}

		IEnumerator BlinkCameraLight ()
		{
			while (true) {
				if (isCapturing) {
					yield return new WaitForSeconds (0.4f);
					camStatusLight.SetActive (true);
					yield return new WaitForSeconds (0.4f);
					camStatusLight.SetActive (false);
				} else
					yield break;
			}
		}
	}
}