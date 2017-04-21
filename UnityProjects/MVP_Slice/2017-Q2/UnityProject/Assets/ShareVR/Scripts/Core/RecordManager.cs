//======= Copyright (c) ShareVR ===============
//
// Purpose: Manage recording status and all capture related scripts
// Version: SDK v1.0
// Date: 4/6/2017
// Created by: Chen Chen
// Revision History:
// 
//=============================================================================

using System.Collections;
using UnityEngine;
using VRCapture;

namespace ShareVR.Core
{
	[RequireComponent (typeof(InputManager))]
	public class RecordManager : MonoBehaviour
	{
		[Header ("ShareVR Camera Control")]
		[Tooltip ("Make the spectator camera visible")]
		public bool showCameraModel = false;
		[Tooltip ("Adjust camera model scale if it's too big or small")]
		public float cameraModelScale = 1.0f;

		[Header ("ShareVR Avatar Control")]
		[Tooltip ("Show a cute avatar for demo")]
		public bool showPlayerAvatar = false;
		[Tooltip ("Adjust avatar scale if it's too big or small")]
		public float avatarScale = 1.0f;
		[Tooltip ("Adjust avatar offset")]
		public Vector3 avatarOffset = new Vector3 (0, -1.0f, 0);

		[Header ("ShareVR Input Control")]
		[Tooltip ("Specify your preferred input method")]
		public ViveCtrlerMapping toggleRecordingInput = ViveCtrlerMapping.KeyboardOnly_Key_X;
		[Tooltip ("Do you want to use our voice command (beta) feature?")]
		public bool useVoiceCommand = true;


		[Header ("ShareVR Game Object Reference (Please link your game objects here)")]
		[Tooltip ("Specify your main player gameobject")]
		public GameObject playerHeadGameObject;
		[Tooltip ("Specify your vive controller gameobject")]
		public Transform[] playerHandTransforms;
		[Tooltip ("Display debug message")]
		public bool showDebugMessage = false;

		[Header ("ShareVR Video Format")]
		[Tooltip ("Specify recording video resolution")]
		public VRCaptureVideo.FrameSizeType frameSize = VRCaptureVideo.FrameSizeType._1280x720;
		public VRCaptureVideo.TargetFramerateType frameRate = VRCaptureVideo.TargetFramerateType._30;

		[HideInInspector]
		public bool isSteamVRActive = false;

		// Internal Scripts Reference
		private GameObject cameraRigPrefab;
		private GameObject playerAvatarPrefab;

		private RecordManager recManager;
		private CameraController camCtrler;
		private InputManager inputManager;
		private AvatarController avatarCtrler;
		private LiveFeed liveFeed;
		private bool isUsingLiveFeed = false;

		IEnumerator Start ()
		{
			InitializeReference ();

			if (recManager.showDebugMessage)
				Debug.Log ("Checking active SteamVR instance...");
			
			// Search for avtive SteamVR Session every one second
			while (true) {
				// This will get refreshed every one second
				yield return new WaitForSeconds (1.0f);

				// Search for SteamVR
				if (SteamVR.active) {
					if (recManager.showDebugMessage)
						Debug.Log ("Found active SteamVR!");
					isSteamVRActive = true;
					yield break;
				}
			}
		}

		void InitializeReference ()
		{
			// Reference ShareVR scripts
			inputManager = FindObjectOfType (typeof(InputManager)) as InputManager;
			recManager = FindObjectOfType (typeof(RecordManager)) as RecordManager;

			// Instantiate a new capture camera
			cameraRigPrefab = Resources.Load ("Prefabs/ShareVRCameraRig") as GameObject;
			camCtrler = Instantiate (cameraRigPrefab).GetComponent <CameraController> ();

			// Avatar Control
			playerAvatarPrefab = Resources.Load ("Prefabs/PlayerAvatar-1") as GameObject;
			if (showPlayerAvatar) {
				avatarCtrler = Instantiate (playerAvatarPrefab).GetComponent <AvatarController> ();
				if (playerHandTransforms.Length == 2)
					avatarCtrler.EnableAvatar (showPlayerAvatar, avatarScale, playerHandTransforms);
				else
					avatarCtrler.EnableAvatar (showPlayerAvatar, avatarScale);

				avatarCtrler.UpdateAvatarOffset (avatarOffset);
			}

			// Initialize LiveFeed Object
			liveFeed = FindObjectOfType (typeof(LiveFeed)) as LiveFeed;
			if (liveFeed != null) {
				isUsingLiveFeed = true;
				liveFeed.InitializeReference ();
				liveFeed.EnableLiveFeed (true);
			}

			// Initialize Camera Model
			if (showCameraModel)
				camCtrler.ShowCameraModel (true, cameraModelScale);

			// Attach WatsonService Object if using voice command feature
			if (useVoiceCommand)
				gameObject.AddComponent <WatsonService> ();
		}

		void Update ()
		{
			ProceedUserAction ();
		}

		private void ProceedUserAction ()
		{
			if (inputManager.userAct.startRec) {
				StartRecording ();
				return;
			}
			if (inputManager.userAct.stopRec) {
				StopRecording ();
				return;
			}
			if (inputManager.userAct.toggleAvatar) {
				avatarCtrler.EnableAvatar (!avatarCtrler.isAvatarEnabled);
				return;
			}
			if (inputManager.userAct.toggleCam) {
				camCtrler.ShowCameraModel (!camCtrler.isCamModelEnabled);
				return;
			}

			if (inputManager.userAct.toggleRec) {
				if (camCtrler.isCapturing)
					StopRecording ();
				else
					StartRecording ();
				return;
			}
		}

		public void StartRecording ()
		{
			camCtrler.StartCapture ();
		}

		public void StopRecording ()
		{
			camCtrler.StopCapture ();
		}

		public Transform GetPlayerTransform ()
		{
			return playerHeadGameObject.transform;
		}
	}
}
