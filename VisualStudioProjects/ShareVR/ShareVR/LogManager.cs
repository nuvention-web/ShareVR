using UnityEngine;
using System.Collections.Generic;
using ShareVR.Capture;
using System.IO;

namespace ShareVR.Utils
{
    using MetaData = Dictionary<string, string>;

    static class LogManager
    {
        private static MetaData metadata = new MetaData();
        //private static VRCapture vrcap;

        public static MetaData Metadata { get => metadata; set => metadata = value; }

        public static void UpdateMetaData( string playerName = null )
        {
            //vrcap = VRCapture.Instance;

            // SDK Info
            metadata["sdk_version"] = GlobalParameters.SDKversion;
            metadata["unity_version"] = Application.unityVersion;

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

            // Recording Settings
            //Debug.Log("Creating log...");
            //Debug.Log(metadata);
        }
    }
}