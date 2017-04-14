//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Controls all the capture events and methods from the spectator camera
// Version: 1.2
// Date: 2/28/2017
// Revision History:  v1.0 - Chen - Created class for MVP Slice
//                    v1.2 - Chen - Merged with CameraFollow
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
	public Camera targetCam;
	public Camera capCam;
	Transform capCamTr;
	public Material camStatusLight;
	public ShareVRManager sharevr;

	private bool isCapturing = true;

	// Smooth LookAt function related variables and parameters
	private float damp = 2.0f;
	private float camDist = 4.0f;
	private Vector3 targetDirection = new Vector3 (-0.9f, 0.5f, 1.1f);
	private Vector3 camPos;
	private Quaternion camRot;
	private Transform playerTransform;

	// Use this for initialization
	void Start ()
	{
		playerTransform = sharevr.GetPlayerTransform ();
		capCamTr = capCam.transform;
	}

	void LateUpdate ()
	{
		if (isCapturing) {
			SmoothLookAt (playerTransform);
			capCamTr.localPosition = targetCam.transform.localPosition;
			capCamTr.localRotation = targetCam.transform.localRotation;

			capCam.depth = 1.0f;
			capCam.fieldOfView = 60.0f;
			capCam.farClipPlane = 500.0f;
			capCam.near = 0.3f;
		}
	}

	public void StartCapture ()
	{
		camStatusLight.color = new Color (0, 1, 0);
		if (!isCapturing) {
			StartCoroutine (CamRenderToTexture (targetCam));
			isCapturing = true;
		}
	}

	public void StopCapture ()
	{
		camStatusLight.color = new Color (1, 0, 0);
		if (isCapturing) {
			StopCoroutine (CamRenderToTexture (targetCam));
			isCapturing = false;
		}
	}

	// Purpose: Get current spectator Capture status
	public bool GetCaptureStatus ()
	{
		return isCapturing;
	}

	private IEnumerator CamRenderToTexture (Camera cam)
	{
		yield return new WaitForSeconds (1.0f);
		cam.Render ();
		yield return new WaitForSeconds (1.0f);
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
