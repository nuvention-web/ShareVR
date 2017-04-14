//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Controls high-level ShareVR events and status
// Version: 1.2
// Date: 2/28/2017
// Revision History:  v1.2 - Chen - Created for version 1.2 release
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRCapture;
using UnityEngine.EventSystems;

public class ShareVRManager : MonoBehaviour
{
	// Game object reference
	public VRCapture.VRCapture vrc;
	public CameraCapture camcap;
	public ShareVRUIManager sharevrui;
	public GameObject vrGameObj;
	public GameObject pcGameObj;
	public ShareVRAvatarManager avatarVR;
	public ShareVRAvatarManager avatarPC;

	// Intermediate Variables

	protected GameObject activeGameObj;
	protected ShareVRAvatarManager activeAvatar;
	protected bool isRecording = false;
	protected bool steamVRChecked = false;
	protected bool isUsingSteamVR = false;
	protected Transform objToTrack;

	// Initialization
	void Start ()
	{
		if (!steamVRChecked)
			CheckSteamVR ();

		camcap.StartCapture ();
	}

	public void StartRecording ()
	{
		if (!isRecording) {
			//camcap.StartCapture ();
			vrc.BeginCaptureSession ();
			isRecording = true;
		}

		sharevrui.UpdateUI ();
		sharevrui.ShowRecDoneText (false);

		activeAvatar.AvatarSayHi ();
	}

	public void StopRecording ()
	{
		if (isRecording) {
			//camcap.StopCapture ();
			vrc.EndCaptureSession ();
			isRecording = false;
		}

		sharevrui.UpdateUI ();

		sharevrui.ShowRecDoneText (true);
	}

	public void EnableLiveFeed (bool status)
	{
		if (status)
			camcap.StartCapture ();
		else
			camcap.StopCapture ();
	}

	// Purpose: Get current available game object. Will return vrGameObj if in VR mode and pcGameObj if in PC mode.
	public GameObject GetPlayerGameObj ()
	{
		if (!steamVRChecked)
			CheckSteamVR ();
		return activeGameObj;
	}

	public Transform GetPlayerTransform ()
	{
		if (!steamVRChecked)
			CheckSteamVR ();
		return activeGameObj.transform;
	}

	// Purpose: Get current spectator recording status
	public bool GetRecordingStatus ()
	{
		return isRecording;
	}

	public bool GetSteamVRStatus ()
	{
		if (!steamVRChecked)
			CheckSteamVR ();	
		return isUsingSteamVR;
	}

	private void CheckSteamVR ()
	{
		// Handle SteamVR detection to provide support for VR/non-VR environment
		if (vrGameObj.activeInHierarchy) {
			// If player is using SteamVR with a supported HMD
			Debug.Log ("SteamVR HMD detected, using VR mode");
			activeGameObj = vrGameObj;
			activeAvatar = avatarVR;
			isUsingSteamVR = true;
		} else {
			// If player is using PC without SteamVR
			Debug.Log ("SteamVR HMD not detected, using PC mode");
			activeGameObj = pcGameObj;
			activeAvatar = avatarPC;
			isUsingSteamVR = false;
		}

		steamVRChecked = true;
	}
}

