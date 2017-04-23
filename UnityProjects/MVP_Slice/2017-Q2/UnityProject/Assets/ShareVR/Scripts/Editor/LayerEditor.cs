//======= Copyright (c) ShareVR ===============================
//
// Purpose: Automatically checks if ShareVR's layer is registered
// Version: 0.3
// Date: 4/23/2017
//=============================================================
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class LayerEditor
{
	static void CreateLayer ()
	{
		SerializedObject tagManager = new SerializedObject (AssetDatabase.LoadAllAssetsAtPath ("ProjectSettings/TagManager.asset") [0]);
		SerializedProperty layers = tagManager.FindProperty ("layers");

		bool ExistViewLayer = false;
		bool ExistCaptureLayer = false;

		for (int i = layers.arraySize - 1; i >= 8; i--) {
			SerializedProperty layerSP = layers.GetArrayElementAtIndex (i);

			if (layerSP.stringValue == "IgnoreInView") {
				ExistViewLayer = true;
				continue;
			}
			if (layerSP.stringValue == "IgnoreInCapture") {
				ExistCaptureLayer = true;
				continue;
			}

			if (ExistCaptureLayer && ExistViewLayer) {
				// Both Layer found
				Debug.Log ("ShareVR: Render layer check passed!");
				EditorApplication.update -= CreateLayer;
				return;
			}
		}
		for (int j = layers.arraySize - 1; j >= 8; j--) {
			SerializedProperty layerSP = layers.GetArrayElementAtIndex (j);
			if (layerSP.stringValue == "" && !ExistViewLayer) {
				ExistViewLayer = true;
				layerSP.stringValue = "IgnoreInView";
				continue;
			}
			if (layerSP.stringValue == "" && !ExistCaptureLayer) {
				ExistCaptureLayer = true;
				layerSP.stringValue = "IgnoreInCapture";
				continue;
			}

			if (ExistCaptureLayer && ExistViewLayer) {
				// Both Layer found
				break;
			}
		}

		tagManager.ApplyModifiedProperties ();
		Debug.Log ("ShareVR: Render layer check passed!");

		// Make sure this only run once!
		EditorApplication.update -= CreateLayer;
	}

	static LayerEditor ()
	{
		EditorApplication.update += CreateLayer;
	}
}
