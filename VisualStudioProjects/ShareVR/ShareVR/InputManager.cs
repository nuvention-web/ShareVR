//======= Copyright (c) ShareVR ===============
//
// Purpose: Manage input events
// Version: SDK v0.4
// Date: 4/5/2017
// Chen Chen
//=============================================================================

using System.Collections;
using UnityEngine;
using System;

namespace ShareVR.Core
{
    // User Avtion Event Types
    internal class UserAction
    {
        bool m_startRec = false;
        bool m_stopRec = false;
        bool m_toggleRec = false;
        bool m_toggleCam = false;
        bool m_toggleAvatar = false;

        public UserAction()
        {
            m_startRec = false;
            m_stopRec = false;
            m_toggleRec = false;
            m_toggleCam = false;
            m_toggleAvatar = false;
        }

        // Auto-reset
        public bool startRec
        {
            get
            {
                if (m_startRec)
                {
                    // Reset status
                    m_startRec = false;
                    return true;
                }

                return m_startRec;
            }
            set { m_startRec = value; }
        }

        public bool stopRec
        {
            get
            {
                if (m_stopRec)
                {
                    // Reset status
                    m_stopRec = false;
                    return true;
                }

                return m_stopRec;
            }
            set { m_stopRec = value; }
        }

        public bool toggleRec
        {
            get
            {
                if (m_toggleRec)
                {
                    // Reset status
                    m_toggleRec = false;
                    return true;
                }

                return m_toggleRec;
            }
            set { m_toggleRec = value; }
        }

        public bool toggleCam
        {
            get
            {
                if (m_toggleCam)
                {
                    // Reset status
                    m_toggleCam = false;
                    return true;
                }

                return m_toggleCam;
            }
            set { m_toggleCam = value; }
        }

        public bool toggleAvatar
        {
            get
            {
                if (m_toggleAvatar)
                {
                    // Reset status
                    m_toggleAvatar = false;
                    return true;
                }

                return m_toggleAvatar;
            }
            set { m_toggleAvatar = value; }
        }
    }

    internal enum ViveCtrlerMapping
    {
        bothTriggerPressed,
        bothMenuKeyPressed,
        leftMenuKeyPressed,
        rightMenuKeyPressed,
        KeyboardOnly_Key_R,
        KeyboardOnly_Key_V,
        KeyboardOnly_Key_X,
        DontUseAnyKey
    }

    internal enum Ctrler
    {
        leftHand,
        rightHand
    }

    internal class InputManager : MonoBehaviour
    {
        [HideInInspector]
        public static UserAction userAct = new UserAction();
        [HideInInspector]
        public static bool isBothCtrlerFound = false;

        // Controller Input KeyCode Mapping
        private static class CtrlerKeyCode
        {
            public const KeyCode viveTriggerL = KeyCode.JoystickButton14;
            public const KeyCode viveTriggerR = KeyCode.JoystickButton15;
            public const KeyCode viveMenuL = KeyCode.JoystickButton2;
            public const KeyCode viveMenuR = KeyCode.JoystickButton0;
            public const KeyCode viveGripL = KeyCode.JoystickButton2;
            public const KeyCode viveGripR = KeyCode.JoystickButton0;
            public const KeyCode viveTrackpadPressL = KeyCode.JoystickButton8;
            public const KeyCode viveTrackpadPressR = KeyCode.JoystickButton9;
            public const KeyCode viveTrackpadTouchL = KeyCode.JoystickButton16;
            public const KeyCode viveTrackpadTouchR = KeyCode.JoystickButton17;
        }

        protected class ViveCtrlerMappingMask
        {
            public bool bothTriggerPressed = true;
            public bool bothGripPressed = true;
            public bool TriggerAndGripPressed = true;
        }

        // Controller Objects
        protected bool isLeftHandFound = false;
        protected bool isRightHandFound = false;

        // Internal Reference
        protected RecordManager recManager;

        protected ViveCtrlerMappingMask viveCtrlerMapMask = null;
        protected bool lTriggerPressed = false;
        protected bool rTriggerPressed = false;
        protected bool lGripPressed = false;
        protected bool rGripPressed = false;
        protected bool anyTriggerPressed = false;
        protected bool anyGripPressed = false;
        protected bool lMenuPressed = false;
        protected bool rMenuPressed = false;
        protected bool anyMenuPressed = false;

        private IEnumerator Start()
        {
            //Debug.Log ("Instantiated!");
            recManager = FindObjectOfType(typeof(RecordManager)) as RecordManager;

            // Search for avtive Vive controllers every one second until both hand has been found
            if (recManager.showDebugMessage)
                Debug.Log("Checking tracked Controllers...");

            while (true)
            {
                // This will get refreshed every one second
                yield return new WaitForSeconds(1.0f);

                string[] joyStickName = Input.GetJoystickNames();

                isLeftHandFound = Array.BinarySearch(joyStickName, "OpenVR Controller - Left") >= 0;
                isRightHandFound = Array.BinarySearch(joyStickName, "OpenVR Controller - Right") >= 0;

                Debug.Log("Checking L Controllers..." + isLeftHandFound);
                Debug.Log("Checking R Controllers..." + isRightHandFound);

                isBothCtrlerFound = isLeftHandFound && isRightHandFound;

                if (isBothCtrlerFound)
                {
                    if (recManager.showDebugMessage)
                        Debug.Log("Found active OpenVR controller pair...");
                    yield break;
                }

            }
        }

        private void Update()
        {
            UpdateUserAction();
        }

        protected void UpdateUserAction()
        {
            // Update Input
            if (isBothCtrlerFound)
            {
                switch (CheckViveInput())
                {
                    //case ViveCtrlerMapping.TriggerAndGripPressed:
                    //    userAct.toggleCam |= recManager.toggleRecordingInput == ViveCtrlerMapping.TriggerAndGripPressed;
                    //Debug.Log ("Vive - Toggle Camera");
                    //    break;
                    //case ViveCtrlerMapping.bothGripPressed:
                    //   userAct.toggleCam |= recManager.toggleRecordingInput == ViveCtrlerMapping.bothGripPressed;
                    //userAct.toggleAvatar = true;
                    //Debug.Log ("Vive - Toggle Avatar");
                    //   break;
                    case ViveCtrlerMapping.bothTriggerPressed:
                        userAct.toggleRec |= recManager.toggleRecordingInput == ViveCtrlerMapping.bothTriggerPressed;
                        //userAct.toggleRec = true;
                        //Debug.Log ("Vive - Toggle Rec");
                        break;
                    case ViveCtrlerMapping.bothMenuKeyPressed:
                        userAct.toggleRec |= recManager.toggleRecordingInput == ViveCtrlerMapping.bothMenuKeyPressed;
                        break;
                    case ViveCtrlerMapping.leftMenuKeyPressed:
                        userAct.toggleRec |= recManager.toggleRecordingInput == ViveCtrlerMapping.leftMenuKeyPressed;
                        break;
                    case ViveCtrlerMapping.rightMenuKeyPressed:
                        userAct.toggleRec |= recManager.toggleRecordingInput == ViveCtrlerMapping.rightMenuKeyPressed;
                        break;
                }
            }

            CheckKeyboardInput();
        }

        protected void RefreshButtonState()
        {
            // Refresh State
            lTriggerPressed |= GetTriggerDown(Ctrler.leftHand);
            rTriggerPressed |= GetTriggerDown(Ctrler.rightHand);
            //lGripPressed |= GetGripDown(Ctrler.leftHand);
            //rGripPressed |= GetGripDown(Ctrler.rightHand);
            lMenuPressed |= GetMenuDown(Ctrler.leftHand);
            rMenuPressed |= GetMenuDown(Ctrler.rightHand);
        }

        protected ViveCtrlerMapping? CheckViveInput()
        {
            RefreshButtonState();

            anyTriggerPressed = lTriggerPressed || rTriggerPressed;
            anyGripPressed = lGripPressed || rGripPressed;
            anyMenuPressed = lMenuPressed || rMenuPressed;

            if (lTriggerPressed && rTriggerPressed)
            {
                lTriggerPressed = rTriggerPressed = false;
                return ViveCtrlerMapping.bothTriggerPressed;
            }
            //else if (lGripPressed && rGripPressed)
            //{
            //   lGripPressed = rGripPressed = false;
            //  return ViveCtrlerMapping.bothGripPressed;
            //}
            //else if (anyTriggerPressed && anyGripPressed)
            //{
            //   lGripPressed = rGripPressed = lTriggerPressed = rTriggerPressed = false;
            //  return ViveCtrlerMapping.TriggerAndGripPressed;
            //}
            else if (lMenuPressed && rMenuPressed)
            {
                lMenuPressed = rMenuPressed = false;
                return ViveCtrlerMapping.bothMenuKeyPressed;
            }
            else if (anyMenuPressed)
            {
                if (lMenuPressed)
                {
                    lMenuPressed = false;
                    return ViveCtrlerMapping.leftMenuKeyPressed;
                }
                else if (rMenuPressed)
                {
                    rMenuPressed = false;
                    return ViveCtrlerMapping.rightMenuKeyPressed;
                }
            }

            return null;
        }

        protected void CheckKeyboardInput()
        {
            if (recManager.toggleRecordingInput == ViveCtrlerMapping.KeyboardOnly_Key_R)
                userAct.toggleRec |= Input.GetKeyDown(KeyCode.R);
            if (recManager.toggleRecordingInput == ViveCtrlerMapping.KeyboardOnly_Key_X)
                userAct.toggleRec |= Input.GetKeyDown(KeyCode.X);
            if (recManager.toggleRecordingInput == ViveCtrlerMapping.KeyboardOnly_Key_V)
                userAct.toggleRec |= Input.GetKeyDown(KeyCode.V);
            //userAct.toggleAvatar |= Input.GetKeyDown (KeyCode.V);
            //userAct.toggleCam |= Input.GetKeyDown (KeyCode.B);
        }

        // Wrapper function for vive controller button state
        protected bool GetTriggerDown(Ctrler ctrler)
        {
            if (ctrler == Ctrler.leftHand)
                return Input.GetKeyDown(CtrlerKeyCode.viveTriggerL);
            if (ctrler == Ctrler.rightHand)
                return Input.GetKeyDown(CtrlerKeyCode.viveTriggerR);

            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetTriggerUp(Ctrler ctrler)
        {
            if (ctrler == Ctrler.leftHand)
                return Input.GetKeyUp(CtrlerKeyCode.viveTriggerL);
            if (ctrler == Ctrler.rightHand)
                return Input.GetKeyUp(CtrlerKeyCode.viveTriggerR);

            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetGripDown(Ctrler ctrler)
        {
            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetGripUp(Ctrler ctrler)
        {
            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetMenuUp(Ctrler ctrler)
        {
            if (ctrler == Ctrler.leftHand)
                return Input.GetKeyUp(CtrlerKeyCode.viveMenuL);
            if (ctrler == Ctrler.rightHand)
                return Input.GetKeyUp(CtrlerKeyCode.viveMenuR);

            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetMenuDown(Ctrler ctrler)
        {
            if (ctrler == Ctrler.leftHand)
                return Input.GetKeyDown(CtrlerKeyCode.viveMenuL);
            if (ctrler == Ctrler.rightHand)
                return Input.GetKeyDown(CtrlerKeyCode.viveMenuR);

            return false;
        }
    }
}