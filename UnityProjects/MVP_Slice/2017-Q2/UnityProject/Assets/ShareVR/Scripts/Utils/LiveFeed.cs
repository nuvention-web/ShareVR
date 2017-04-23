using UnityEngine;
using System;
using System.Collections;
using VRCapture;

namespace ShareVR.Core
{
	public class LiveFeed : MonoBehaviour
	{
		[HideInInspector]
		public RenderTexture livePlayRT;
		[HideInInspector]
		public float fps = 30.0f;

		[NonSerializedAttribute]
		public bool enableLiveFeed = false;


		private CameraController camCtrler;
		private RecordManager recManager;
		private Camera capCam;
		private Material liveFeedMaterial;

		private int rtHeight;
		private int rtWidth;
		bool camFound = false;

		public void InitializeReference ()
		{
			camCtrler = FindObjectOfType (typeof(CameraController)) as CameraController;
			recManager = FindObjectOfType (typeof(RecordManager)) as RecordManager;

			liveFeedMaterial = Resources.Load ("Materials/LiveFeedMaterial") as Material;

			switch (recManager.frameSize) {
			case VRCaptureVideo.FrameSizeType._640x480:
				rtHeight = 480;
				rtWidth = 640;
				break;
			case VRCaptureVideo.FrameSizeType._720x480:
				rtHeight = 480;
				rtWidth = 720;
				break;
			case VRCaptureVideo.FrameSizeType._960x540:
				rtHeight = 540;
				rtWidth = 960;
				break;
			case VRCaptureVideo.FrameSizeType._1280x720:
				rtHeight = 720;
				rtWidth = 1280;
				break;
			case VRCaptureVideo.FrameSizeType._1920x1080:
				rtHeight = 1080;
				rtWidth = 1920;
				break;
			case VRCaptureVideo.FrameSizeType._2048x1080:
				rtHeight = 1080;
				rtWidth = 2048;
				break;
			case VRCaptureVideo.FrameSizeType._3840x2160:
				rtHeight = 2160;
				rtWidth = 3840;
				break;
			case VRCaptureVideo.FrameSizeType._4096x2160:
				rtHeight = 2160;
				rtWidth = 4096;
				break;
			}

			// Create render texture
			livePlayRT = new RenderTexture (rtWidth, rtHeight, 24);
			liveFeedMaterial.mainTexture = livePlayRT;

			if (recManager.showDebugMessage)
				Debug.Log ("Live Feed Instantialized!!");
		}

		public void EnableLiveFeed (bool state)
		{
			enableLiveFeed = state;

			if (enableLiveFeed) {
				if (camCtrler == null)
					camCtrler = FindObjectOfType (typeof(CameraController)) as CameraController;
				else if (!camFound) {
					capCam = camCtrler.GetCaptureCamera ();
					capCam.targetTexture = livePlayRT;
					camFound = true;
				}
			}
		}

		public void TerminateRenderThread ()
		{
			enableLiveFeed = false;
		}

		public void ContinueRenderThread ()
		{
			enableLiveFeed = true;
			capCam.targetTexture = livePlayRT;
		}

		IEnumerator LiveFeedRenderThread ()
		{
			while (enableLiveFeed) {
				yield return new WaitForSeconds (1.0f / fps);

				if (camCtrler == null)
					camCtrler = FindObjectOfType (typeof(CameraController)) as CameraController;
				else if (!camFound) {
					capCam = camCtrler.GetCaptureCamera ();
					capCam.targetTexture = livePlayRT;
					camFound = true;
				}
			}
		}
	}
}