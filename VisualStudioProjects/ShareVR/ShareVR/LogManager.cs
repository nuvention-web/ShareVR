using UnityEngine;
using System.Collections.Generic;
using ShareVR.Capture;

namespace ShareVR.Utils
{
    using MetaData = Dictionary<string, string>;

    static class LogManager
    {
        private static MetaData metadata;
        private static VRCapture vrcap;

        public static MetaData Metadata { get => metadata; set => metadata = value; }

        public static void UpdateMetaData()
        {
            vrcap = VRCapture.Instance;

            // SDK Info
            metadata["sdk_version"] = GlobalParameters.SDKversion;
            metadata["unity_version"] = Application.version;

            // Client Info
            metadata["client_id"] = GlobalParameters.ClientID;
            metadata["game_string"] = GlobalParameters.GameString;

            // Recording Settings

        }
    }
}