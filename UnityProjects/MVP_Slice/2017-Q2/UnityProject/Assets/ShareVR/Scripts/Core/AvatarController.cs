//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Controls avatar animation and status
// Version: 1.0
// Date: 4/5/2017
// Revision History:  v1.0 - Adam - Created class for SDK v1.0
// 
// Still to do in v1.0 - define GetPlayerTransform() and
//                       GetSteamVRStatus() in RecordManager
//                       (lines 77, 96, 152)
//                     - set reference to avatarGameObjVR & PC
//                       (lines 69, 70)
//                     - set reference to avatarHandAnchor and
//                       LookatTarget internally in script
//                       (lines 44, 45)
//=============================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace ShareVR.Core
{
	public class AvatarController : MonoBehaviour
	{
		struct AvatarHandTr
		{
			public Transform Left;
			public Transform Right;
		};

		[HideInInspector]
		public bool isAvatarEnabled = false;

		// Interal References
		private RecordManager recManager;
		private InputManager inputManager;

		// Player Animator
		private Animator anim;
		// Player Transform
		private Transform playerTr;

		// ID number of avatar that determines physical appearance in game
		private int avatarID = 0;

		// Avatar body transforms
		private bool EnableHandIK = false;
		private bool EnableGazeIK = false;

		// Avatar References
		private bool avatarHandAnchorFound = false;
		private Transform[] avatarHandAnchor;
		private Transform lookatTarget;
		private AvatarHandTr avatarHandTr;
		private Transform avatarHeadTr;

		private Vector3 avatarRefPos = new Vector3 (0.0f, 0f, 0f);
		private const float offsetCtrlGain = 2.0f;

		// UpdateAvatarPos related variables
		private Vector3 playerRot;

		// EstimateVelocity related variables
		private Vector3 prevPos;
		private Vector3 posDiff;
		private float velocity = 0.0f;
		// Velocity smooth damp
		private const float veloSmoothFactor = 4.0f;
		private float angle = 0.0f;

		// Use this for initialization
		void Start ()
		{
			prevPos = transform.position;

			InitializeReference ();
			InitializeIK ();
			AvatarSayHi ();
		}

		void InitializeReference ()
		{
			recManager = FindObjectOfType (typeof(RecordManager)) as RecordManager;
			inputManager = FindObjectOfType (typeof(InputManager)) as InputManager;

			playerTr = recManager.GetPlayerTransform ();
		}

		void Update ()
		{
			if (isAvatarEnabled)
				UpdateAvatarOffset ();
		}

		void LateUpdate ()
		{
			//EstimatePlayerVelocity ();
			if (isAvatarEnabled)
				UpdateAvatarPos ();
		}

		public void SetAvatarID (int ID)
		{
			avatarID = ID;
		}

		public int GetAvatarID ()
		{
			return avatarID;
		}

		public void UpdateAvatarOffset (Vector3 offset)
		{
			avatarRefPos = offset;
		}

		public void EnableAvatar (bool state, float scale = 1.0f, Transform[] handTr = null, Vector3? offset = null)
		{
			isAvatarEnabled = state;

			this.transform.localScale *= scale;
			this.gameObject.SetActive (isAvatarEnabled);

			if (handTr != null) {
				avatarHandAnchor = handTr;
				avatarHandAnchorFound = true;
				EnableHandIK = true;
			}

			if (offset != null)
				this.transform.position = new Vector3 (offset.Value.x, offset.Value.y, offset.Value.z);
		}

		// Purpose: Update Animator IK status and target
		void OnAnimatorIK (int layerIndex)
		{
			if (EnableGazeIK) {
				anim.SetLookAtWeight (1, 1, 1);
				anim.SetLookAtPosition (lookatTarget.position);
			}

			if (EnableHandIK) {
				anim.SetIKPositionWeight (AvatarIKGoal.LeftHand, 1);
				anim.SetIKPositionWeight (AvatarIKGoal.RightHand, 1);
				anim.SetIKRotationWeight (AvatarIKGoal.LeftHand, 1);
				anim.SetIKRotationWeight (AvatarIKGoal.RightHand, 1);

				if (inputManager.isViveDeviceFound) {
					if (!avatarHandAnchorFound && recManager.playerHandTransforms.Length == 2 && recManager.playerHandTransforms [0] != null && recManager.playerHandTransforms [1] != null) {
						avatarHandAnchor = recManager.playerHandTransforms;
						EnableHandIK = true;
					}
					
					anim.SetIKPosition (AvatarIKGoal.LeftHand, avatarHandAnchor [0].transform.position);
					anim.SetIKPosition (AvatarIKGoal.RightHand, avatarHandAnchor [1].transform.position);
					anim.SetIKRotation (AvatarIKGoal.LeftHand, avatarHandAnchor [0].transform.rotation);
					anim.SetIKRotation (AvatarIKGoal.RightHand, avatarHandAnchor [1].transform.rotation);
				}
			}
		}

		// Purpose: Initialize Animator IK based on SteamVR status
		private void InitializeIK ()
		{
			anim = GetComponent <Animator> ();

			avatarHeadTr = anim.GetBoneTransform (HumanBodyBones.Head);
			avatarHandTr.Left = anim.GetBoneTransform (HumanBodyBones.LeftHand);
			avatarHandTr.Right = anim.GetBoneTransform (HumanBodyBones.RightHand);

			// Will only use VR mode for now
			if (avatarHandAnchorFound)
				EnableHandIK = true;
			EnableGazeIK = false;
		}

		// Purpose: (Temperary) Allow using keyboard to change avatar vertical offset
		private void UpdateAvatarOffset ()
		{
			if (Input.GetKey (KeyCode.Equals))
				avatarRefPos = new Vector3 (0.0f, avatarRefPos.y + offsetCtrlGain * Time.deltaTime, 0.0f);
			if (Input.GetKey (KeyCode.Minus))
				avatarRefPos = new Vector3 (0.0f, avatarRefPos.y - offsetCtrlGain * Time.deltaTime, 0.0f);
		}

		// Purpose: Update avatar pose
		private void UpdateAvatarPos ()
		{
			// Update Animator Controller Parameters
			anim.SetFloat ("MoveSpeed", velocity);

			// Get Player Rotation
			playerRot = playerTr.eulerAngles;

			// Update Avatar position and rotation
			transform.position = playerTr.position + avatarRefPos;
			transform.eulerAngles = new Vector3 (0.0f, playerRot.y, 0 * playerRot.z);

			// Update Head pitch angle only if gaze IK is not enabled
			if (!EnableGazeIK)
				avatarHeadTr.eulerAngles = new Vector3 (avatarHeadTr.eulerAngles.x, avatarHeadTr.eulerAngles.y, -playerRot.x - 90.0f);
		}

		// Purpose: Estimate player velocity
		public void EstimatePlayerVelocity ()
		{
			// Calculate movement vector
			posDiff = transform.position - prevPos;
			prevPos = transform.position;

			// Estimate movement direction
			angle = Vector3.Angle (posDiff, transform.forward);

			// Determine diretion of motion
			if (angle < 100.0f)
				velocity = Mathf.Lerp (velocity, 0.2f * posDiff.magnitude / Time.deltaTime, veloSmoothFactor * Time.deltaTime);
			else
				velocity = Mathf.Lerp (velocity, -0.2f * posDiff.magnitude / Time.deltaTime, veloSmoothFactor * Time.deltaTime);

			// Noise Gate to Avoid Jitter
			if (Mathf.Abs (velocity) < 0.05f)
				velocity = 0.0f;
		}

		public void AvatarSayHi ()
		{
			anim.SetTrigger ("SayHi");
		}
	}

}