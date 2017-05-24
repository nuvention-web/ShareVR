//======= Copyright (c) ShareVR ===============================
//
// Purpose: Global parameters
// Version: 0.4b
// Chen Chen 
// 4/30/2017
//=============================================================
namespace ShareVR.Utils
{
    public static class GlobalParameters
    {
        public const string SDKversion = "0.4c";
        internal const string ClientID = "August";
        internal const string GameName = "Beta Testing";

        internal const string GameString = GameName;
        internal const string GameTag = ClientID;

        public static int VideoID = 0;

        public const string WatsonConfigJSON = "{"+
    "\"m_ClassifierDirectory\": \"Watson/Scripts/Editor/Classifiers/\","+
    "\"m_TimeOut\": 30.0,"+
    "\"m_MaxRestConnections\": 5,"+
    "\"m_Credentials\": ["+
    "    {"+
            "\"m_ServiceID\": \"SpeechToTextV1\","+
            "\"m_URL\": \"https://stream.watsonplatform.net/speech-to-text/api\","+
            "\"m_User\": \"12287df8-eac5-49ee-a660-76437eb6626f\","+
            "\"m_Password\": \"AhOF708NXy5l\","+
            "\"m_Apikey\": null,"+
            "\"m_Note\": null"+
    "    }"+
    "],"+
    "\"m_Variables\": []}";
    }
}