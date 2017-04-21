//======= Copyright (c) ShareVR ===============
//
// Purpose: Manage input events
// Version: SDK v1.0
// Date: 4/5/2017
// Created by: Chen Chen
// Revision History:
// 
//=============================================================================

using System.Collections;
using UnityEngine;
using Valve.VR;
using System;

namespace ShareVR.Core
{
	// User Avtion Event Types
	public class UserAction
	{
		bool m_startRec = false;
		bool m_stopRec = false;
		bool m_toggleRec = false;
		bool m_toggleCam = false;
		bool m_toggleAvatar = false;

		// Auto-reset
		public bool startRec {
			get {
				if (m_startRec) {
					// Reset status
					m_startRec = false;
					return true;
				}

				return m_startRec;
			}
			set{ m_startRec = value; }
		}

		public bool stopRec {
			get {
				if (m_stopRec) {
					// Reset status
					m_stopRec = false;
					return true;
				}

				return m_stopRec;
			}
			set{ m_stopRec = value; }
		}

		public bool toggleRec {
			get {
				if (m_toggleRec) {
					// Reset status
					m_toggleRec = false;
					return true;
				}

				return m_toggleRec;
			}
			set{ m_toggleRec = value; }
		}

		public bool toggleCam {
			get {
				if (m_toggleCam) {
					// Reset status
					m_toggleCam = false;
					return true;
				}

				return m_toggleCam;
			}
			set{ m_toggleCam = value; }
		}

		public bool toggleAvatar {
			get {
				if (m_toggleAvatar) {
					// Reset status
					m_toggleAvatar = false;
					return true;
				}

				return m_toggleAvatar;
			}
			set{ m_toggleAvatar = value; }
		}
	}

	public enum ViveCtrlerMapping
	{
		bothTriggerPressed,
		bothGripPressed,
		TriggerAndGripPressed,
		KeyboardOnly_Key_R,
		KeyboardOnly_Key_V,
		KeyboardOnly_Key_X
	}

	public class InputManager : MonoBehaviour
	{
		[HideInInspector]
		public UserAction userAct = new UserAction ();
		[HideInInspector]
		public bool isViveDeviceFound = false;



		protected enum ViveCtrlerMapping
		{
			bothTriggerPressed,
			bothGripPressed,
			TriggerAndGripPressed
		}

		[HideInInspector]
		protected enum ViveCtrler
		{
			leftHand,
			rightHand
		}

		private class ViveCtrlerMappingMask
		{
			public bool bothTriggerPressed = true;
			public bool bothGripPressed = true;
			public bool TriggerAndGripPressed = true;
		}

		// SteamVR Objects
		private uint leftHand, rightHand;
		[NonSerializedAttribute]
		public SteamVR_Controller.Device leftCtrlerDevice;
		[NonSerializedAttribute]
		public SteamVR_Controller.Device rightCtrlerDevice;
		[HideInInspector]
		public SteamVR_TrackedController[] viveCtrlers;


		// Internal Reference
		private RecordManager recManager;

		ViveCtrlerMappingMask viveCtrlerMapMask;
		bool lTriggerPressed = false;
		bool rTriggerPressed = false;
		bool lGripPressed = false;
		bool rGripPressed = false;
		bool anyTriggerPressed = false;
		bool anyGripPressed = false;

		// Use this for initialization
		IEnumerator Start ()
		{
			//Debug.Log ("Instantiated!");
			recManager = FindObjectOfType (typeof(RecordManager)) as RecordManager;

			// Search for avtive Vive controllers every one second until both hand has been found
			if (recManager.showDebugMessage)
				Debug.Log ("Checking tracked Vive Controllers...");
			
			while (true) {
				// This will get refreshed every one second
				yield return new WaitForSeconds (1.0f);

				if (SteamVR.active) {
					// Search both left and right hand
					leftHand = SteamVR.instance.hmd.GetTrackedDeviceIndexForControllerRole (ETrackedControllerRole.LeftHand);
					rightHand = SteamVR.instance.hmd.GetTrackedDeviceIndexForControllerRole (ETrackedControllerRole.RightHand);

					//if (recManager.showDebugMessage) {
					//Debug.Log (
					//"Left Controller: " + (leftHand == OpenVR.k_unTrackedDeviceIndexInvalid ? "Not Found" : ("ID " + leftHand)) +
					//", Right Controller: " + (rightHand == OpenVR.k_unTrackedDeviceIndexInvalid ? "Not Found" : ("ID: " + rightHand)));
					//}
					// If both were found
					if (leftHand != OpenVR.k_unTrackedDeviceIndexInvalid && rightHand != OpenVR.k_unTrackedDeviceIndexInvalid) {
						if (recManager.showDebugMessage)
							Debug.Log ("Both Controllers are found!");
						
						leftCtrlerDevice = SteamVR_Controller.Input ((int)leftHand);
						rightCtrlerDevice = SteamVR_Controller.Input ((int)rightHand);
						viveCtrlers = FindObjectsOfType (typeof(SteamVR_TrackedController)) as SteamVR_TrackedController[];
						//Debug.Log (viveCtrlers);

						isViveDeviceFound = true;
						yield break;
					}
				}
			}
		}

		// Update is called once per frame
		void Update ()
		{
			UpdateUserAction ();
		}

		public void ViveHapticPulse (ushort duration = 500)
		{
			leftCtrlerDevice.TriggerHapticPulse (duration);
			rightCtrlerDevice.TriggerHapticPulse (duration);
		}

		private void UpdateUserAction ()
		{
			// Update Vive Input
			if (isViveDeviceFound) {
				switch (CheckViveInput ()) {
				case ViveCtrlerMapping.TriggerAndGripPressed:
					userAct.toggleCam |= recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.TriggerAndGripPressed;
					//Debug.Log ("Vive - Toggle Camera");
					break;
				case ViveCtrlerMapping.bothGripPressed:
					userAct.toggleCam |= recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.bothGripPressed;
					//userAct.toggleAvatar = true;
					//Debug.Log ("Vive - Toggle Avatar");
					break;
				case ViveCtrlerMapping.bothTriggerPressed:
					userAct.toggleCam |= recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.bothTriggerPressed;
					//userAct.toggleRec = true;
					//Debug.Log ("Vive - Toggle Rec");
					break;
				}
			}

			CheckKeyboardInput ();
		}

		private ViveCtrlerMapping? CheckViveInput ()
		{
			// Refresh State
			lTriggerPressed |= GetTriggerDown (ViveCtrler.leftHand);
			rTriggerPressed |= GetTriggerDown (ViveCtrler.rightHand);
			lGripPressed |= GetGripDown (ViveCtrler.leftHand);
			rGripPressed |= GetGripDown (ViveCtrler.rightHand);

			anyTriggerPressed = lTriggerPressed || rTriggerPressed;
			anyGripPressed = lGripPressed || rGripPressed;

			if (lTriggerPressed && rTriggerPressed) {
				lTriggerPressed = rTriggerPressed = false;
				return ViveCtrlerMapping.bothTriggerPressed;
			}
			
			if (lGripPressed && rGripPressed) {
				lGripPressed = rGripPressed = false;
				return ViveCtrlerMapping.bothGripPressed;
			}

			if (anyTriggerPressed && anyGripPressed) {
				lGripPressed = rGripPressed = lTriggerPressed = rTriggerPressed = false;
				return ViveCtrlerMapping.TriggerAndGripPressed;
			}

			return null;
		}

		private void CheckKeyboardInput ()
		{
			if (recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.KeyboardOnly_Key_R)
				userAct.toggleRec |= Input.GetKeyDown (KeyCode.R);
			if (recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.KeyboardOnly_Key_X)
				userAct.toggleRec |= Input.GetKeyDown (KeyCode.X);
			if (recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.KeyboardOnly_Key_V)
				userAct.toggleRec |= Input.GetKeyDown (KeyCode.V);
			//userAct.toggleAvatar |= Input.GetKeyDown (KeyCode.V);
			//userAct.toggleCam |= Input.GetKeyDown (KeyCode.B);
		}

		// Wrapper function for vive controller button state
		protected bool GetTriggerDown (ViveCtrler ctrler)
		{
			if (ctrler == ViveCtrler.leftHand)
				return leftCtrlerDevice.GetPressDown (EVRButtonId.k_EButton_SteamVR_Trigger);
			if (ctrler == ViveCtrler.rightHand)
				return rightCtrlerDevice.GetPressDown (EVRButtonId.k_EButton_SteamVR_Trigger);

			return false;
		}
		// Wrapper function for vive controller button state
		protected bool GetTriggerUp (ViveCtrler ctrler)
		{
			if (ctrler == ViveCtrler.leftHand)
				return leftCtrlerDevice.GetPressUp (EVRButtonId.k_EButton_SteamVR_Trigger);
			if (ctrler == ViveCtrler.rightHand)
				return rightCtrlerDevice.GetPressUp (EVRButtonId.k_EButton_SteamVR_Trigger);

			return false;
		}
		// Wrapper function for vive controller button state
		protected bool GetGripDown (ViveCtrler ctrler)
		{
			if (ctrler == ViveCtrler.leftHand)
				return leftCtrlerDevice.GetPressDown (EVRButtonId.k_EButton_Grip);
			if (ctrler == ViveCtrler.rightHand)
				return rightCtrlerDevice.GetPressDown (EVRButtonId.k_EButton_Grip);

			return false;
		}
		// Wrapper function for vive controller button state
		protected bool GetGripUp (ViveCtrler ctrler)
		{
			if (ctrler == ViveCtrler.leftHand)
				return leftCtrlerDevice.GetPressUp (EVRButtonId.k_EButton_Grip);
			if (ctrler == ViveCtrler.rightHand)
				return rightCtrlerDevice.GetPressUp (EVRButtonId.k_EButton_Grip);

			return false;
		}
	}

}
