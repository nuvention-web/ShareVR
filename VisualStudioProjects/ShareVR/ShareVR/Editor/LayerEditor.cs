//======= Copyright (c) ShareVR ===============================
//
// Purpose: Automatically checks if ShareVR's layer is registered
// Version: 0.4
// Chen Chen
// 4/29/2017
//=============================================================
using UnityEngine;
using UnityEditor;
using System.IO;

namespace ShareVR.EditorScripts
{
    [InitializeOnLoad]
    internal class LayerEditor
    {
        static void CreateLayer()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layers = tagManager.FindProperty("layers");

            bool existViewLayer = false;
            bool existCaptureLayer = false;
            bool existShareVRLayer = false;

            for (int i = layers.arraySize - 1; i >= 8; i--)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);

                if (layerSP.stringValue == "ShareVRIgnoreViewOnly")
                {
                    existViewLayer = true;
                    continue;
                }
                if (layerSP.stringValue == "ShareVRIgnoreCaptureOnly")
                {
                    existCaptureLayer = true;
                    continue;
                }
                if (layerSP.stringValue == "ShareVRIgnoreAll")
                {
                    existShareVRLayer = true;
                    continue;
                }
                if (existCaptureLayer && existViewLayer && existShareVRLayer)
                {
                    // Both Layer found
                    //Debug.Log ("ShareVR: Render layer check passed!");
                    EditorApplication.update -= CreateLayer;
                    return;
                }
            }
            for (int j = layers.arraySize - 1; j >= 8; j--)
            {
                SerializedProperty layerSP = layers.GetArrayElementAtIndex(j);
                if (layerSP.stringValue == "" && !existViewLayer)
                {
                    existViewLayer = true;
                    layerSP.stringValue = "ShareVRIgnoreViewOnly";
                    continue;
                }
                if (layerSP.stringValue == "" && !existCaptureLayer)
                {
                    existCaptureLayer = true;
                    layerSP.stringValue = "ShareVRIgnoreCaptureOnly";
                    continue;
                }
                if (layerSP.stringValue == "" && !existShareVRLayer)
                {
                    existShareVRLayer = true;
                    layerSP.stringValue = "ShareVRIgnoreAll";
                    continue;
                }

                if (existCaptureLayer && existViewLayer && existShareVRLayer)
                {
                    // All Layers found
                    break;
                }
            }

            tagManager.ApplyModifiedProperties();
            Debug.Log("ShareVR: Render layer check passed!");

            // Make sure this only run once!
            EditorApplication.update -= CreateLayer;
        }

        static LayerEditor()
        {
            EditorApplication.update += CreateLayer;
        }
    }

    [InitializeOnLoad]
    internal class PluginManager
    {
        static string dataPath = Application.dataPath;
        static string streamPath = Application.streamingAssetsPath;
        static string pluginPath = Path.GetFullPath(streamPath + "/ShareVR/");

        static void CopyPlugin()
        {
            string ffmpegPath = Path.GetFullPath(pluginPath + "/ffmpeg.exe");

            if (!Directory.Exists(pluginPath))
                Directory.CreateDirectory(pluginPath);

            if (!File.Exists(ffmpegPath))
                File.Copy(Path.GetFullPath(dataPath + "/ShareVR/Plugins/ThirdParty/FFmpeg/ffmpeg.exe"), Path.GetFullPath(pluginPath + "/ffmpeg.exe"));

            //Debug.Log ("ShareVR: Plugin copied to " + pluginPath);
            // Make sure this only run once!
            EditorApplication.update -= CopyPlugin;
        }

        static PluginManager()
        {
            EditorApplication.update += CopyPlugin;
        }
    }
}
