//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Controls all the capture events and methods from the spectator camera
// Version: 1.0
// Date: 2/20/2017
// Revision History:  v1.0 - Chen Chen - Created class for MVP Slice
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCapture : MonoBehaviour
{
	public Camera targetCam;
	public RenderTexture rtex;

	CameraFollow camFollow;
	bool isRendering = false;

	// Use this for initialization
	void Start ()
	{
		camFollow = gameObject.GetComponent <CameraFollow> ();
	}

	void Update ()
	{
		if (camFollow.GetCamStatus () && !isRendering) {
			StartCoroutine (CamRenderToTexture (targetCam));
			isRendering = true;
		} else if (!camFollow.GetCamStatus ()) {
			StopCoroutine (CamRenderToTexture (targetCam));
			isRendering = false;
		}

	}

	private IEnumerator CamRenderToTexture (Camera cam)
	{
		cam.targetTexture = rtex;
		cam.Render ();
		yield return new WaitForSeconds (1.0f);
	}
}
