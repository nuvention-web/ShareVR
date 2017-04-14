//======= Copyright (c) NUVention TeamH ShareVR ===============
//
// Purpose: Controls ShareVR Control Panel and other UI
// Version: 1.2
// Date: 2/28/2017
// Revision History:  v1.2 - Chen - Created for v1.2 release
// 
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VRCapture;
using UnityEngine.Events;

public class ShareVRUIManager : MonoBehaviour
{
	public ShareVRManager sharevr;
	public Text recStatusText;
	public Text recDoneText;
	public VRCaptureVideo vrcv;
	public Button recStartButton;
	public Button recEndButton;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		UpdateFrameCountText ();
	}

	public void UpdateUI ()
	{
		if (sharevr.GetRecordingStatus ()) {
			// Update Recording Status Text Color and Text
			recStatusText.color = new Color (0, 1, 0);

			// Update UI Button Status
			recStartButton.gameObject.SetActive (false);
			recEndButton.gameObject.SetActive (true);
		} else {
			// Update Recording Status Text Color and Text
			recStatusText.color = new Color (1, 0, 0);
			recStatusText.text = "Recording Stopped";

			// Update UI Button Status
			recStartButton.gameObject.SetActive (true);
			recEndButton.gameObject.SetActive (false);
		}
	}


	public void ShowRecDoneText (bool status)
	{
		if (status)
			recDoneText.text = "Recording Complete! File saved at " + VRCaptureConfig.SaveFolder;
		else
			recDoneText.text = "";
	}

	private void UpdateFrameCountText ()
	{
		if (sharevr.GetRecordingStatus ())
			recStatusText.text = "Recording frame #" + vrcv.GetEncodedFrameCount ().ToString ();
	}
}
