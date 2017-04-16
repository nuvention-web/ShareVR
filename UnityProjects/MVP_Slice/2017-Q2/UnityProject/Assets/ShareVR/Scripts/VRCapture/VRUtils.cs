using System;
using UnityEngine;
using System.IO;

namespace VRCapture
{
	////////////////////////////////////////////////////////////////////////////
	///                           Common Utils.                              ///
	////////////////////////////////////////////////////////////////////////////
	public class VRCommonUtils
	{
		public const float EPSILON = 0.000001f;
		public static string DATA_PATH = Application.dataPath;
		public static string PERSISTENT_DATA_PATH = Application.persistentDataPath;
		public static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
		public static string MY_DOCUMENTS_PATH = Environment.GetFolderPath (
			                                         Environment.SpecialFolder.MyDocuments);

		static string GetTimeString ()
		{
			return DateTime.Now.ToString ("yyyy-MM-dd-HH-mm-ss");
		}

		public static string GetPngFileName ()
		{
			return GetPngFileName (null);
		}

		public static string GetPngFileName (string name)
		{
			return GetTimeString () + (name == null ? "" : "-") + name + ".png";
		}

		public static string GetMp4FileName ()
		{
			return GetMp4FileName (null);
		}

		public static string GetMp4FileName (string name)
		{
			string fileName;
			int videoID = 0;

			fileName = GetTimeString () + "-Camera-" + (name ?? "?") + "-Session-" + videoID + ".mp4";
			while (File.Exists (VRCaptureUtils.SaveFolder + fileName)) {
				videoID++;
				fileName = GetTimeString () + "-Camera-" + (name ?? "?") + "-Session-" + videoID + ".mp4";
			}

			return fileName;
		}

		public static string GetWavFileName ()
		{
			return GetWavFileName (null);
		}

		public static string GetWavFileName (string name)
		{
			return GetMp4FileName ().Replace (".mp4", ".wav");
		}

		public static string GetTxtFileName ()
		{
			return GetTxtFileName (null);
		}

		public static string GetTxtFileName (string name)
		{
			return GetTimeString () + (name == null ? "" : "-") + name + ".txt";
		}
	}

	////////////////////////////////////////////////////////////////////////////
	///                    Basic Utils for VRCapture.                        ///
	////////////////////////////////////////////////////////////////////////////
	public class VRCaptureUtils
	{

		public static string SaveFolder {
			get {
				return VRCommonUtils.MY_DOCUMENTS_PATH + "/ShareVR/";
			}
		}

		public static string FFmpegEditorFolder {
			get {
				return VRCommonUtils.DATA_PATH + "/ShareVR/Plugins/ThirdParty/FFmpeg/";
			}
		}

		public static string FFmpegEditorPath {
			get {
				return FFmpegEditorFolder + "ffmpeg.exe";
			}
		}

		public static string FFmpegBuildFolder {
			get {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
				return VRCommonUtils.STREAMING_ASSETS_PATH + "/";
#endif
			}
		}

		// TODO, fix path using Unity Build Pipeline
		public static string FFmpegBuildPath {
			get {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
				return FFmpegBuildFolder + "ffmpeg.exe";
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

	////////////////////////////////////////////////////////////////////////////
	///                     Basic Utils for VRReplay.                        ///
	////////////////////////////////////////////////////////////////////////////
	public class VRReplayUtils
	{

		public static string SaveFolder {
			get {
				return VRCommonUtils.MY_DOCUMENTS_PATH + "/ShareVR/Replays/";
			}
		}

		public const string UPLOAD_ADDRESS = "";
		public const int MAX_DEVICE_NUM = 16;
	}

	////////////////////////////////////////////////////////////////////////////
	///                     VR device releated Utils.                        ///
	////////////////////////////////////////////////////////////////////////////
	public class VRDeviceUtils
	{

		public static bool UseSteamVR {
			get {
				Type steamvr = Type.GetType ("SteamVR");
				if (steamvr != null) {
					return true;
				}
				return false;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////
	///                     Serializable data struct.                        ///
	////////////////////////////////////////////////////////////////////////////

	/// <summary>
	/// Serializable Vector2 to replace Unity Vector2 struct.
	/// </summary>
	[Serializable]
	public struct SerializableVector2
	{
		public float x;
		public float y;

		public SerializableVector2 (Vector2 v2)
		{
			x = v2.x;
			y = v2.y;
		}

		public Vector2 ToVector2 ()
		{
			return new Vector3 (x, y);
		}
	}

	/// <summary>
	/// Serializable Vector3 to replace Unity Vector3 struct.
	/// </summary>
	[Serializable]
	public struct SerializableVector3
	{
		public float x;
		public float y;
		public float z;

		public SerializableVector3 (Vector3 v3)
		{
			x = v3.x;
			y = v3.y;
			z = v3.z;
		}

		public Vector3 ToVector3 ()
		{
			return new Vector3 (x, y, z);
		}
	}

	/// <summary>
	/// Serializable Quaternion to replace Unity Quaternion struct.
	/// </summary>
	[Serializable]
	public struct SerializableQuaternion
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public SerializableQuaternion (Quaternion q)
		{
			x = q.x;
			y = q.y;
			z = q.z;
			w = q.w;
		}

		public Quaternion ToQuaternion ()
		{
			return new Quaternion (x, y, z, w);
		}
	}

	/// <summary>
	/// Serializable Transform to replace Unity Transform struct.
	/// </summary>
	[Serializable]
	public class SerializableTransform
	{
		public string Name { get; private set; }

		SerializableVector3 postion;
		SerializableQuaternion rotation;

		public SerializableTransform (string name, Vector3 pos, Quaternion rot)
		{
			Name = name;
			postion = new SerializableVector3 (pos);
			rotation = new SerializableQuaternion (rot);
		}

		public Vector3 GetPostion ()
		{
			return postion.ToVector3 ();
		}

		public Quaternion GetRotation ()
		{
			return rotation.ToQuaternion ();
		}
	}
}