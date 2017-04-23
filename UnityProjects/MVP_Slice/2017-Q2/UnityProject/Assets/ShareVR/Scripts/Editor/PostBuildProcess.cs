using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using VRCapture;

public class PostBuildProcess
{
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject)
	{
		string saveFolder = pathToBuiltProject.Replace (".exe", "_Data/StreamingAssets/");
		if (!Directory.Exists (saveFolder))
			Directory.CreateDirectory (saveFolder);
		
		File.Copy (VRCaptureUtils.FFmpegEditorPath, saveFolder + "/ffmpeg.exe");
		File.Copy (Application.dataPath + "/ShareVR/Watson/Config.json", saveFolder + "/Config.json");
	}
}