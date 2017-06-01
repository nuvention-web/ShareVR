//======= Copyright (c) ShareVR ===========================
//
// Purpose: Controls avatar animation and status
// Version: 0.4b
// Chen Chen
// Date: 4/30/2017
//=========================================================
using UnityEngine;
using UnityEngine.VR;

namespace ShareVR.Core
{
    public class AvatarController : MonoBehaviour
    {
        struct AvatarHandTr
        {
            public Transform Left;
            public Transform Right;
        };

        [HideInInspector]
        public bool isAvatarEnabled = false;

        // Interal References
        private RecordManager recManager;

        // Player Animator
        private Animator anim;
        // Player Transform
        private Transform playerTr;

        // ID number of avatar that determines physical appearance in game
        private int avatarID = 0;

        // Avatar body transforms
        private bool EnableHandIK = false;
        private bool EnableGazeIK = false;

        // Avatar References
        private bool avatarHandAnchorFound = false;
        private Transform lookatTarget = null;
        private AvatarHandTr avatarHandTr;
        private Transform avatarHeadTr;

        // Local component reference
        private Transform m_Transform;

        private Vector3 avatarRefPos = new Vector3 (0.0f, 0f, 0f);
        private const float offsetCtrlGain = 2.0f;

        // UpdateAvatarPos related variables
        private Vector3 playerRot;

        // EstimateVelocity related variables
        private Vector3 prevPos;
        private Vector3 posDiff;
        private float velocity = 0.0f;
        // Velocity smooth damp
        private const float veloSmoothFactor = 4.0f;
        private float angle = 0.0f;

        // ShareVR Object Reference
        private InputManager inputManager;

        // Use this for initialization
        void Start()
        {
            prevPos = transform.position;

            inputManager = FindObjectOfType(typeof(InputManager)) as InputManager;

            InitializeReference();
            InitializeIK();
            AvatarSayHi();
        }

        void InitializeReference()
        {
            recManager = FindObjectOfType(typeof(RecordManager)) as RecordManager;

            playerTr = recManager.GetPlayerTransform();

            m_Transform = transform;
        }

        void LateUpdate()
        {
            //EstimatePlayerVelocity ();
            if (isAvatarEnabled)
                UpdateAvatarPos();
        }

        public void SetAvatarID( int ID )
        {
            avatarID = ID;
        }

        public int GetAvatarID()
        {
            return avatarID;
        }

        public void UpdateAvatarOffset( Vector3 offset )
        {
            avatarRefPos = offset;
        }

        public void EnableAvatar( bool state, float scale = 1.0f, Vector3? offset = null )
        {
            isAvatarEnabled = state;

            this.transform.localScale *= scale;
            this.gameObject.SetActive(isAvatarEnabled);

            EnableHandIK = InputManager.isBothCtrlerFound;

            if (offset != null)
                this.transform.position = new Vector3(offset.Value.x, offset.Value.y, offset.Value.z);
        }

        // Purpose: Update Animator IK status and target
        void OnAnimatorIK( int layerIndex )
        {
            if (EnableGazeIK)
            {
                anim.SetLookAtWeight(1, 1, 1);
                anim.SetLookAtPosition(lookatTarget.position);
            }

            if (EnableHandIK)
            {
                // Set weight
                anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                anim.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

                // Update target position and rotation
                anim.SetIKPosition(AvatarIKGoal.LeftHand, InputTracking.GetLocalPosition(VRNode.LeftHand));
                anim.SetIKPosition(AvatarIKGoal.RightHand, InputTracking.GetLocalPosition(VRNode.RightHand));
                anim.SetIKRotation(AvatarIKGoal.LeftHand, InputTracking.GetLocalRotation(VRNode.LeftHand));
                anim.SetIKRotation(AvatarIKGoal.RightHand, InputTracking.GetLocalRotation(VRNode.RightHand));
            }
            else
            {
                EnableHandIK = InputManager.isBothCtrlerFound;
            }
        }

        // Purpose: Initialize Animator IK based on SteamVR status
        private void InitializeIK()
        {
            anim = GetComponent<Animator>();

            avatarHeadTr = anim.GetBoneTransform(HumanBodyBones.Head);
            avatarHandTr.Left = anim.GetBoneTransform(HumanBodyBones.LeftHand);
            avatarHandTr.Right = anim.GetBoneTransform(HumanBodyBones.RightHand);

            // Will only use VR mode for now
            EnableHandIK |= avatarHandAnchorFound;
            EnableGazeIK = false;
        }
        // Purpose: Update avatar pose
        private void UpdateAvatarPos()
        {
            // Update Animator Controller Parameters
            anim.SetFloat("MoveSpeed", velocity);

            // Get Player Rotation
            playerRot = playerTr.eulerAngles;

            // Update Avatar position and rotation
            m_Transform.position = playerTr.position + avatarRefPos;
            m_Transform.eulerAngles = new Vector3(0.0f, playerRot.y, 0 * playerRot.z);

            // Update Head pitch angle only if gaze IK is not enabled
            if (!EnableGazeIK)
                avatarHeadTr.eulerAngles = new Vector3(playerRot.x, avatarHeadTr.eulerAngles.y, playerRot.z);
            //avatarHeadTr.eulerAngles = new Vector3 (avatarHeadTr.eulerAngles.x, avatarHeadTr.eulerAngles.y, -playerRot.x - 90.0f);
        }

        // Purpose: Estimate player velocity
        public void EstimatePlayerVelocity()
        {
            // Calculate movement vector
            posDiff = m_Transform.position - prevPos;
            prevPos = m_Transform.position;

            // Estimate movement direction
            angle = Vector3.Angle(posDiff, m_Transform.forward);

            // Determine diretion of motion
            if (angle < 100.0f)
                velocity = Mathf.Lerp(velocity, 0.2f * posDiff.magnitude / Time.deltaTime, veloSmoothFactor * Time.deltaTime);
            else
                velocity = Mathf.Lerp(velocity, -0.2f * posDiff.magnitude / Time.deltaTime, veloSmoothFactor * Time.deltaTime);

            // Noise Gate to Avoid Jitter
            if (Mathf.Abs(velocity) < 0.05f)
                velocity = 0.0f;
        }

        public void AvatarSayHi()
        {
            anim.SetTrigger("SayHi");
        }
    }

}