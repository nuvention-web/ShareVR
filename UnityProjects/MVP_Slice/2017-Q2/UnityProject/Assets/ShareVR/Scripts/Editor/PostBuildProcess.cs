using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using ShareVR.Capture;

public static class PostBuildProcess
{
	[PostProcessBuild]
	public static void OnPostprocessBuild (BuildTarget target, string pathToBuiltProject)
	{
		string saveFolder = pathToBuiltProject.Replace (".exe", "_Data/StreamingAssets/ShareVR/");
		if (!Directory.Exists (saveFolder))
			Directory.CreateDirectory (saveFolder);
		saveFolder = Path.GetFullPath (saveFolder);
		
		File.Copy (VRCaptureUtils.FFmpegEditorPath, saveFolder + "/ffmpeg.exe", true);
	}
}