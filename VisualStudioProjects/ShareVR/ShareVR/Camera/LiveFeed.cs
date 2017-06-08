//======= Copyright (c) ShareVR ===========================
//
// Purpose: Display live preview of the spectator camera
// Version: 0.4b
// Chen Chen
// 4/30/2017
//=========================================================
using UnityEngine;
using System;
using System.Collections;
using ShareVR.Capture;

namespace ShareVR.Core
{
    internal class LiveFeed : MonoBehaviour
    {
        [HideInInspector]
        public RenderTexture livePlayRT;
        [HideInInspector]
        public float fps = 30.0f;

        [NonSerialized]
        public bool enableLiveFeed = false;

        private RecordManager recManager;
        private Camera capCam;
        private Material liveFeedMaterial;

        private int rtHeight;
        private int rtWidth;

        private Transform recStatus;
        private bool enableRecStatus = false;

        private void Start()
        {
            recStatus = transform.FindChild("RecStatus");
            if (recStatus != null) {
                //Debug.Log("Found RecStatus");
                recStatus.gameObject.SetActive(false);
            }
        }

        public void InitializeReference()
        {
            recManager = FindObjectOfType(typeof(RecordManager)) as RecordManager;
            liveFeedMaterial = Resources.Load("Materials/LiveFeedMaterial") as Material;

            switch (recManager.frameSize)
            {
                case FrameSizeType._640x480:
                    rtHeight = 480;
                    rtWidth = 640;
                    break;
                case FrameSizeType._720x480:
                    rtHeight = 480;
                    rtWidth = 720;
                    break;
                case FrameSizeType._960x540:
                    rtHeight = 540;
                    rtWidth = 960;
                    break;
                case FrameSizeType._1280x720:
                    rtHeight = 720;
                    rtWidth = 1280;
                    break;
                case FrameSizeType._1920x1080:
                    rtHeight = 1080;
                    rtWidth = 1920;
                    break;
                case FrameSizeType._2048x1080:
                    rtHeight = 1080;
                    rtWidth = 2048;
                    break;
                case FrameSizeType._3840x2160:
                    rtHeight = 2160;
                    rtWidth = 3840;
                    break;
                case FrameSizeType._4096x2160:
                    rtHeight = 2160;
                    rtWidth = 4096;
                    break;
            }

            // Create render texture
            livePlayRT = new RenderTexture(rtWidth, rtHeight, 24);
            liveFeedMaterial.mainTexture = livePlayRT;
            var vrcapv = FindObjectOfType (typeof(VRCaptureVideo)) as VRCaptureVideo;
            vrcapv.copyTargetRT = livePlayRT;

            if (recManager.showDebugMessage)
                Debug.Log("Live Feed Instantialized!!");
        }

        public void EnableLiveFeed( bool state )
        {
            enableLiveFeed = state;

            if (enableLiveFeed)
            {
                capCam = recManager.GetCaptureCamera();
                capCam.targetTexture = livePlayRT;

            }
        }

        public void TerminateRenderThread()
        {
            enableLiveFeed = false;
        }

        public void ContinueRenderThread()
        {
            enableLiveFeed = true;
            capCam.targetTexture = livePlayRT;
        }

        public void EnableRecStatus(bool state)
        {
            enableRecStatus = state;

            if (enableRecStatus)
                StartCoroutine(BlinkRecStatus());
            else
            {
                StopCoroutine(BlinkRecStatus());
                recStatus.gameObject.SetActive(false);
            }
        }

        IEnumerator LiveFeedRenderThread()
        {
            while (enableLiveFeed)
            {
                yield return new WaitForSeconds(1.0f / fps);

                capCam = recManager.GetCaptureCamera();
                capCam.targetTexture = livePlayRT;
            }
        }

        IEnumerator BlinkRecStatus()
        {
            while (enableRecStatus)
            {
                yield return new WaitForSeconds(0.6f);

                if(recStatus != null)
                {
                    recStatus.gameObject.SetActive(!recStatus.gameObject.activeInHierarchy);
                }
            }
            if (!enableRecStatus)
            {
                recStatus.gameObject.SetActive(false);
                yield break;
            }
        }
    }
}
