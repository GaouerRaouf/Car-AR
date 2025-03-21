using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Imagine.WebAR.Editor
{
    [CustomEditor(typeof(ImageTracker))]
    public class ImageTrackerEditor : UnityEditor.Editor
    {
        private ImageTracker _target;

        private void OnEnable()
        {
            _target = (ImageTracker)target;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trackerCam"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("imageTargets"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trackerOrigin"));
            EditorGUILayout.Space(20);
            var overrideTrackerSettingsProp = serializedObject.FindProperty("overrideTrackerSettings");
            EditorGUILayout.PropertyField(overrideTrackerSettingsProp);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            var overrideTrackerSettingsPropVal = overrideTrackerSettingsProp.boolValue;
            if (!overrideTrackerSettingsPropVal)
            {
                GUI.enabled = false;
            }
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("trackerSettings"), true);
            EditorGUI.indentLevel--;
            GUI.enabled = true;
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(20);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnImageFound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("OnImageLost"));


            DrawEditorDebugger(); 

            serializedObject.ApplyModifiedProperties();
        }


        bool showKeyboardCameraControls = false;
        void DrawEditorDebugger(){
            //Editor Runtime Debugger
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Editor Debug Mode");
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if(Application.IsPlaying(_target)){
                //Enable Disable
                var imageTargetsProp = serializedObject.FindProperty("imageTargets");
                var trackedIdsProp = serializedObject.FindProperty("trackedIds");
                var trackedIds = new List<string>();
                if(trackedIdsProp != null){
                    for(var i = 0; i < trackedIdsProp.arraySize; i++){
                        trackedIds.Add(trackedIdsProp.GetArrayElementAtIndex(i).stringValue);
                    }
                }
                
                for(var i = 0; i < imageTargetsProp.arraySize; i++){
                    EditorGUILayout.BeginHorizontal();
                    var imageTargetProp = imageTargetsProp.GetArrayElementAtIndex(i);
                    var id = imageTargetProp.FindPropertyRelative("id").stringValue;
                    EditorGUILayout.LabelField(id);
                    var imageFound = trackedIds.Contains(id);
                    GUI.enabled = !imageFound;
                    if(GUILayout.Button("Found")){
                        _target.SendMessage("OnTrackingFound",id);

                        var imageTargetTransform = ((Transform)imageTargetProp.FindPropertyRelative("transform").objectReferenceValue);
                        var cam = ((ImageTrackerCamera)serializedObject.FindProperty("trackerCam").objectReferenceValue).transform;

                        cam.transform.position = imageTargetTransform.position + imageTargetTransform.forward * -3;
                        cam.LookAt(imageTargetTransform);
                    }
                    GUI.enabled = imageFound;
                    if(GUILayout.Button("Lost")){
                        _target.SendMessage("OnTrackingLost",id);
                    }
                    GUI.enabled = true;
                    EditorGUILayout.EndHorizontal();
                }    

                  
            }
            else{
                GUI.color = Color.yellow;
                EditorGUILayout.LabelField("Enter Play-mode to Debug In Editor");
                GUI.color = Color.white;
            }

            EditorGUILayout.Space();
            //keyboard camera controls
            showKeyboardCameraControls = EditorGUILayout.Toggle ("Show Keyboard Camera Controls", showKeyboardCameraControls);
            if(showKeyboardCameraControls){
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField("W", "Move Forward (Z)");
                EditorGUILayout.LabelField("S", "Move Backward (Z)");
                EditorGUILayout.LabelField("A", "Move Left (X)");
                EditorGUILayout.LabelField("D", "Move Right (X)");
                EditorGUILayout.LabelField("R", "Move Up (Y)");
                EditorGUILayout.LabelField("F", "Move Down (Y)");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Up Arrow", "Tilt Up (along X-Axis)");
                EditorGUILayout.LabelField("Down Arrow", "Tilt Down (along X-Axis)");
                EditorGUILayout.LabelField("Left Arrow", "Tilt Left (along Y-Axis)");
                EditorGUILayout.LabelField("Right Arrow", "Tilt Right (Along Y-Axis)");
                EditorGUILayout.LabelField("Period", "Tilt Clockwise (Along Z-Axis)");
                EditorGUILayout.LabelField("Comma", "Tilt Counter Clockwise (Along Z-Axis)");
                EditorGUILayout.Space(40);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("debugCamMoveSensitivity"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("debugCamTiltSensitivity"));
                EditorGUILayout.EndVertical();
                
            }    

            EditorGUILayout.EndVertical();

            
        }
    }
}

