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
        // Behavior control
        internal const bool showDebugMessage = false;

        public const string SDKversion = "0.4c";
        internal const string ClientID = "Fight 4 Dream";
        internal const string GameName = "VR Monster Awaken";

        internal const string GameString = GameName;
        internal const string GameTag = GameName;

        public static int VideoID = 0;

        internal const string WatsonConfigJSON = "{"+
    "\"m_ClassifierDirectory\": \"Watson/Scripts/Editor/Classifiers/\","+
    "\"m_TimeOut\": 60.0,"+
    "\"m_MaxRestConnections\": 5,"+
    "\"m_Credentials\": ["+
    "    {"+
            "\"m_ServiceID\": \"SpeechToTextV1\","+
            "\"m_URL\": \"https://stream.watsonplatform.net/speech-to-text/api\","+
            "\"m_User\": \"87c7ea6b-ef50-48f2-b495-9707d182f844\","+
            "\"m_Password\": \"3juHm2Q455YW\","+
            "\"m_Apikey\": null,"+
            "\"m_Note\": null"+
    "    }"+
    "],"+
    "\"m_Variables\": []}";

        /*
    internal const string WatsonConfigJSON = "{"+
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
*/
        /*
            internal const string WatsonConfigJSON = "{" +
                 "\"url\": \"https://stream.watsonplatform.net/speech-to-text/api\"," +
                "\"username\": \"bffa6580-ea0d-4ca4-bb16-19603ff6bf24\"," +
                "\"password\": \"QFnb0nk8GrAm\"";*/
    }
}