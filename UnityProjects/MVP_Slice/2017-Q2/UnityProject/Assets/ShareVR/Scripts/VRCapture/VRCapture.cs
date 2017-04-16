using UnityEngine;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;
using System;

namespace VRCapture
{
	/// <summary>
	/// VRCapture is a plugin helping VR player to record and share
	/// their gameplay easily and nicely.
	/// </summary>
	public class VRCapture : MonoBehaviour
	{
		/// <summary>
		/// VRCapture instance reference.
		/// </summary>
		/// <value>The instance.</value>
		public static VRCapture Instance {
			get;
			private set;
		}

		/// <summary>
		/// Indicates the current status of the capturing session.
		/// </summary>
		public enum StatusCode
		{
			/// <summary>
			/// The capturing session has encountered no errors.
			/// </summary>
			Success = 1,
			/// <summary>
			/// No camera or audio was found to perform video or audio recording.
			/// You must specify one or the other before calling BeginCapture().
			/// </summary>
			CaptureNotFound = -1,
			/// <summary>
			/// The ffmpeg executable file not found, this plugin current is depend
			/// on this to generate the videos.
			/// </summary>
			FFmpegNotFound = -2,
			/// <summary>
			/// The capture process is interrupted by user or unexcept quit.
			/// </summary>
			Interrupted = -3,
			/// <summary>
			/// The process of merge video and audio failed.
			/// </summary>
			MergeProcessFailed = -4,
		}

		/// <summary>
		/// To be notified when an error occurs during a capture session, register
		/// a delegate using this signature by calling RegisterErrorDelegate.
		/// </summary>
        public delegate void ErrorDelegate (StatusCode code);

		/// <summary>
		/// To be notified when the capture is complete, register a delegate 
		/// using this signature by calling RegisterCompleteDelegate.
		/// </summary>
        public delegate void CompleteDelegate ();

		/// <summary>
		/// Register a delegate to be invoked when an error occurs during a
		/// capture session. Multiple delegates may be registered and 
		/// they will be invoked in the order of registration.
		/// </summary>
		/// <param name='del'>
		/// The delegate to be invoked when an error occurs.
		/// </param>
		public void RegisterErrorDelegate (ErrorDelegate del)
		{
			errorDelegate += del;
		}

		/// <summary>
		/// Unregister a previously registered session error delegate.
		/// </summary>
		/// <param name='del'>
		/// The delegate to be unregistered.
		/// </param>
		public void UnregisterErrorDelegate (ErrorDelegate del)
		{
			errorDelegate -= del;
		}

		/// <summary>
		/// Register a delegate to be invoked when the capture is complete.
		/// </summary>
		/// <param name='del'>
		/// The delegate to be invoked when capture complete.
		/// </param>
		public void RegisterCompleteDelegate (CompleteDelegate del)
		{
			completeDelegate += del;
		}

		/// <summary>
		/// Unregister a previously registered session complete delegate.
		/// </summary>
		/// <param name='del'>
		/// The delegate to be unregistered.
		/// </param>
		public void UnregisterCompleteDelegate (CompleteDelegate del)
		{
			completeDelegate -= del;
		}
		// The video record session error delegate variable.
		ErrorDelegate errorDelegate;
		// The video record session complete delegate variable.
		CompleteDelegate completeDelegate;
		/// <summary>
		/// Reference to the VRCaptureVideo capture objects (i.e. cameras) from
		/// which video will be recorded.
		/// Generally you will want to specify at least one.
		/// </summary>
		[Tooltip ("Capture cameras for video recording")]
		[SerializeField, HideInInspector]
		VRCaptureVideo[] vrCaptureVideos;
		/// <summary>
		/// Reference to the VRCaptureAudio object for writing audio files.
		/// This needs to be set when you are recording a video with audio.
		/// </summary>
		[Tooltip ("Audiolistener for audio recording")]
		[SerializeField, HideInInspector]
		VRCaptureAudio vrCaptureAudio;
		/// <summary>
		/// Show debug message.
		/// </summary>
		[Tooltip ("Show debug info message")]
		[SerializeField, HideInInspector]
		bool debug = true;
		/// <summary>
		/// Capturing session status.
		/// </summary>
		StatusCode sessionStatus;
		/// <summary>
		/// Check how many video capture is complete currently.
		/// </summary>
		int captureFinishCount;
		/// <summary>
		/// The count of camera is enabled for capturing.
		/// </summary>
		int captureRequiredCount;
		/// <summary>
		/// Check wether the capture process is interrupted.
		/// </summary>
		bool isInterrupted;
		/// <summary>
		/// The audio/video merge thread.
		/// </summary>
		Thread mergeThread;
		/// <summary>
		/// The garbage collect thread.
		/// </summary>
		Thread gcThread;

		/// <summary>
		/// Get or set the capture videos.
		/// </summary>
		/// <value>The capture videos.</value>
		public VRCaptureVideo[] CaptureVideos {
			get {
				return vrCaptureVideos;
			}
			set {
				vrCaptureVideos = value;
			}
		}

		/// <summary>
		/// Get or set the capture audio.
		/// </summary>
		/// <value>The capture audio.</value>
		public VRCaptureAudio CaptureAudio {
			get {
				return vrCaptureAudio;
			}
			set {
				vrCaptureAudio = value;
			}
		}

		/// <summary>
		/// Show the debug message.
		/// </summary>
		/// <returns><c>true</c>, if debug was shown, <c>false</c> otherwise.</returns>
		public bool ShowDebug ()
		{
			return debug;
		}

		/// <summary>
		/// Check any of video capture still processing.
		/// </summary>
		/// <returns><c>true</c>, if still processing, <c>false</c> otherwise.</returns>
		public bool IsProcessing ()
		{
			bool isPorcessing = false;
			foreach (VRCaptureVideo captureVideo in vrCaptureVideos) {
				if (captureVideo.IsProcessing ()) {
					isPorcessing = true;
					break;
				}
			}
			return isPorcessing;
		}

		/// <summary>
		/// Initialize the attributes of the capture session and and start capturing. 
		/// </summary>
		/// <returns>
		/// The status of the capturing session. This may be Success or a failure code.
		/// See StatusCode for more information.
		/// </returns>
		public StatusCode StartCapture ()
		{
			// Check if can begin capture session.
			if (vrCaptureVideos.Length < 1 && vrCaptureAudio == null) {
				Debug.LogError (
					"VRCapture: StartCapture called but no attached " +
					"cameras and audio listener were found!"
				);
				sessionStatus = StatusCode.CaptureNotFound;
				return sessionStatus;
			}
			if (IsProcessing ()) {
				Debug.LogWarning (
					"VRCapture: StartCapture called before, and " +
					"capture still processing!"
				);
				return sessionStatus;
			}
			if (mergeThread != null && mergeThread.IsAlive) {
				Debug.LogWarning (
					"VRCapture: StartCapture called before, and " +
					"mergeThread still running!"
				);
				return sessionStatus;
			}
			if (gcThread != null && gcThread.IsAlive) {
				Debug.LogWarning (
					"VRCapture: StartCapture called before, and " +
					"gcThread still running!"
				);
				return sessionStatus;
			}
			if (!File.Exists (VRCaptureUtils.FFmpegPath)) {
				Debug.LogError (
					"VRCapture: FFmpeg not found, please fix this " +
					"before capture!"
				);
				sessionStatus = StatusCode.FFmpegNotFound;
				return sessionStatus;
			}

			if (!Directory.Exists (VRCaptureUtils.SaveFolder)) {
				Directory.CreateDirectory (VRCaptureUtils.SaveFolder);
			}

			// Reset sessionStatus.
			sessionStatus = StatusCode.Success;
			isInterrupted = false;
			// Loop through each of the video capture objects, initialize them 
			// and start them recording.
			captureRequiredCount = 0;
			for (int i = 0; i < vrCaptureVideos.Length; i++) {
				VRCaptureVideo vrCaptureVideo = vrCaptureVideos [i];
				if (vrCaptureVideo == null || !vrCaptureVideo.isEnabled) {
					continue;
				}
				captureRequiredCount++;
				vrCaptureVideo.Index = i;
				vrCaptureVideo.StartCapture ();
				vrCaptureVideo.RegisterCaptureCompleteDelegate (
					HandleVideoCaptureComplete);
			}

			// Check if capture audio.
			if (vrCaptureAudio != null && vrCaptureAudio.isEnabled) {
				vrCaptureAudio.StartCapture ();
				vrCaptureAudio.RegisterCaptureCompleteDelegate (
					HandleAudioCaptureComplete);
			}
			captureFinishCount = 0;

			// Start gc thread.
			//gcThread = new Thread(GCThreadFunction);
			//gcThread.Priority = System.Threading.ThreadPriority.Lowest;
			//gcThread.IsBackground = true;
			//gcThread.Start();

			sessionStatus = StatusCode.Success;
			return sessionStatus;
		}

		/// <summary>
		/// Stop capturing and produce the finalized video. Note that the video file
		/// may not be completely written when this method returns. In order to know
		/// when the video file is complete, register a CompleteDelegate.
		/// </summary>
		/// <returns>The capture session.</returns>
		public StatusCode StopCapture ()
		{
			// If the client calls EndRecordingSession for a failed session, do nothing.
			if (sessionStatus != StatusCode.Success)
				return sessionStatus;

			foreach (VRCaptureVideo captureVideo in vrCaptureVideos) {
				if (!captureVideo.isEnabled) {
					continue;
				}
				captureVideo.FinishCapture ();
			}

			if (vrCaptureAudio != null && vrCaptureAudio.isEnabled) {
				vrCaptureAudio.FinishCapture ();
			}

			sessionStatus = StatusCode.Success;
			return sessionStatus;
		}

		/// <summary>
		/// Interrupt the capture session.
		/// </summary>
		/// <returns>The capture session.</returns>
		public StatusCode InterruptCapture ()
		{
			isInterrupted = true;
			StopCapture ();
			sessionStatus = StatusCode.Interrupted;
			return sessionStatus;
		}

		/// <summary>
		/// Handle callbacks for the video capture complete.
		/// </summary>
		void HandleVideoCaptureComplete ()
		{
			captureFinishCount++;
			if (
				captureFinishCount == captureRequiredCount &&
				(vrCaptureAudio == null || !vrCaptureAudio.isEnabled)) {
				if (completeDelegate != null) {
					completeDelegate ();
				}
			}

			Cleanup ();
		}

		/// <summary>
		/// Handles callbacks for the audio capture complete.
		/// </summary>
		void HandleAudioCaptureComplete ()
		{
			if (isInterrupted) {
				isInterrupted = false;
				Cleanup ();
				return;
			}
			bool hasVideoCapture = false;
			foreach (VRCaptureVideo captureVideo in vrCaptureVideos) {
				if (captureVideo.isEnabled) {
					hasVideoCapture = true;
					break;
				}
			}
			if (hasVideoCapture) {
				// Start merging thread when we have videos captured.
				mergeThread = new Thread (MergeThreadFunction);
				mergeThread.Priority = System.Threading.ThreadPriority.Lowest;
				mergeThread.IsBackground = true;
				mergeThread.Start ();
			}
		}

		/// <summary>
		/// Video/Audio merge the thread function.
		/// </summary>
		void MergeThreadFunction ()
		{
			while (captureFinishCount < captureRequiredCount) {
				// Wait for all video capture finish.
				Thread.Sleep (1000);
				if (isInterrupted) {
					return;
				}
			}

			foreach (VRCaptureVideo captureVideo in vrCaptureVideos) {
				// TODO, make audio live streaming work
				if (
					captureVideo == null ||
					!captureVideo.isEnabled ||
                    // Dont merge audio when capture equirectangular, its not sync.
					captureVideo.formatType == VRCaptureVideo.FormatType.PANORAMA) {
					continue;
				}
				VRCaptureMerger merger = new VRCaptureMerger (captureVideo, vrCaptureAudio);
				merger.Merge ();
				if (merger.Failed) {
					sessionStatus = StatusCode.MergeProcessFailed;
					break;
				}
			}
			Cleanup ();
			if (sessionStatus != StatusCode.Success) {
				if (errorDelegate != null)
					errorDelegate (sessionStatus);
			} else {
				if (completeDelegate != null) {
					completeDelegate ();
				}
			}
		}

		/// <summary>
		/// Garbage collect thread function.
		/// </summary>
		void GCThreadFunction ()
		{
			while (IsProcessing ()) {
				// TODO, adjust gc interval dynamic.
				Thread.Sleep (20);
				System.GC.Collect ();
			}
		}

		/// <summary>
		/// Cleanup this instance.
		/// </summary>
		void Cleanup ()
		{
			captureFinishCount = 0;
			foreach (VRCaptureVideo captureVideo in vrCaptureVideos) {
				// Dont clean panorama video, its not include in merge thread.
				if (captureVideo.formatType == VRCaptureVideo.FormatType.PANORAMA) {
					continue;
				}
				captureVideo.Cleanup ();
			}
			if (vrCaptureAudio != null) {
				vrCaptureAudio.Cleanup ();
			}
		}

		/// <summary>
		/// Initial instance and init variable.
		/// </summary>
		void Awake ()
		{
			if (Instance == null)
				Instance = this;
			// For easy access the vrCaptureVideos var.
			if (vrCaptureVideos == null)
				vrCaptureVideos = new VRCaptureVideo[0];
		}

		/// <summary>
		/// Check if still processing on application quit.
		/// </summary>
		void OnApplicationQuit ()
		{
			// Issue an interrupt if still capturing.
			if (IsProcessing ()) {
				InterruptCapture ();
			}
		}
	}

	/// <summary>
	/// VRCaptureMerger is processed after temp video captured, with or without
	/// temp audio captured. If audio captured, it will merge the video and audio
	/// within same file.
	/// </summary>
	class VRCaptureMerger
	{
		/// <summary>
		/// The capture video instance.
		/// </summary>
		VRCaptureVideo captureVideo;
		/// <summary>
		/// The capture audio instance.
		/// </summary>
		VRCaptureAudio captureAudio;

		/// <summary>
		/// Get/Set the destination path.
		/// </summary>
		/// <value>The destination path.</value>
		public string DestinationPath {
			get {
				if (destinationPath != null)
					return destinationPath;
				destinationPath = VRCaptureUtils.SaveFolder + VRCommonUtils.GetMp4FileName (captureVideo.Index.ToString ());
				return destinationPath;
			}
			set {
				destinationPath = value;
			}
		}

		/// <summary>
		/// Destination of merged video/audio.
		/// </summary>
		string destinationPath = null;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:VRCapture.VRCaptureMerger"/> class.
		/// </summary>
		/// <param name="video">Video.</param>
		/// <param name="audio">Audio.</param>
		public VRCaptureMerger (VRCaptureVideo video, VRCaptureAudio audio)
		{
			captureVideo = video;
			captureAudio = audio;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="T:VRCapture.VRCaptureMerger"/> is failed.
		/// </summary>
		/// <value><c>true</c> if failed; otherwise, <c>false</c>.</value>
		public bool Failed {
			get;
			private set;
		}

		/// <summary>
		/// Video/Audio merge function impl.
		/// </summary>
		public void Merge ()
		{
			IntPtr libAPI = LibVideoMergeAPI_Get (
				                captureVideo.Bitrate,
				                DestinationPath,
				                captureVideo.DestinationPath,
				                captureAudio.DestinationPath,
				                VRCaptureUtils.FFmpegPath
			                );

			if (libAPI == IntPtr.Zero) {
				Debug.LogWarning ("VRCapture: get native LibVideoMergeAPI failed!");
				return;
			}
			LibVideoMergeAPI_Merge (libAPI);
			// Make sure generated the merge file.
			int waitTotal = 100;
			int waitCount = 0;
			while (!File.Exists (DestinationPath)) {
				if (waitCount++ < waitTotal)
					Thread.Sleep (500);
			}
			if (waitCount >= waitTotal) {
				Failed = true;
			}
			LibVideoMergeAPI_Clean (libAPI);
		}

		[DllImport ("VRCaptureLib")]
		static extern System.IntPtr LibVideoMergeAPI_Get (int rate, string path, string vpath, string apath, string ffpath);

		[DllImport ("VRCaptureLib")]
		static extern void LibVideoMergeAPI_Merge (System.IntPtr api);

		[DllImport ("VRCaptureLib")]
		static extern void LibVideoMergeAPI_Clean (System.IntPtr api);
	}
}