//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Controls the third person spectator camera
// Version: 1.0
// Date: 2/20/2017
// Revision History:  v1.0 - Chen Chen - Created class for MVP Slice
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Material camStatus;
	public GameObject vrGameObj;
	public GameObject pcGameObj;
	protected GameObject activeGameObj;
	protected bool camEnabled = true;
	protected Transform objToTrack;

	//-----------------------------------
	// Smooth LookAt function related variables and parameters
	private float damp = 2.0f;
	private float camDist = 3.0f;
	private Vector3 targetDirection = new Vector3 (-0.9f, 0.5f, 1.1f);
	private Vector3 camPos;
	private Quaternion camRot;
	private GameObject targetGobj;

	void Start ()
	{
		// Handle SteamVR detection to provide support for VR/non-VR environment
		if (vrGameObj.activeInHierarchy) {
			// If player is using SteamVR with a supported HMD
			Debug.Log ("SteamVR HMD detected, using VR mode");
			objToTrack = vrGameObj.transform;
			activeGameObj = vrGameObj;
		} else {
			// If player is using PC without SteamVR
			Debug.Log ("SteamVR HMD not detected, using PC mode");
			objToTrack = pcGameObj.transform;
			activeGameObj = pcGameObj;
		}
	}

	void LateUpdate ()
	{
		if (camEnabled)
			SmoothLookAt (objToTrack);
	}

	public void SetCamStatus (bool status)
	{
		camEnabled = status;
		if (camEnabled)
			camStatus.color = new Color (0, 1, 0);
		else
			camStatus.color = new Color (1, 0, 0);
	}

	public bool GetCamStatus ()
	{
		return camEnabled;
	}

	public GameObject GetActiveGameObj ()
	{
		return activeGameObj;
	}

	// Smoothly move and rotate the camera so that it will always follow and look at player
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
