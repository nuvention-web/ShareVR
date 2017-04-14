//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Controls the third person spectator camera
// Version: 1.1
// Date: 2/20/2017
// Revision History:  v1.0 - Chen Chen - Created class for MVP Slice
//                    v1.1 - Chen Chen - Created UpdateCamStatusText method
//                                       to show number of frames recorded
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRCapture;

public class CameraFollow : MonoBehaviour
{
	// Game object reference
	public Material camStatus;
	public GameObject vrGameObj;
	public GameObject pcGameObj;
	public Text recStatusText;
	public VRCaptureVideo vrcv;
	public Button recStartButton;
	public Button recEndButton;

	// Intermediate Variables
	protected GameObject activeGameObj;
	protected bool camEnabled = true;
	protected Transform objToTrack;

	// Smooth LookAt function related variables and parameters
	private float damp = 2.0f;
	private float camDist = 4.0f;
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
		if (camEnabled) {
			SmoothLookAt (objToTrack);
			UpdateCamStatusText ();
		}
	}

	public void SetCamStatus (bool status)
	{
		camEnabled = status;
		if (camEnabled) {
			camStatus.color = new Color (0, 1, 0);
			recStatusText.color = new Color (0, 1, 0);
		} else {
			camStatus.color = new Color (1, 0, 0);
			recStatusText.color = new Color (1, 0, 0);
			recStatusText.text = "Recording Stopped";
		}
		UpdateButtonStatus ();
	}

	// Purpose: Update camera status text with the number of frames recorded
	private void UpdateCamStatusText ()
	{
		if (vrcv.GetCaptureStatus ()) {
			recStatusText.text = "Recording frame #" + vrcv.GetEncodedFrameCount ().ToString ();
		}
	}

	private void UpdateButtonStatus ()
	{
		if (camEnabled) {
			recStartButton.gameObject.SetActive (false);
			recEndButton.gameObject.SetActive (true);
		} else {
			recStartButton.gameObject.SetActive (true);
			recEndButton.gameObject.SetActive (false);
		}
	}
	// Purpose: Get current spectator recording status
	public bool GetCamStatus ()
	{
		return camEnabled;
	}

	// Purpose: Get current available game object. Will return vrGameObj if in VR mode and pcGameObj if in PC mode.
	public GameObject GetActiveGameObj ()
	{
		return activeGameObj;
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
