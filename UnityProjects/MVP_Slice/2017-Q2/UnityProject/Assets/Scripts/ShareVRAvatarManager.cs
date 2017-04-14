//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Controls avatar animation and status
// Version: 1.2
// Date: 3/1/2017
// Revision History:  v1.2 - Chen - Created for v1.2 release
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShareVRAvatarManager : MonoBehaviour
{

	struct AvatarHandTr
	{
		public Transform Left;
		public Transform Right;
	};

	public ShareVRManager sharevr;

	// Player Animator
	Animator anim;

	// Player Transform
	Transform playerTr;

	// Avatar body transforms
	public bool EnableHandIK = false;
	public bool EnableGazeIK = false;
	public Transform[] avatarHandAnchor;
	public Transform lookatTarget;
	AvatarHandTr avatarHandTr;
	Transform avatarHeadTr;
	Vector3 avatarRefPos = new Vector3 (0.0f, -1.0f, -0.5f);
	float offsetCtrlGain = 2.0f;

	// UpdateAvatarPose related variables
	Vector3 playerRot;

	// EstimateVelocity related variables
	Vector3 prevPos;
	Vector3 posDiff;
	float velocity = 0.0f;
	// Velocity smooth damp
	float veloSmoothFactor = 4.0f;
	float angle = 0.0f;


	// Use this for initialization
	void Start ()
	{
		prevPos = transform.position;

		playerTr = sharevr.GetPlayerTransform ();

		InitializeIK ();

		AvatarSayHi ();
	}

	void Update ()
	{
		UpdateAvatarOffset ();
	}

	void LateUpdate ()
	{
		if (!sharevr.GetSteamVRStatus ())
			EstimatePlayerVelocity ();

		UpdateAvatarPose ();
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

			anim.SetIKPosition (AvatarIKGoal.LeftHand, avatarHandAnchor [0].position);
			anim.SetIKPosition (AvatarIKGoal.RightHand, avatarHandAnchor [1].position);
			anim.SetIKRotation (AvatarIKGoal.LeftHand, avatarHandAnchor [0].rotation);
			anim.SetIKRotation (AvatarIKGoal.RightHand, avatarHandAnchor [1].rotation);
		}
	}

	// Purpose: Initialize Animator IK based on SteamVR status
	private void InitializeIK ()
	{
		anim = GetComponent <Animator> ();

		avatarHeadTr = anim.GetBoneTransform (HumanBodyBones.Head);
		avatarHandTr.Left = anim.GetBoneTransform (HumanBodyBones.LeftHand);
		avatarHandTr.Right = anim.GetBoneTransform (HumanBodyBones.RightHand);

		if (sharevr.GetSteamVRStatus ()) {
			// VR Mode
			EnableHandIK = true;
			EnableGazeIK = false;
		} else {
			// PC Mode
			EnableHandIK = false;
			EnableGazeIK = true;
		}
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
	private void UpdateAvatarPose ()
	{
		// Update Animator Controller Parameters
		anim.SetFloat ("MoveSpeed", velocity);

		// Get Player Rotation
		playerRot = playerTr.eulerAngles;

		// Update Avatar position and rotation
		transform.position = playerTr.position + avatarRefPos;
		transform.eulerAngles = new Vector3 (0.0f, playerRot.y, playerRot.z);

		// Update Head pitch angle only if gaze IK is not enabled
		if (!EnableGazeIK)
			avatarHeadTr.eulerAngles = new Vector3 (avatarHeadTr.eulerAngles.x, avatarHeadTr.eulerAngles.y, -playerRot.x - 90.0f);
	}

	public void AvatarSayHi ()
	{
		anim.SetTrigger ("SayHi");
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

}
