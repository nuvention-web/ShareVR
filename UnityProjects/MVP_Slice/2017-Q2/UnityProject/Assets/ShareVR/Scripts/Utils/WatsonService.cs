using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IBM.Watson.DeveloperCloud.Services.SpeechToText.v1;
using IBM.Watson.DeveloperCloud.DataTypes;
using IBM.Watson.DeveloperCloud.Utilities;

namespace ShareVR.Utils
{
	using ShareVR.Core;

	public class WatsonService : MonoBehaviour
	{
		[HideInInspector]
		public bool isActive = false;

		private int m_RecordingRoutine = 0;
		private string m_MicrophoneID = null;
		private AudioClip m_Recording = null;
		private const int m_RecordingBufferSize = 2;
		private const int m_RecordingHZ = 22050;

		private SpeechToText m_SpeechToText = new SpeechToText ();

		// ShareVR Object Reference
		private RecordManager recManager;

		// User Intention
		//bool intentAvatar, intentCamera;
		//bool? actionType;

		// Use this for initialization
		void Start ()
		{
			recManager = FindObjectOfType (typeof(RecordManager)) as RecordManager;

			InitializeWatsonSTT ();

			if (recManager.useVoiceCommand) {
				StartRecording ();
				StartListening ();
			}
		}

		void InitializeWatsonSTT ()
		{
			m_SpeechToText.DetectSilence = true;
			m_SpeechToText.EnableWordConfidence = false;
			m_SpeechToText.EnableTimestamps = false;
			m_SpeechToText.SilenceThreshold = 0.03f;
			m_SpeechToText.MaxAlternatives = 1;
			m_SpeechToText.EnableContinousRecognition = true;
			m_SpeechToText.EnableInterimResults = false;
			m_SpeechToText.OnError = OnError;
		}

		private void OnError (string error)
		{
			isActive = false;
			Debug.LogError ("ShareVR - Watson STT Service: " + "Error! " + error);
		}

		private IEnumerator RecordingHandler ()
		{
			//Log.Debug ("ExampleStreaming", "devices: {0}", Microphone.devices);
			m_Recording = Microphone.Start (m_MicrophoneID, true, m_RecordingBufferSize, m_RecordingHZ);
			yield return null;      // let m_RecordingRoutine get set..

			if (m_Recording == null) {
				StopRecording ();
				yield break;
			}

			bool bFirstBlock = true;
			int midPoint = m_Recording.samples / 2;
			float[] samples = null;

			while (m_RecordingRoutine != 0 && m_Recording != null) {
				int writePos = Microphone.GetPosition (m_MicrophoneID);
				if (writePos > m_Recording.samples || !Microphone.IsRecording (m_MicrophoneID)) {
					Debug.LogError ("ShareVR - Watson STT Service: " + "Microphone disconnected.");

					StopRecording ();
					yield break;
				}

				if ((bFirstBlock && writePos >= midPoint)
				    || (!bFirstBlock && writePos < midPoint)) {
					// front block is recorded, make a RecordClip and pass it onto our callback.
					samples = new float[midPoint];
					m_Recording.GetData (samples, bFirstBlock ? 0 : midPoint);

					AudioData record = new AudioData ();
					record.MaxLevel = Mathf.Max (samples);
					record.Clip = AudioClip.Create ("Recording", midPoint, m_Recording.channels, m_RecordingHZ, false);
					record.Clip.SetData (samples, 0);

					//Debug.Log ("Audio Maxlevel: " + record.MaxLevel);

					m_SpeechToText.OnListen (record);

					bFirstBlock = !bFirstBlock;
				} else {
					// calculate the number of samples remaining until we ready for a block of audio,
					// and wait that amount of time it will take to record.
					int remaining = bFirstBlock ? (midPoint - writePos) : (m_Recording.samples - writePos);
					float timeRemaining = (float)remaining / (float)m_RecordingHZ;

					yield return new WaitForSeconds (timeRemaining);
				}
			}
			yield break;
		}

		private void OnRecognize (SpeechRecognitionEvent result)
		{
			if (result != null && result.results.Length > 0) {
				foreach (var res in result.results) {
					if (res.keywords_result != null) {
						if (res.keywords_result.keyword != null) {
							//intentAvatar = false;
							//intentCamera = false;
							//actionType = null;
							foreach (var keyword in res.keywords_result.keyword) {
								if (recManager.showDebugMessage)
									Debug.Log ("ShareVR - Watson STT Service: " + string.Format ("{0} ({1}, {2:0.00})\n",
										keyword.normalized_text, res.final ? "Final" : "Interim", keyword.confidence));

								// Determine Action
								if (keyword.normalized_text == "start")
									recManager.StartRecording ();
								if (keyword.normalized_text == "stop")
									recManager.StopRecording ();
							}
						}
					}
				}
			}
		}

		private void StartRecording ()
		{
			if (m_RecordingRoutine == 0) {
				UnityObjectUtil.StartDestroyQueue ();
				m_RecordingRoutine = Runnable.Run (RecordingHandler ());
			}
		}

		private void StopRecording ()
		{
			if (m_RecordingRoutine != 0) {
				Microphone.End (m_MicrophoneID);
				Runnable.Stop (m_RecordingRoutine);
				m_RecordingRoutine = 0;
			}
		}

		public void StartListening ()
		{
			isActive = true;
			m_SpeechToText.StartListening (OnRecognize);
		}

		public void StopListening ()
		{
			isActive = false;
			m_SpeechToText.StopListening ();
		}

		// Update is called once per frame
		void Update ()
		{
		
		}
	}
}