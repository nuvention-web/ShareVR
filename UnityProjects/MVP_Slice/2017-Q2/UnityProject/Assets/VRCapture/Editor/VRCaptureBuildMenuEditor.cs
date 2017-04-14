using System.Collections.Generic;
using System.IO;
using UnityEditor;
using VRCapture;
namespace VRCapture.Editor {
    public class VRCaptureBuildMenuEditor {
        [MenuItem("VRCapture/Build option/Copy ffmpeg to streamingassets in win")]
        private static void BuildInWin() {
            CopyBuildFile(VRCommonConfig.DATA_PATH + "/VRCapture/FFmpeg/Win/", VRCommonConfig.STREAMING_ASSETS_PATH + "/VRCapture/FFmpeg/Win/");
        }

        // Add a menu item with multiple levels of nesting

        [MenuItem("VRCapture/Build option/Copy ffmpeg to streamingassets in mac")]
        private static void BuildInMac() {
            CopyBuildFile(VRCommonConfig.DATA_PATH + "/VRCapture/FFmpeg/Mac/", VRCommonConfig.STREAMING_ASSETS_PATH + "/VRCapture/FFmpeg/Mac/");
        }
        /// <summary>
        /// Copy ffmpeg executable along with prod build.
        /// </summary>
        public static void CopyBuildFile(string sourcePath, string destPath) {
            if(Directory.Exists(sourcePath)) {
                if(!Directory.Exists(destPath)) {
                    Directory.CreateDirectory(destPath);
                }
                else {
                    return;
                }
                List<string> files = new List<string>(Directory.GetFiles(sourcePath));
                files.ForEach(c => {
                    string destFile = Path.Combine(destPath, Path.GetFileName(c));
                    if(File.Exists(destFile)) {
                        File.Delete(destFile);
                    }
                    File.Copy(c, destFile);
                });
                List<string> folders = new List<string>(Directory.GetDirectories(sourcePath));

                folders.ForEach(c => {
                    string destDir = Path.Combine(destPath, Path.GetFileName(c));
                    CopyBuildFile(c, destDir);
                });
            }
        }
    }
}