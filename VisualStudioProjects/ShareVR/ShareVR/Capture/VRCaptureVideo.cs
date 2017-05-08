using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;

namespace ShareVR.Capture
{
    /// <summary>
    /// VRCapture video component.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class VRCaptureVideo : MonoBehaviour
    {
        /// <summary>
        /// Format type.
        /// </summary>
        public enum FormatType
        {
            /// <summary>
            /// Normal 2D video.
            /// </summary>
            NORMAL,
            /// <summary>
            /// Panorama video.
            /// </summary>
            PANORAMA
        }

        /// <summary>
        /// Panorama projection type.
        /// </summary>
        public enum PanoramaProjectionType
        {
            /// <summary>
            /// Cubemap format.
            /// https://docs.unity3d.com/Manual/class-Cubemap.html
            /// </summary>
            /// Saved cubemap video format:
            /// +------------------+------------------+------------------+
            /// |                  |                  |                  |
            /// |                  |                  |                  |
            /// |    +X (Right)    |    -X (Left)     |     +Y (Top)     |
            /// |                  |                  |                  |
            /// |                  |                  |                  |
            /// +------------------+------------------+------------------+
            /// |                  |                  |                  |
            /// |                  |                  |                  |
            /// |   +Y (Bottom)    |   +Z (Fromt)     |    -Z (Back)     |
            /// |                  |                  |                  |
            /// |                  |                  |                  |
            /// +------------------+------------------+------------------+
            ///
            CUBEMAP,
            /// <summary>
            /// Equirectangular format.
            /// https://en.wikipedia.org/wiki/Equirectangular_projection
            /// </summary>
            EQUIRECTANGULAR
        }

        /// <summary>
        /// Frame size type.
        /// </summary>
        public enum FrameSizeType
        {
            /// <summary>
            /// 480p (640 x 480) Standard Definition (SD).
            /// </summary>
            _640x480,
            /// <summary>
            /// 480p (720 x 480) Standard Definition (SD) (resolution of DVD video).
            /// </summary>
            _720x480,
            /// <summary>
            /// 540p (960 x 540).
            /// </summary>
            _960x540,
            /// <summary>
            /// 720p (1280 x 720) High Definition (HD).
            /// </summary>
            _1280x720,
            /// <summary>
            /// 1080p (1920 x 1080) Full High Definition (FHD).
            /// </summary>
            _1920x1080,
            /// <summary>
            /// 2K (2048 x 1080).
            /// </summary>
            _2048x1080,
            /// <summary>
            /// 4K (3840 x 2160) Quad Full High Definition (QFHD)
            /// (also known as UHDTV/UHD-1, resolution of Ultra High Definition TV).
            /// </summary>
            _3840x2160,
            /// <summary>
            /// 4K (4096 x 2160) Ultra High Definition (UHD).
            /// </summary>
            _4096x2160,
        }

        /// <summary>
        /// Cubemap size type.
        /// </summary>
        public enum CubemapSizeType
        {
            _512,
            _1024,
            _2048,
            _4096,
        }

        /// <summary>
        /// Encode quality type.
        /// </summary>
        public enum EncodeQualityType
        {
            /// <summary>
            /// Lower quality will decrease filesize on disk.
            /// Low = 1000 bitrate.
            /// </summary>
            Low,
            /// <summary>
            /// Medium = 2500 bitrate.
            /// </summary>
            Medium,
            /// <summary>
            /// High = 5000 bitrate.
            /// </summary>
            High,
        }

        /// <summary>
        /// Anti aliasing type.
        /// </summary>
        public enum AntiAliasingType
        {
            _1,
            _2,
            _4,
            _8,
        }

        /// <summary>
        /// Target framerate type.
        /// </summary>
        public enum TargetFramerateType
        {
            _18,
            _24,
            _30,
            _45,
            _60,
        }

        /// <summary>
        /// The type of the format.
        /// </summary>
        //[Tooltip ("Decide record normal or panorama video")]
        [HideInInspector]
        public FormatType formatType = FormatType.NORMAL;
        /// <summary>
        /// The size of the frame.
        /// </summary>
        [Tooltip ("Resolution of recorded video"), HideInInspector]
        public FrameSizeType frameSize = FrameSizeType._1280x720;
        /// <summary>
        /// The size of the cubemap.
        /// </summary>
        [Tooltip ("The cubemap size capture render to"), HideInInspector]
        public CubemapSizeType cubemapSize = CubemapSizeType._1024;
        /// <summary>
        /// The type of the projection.
        /// </summary>
        [Tooltip ("The panorama projection type"), HideInInspector]
        public PanoramaProjectionType projectionType = PanoramaProjectionType.CUBEMAP;
        /// <summary>
        /// The encode quality.
        /// </summary>
        [Tooltip ("Lower quality will decrease filesize on disk"), HideInInspector]
        public EncodeQualityType encodeQuality = EncodeQualityType.Medium;
        /// <summary>
        /// The anti aliasing.
        /// </summary>
        [Tooltip ("Anti aliasing setting for recorded video"), HideInInspector]
        public AntiAliasingType antiAliasing = AntiAliasingType._1;
        /// <summary>
        /// The target framerate.
        /// </summary>
        [Tooltip ("Target frameRate for recorded video"), HideInInspector]
        public TargetFramerateType targetFramerate = TargetFramerateType._30;

        /// <summary>
        /// Get the width of the frame.
        /// </summary>
        /// <value>The width of the frame.</value>
        public int FrameWidth
        {
            get
            {
                int width = 1280;
                if (frameSize == FrameSizeType._640x480)
                {
                    width = 640;
                }
                else if (frameSize == FrameSizeType._720x480)
                {
                    width = 720;
                }
                else if (frameSize == FrameSizeType._960x540)
                {
                    width = 960;
                }
                else if (frameSize == FrameSizeType._1280x720)
                {
                    width = 1280;
                }
                else if (frameSize == FrameSizeType._1920x1080)
                {
                    width = 1920;
                }
                else if (frameSize == FrameSizeType._2048x1080)
                {
                    width = 2048;
                }
                else if (frameSize == FrameSizeType._3840x2160)
                {
                    width = 3840;
                }
                else if (frameSize == FrameSizeType._4096x2160)
                {
                    width = 4096;
                }
                if (formatType == FormatType.PANORAMA)
                {
                    if (projectionType == PanoramaProjectionType.CUBEMAP)
                    {
                        width = CubemapSize * 3;
                    }
                }
                if (!isDedicated)
                {
                    // If frame size odd number, encode will stuck.
                    if (videoCamera.pixelWidth % 2 == 0)
                    {
                        width = videoCamera.pixelWidth;
                    }
                    else
                    {
                        width = videoCamera.pixelWidth - 1;
                    }
                }
                return width;
            }
        }

        /// <summary>
        /// Get the height of the frame.
        /// </summary>
        /// <value>The height of the frame.</value>
        public int FrameHeight
        {
            get
            {
                int height = 720;
                if (frameSize == FrameSizeType._640x480 || frameSize == FrameSizeType._720x480)
                {
                    height = 480;
                }
                else if (frameSize == FrameSizeType._960x540)
                {
                    height = 540;
                }
                else if (frameSize == FrameSizeType._1280x720)
                {
                    height = 720;
                }
                else if (frameSize == FrameSizeType._1920x1080 || frameSize == FrameSizeType._2048x1080)
                {
                    height = 1080;
                }
                else if (frameSize == FrameSizeType._3840x2160 || frameSize == FrameSizeType._4096x2160)
                {
                    height = 2160;
                }
                if (formatType == FormatType.PANORAMA)
                {
                    if (projectionType == PanoramaProjectionType.CUBEMAP)
                    {
                        height = CubemapSize * 2;
                    }
                }
                if (!isDedicated)
                {
                    // If frame size odd number, encode will stuck.
                    if (videoCamera.pixelHeight % 2 == 0)
                    {
                        height = videoCamera.pixelHeight;
                    }
                    else
                    {
                        height = videoCamera.pixelHeight - 1;
                    }
                }
                return height;
            }
        }

        /// <summary>
        /// Get the size of the cubemap.
        /// </summary>
        /// <value>The size of the cubemap.</value>
        public int CubemapSize
        {
            get
            {
                int size = 1024;
                if (cubemapSize == CubemapSizeType._512)
                {
                    size = 512;
                }
                else if (cubemapSize == CubemapSizeType._1024)
                {
                    size = 1024;
                }
                else if (cubemapSize == CubemapSizeType._2048)
                {
                    size = 2048;
                }
                else if (cubemapSize == CubemapSizeType._4096)
                {
                    size = 4096;
                }
                return size;
            }
        }

        /// <summary>
        /// Get the anti aliasing.
        /// </summary>
        /// <value>The anti aliasing.</value>
        public int AntiAliasing
        {
            get
            {
                int anti = 1;
                if (antiAliasing == AntiAliasingType._1)
                {
                    anti = 1;
                }
                else if (antiAliasing == AntiAliasingType._2)
                {
                    anti = 2;
                }
                else if (antiAliasing == AntiAliasingType._4)
                {
                    anti = 4;
                }
                else if (antiAliasing == AntiAliasingType._8)
                {
                    anti = 8;
                }
                return anti;
            }
        }

        /// <summary>
        /// Get the bitrate.
        /// </summary>
        /// <value>The bitrate.</value>
        public int Bitrate
        {
            get
            {
                int bitrate = 1000;
                if (encodeQuality == EncodeQualityType.Low)
                {
                    bitrate = 1000;
                }
                else if (encodeQuality == EncodeQualityType.Medium)
                {
                    bitrate = 2500;
                }
                else if (encodeQuality == EncodeQualityType.High)
                {
                    bitrate = 5000;
                }
                return bitrate;
            }
        }

        /// <summary>
        /// Get the target framerate.
        /// </summary>
        /// <value>The target framerate.</value>
        public int TargetFramerate
        {
            get
            {
                int framerate = 30;
                if (targetFramerate == TargetFramerateType._18)
                {
                    framerate = 18;
                }
                else if (targetFramerate == TargetFramerateType._24)
                {
                    framerate = 24;
                }
                else if (targetFramerate == TargetFramerateType._30)
                {
                    framerate = 30;
                }
                else if (targetFramerate == TargetFramerateType._45)
                {
                    framerate = 45;
                }
                else if (targetFramerate == TargetFramerateType._60)
                {
                    framerate = 60;
                }
                return framerate;
            }
        }

        /// <summary>
        /// To be notified when the video is complete, register a delegate
        /// using this signature by calling VideoCaptureCompleteDelegate.
        /// </summary>
        public delegate void VideoCaptureCompleteDelegate();

        /// <summary>
        /// The video capturing complete delegate variable.
        /// </summary>
        VideoCaptureCompleteDelegate videoCaptureCompleteDelegate;

        /// <summary>
        /// Register a delegate to be invoked when the video is complete.
        /// </summary>
        /// <param name='del'>
        /// The delegate to be invoked when complete.
        /// </param>
        public void RegisterCaptureCompleteDelegate( VideoCaptureCompleteDelegate del )
        {
            videoCaptureCompleteDelegate += del;
        }

        /// <summary>
        /// Specifies whether or not the camera being used to capture video is
        /// dedicated solely to video capture. When a dedicated camera is used,
        /// the camera's aspect ratio will automatically be set to the specified
        /// frame size.
        /// If a non-dedicated camera is specified it is assumed the camera will
        /// also be used to render to the screen, and so the camera's aspect
        /// ratio will not be adjusted.
        /// Use a dedicated camera to capture video at resolutions that have a
        /// different aspect ratio than the device screen.
        /// </summary>
        [HideInInspector]
        public bool isDedicated = true;
        /// <summary>
        /// The RTMP server address.
        /// </summary>
        [HideInInspector]
        public string rtmpServerAddress = null;
        /// <summary>
        /// Setup Time.maximumDeltaTime to avoiding nasty stuttering.
        /// https://docs.unity3d.com/ScriptReference/Time-maximumDeltaTime.html
        /// </summary>
        [HideInInspector]
        public bool offlineRender = false;
        /// <summary>
        /// For generate equirectangular video.
        /// </summary>
        [HideInInspector]
        public Material transformMaterial;
        /// <summary>
        /// The original maximum delta time.
        /// </summary>
        [HideInInspector]
        float originalMaximumDeltaTime;
        /// <summary>
        /// The original color space.
        /// </summary>
        ColorSpace originalColorSpace;

        /// <summary>
        /// Whether this camera is enabled for capture.
        /// </summary>
        [HideInInspector]
        public bool isEnabled = true;

        /// <summary>
        /// The index of the camera.
        /// </summary>
        public int Index
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the video capture process failed.
        /// </summary>
        /// <value><c>true</c> if failed; otherwise, <c>false</c>.</value>
        public bool Failed
        {
            get;
            private set;
        }

        /// <summary>
        /// The camera that resides on the same game object as this script.
        /// It will be used for capturing video.
        /// </summary>
        Camera videoCamera;
        /// <summary>
        /// The texture holding the video frame data.
        /// </summary>
        Texture2D frameTexture;
        RenderTexture frameRenderTexture;
        Cubemap frameCubemap;

        /// <summary>
        /// Whether or not capturing from this camera is currently in progress.
        /// </summary>
        bool isCapturing;
        /// <summary>
        /// Whether or not there is a frame capturing now.
        /// </summary>
        bool isCapturingFrame;
        /// <summary>
        /// The time spent during capturing.
        /// </summary>
        float capturingTime;
        /// <summary>
        /// The delta time of each frame.
        /// </summary>
        float deltaFrameTime;
        /// <summary>
        /// Frame statistics.
        /// </summary>
        int capturedFrameCount;
        int encodedFrameCount;
        /// <summary>
        /// Reference to native lib API.
        /// </summary>
        System.IntPtr libAPI;

        /// <summary>
        /// Frame data will be sent to frame encode queue.
        /// </summary>
        struct FrameData
        {
            /// <summary>
            /// The rgb pixels will be encoded.
            /// </summary>a
            public byte[] pixels;
            /// <summary>
            /// How many this frame will be counted.
            /// </summary>
            public int count;

            /// <summary>
            /// Constructor.
            /// </summary>
            public FrameData( byte[] p, int c )
            {
                pixels = p;
                count = c;
            }
        }

        /// <summary>
        /// The frame encode queue.
        /// </summary>
        Queue<FrameData> frameQueue;
        Object frameQueueLock;
        /// <summary>
        /// The frame encode thread.
        /// </summary>
        Thread encodeThread;

        /// <summary>
        /// If capture video still processing.
        /// </summary>
        /// <returns><c>true</c>, if processing was ised, <c>false</c> otherwise.</returns>
        public bool IsProcessing()
        {
            return isCapturing ||
            ( frameQueue != null && frameQueue.Count > 0 ) ||
            ( encodeThread != null && encodeThread.IsAlive );
        }

        /// <summary>
        /// Get/Set the destination path.
        /// </summary>
        /// <value>The destination path.</value>
        public string DestinationPath
        {
            get
            {
                if (destinationPath != null)
                    return destinationPath;
                else
                {
                    destinationPath = VRCaptureUtils.SaveFolder + VRCommonUtils.GetMp4FileName(Index.ToString());
                }
                return destinationPath;
            }
            set
            {
                destinationPath = value;
            }
        }

        public void UpdateDestinationPath()
        {
            destinationPath = VRCaptureUtils.SaveFolder + VRCommonUtils.GetMp4FileName(Index.ToString());
        }

        /// <summary>
        /// Destination the captured video will pushed to.
        /// </summary>
        string destinationPath = null;

        public RenderTexture copyTargetRT;
        public bool showDebugMessage = false;

        void OnRenderImage( RenderTexture src, RenderTexture dest )
        {
            // Passthrough
            Graphics.Blit(src, dest);

            // Copy rendertexture for live play rendertexture
            Graphics.Blit(null, copyTargetRT);
        }

        /// <summary>
        /// Cleanup this instance.
        /// </summary>
        public void Cleanup()
        {
            if (!isEnabled)
            {
                return;
            }
            frameTexture = null;
            frameRenderTexture = null;
            frameCubemap = null;
            frameQueue = null;
            frameQueueLock = null;
            videoCaptureCompleteDelegate = null;
            capturedFrameCount = 0;
            encodedFrameCount = 0;

            LibVideoCaptureAPI_Clean(libAPI);
        }

        /// <summary>
        /// Start capture video.
        /// </summary>
        public void StartCapture()
        {
            // Check if we can start capture.
            if (!isEnabled)
            {
                return;
            }
            if (IsProcessing())
            {
                Debug.LogWarning("VRCaptureVideo: capture still processing!");
                return;
            }
            if (formatType == FormatType.PANORAMA && !isDedicated)
            {
                Debug.LogWarning(
                    "VRCaptureVideo: capture equirectangular video " +
                    "require dedicated camera!"
                );
                return;
            }
            // Create a RenderTexture with desired frame size to store pixels in GPU.
            // Use camera.targetTexture as RenderTexture if already existed.
            if (isDedicated)
            {
                // Prepare for dedicated camera capture.
                if (videoCamera.targetTexture != null)
                {
                    // Use binded rendertexture will ignore antiAliasing config.
                    frameRenderTexture = videoCamera.targetTexture;
                }
                else
                {
                    // Create a rendertexture for video capture.
                    // Size it according to the desired video frame size.
                    frameRenderTexture = new RenderTexture(FrameWidth, FrameHeight, 24);
                    frameRenderTexture.antiAliasing = AntiAliasing;
                    frameRenderTexture.wrapMode = TextureWrapMode.Clamp;
                    frameRenderTexture.filterMode = FilterMode.Trilinear;
                    frameRenderTexture.anisoLevel = 0;
                    frameRenderTexture.hideFlags = HideFlags.HideAndDontSave;
                    // Make sure the rendertexture is created.
                    frameRenderTexture.Create();
                    videoCamera.targetTexture = frameRenderTexture;
                }
            }
            // For capturing normal 2D video, use frameTexture(Texture2D) for intermediate cpu saving,
            // frameRenderTexture(RenderTexture) store the pixels read by frameTexture.
            if (formatType == FormatType.NORMAL)
            {
                if (isDedicated)
                {
                    // Set the aspect ratio of the camera to match the rendertexture.
                    videoCamera.aspect = FrameWidth / ( ( float ) FrameHeight );
                    videoCamera.targetTexture = frameRenderTexture;
                }
            }
            // For capture panorama video:
            // EQUIRECTANGULAR: use frameCubemap(Cubemap) for intermediate cpu saving.
            // CUBEMAP: use frameTexture(Texture2D) for intermediate cpu saving.
            // TODO, panorama capture current always use dedicated camera, improve to use only one camera.
            if (formatType == FormatType.PANORAMA)
            {
                // Create render cubemap.
                frameCubemap = new Cubemap(CubemapSize, TextureFormat.RGB24, false);
                // Setup camera as required for panorama capture.
                videoCamera.aspect = 1.0f;
                videoCamera.fieldOfView = 90;
            }
            // Pixels stored in frameRenderTexture(RenderTexture) always read by frameTexture(Texture2D).
            // NORMAL: camera render -> frameRenderTexture -> frameTexture -> frameQueue
            // CUBEMAP: 6 cameras render -> 6 faceRenderTexture -> frameTexture -> frameQueue
            // EQUIRECTANGULAR: 6 camera render -> 6 faceRenderTexture-> frameCubemap ->
            //                  Cubemap2Equirect -> frameRenderTexture -> frameTexture -> frameQueue
            frameTexture = new Texture2D(FrameWidth, FrameHeight, TextureFormat.RGB24, false);
            frameTexture.hideFlags = HideFlags.HideAndDontSave;
            frameTexture.wrapMode = TextureWrapMode.Clamp;
            frameTexture.filterMode = FilterMode.Trilinear;
            frameTexture.hideFlags = HideFlags.HideAndDontSave;
            frameTexture.anisoLevel = 0;

            deltaFrameTime = 1f / TargetFramerate;
            capturingTime = 0f;
            frameQueue = new Queue<FrameData>();
            frameQueueLock = new Object();
            // TODO, merge streaming and capture native api.
            UpdateDestinationPath();
            libAPI = LibVideoCaptureAPI_Get(FrameWidth, FrameHeight, TargetFramerate, DestinationPath, VRCaptureUtils.FFmpegPath);

            if (libAPI == System.IntPtr.Zero)
            {
                Debug.LogWarning("VRCaptureVideo: get native capture api failed!");
                return;
            }
            if (offlineRender)
            {
                // Backup maximumDeltaTime states.
                originalMaximumDeltaTime = Time.maximumDeltaTime;
                Time.maximumDeltaTime = Time.fixedDeltaTime;
            }
            isCapturing = true;
            // Start encoding thread.
            encodeThread = new Thread(FrameEncodeThreadFunction);
            encodeThread.Priority = System.Threading.ThreadPriority.Lowest;
            encodeThread.IsBackground = true;
            encodeThread.Start();
        }

        /// <summary>
        /// Finish capture video.
        /// </summary>
        public void FinishCapture()
        {
            if (!isEnabled)
            {
                return;
            }
            if (!isCapturing)
            {
                Debug.LogWarning("VRCaptureVideo: capture not start yet!");
            }
            if (offlineRender)
            {
                // Restore maximumDeltaTime states.
                Time.maximumDeltaTime = originalMaximumDeltaTime;
            }
            isCapturing = false;
        }

        /// <summary>
        /// Called before any Start functions and also just after a prefab is instantiated.
        /// </summary>
        void Awake()
        {
            videoCamera = GetComponent<Camera>();
        }

        /// <summary>
        /// Called after a camera finishes rendering the scene.
        /// </summary>
        void OnPostRender()
        {
            // NORMAL run in OnPostRender.
            if (formatType != FormatType.NORMAL)
            {
                return;
            }
            if (isCapturing)
            {
                capturingTime += Time.deltaTime;
            }
            if (!isCapturingFrame && isCapturing)
            {
                int totalRequiredFrameCount =
                    (int)(capturingTime / deltaFrameTime);
                // Skip frames if we already got enough.
                if (totalRequiredFrameCount > capturedFrameCount)
                {
                    StartCoroutine(CaptureFrameAsync());
                }
            }
        }

        /// <summary>
        /// Called once per frame, after Update has finished.
        /// </summary>
        void LateUpdate()
        {
            // EQUIRECTANGULAR run in LateUpdate.
            if (formatType != FormatType.PANORAMA)
            {
                return;
            }
            if (isCapturing)
            {
                capturingTime += Time.deltaTime;
            }
            if (!isCapturingFrame && isCapturing)
            {
                int totalRequiredFrameCount =
                    (int)(capturingTime / deltaFrameTime);
                // Skip frames if we already got enough.
                if (totalRequiredFrameCount > capturedFrameCount)
                {
                    CaptureCubemapFrameSync();
                }
            }
        }

        /// <summary>
        ///  Capture frame async impl.
        /// </summary>
        IEnumerator CaptureFrameAsync()
        {
            isCapturingFrame = true;
            if (isCapturing)
            {
                CopyFrameTexture();
            }
            // Chen: Wait for one frame to avoid pipline stall
            yield return null;

            // User may terminate the capture process during capturing frame.
            if (isCapturing)
            {
                EnqueueFrameTexture();
            }
            isCapturingFrame = false;
        }

        /// <summary>
        /// Capture frame sync impl.
        /// </summary>
        void CaptureCubemapFrameSync()
        {
            int width = CubemapSize;
            int height = CubemapSize;

            CubemapFace[] faces = new CubemapFace[] {
                CubemapFace.PositiveX,
                CubemapFace.NegativeX,
                CubemapFace.PositiveY,
                CubemapFace.NegativeY,
                CubemapFace.PositiveZ,
                CubemapFace.NegativeZ
            };
            Vector3[] faceAngles = new Vector3[] {
                new Vector3 (0.0f, 90.0f, 0.0f),
                new Vector3 (0.0f, -90.0f, 0.0f),
                new Vector3 (-90.0f, 0.0f, 0.0f),
                new Vector3 (90.0f, 0.0f, 0.0f),
                new Vector3 (0.0f, 0.0f, 0.0f),
                new Vector3 (0.0f, 180.0f, 0.0f)
            };

            // Reset capture camera rotation.
            videoCamera.transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);

            // Create cubemap face render texture.
            RenderTexture faceTexture = new RenderTexture (width, height, 24);
            faceTexture.antiAliasing = AntiAliasing;
#if !( UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 )
            faceTexture.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
#endif
            faceTexture.hideFlags = HideFlags.HideAndDontSave;
            // For intermediate saving
            Texture2D swapTexture = new Texture2D (width, height, TextureFormat.RGB24, false);
            swapTexture.hideFlags = HideFlags.HideAndDontSave;
            // Prepare for target render texture.
            videoCamera.targetTexture = faceTexture;

            // TODO, make this into shader for GPU fast processing.
            if (projectionType == PanoramaProjectionType.CUBEMAP)
            {
                for (int i = 0; i < faces.Length; i++)
                {
                    videoCamera.transform.eulerAngles = faceAngles[i];
                    videoCamera.Render();
                    RenderTexture.active = faceTexture;
                    swapTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                    Color[] pixels = swapTexture.GetPixels ();
                    switch (i)
                    {
                        case ( int ) CubemapFace.PositiveX:
                            frameTexture.SetPixels(0, height, width, height, pixels);
                            break;
                        case ( int ) CubemapFace.NegativeX:
                            frameTexture.SetPixels(width, height, width, height, pixels);
                            break;
                        case ( int ) CubemapFace.PositiveY:
                            frameTexture.SetPixels(width * 2, height, width, height, pixels);
                            break;
                        case ( int ) CubemapFace.NegativeY:
                            frameTexture.SetPixels(0, 0, width, height, pixels);
                            break;
                        case ( int ) CubemapFace.PositiveZ:
                            frameTexture.SetPixels(width, 0, width, height, pixels);
                            break;
                        case ( int ) CubemapFace.NegativeZ:
                            frameTexture.SetPixels(width * 2, 0, width, height, pixels);
                            break;
                    }
                }
                frameTexture.Apply();
            }
            else if (projectionType == PanoramaProjectionType.EQUIRECTANGULAR)
            {
                Color[] mirroredPixels = new Color[swapTexture.height * swapTexture.width];
                for (int i = 0; i < faces.Length; i++)
                {
                    videoCamera.transform.eulerAngles = faceAngles[i];
                    videoCamera.Render();
                    RenderTexture.active = faceTexture;
                    swapTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                    // Mirror vertically to meet the standard of unity cubemap.
                    Color[] OrignalPixels = swapTexture.GetPixels ();
                    for (int y1 = 0; y1 < height; y1++)
                    {
                        for (int x1 = 0; x1 < width; x1++)
                        {
                            mirroredPixels[y1 * width + x1] = OrignalPixels[( ( height - 1 - y1 ) * width ) + x1];
                        }
                    }
                    frameCubemap.SetPixels(mirroredPixels, faces[i]);
                }
                frameCubemap.SmoothEdges();
                frameCubemap.Apply();
                // Convert to equirectangular projection.
                Graphics.Blit(frameCubemap, frameRenderTexture, transformMaterial);
                // From frameRenderTexture to frameTexture.
                CopyFrameTexture();
            }

            RenderTexture.active = null;
            videoCamera.targetTexture = null;

            // Clean temp texture.
            DestroyImmediate(swapTexture);
            DestroyImmediate(faceTexture);

            // Send for encoding.
            EnqueueFrameTexture();
        }

        /// <summary>
        /// Copy the frame texture from GPU to CPU.
        /// </summary>
        void CopyFrameTexture()
        {
            // Bind texture.
            RenderTexture.active = frameRenderTexture;
            // TODO, remove expensive step of copying pixel data from GPU to CPU.
            frameTexture.ReadPixels(new Rect(0, 0, FrameWidth, FrameHeight), 0, 0, false);
            //frameTexture.Apply ();
            // Restore RenderTexture states.
            RenderTexture.active = null;
        }

        /// <summary>
        /// Send the captured frame texture to encode queue.
        /// </summary>
        void EnqueueFrameTexture()
        {
            int totalRequiredFrameCount = (int)(capturingTime / deltaFrameTime);
            int requiredFrameCount = totalRequiredFrameCount - capturedFrameCount;
            lock (frameQueueLock)
            {
                frameQueue.Enqueue(new FrameData(frameTexture.GetRawTextureData(), requiredFrameCount));
            }
            capturedFrameCount = totalRequiredFrameCount;
        }

        /// <summary>
        /// Frame encoding thread impl.
        /// </summary>
        void FrameEncodeThreadFunction()
        {
            while (isCapturing || frameQueue.Count > 0)
            {
                if (frameQueue.Count > 0)
                {
                    FrameData frame;
                    lock (frameQueueLock)
                    {
                        frame = frameQueue.Dequeue();
                    }

                    LibVideoCaptureAPI_SendFrames(libAPI, frame.pixels, frame.count);
                    encodedFrameCount++;
                    if (showDebugMessage)
                    {
                        Debug.Log(
                            "ShareVR: Encoded " +
                            encodedFrameCount + " frames. " +
                            frameQueue.Count + " frames remaining."
                        );
                    }
                }
                else
                {
                    Thread.Sleep(1);
                }
            }

            if (showDebugMessage)
            {
                Debug.Log("ShareVR: Video created, file located at: " + DestinationPath.Replace('/', '\\'));
            }
            // Notify native encoding process finish.
            LibVideoCaptureAPI_Close(libAPI);

            // Notify caller video capture complete.
            if (videoCaptureCompleteDelegate != null)
            {
                videoCaptureCompleteDelegate();
            }
        }

        /// <summary>
        /// Save render texture to PNG image file.
        /// </summary>
        /// <param name="rtex">Rtex.</param>
        /// <param name="fileName">File name.</param>
        void RenderTextureToPNG( RenderTexture rtex, string fileName )
        {
            Texture2D tex = new Texture2D (rtex.width, rtex.height, TextureFormat.RGB24, false);
            RenderTexture.active = rtex;
            tex.ReadPixels(new Rect(0, 0, rtex.width, rtex.height), 0, 0, false);
            RenderTexture.active = null;
            TextureToPNG(tex, fileName);
        }

        /// <summary>
        /// Save texture to PNG image file.
        /// </summary>
        /// <param name="tex">Tex.</param>
        /// <param name="fileName">File name.</param>
        void TextureToPNG( Texture2D tex, string fileName )
        {
            string filePath = VRCaptureUtils.SaveFolder + fileName;
            byte[] imageBytes = tex.EncodeToPNG ();
            System.IO.File.WriteAllBytes(filePath, imageBytes);
        }

        [DllImport("VRCaptureLib")]
        static extern System.IntPtr LibVideoCaptureAPI_Get( int width, int height, int rate, string path, string ffpath );

        [DllImport("VRCaptureLib")]
        static extern void LibVideoCaptureAPI_SendFrames( System.IntPtr api, byte[] data, int count );

        [DllImport("VRCaptureLib")]
        static extern void LibVideoCaptureAPI_Close( System.IntPtr api );

        [DllImport("VRCaptureLib")]
        static extern void LibVideoCaptureAPI_Clean( System.IntPtr api );

    }
}
