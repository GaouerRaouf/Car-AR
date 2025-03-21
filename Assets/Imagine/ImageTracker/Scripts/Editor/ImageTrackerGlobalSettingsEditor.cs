using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Imagine.WebAR.Editor
{
    [CustomEditor(typeof(ImageTrackerGlobalSettings))]
    public class ImageTrackerGlobalSettingsEditor : UnityEditor.Editor
        {
        private ImageTrackerGlobalSettings _target;

        private void OnEnable()
        {
            _target = (ImageTrackerGlobalSettings)target;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            GUI.enabled = false;
            var prop = serializedObject.FindProperty("imageTargetInfos");
            EditorGUILayout.PropertyField(prop);
            GUI.enabled = true;
            EditorGUILayout.HelpBox("Note: Custom Imagetargets are not available in the Free version of this asset", MessageType.Warning, true);

            var bgc = GUI.backgroundColor;
            GUI.backgroundColor = new Color(340f / 255, 200f / 255, 440f / 255);
            if(GUILayout.Button("Upgrade Asset"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/camera/imagine-webar-image-tracker-240128");
            }
            GUI.backgroundColor = new Color(160f / 255, 300f / 255, 380f / 255);
            if (GUILayout.Button("Inquire on Discord"))
            {
                Application.OpenURL("https://discord.gg/ypNARJJEbB");
            }
        
            GUI.backgroundColor = bgc;

            EditorGUILayout.Space(20);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTrackerSettings"), true);

            serializedObject.ApplyModifiedProperties();


        }
    }
}
