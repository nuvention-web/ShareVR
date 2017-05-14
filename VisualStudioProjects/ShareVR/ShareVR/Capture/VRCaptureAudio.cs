using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

namespace ShareVR.Capture
{
    public class VRCaptureAudio : MonoBehaviour
    {
        /// <summary>
        /// Whether this audio listiner is enabled for capture.
        /// </summary>
        public bool isEnabled = true;
        /// <summary>
        /// Whether or not capturing from this audio listener is currently in progress.
        /// </summary>
        bool isCapturing;
        /// <summary>
        /// Reference to native lib API.
        /// </summary>
        System.IntPtr libAPI;
        /// <summary>
        /// The audio capture prepare.
        /// </summary>
        System.IntPtr audioPointer;
        System.Byte[] audioByteBuffer;

        //AudioListener audioListener;

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
                destinationPath = VRCaptureUtils.SaveFolder + VRCommonUtils.GetWavFileName();
                return destinationPath;
            }
            set
            {
                destinationPath = value;
            }
        }

        /// <summary>
        /// Destination the captured audio will pushed to.
        /// </summary>
        string destinationPath = null;

        /// <summary>
        /// To be notified when the audio is complete, register a delegate 
        /// using this signature by calling RegisterSessionCompleteDelegate.
        /// </summary>
        public delegate void AudioCaptureCompleteDelegate();

        /// <summary>
        /// The audio capturing complete delegate variable.
        /// </summary>
        AudioCaptureCompleteDelegate audioCaptureCompleteDelegate;

        /// <summary>
        /// Register a delegate to be invoked when the audio is complete.
        /// </summary>
        /// <param name='del'>
        /// The delegate to be invoked when complete.
        /// </param>
        public void RegisterCaptureCompleteDelegate( AudioCaptureCompleteDelegate del )
        {
            audioCaptureCompleteDelegate += del;
        }

        /// <summary>
        /// If capture audio still processing.
        /// </summary>
        /// <returns><c>true</c>, if processing was ised, <c>false</c> otherwise.</returns>
        public bool IsProcessing()
        {
            return isCapturing;
        }

        /// <summary>
        /// Cleanup this instance.
        /// </summary>
        public void Cleanup()
        {
            audioCaptureCompleteDelegate = null;
            if (File.Exists(DestinationPath))
            {
                File.Delete(DestinationPath);
            }
            LibAudioCaptureAPI_Clean(libAPI);
        }

        /// <summary>
        /// Start capture audio.
        /// </summary>
        public void StartCapture()
        {
            if (!isEnabled)
            {
                return;
            }
            if (IsProcessing())
            {
                Debug.LogWarning("VRCaptureAudio: capture still processing!");
                return;
            }
            libAPI = LibAudioCaptureAPI_Get(
                AudioSettings.outputSampleRate,
                DestinationPath,
                VRCaptureUtils.FFmpegPath);
            if (libAPI == System.IntPtr.Zero)
            {
                Debug.LogWarning("VRCaptureAudio: get native LibAudioCaptureAPI failed!");
                return;
            }
            InitCapture();
            isCapturing = true;
        }

        /// <summary>
        /// Finish capture audio.
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
            isCapturing = false;
            LibAudioCaptureAPI_Close(libAPI);

            // Notif caller audio capture complete.
            if (audioCaptureCompleteDelegate != null)
            {
                audioCaptureCompleteDelegate();
            }

            if (VRCapture.Instance.ShowDebug())
            {
                Debug.Log("VRCaptureAudio: Encode process finish!");
            }
        }

        /// <summary>
        /// Init capture audio.
        /// </summary>
        void InitCapture()
        {
            audioByteBuffer = new System.Byte[8192];
            GCHandle audioHandle = GCHandle.Alloc (audioByteBuffer, GCHandleType.Pinned);
            audioPointer = audioHandle.AddrOfPinnedObject();
        }

        void OnAudioFilterRead( float[] data, int channels )
        {
            if (isCapturing)
            {
                Marshal.Copy(data, 0, audioPointer, 2048);
                LibAudioCaptureAPI_SendFrame(libAPI, audioByteBuffer);
            }
        }

        [DllImport("VRCaptureLib")]
        static extern System.IntPtr LibAudioCaptureAPI_Get( int rate, string path, string ffpath );

        [DllImport("VRCaptureLib")]
        static extern void LibAudioCaptureAPI_SendFrame( System.IntPtr api, byte[] data );

        [DllImport("VRCaptureLib")]
        static extern void LibAudioCaptureAPI_Close( System.IntPtr api );

        [DllImport("VRCaptureLib")]
        static extern void LibAudioCaptureAPI_Clean( System.IntPtr api );
    }
}
