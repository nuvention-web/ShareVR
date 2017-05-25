//======= Copyright (c) ShareVR ===============
//
// Purpose: Manage input events
// Version: SDK v0.4b
// Chen Chen
// 4/30/2017
//=============================================

using System.Collections;
using UnityEngine;
using System;
using Valve.VR;

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

    public enum ViveCtrlerMapping
    {
        bothTriggerPressed,
        bothGripPressed,
        TriggerAndGripPressed,
        bothMenuKeyPressed,
        leftMenuKeyPressed,
        rightMenuKeyPressed,
        KeyboardOnly_Key_R,
        KeyboardOnly_Key_V,
        KeyboardOnly_Key_X,
        DontUseAnyKey
    }

    [Serializable]
    public class PlayerHandTransform
    {
        public Transform leftHand;
        public Transform rightHand;

        public bool isHandTransformValid
        {
            get
            {
                return ( leftHand != null ) & ( rightHand != null );
            }
        }
    }

    public class InputManager : MonoBehaviour
    {
        [HideInInspector]
        public UserAction userAct = new UserAction ();
        [HideInInspector]
        public bool isViveDeviceFound = false;

        [HideInInspector]
        protected enum ViveCtrler
        {
            leftHand,
            rightHand
        }

        protected class ViveCtrlerMappingMask
        {
            public bool bothTriggerPressed = true;
            public bool bothGripPressed = true;
            public bool TriggerAndGripPressed = true;
        }

        // SteamVR Objects
        protected  uint leftHand, rightHand;
        [NonSerializedAttribute]
        public SteamVR_Controller.Device leftCtrlerDevice;
        [NonSerializedAttribute]
        public SteamVR_Controller.Device rightCtrlerDevice;
        [HideInInspector]
        public SteamVR_TrackedController[] viveCtrlers;


        // Internal Reference
        protected RecordManager recManager;

        protected ViveCtrlerMappingMask viveCtrlerMapMask;
        protected bool lTriggerPressed = false;
        protected bool rTriggerPressed = false;
        protected bool lGripPressed = false;
        protected bool rGripPressed = false;
        protected bool anyTriggerPressed = false;
        protected bool anyGripPressed = false;
        protected bool lMenuPressed = false;
        protected bool rMenuPressed = false;
        protected bool anyMenuPressed = false;

        // Use this for initialization
        protected virtual IEnumerator Start()
        {
            yield break;
        }

        // Update is called once per frame
        protected void Update()
        {
            UpdateUserAction();
        }

        public void ViveHapticPulse( ushort duration = 500 )
        {
            leftCtrlerDevice.TriggerHapticPulse(duration);
            rightCtrlerDevice.TriggerHapticPulse(duration);
        }

        protected void UpdateUserAction()
        {
            // Update Vive Input
            if (isViveDeviceFound)
            {
                switch (CheckViveInput())
                {
                    case ViveCtrlerMapping.TriggerAndGripPressed:
                        userAct.toggleRec |= recManager.toggleRecordingInput == ViveCtrlerMapping.TriggerAndGripPressed;
                        //Debug.Log ("Vive - Toggle Camera");
                        break;
                    case ViveCtrlerMapping.bothGripPressed:
                        userAct.toggleRec |= recManager.toggleRecordingInput == ViveCtrlerMapping.bothGripPressed;
                        //userAct.toggleAvatar = true;
                        //Debug.Log ("Vive - Toggle Avatar");
                        break;
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

        protected ViveCtrlerMapping? CheckViveInput()
        {
            // Refresh State
            lTriggerPressed |= GetTriggerDown(ViveCtrler.leftHand);
            rTriggerPressed |= GetTriggerDown(ViveCtrler.rightHand);
            lGripPressed |= GetGripDown(ViveCtrler.leftHand);
            rGripPressed |= GetGripDown(ViveCtrler.rightHand);
            lMenuPressed |= GetMenuDown(ViveCtrler.leftHand);
            rMenuPressed |= GetMenuDown(ViveCtrler.rightHand);

            anyTriggerPressed = lTriggerPressed || rTriggerPressed;
            anyGripPressed = lGripPressed || rGripPressed;
            anyMenuPressed = lMenuPressed || rMenuPressed;

            if (lTriggerPressed && rTriggerPressed)
            {
                lTriggerPressed = rTriggerPressed = false;
                return ViveCtrlerMapping.bothTriggerPressed;
            }
            else if (lGripPressed && rGripPressed)
            {
                lGripPressed = rGripPressed = false;
                return ViveCtrlerMapping.bothGripPressed;
            }
            else if (anyTriggerPressed && anyGripPressed)
            {
                lGripPressed = rGripPressed = lTriggerPressed = rTriggerPressed = false;
                return ViveCtrlerMapping.TriggerAndGripPressed;
            }
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
            if (recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.KeyboardOnly_Key_R)
                userAct.toggleRec |= Input.GetKeyDown(KeyCode.R);
            else if (recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.KeyboardOnly_Key_X)
                userAct.toggleRec |= Input.GetKeyDown(KeyCode.X);
            else if (recManager.toggleRecordingInput == ShareVR.Core.ViveCtrlerMapping.KeyboardOnly_Key_V)
                userAct.toggleRec |= Input.GetKeyDown(KeyCode.V);
        }

        // Wrapper function for vive controller button state
        protected bool GetTriggerDown( ViveCtrler ctrler )
        {
            if (ctrler == ViveCtrler.leftHand)
                return leftCtrlerDevice.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger);
            else if (ctrler == ViveCtrler.rightHand)
                return rightCtrlerDevice.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger);

            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetTriggerUp( ViveCtrler ctrler )
        {
            if (ctrler == ViveCtrler.leftHand)
                return leftCtrlerDevice.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger);
            if (ctrler == ViveCtrler.rightHand)
                return rightCtrlerDevice.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger);

            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetGripDown( ViveCtrler ctrler )
        {
            if (ctrler == ViveCtrler.leftHand)
                return leftCtrlerDevice.GetPressDown(EVRButtonId.k_EButton_Grip);
            else if (ctrler == ViveCtrler.rightHand)
                return rightCtrlerDevice.GetPressDown(EVRButtonId.k_EButton_Grip);

            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetGripUp( ViveCtrler ctrler )
        {
            if (ctrler == ViveCtrler.leftHand)
                return leftCtrlerDevice.GetPressUp(EVRButtonId.k_EButton_Grip);
            else if (ctrler == ViveCtrler.rightHand)
                return rightCtrlerDevice.GetPressUp(EVRButtonId.k_EButton_Grip);

            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetMenuUp( ViveCtrler ctrler )
        {
            if (ctrler == ViveCtrler.leftHand)
                return leftCtrlerDevice.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu);
            else if (ctrler == ViveCtrler.rightHand)
                return rightCtrlerDevice.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu);

            return false;
        }
        // Wrapper function for vive controller button state
        protected bool GetMenuDown( ViveCtrler ctrler )
        {
            if (ctrler == ViveCtrler.leftHand)
                return leftCtrlerDevice.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu);
            else if (ctrler == ViveCtrler.rightHand)
                return rightCtrlerDevice.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu);

            return false;
        }
    }
}