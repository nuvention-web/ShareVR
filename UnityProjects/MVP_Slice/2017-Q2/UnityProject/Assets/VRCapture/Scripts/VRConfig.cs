using System;
using UnityEngine;

namespace VRCapture
{
	/// <summary>
	/// Common config.
	/// </summary>
	public class VRCommonConfig
	{
		public const float EPSILON = 0.000001f;
		public static string DATA_PATH = Application.dataPath;
		public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
		public static string MY_DOCUMENTS_PATH = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
	}

	/// <summary>
	/// Basic config for VRCapture.
	/// </summary>
	public class VRCaptureConfig
	{
		public static string SaveFolder {
			get {
				return VRCommonConfig.MY_DOCUMENTS_PATH + "/VRCapture/";
			}
		}

		public static string FFmpegEditorFolder {
			get {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
				return VRCommonConfig.DATA_PATH + "/VRCapture/FFmpeg/Win/";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                return VRCommonConfig.DATA_PATH + "/VRCapture/FFmpeg/Mac/";
#endif
			}
		}

		public static string FFmpegEditorPath {
			get {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
				return FFmpegEditorFolder + "ffmpeg.exe";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                return FFmpegEditorFolder + "ffmpeg";
#endif
			}
		}

		public static string FFmpegBuildFolder {
			get {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
				return VRCommonConfig.STREAMING_ASSETS_PATH + "/VRCapture/FFmpeg/Win/";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                return VRCommonConfig.STREAMING_ASSETS_PATH + "/VRCapture/FFmpeg/Mac/";
#endif
			}
		}

		// TODO, fix path using Unity Build Pipeline
		public static string FFmpegBuildPath {
			get {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
				return FFmpegBuildFolder + "ffmpeg.exe";
#elif UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
                return FFmpegBuildFolder + "ffmpeg";
#endif
			}
		}

		public static string FFmpegPath {
			get {
#if UNITY_EDITOR
				return FFmpegEditorPath;
#else
                return FFmpegBuildPath;
#endif
			}
		}
	}

	/// <summary>
	/// Basic config for VRReplay.
	/// </summary>
	public class VRReplayConfig
	{

		public static string SaveFolder {
			get {
				return VRCommonConfig.MY_DOCUMENTS_PATH + "/VRCapture/Replays/";
			}
		}

		public static string SavePath {
			get {
				return SaveFolder + "Replaytrace.bin";
			}
		}

		public const int MAX_DEVICE_NUM = 16;
	}
}