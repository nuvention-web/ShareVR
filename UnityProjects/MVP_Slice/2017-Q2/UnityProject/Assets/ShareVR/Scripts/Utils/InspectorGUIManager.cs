//======= Copyright (c) ShareVR ===============================
//
// Purpose: Inspector field GUI control
// Version: 0.4
// Chen Chen 
// 4/29/2017
//=============================================================
using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace ShareVR.Utils
{
	using ShareVR.Core;

	[CustomEditor (typeof(RecordManager))]
	public class InspectorGUIManager : Editor
	{
		SerializedProperty showCameraModel;
		SerializedProperty cameraModelScale;
		SerializedProperty showPlayerAvatar;
		SerializedProperty avatarScale;
		SerializedProperty avatarOffset;
		SerializedProperty toggleRecordingInput;
		SerializedProperty useVoiceCommand;
		SerializedProperty playerHeadGameObject;
		SerializedProperty playerHandTransform;
		//SerializedProperty showDebugMessage;
		SerializedProperty cameraFollowMethod;
		SerializedProperty frameSize;
		SerializedProperty frameRate;
		SerializedProperty saveFolder;
		SerializedProperty uploadFileOnline;
		SerializedProperty cameraOrbitSpeed;
		SerializedProperty camHeight;
		SerializedProperty camDistance;
		SerializedProperty camAngle;
		SerializedProperty camMotionDamp;
		SerializedProperty showCameraPreview;

		const float minScale = 0.001f;
		const float maxScale = 25.0f;
		const float minSpeed = 0.001f;
		const float maxSpeed = 10.0f;

		void OnEnable ()
		{
			GetShareVRProperties ();
		}

		public override void OnInspectorGUI ()
		{
			// Begin of InspectorGUI
			serializedObject.Update ();

			RecordManager sharevrObj = (RecordManager)target;

			#region ShareVR Inspector Field
			EditorGUILayout.BeginVertical ("Box");
			EditorGUIUtility.labelWidth = 160.0f;

			// ShareVR SDK Version
			AddSDKVersionHeader ();

			// 
			{
				#region Layout - Spectator Camera Control
				EditorGUILayout.BeginVertical ("Box");

				// Section Header
				AddHeader ("Spectator Camera Control", false);

				// Property Fields
				EditorGUILayout.PropertyField (cameraFollowMethod, new GUIContent ("Camera Follow Regime"));
				EditorGUILayout.Slider (camDistance, minSpeed, maxSpeed, new GUIContent ("Camera Distance"));
				EditorGUILayout.Slider (camHeight, -10.0f, maxSpeed, new GUIContent ("Camera Height"));
				switch (cameraFollowMethod.enumValueIndex) {
				case (int)CameraFollowMethod.OrbitSmooth:
					EditorGUILayout.Slider (cameraOrbitSpeed, minSpeed, maxSpeed, new GUIContent ("Camera Orbit Speed"));
					break;
				case (int)CameraFollowMethod.FixedSmooth:
					EditorGUILayout.Slider (camAngle, -180.0f, 180.0f, new GUIContent ("Camera Angle"));
					EditorGUILayout.Slider (camMotionDamp, 0.1f, 10.0f, new GUIContent ("Camera Motion Damp"));
					break;
				}
				EditorGUILayout.PropertyField (showCameraModel, new GUIContent ("Show Camera Model"));
				if (showCameraModel.boolValue) {
					EditorGUILayout.Slider (cameraModelScale, minScale, 5.0f, new GUIContent ("Camera Model Scale"));
				}
				EditorGUILayout.PropertyField (showCameraPreview, new GUIContent ("Show Camera Preview"));

				EditorGUILayout.EndVertical ();
				#endregion

				#region Layout - Avatar Control
				EditorGUILayout.Space ();
				EditorGUILayout.BeginVertical ("Box");

				// Section Header
				AddHeader ("Avatar Control", false);

				// Property Fields
				EditorGUILayout.PropertyField (showPlayerAvatar, new GUIContent ("Show Avatar"));
				if (showPlayerAvatar.boolValue) {
					// If user want to use our avatar
					EditorGUILayout.Slider (avatarScale, minScale, maxScale, new GUIContent ("Avatar Scale"));
					EditorGUILayout.PropertyField (avatarOffset, new GUIContent ("Avatar Offset"));
				}

				EditorGUILayout.EndVertical ();
				#endregion

				#region Layout - Input Control
				EditorGUILayout.Space ();
				EditorGUILayout.BeginVertical ("Box");

				// Section Header
				AddHeader ("Input Control", false);

				// Property Fields
				EditorGUILayout.PropertyField (useVoiceCommand, new GUIContent ("Use Voice Command"));
				EditorGUILayout.PropertyField (toggleRecordingInput, new GUIContent ("Recording Key Trigger"));

				EditorGUILayout.EndVertical ();
				#endregion

				#region Layout - GameObject Reference
				EditorGUILayout.Space ();
				EditorGUILayout.BeginVertical ("Box");

				// Section Header
				AddHeader ("GameObject Reference", false);

				// Property Fields
				EditorGUILayout.PropertyField (playerHeadGameObject, new GUIContent ("Main Player"));
				if (playerHeadGameObject.objectReferenceValue == null)
					EditorGUILayout.LabelField ("You MUST specify a main player to track", EditorStyles.helpBox);

				if (showPlayerAvatar.boolValue) {
					EditorGUILayout.PropertyField (playerHandTransform, new GUIContent ("Hands of your player"), true);

					if (!sharevrObj.playerHandTransform.isHandTransformValid)
						EditorGUILayout.LabelField ("(Optional) Specify both hands of your player to track", EditorStyles.helpBox);
				}

				EditorGUILayout.EndVertical ();
				#endregion

				#region Layout - Recording Control
				EditorGUILayout.Space ();
				EditorGUILayout.BeginVertical ("Box");

				// Section Header
				AddHeader ("Recording Control", false);

				// Property Fields
				EditorGUILayout.PropertyField (frameSize, new GUIContent ("Frame Size"));
				EditorGUILayout.PropertyField (frameRate, new GUIContent ("Frame Rate"));

				EditorGUILayout.PropertyField (saveFolder, new GUIContent ("Save Folder"));
				VRCapture.VRCaptureUtils.SaveFolder = saveFolder.stringValue;
				EditorGUILayout.LabelField ("Save path: " + VRCapture.VRCaptureUtils.GetCurrentSaveFolder, EditorStyles.helpBox);

				EditorGUILayout.EndVertical ();
				#endregion

				#region Layout - Sharing Control
				EditorGUILayout.Space ();
				EditorGUILayout.BeginVertical ("Box");

				// Section Header
				AddHeader ("Sharing Control", false);

				// Property Fields
				EditorGUILayout.PropertyField (uploadFileOnline, new GUIContent ("Upload Video Online?"));
				EditorGUILayout.EndVertical ();
				#endregion
			}

			EditorGUILayout.EndVertical ();

			#endregion

			// Handle all the changes
			if (GUI.changed) {
				VRCapture.VRCaptureUtils.SaveFolder = saveFolder.stringValue;
				sharevrObj.UpdateCameraParameters ();
			}

			// End of InspectorGUI
			serializedObject.ApplyModifiedProperties ();
		}

		void AddHeader (string header, bool spaceBeforeHeader = true)
		{
			if (spaceBeforeHeader) {
				EditorGUILayout.Space ();
			}

			EditorGUILayout.LabelField (header, EditorStyles.boldLabel);
		}

		void AddSDKVersionHeader ()
		{
			EditorGUILayout.LabelField ("ShareVR SDK v" + GlobalParameters.SDKversion, EditorStyles.centeredGreyMiniLabel);
		}

		void GetShareVRProperties ()
		{
			// Get Camera Control Parameters
			showCameraModel = serializedObject.FindProperty ("showCameraModel");
			cameraModelScale = serializedObject.FindProperty ("cameraModelScale");
			cameraFollowMethod = serializedObject.FindProperty ("cameraFollowMethod");
			cameraOrbitSpeed = serializedObject.FindProperty ("cameraOrbitSpeed");
			camHeight = serializedObject.FindProperty ("camHeight");
			camDistance = serializedObject.FindProperty ("camDistance");
			camAngle = serializedObject.FindProperty ("camAngle");
			camMotionDamp = serializedObject.FindProperty ("camMotionDamp");
			showCameraPreview = serializedObject.FindProperty ("showCameraPreview");
				
			showPlayerAvatar = serializedObject.FindProperty ("showPlayerAvatar");
			avatarScale = serializedObject.FindProperty ("avatarScale");
			avatarOffset = serializedObject.FindProperty ("avatarOffset");

			useVoiceCommand = serializedObject.FindProperty ("useVoiceCommand");

			toggleRecordingInput = serializedObject.FindProperty ("toggleRecordingInput");
			playerHeadGameObject = serializedObject.FindProperty ("playerHeadGameObject");
			playerHandTransform = serializedObject.FindProperty ("playerHandTransform");

			frameSize = serializedObject.FindProperty ("frameSize");
			frameRate = serializedObject.FindProperty ("frameRate");

			saveFolder = serializedObject.FindProperty ("saveFolder");
			uploadFileOnline = serializedObject.FindProperty ("uploadFileOnline");
			//showDebugMessage = serializedObject.FindProperty ("showDebugMessage");
		}
	}
}