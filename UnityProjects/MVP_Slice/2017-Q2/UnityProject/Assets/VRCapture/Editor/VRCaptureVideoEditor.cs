using UnityEditor;

namespace VRCapture.Editor {
    /// <summary>
    /// VR Capture video editor.
    /// </summary>
    [CustomEditor(typeof(VRCaptureVideo))]
    public class VRCaptureVideoEditor : UnityEditor.Editor {
        VRCaptureVideo captureVideo;

        void Awake() {
            captureVideo = (VRCaptureVideo)target;
        }

        public override void OnInspectorGUI() {
            captureVideo.formatType =
                (VRCaptureVideo.FormatType)EditorGUILayout.EnumPopup(
                "Format Type", captureVideo.formatType);
            if(captureVideo.formatType == VRCaptureVideo.FormatType.NORMAL) {
                captureVideo.frameSize = (VRCaptureVideo.FrameSizeType)EditorGUILayout.EnumPopup(
                    "Frame Size", captureVideo.frameSize);
            }
            else if(captureVideo.formatType == VRCaptureVideo.FormatType.PANORAMA) {
                captureVideo.projectionType = (VRCaptureVideo.PanoramaProjectionType)EditorGUILayout.EnumPopup(
                    "Projection Type", captureVideo.projectionType);
                if(captureVideo.projectionType == VRCaptureVideo.PanoramaProjectionType.EQUIRECTANGULAR) {
                    captureVideo.frameSize = (VRCaptureVideo.FrameSizeType)EditorGUILayout.EnumPopup(
                        "Frame Size", captureVideo.frameSize);
                }
                captureVideo.cubemapSize = (VRCaptureVideo.CubemapSizeType)EditorGUILayout.EnumPopup(
                    "Cubemap Size", captureVideo.cubemapSize);
                captureVideo.offlineRender = EditorGUILayout.Toggle(
                    "Offline Render", captureVideo.offlineRender);
            }
            captureVideo.encodeQuality = (VRCaptureVideo.EncodeQualityType)EditorGUILayout.EnumPopup(
                "Encode Quality", captureVideo.encodeQuality);
            captureVideo.antiAliasing = (VRCaptureVideo.AntiAliasingType)EditorGUILayout.EnumPopup(
                "Anti Aliasing", captureVideo.antiAliasing);
            captureVideo.targetFramerate = (VRCaptureVideo.TargetFramerateType)EditorGUILayout.EnumPopup(
                "Target FrameRate", captureVideo.targetFramerate);
            captureVideo.isDedicated = EditorGUILayout.Toggle(
                "Dedicated Camera", captureVideo.isDedicated);
            captureVideo.isEnabled = EditorGUILayout.Toggle(
                "Enabled", captureVideo.isEnabled);
        }
    }
}
