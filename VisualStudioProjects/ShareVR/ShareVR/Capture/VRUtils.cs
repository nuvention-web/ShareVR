using System;
using UnityEngine;
using System.IO;
using ShareVR.Utils;

namespace ShareVR.Capture
{
    ////////////////////////////////////////////////////////////////////////////
    ///                           Common Utils.                              ///
    ////////////////////////////////////////////////////////////////////////////
    public class VRCommonUtils
    {
        internal const float EPSILON = 0.000001f;
        internal static string DATA_PATH = Application.dataPath;
        internal static string PERSISTENT_DATA_PATH = Application.persistentDataPath;
        internal static string STREAMING_ASSETS_PATH = Application.streamingAssetsPath;
        internal static string MY_DOCUMENTS_PATH = Environment.GetFolderPath (
                                                     Environment.SpecialFolder.MyDocuments);

        internal static string GetTimeString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd-HH-mm");
        }

        internal static string GetPngFileName()
        {
            return GetPngFileName(null);
        }

        internal static string GetPngFileName( string name )
        {
            return GetTimeString() + ( name == null ? "" : "-" ) + name + ".png";
        }

        internal static string GetMp4FileName()
        {
            return GetMp4FileName(null);
        }

        internal static string GetMp4FileName( string name )
        {
            string fileName;
            int videoID = 0;

            //fileName = GetTimeString () + "-Camera-" + (name ?? "?") + "-Session-" + videoID + ".mp4";
            fileName = GlobalParameters.ClientID + "-Session-" + videoID + ".mp4";
            while (File.Exists(VRCaptureUtils.SaveFolder + fileName))
            {
                videoID++;
                fileName = GlobalParameters.ClientID + "-Session-" + videoID + ".mp4";
            }

            return fileName;
        }

        public static string GetFinalVideoFileName()
        {
            string fileName;
            string namePrefix = null;
            int videoID = 0;
            string playerNameFile = VRCaptureUtils.ShareVRConfigPath + "ShareVR_Config.txt";

            if (File.Exists(playerNameFile))
            {
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(playerNameFile))
                {
                    // Read the stream to a string
                    namePrefix = sr.ReadToEnd().Replace(" ", "_");
                }
            }
            else if (!String.IsNullOrEmpty(VRCaptureUtils.SaveFileName))
                namePrefix = VRCaptureUtils.SaveFileName;

            if (String.IsNullOrEmpty(namePrefix))
                fileName = GlobalParameters.GameName + "-Demo-" + videoID + ".mp4";
            else
                fileName = GlobalParameters.GameName + "-" + namePrefix + "-Demo-" + videoID + ".mp4";

            // Make sure there's no space in the filename, otherwise it will raise exception on AWS Lambda
            fileName = fileName.Replace(" ", "_");

            while (File.Exists(VRCaptureUtils.SaveFolder + fileName))
            {
                videoID++;
                if (String.IsNullOrEmpty(namePrefix))
                    fileName = GlobalParameters.GameName.Replace(" ", "_") + "-Demo-" + videoID + ".mp4";
                else
                    fileName = GlobalParameters.GameName.Replace(" ", "_") + "-" + namePrefix + "-Demo-" + videoID + ".mp4";

                fileName = fileName.Replace(" ", "_");
            }
            GlobalParameters.VideoID = videoID;
            return fileName;
        }

        internal static string GetWavFileName()
        {
            return GetWavFileName(null);
        }

        internal static string GetWavFileName( string name )
        {
            string fileName;
            int audioFileID = 0;

            //fileName = GetTimeString () + "-Session-" + audioFileID + ".wav";
            fileName = GlobalParameters.ClientID + "-Session-" + audioFileID + ".wav";
            while (File.Exists(VRCaptureUtils.SaveFolder + fileName))
            {
                audioFileID++;
                fileName = GlobalParameters.ClientID + "-Session-" + audioFileID + ".wav";
            }

            return fileName;
        }

        internal static string GetTxtFileName()
        {
            return GetTxtFileName(null);
        }

        internal static string GetTxtFileName( string name )
        {
            return GetTimeString() + ( name == null ? "" : "-" ) + name + ".txt";
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ///                    Basic Utils for VRCapture.                        ///
    ////////////////////////////////////////////////////////////////////////////
    public class VRCaptureUtils
    {
        // ShareVR
        private static bool m_userDefinedSaveFolder = false;
        private static string m_saveFolder;
        private static string m_saveFileName = null;

        public static string SaveFolder
        {
            get
            {
                if (m_userDefinedSaveFolder)
                {
                    // Check folder
                    if (m_saveFolder != null)
                    {
                        if (!Directory.Exists(m_saveFolder))
                            Directory.CreateDirectory(m_saveFolder);
                        return m_saveFolder;
                    }
                    else
                        return VRCommonUtils.MY_DOCUMENTS_PATH + "/ShareVR/";
                }
                else
                    return VRCommonUtils.MY_DOCUMENTS_PATH + "/ShareVR/";
            }
            set
            {
                if (value.Length > 1)
                {
                    m_saveFolder = value + "/";
                    m_saveFolder = Path.GetFullPath(m_saveFolder);
                    m_userDefinedSaveFolder = true;
                }
                else
                {
                    m_saveFolder = Path.GetFullPath(VRCommonUtils.MY_DOCUMENTS_PATH + "/ShareVR/");
                    m_userDefinedSaveFolder = false;
                }
            }
        }

        public static string SaveFileName
        {
            get { return m_saveFileName; }
            set { m_saveFileName = value; }
        }

        public static string GetCurrentSaveFolder
        {
            get
            {
                return m_saveFolder;
            }
        }

        public static string FFmpegEditorFolder
        {
            get
            {
                return VRCommonUtils.DATA_PATH + "/ShareVR/Plugins/ThirdParty/FFmpeg/";
            }
        }

        public static string FFmpegEditorPath
        {
            get
            {
                return FFmpegEditorFolder + "ffmpeg.exe";
            }
        }

        public static string FFmpegBuildFolder
        {
            get
            {
                return VRCommonUtils.STREAMING_ASSETS_PATH + "/ShareVR/";
            }
        }

        public static string ShareVRAssetsPath
        {
            get
            {
                return VRCommonUtils.STREAMING_ASSETS_PATH + "/ShareVR/";
            }
        }

        public static string ShareVRConfigPath
        {
            get
            {
                return VRCommonUtils.MY_DOCUMENTS_PATH + "/ShareVR/";
            }
        }

        public static string FFmpegBuildPath
        {
            get
            {
                return FFmpegBuildFolder + "ffmpeg.exe";
            }
        }

        public static string FFmpegPath
        {
            get
            {
#if UNITY_EDITOR
				return FFmpegEditorPath;
#else
                return FFmpegBuildPath;
#endif
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ///                     VR device releated Utils.                        ///
    ////////////////////////////////////////////////////////////////////////////
    internal class VRDeviceUtils
    {

        public static bool UseSteamVR
        {
            get
            {
                Type steamvr = Type.GetType ("SteamVR");
                if (steamvr != null)
                {
                    return true;
                }
                return false;
            }
        }
    }

    ////////////////////////////////////////////////////////////////////////////
    ///                     Serializable data struct.                        ///
    ////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Serializable Vector2 to replace Unity Vector2 struct.
    /// </summary>
    [Serializable]
    internal struct SerializableVector2
    {
        public float x;
        public float y;

        public SerializableVector2( Vector2 v2 )
        {
            x = v2.x;
            y = v2.y;
        }

        public Vector2 ToVector2()
        {
            return new Vector3(x, y);
        }
    }

    /// <summary>
    /// Serializable Vector3 to replace Unity Vector3 struct.
    /// </summary>
    [Serializable]
    internal struct SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializableVector3( Vector3 v3 )
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    /// <summary>
    /// Serializable Quaternion to replace Unity Quaternion struct.
    /// </summary>
    [Serializable]
    internal struct SerializableQuaternion
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public SerializableQuaternion( Quaternion q )
        {
            x = q.x;
            y = q.y;
            z = q.z;
            w = q.w;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }

    /// <summary>
    /// Serializable Transform to replace Unity Transform struct.
    /// </summary>
    [Serializable]
    internal class SerializableTransform
    {
        public string Name { get; private set; }

        SerializableVector3 postion;
        SerializableQuaternion rotation;

        public SerializableTransform( string name, Vector3 pos, Quaternion rot )
        {
            Name = name;
            postion = new SerializableVector3(pos);
            rotation = new SerializableQuaternion(rot);
        }

        public Vector3 GetPostion()
        {
            return postion.ToVector3();
        }

        public Quaternion GetRotation()
        {
            return rotation.ToQuaternion();
        }
    }
}