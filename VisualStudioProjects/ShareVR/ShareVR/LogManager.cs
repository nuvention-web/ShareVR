using UnityEngine;
using System.Collections.Generic;

namespace ShareVR.Utils
{
    using MetaData = Dictionary<string, string>;

    static class LogManager
    {
        private static MetaData metadata;

        public static MetaData Metadata { get => metadata; set => metadata = value; }
    }
}