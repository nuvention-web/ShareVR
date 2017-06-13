using UnityEngine;
using System.Collections.Generic;
using ShareVR.Capture;
using ShareVR.Core;
using System.IO;

namespace ShareVR.Utils
{
    using MetaData = Dictionary<string, string>;

    internal static class ConfigSetContainer
    {
        private static RecordManager m_recManager = null;

        public static RecordManager recManager
        {
            get
            {
                // Search and store local copy
                if (m_recManager == null)
                    m_recManager = RecordManager._Instance;

                return m_recManager;
            }
        }

        public static CameraFollowMethod cameraFollowMethod
        {
            get
            {
                return recManager.cameraFollowMethod;
            }
        }

        public static float cameraOrbitSpeed
        {
            get
            {
                return recManager.cameraOrbitSpeed;
            }
        }

        public static float camHeight
        {
            get
            {
                return recManager.camHeight;
            }
        }
        public static float camDistance
        {
            get
            {
                return recManager.camDistance;
            }
        }
        public static float camAngle
        {
            get
            {
                return recManager.camAngle;
            }
        }
        public static float camMotionDamp
        {
            get
            {
                return recManager.camMotionDamp;
            }
        }
        public static bool showCameraModel
        {
            get
            {
                return recManager.showCameraModel;
            }
        }
        public static float cameraModelScale
        {
            get
            {
                return recManager.cameraModelScale;
            }
        }
        public static bool showCameraPreview
        {
            get
            {
                return recManager.showCameraPreview;
            }
        }

        public static bool showPlayerAvatar
        {
            get
            {
                return recManager.showPlayerAvatar;
            }
        }
        public static float avatarScale
        {
            get
            {
                return recManager.avatarScale;
            }
        }
        public static Vector3 avatarOffset
        {
            get
            {
                return recManager.avatarOffset;
            }
        }

        public static ViveCtrlerMapping toggleRecordingInput
        {
            get
            {
                return recManager.toggleRecordingInput;
            }
        }
        public static bool useVoiceCommand
        {
            get
            {
                return recManager.useVoiceCommand;
            }
        }

        public static GameObject trackingTarget
        {
            get
            {
                return recManager.trackingTarget;
            }
        }
        public static bool showDebugMessage
        {
            get
            {
                return recManager.showDebugMessage;
            }
        }

        public static FrameSizeType frameSize
        {
            get
            {
                return recManager.frameSize;
            }
        }
        public static TargetFramerateType frameRate
        {
            get
            {
                return recManager.frameRate;
            }
        }
        public static string saveFolder
        {
            get
            {
                return recManager.saveFolder;
            }
        }
        public static string saveFileName
        {
            get
            {
                return recManager.saveFileName;
            }
        }

        public static bool uploadFileOnline
        {
            get
            {
                return recManager.uploadFileOnline;
            }
        }
    }

    internal static class MetaDataContainer
    {
        private static MetaData metadata = new MetaData();

        public static MetaData Metadata { get => metadata; set => metadata = value; }

        public static string unityVersion;

        public static void UpdateMetaData(string playerName = null)
        {
            // SDK Info
            metadata["sdk_version"] = GlobalParameters.SDKversion;
            metadata["unity_version"] = unityVersion;

            // Client and Game Info
            metadata["client_id"] = GlobalParameters.ClientID;
            metadata["game_string"] = GlobalParameters.GameString;
            metadata["game_name"] = GlobalParameters.GameName;
            metadata["video_id"] = GlobalParameters.VideoID.ToString();

            // Player Info
            string playerNameFile = VRCaptureUtils.ShareVRConfigPath + "ShareVR_Config.txt";
            if (File.Exists(playerNameFile))
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(playerNameFile))
                {
                    // Read the stream to a string
                    metadata["player_name"] = sr.ReadToEnd();
                }
            }
            else if (!string.IsNullOrEmpty(playerName))
            {
                metadata["player_name"] = playerName;
            }
        }
    }

    internal static class LogManager
    {
        public static MetaData GetMetadata
        {
            get
            {
                MetaDataContainer.UpdateMetaData(ConfigSetContainer.saveFileName);

                return MetaDataContainer.Metadata;
            }
        }
    }
}