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

            // Client Info
            metadata["ClientID"] = "VR Monster Awaken Team";

            // Recording Settings

        }
    }
}