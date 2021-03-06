using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;
using UnityEditor.SceneManagement;



namespace Autohand {
    [CustomEditor(typeof(GrabbablePoseAdvanced))]
    public class GrabPoseRadiusEditor : Editor {
        GrabbablePoseAdvanced grabbablePose;

        private void OnEnable() {
            grabbablePose = target as GrabbablePoseAdvanced;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (grabbablePose.gameObject.scene.name == null) {
                EditorGUILayout.LabelField("This must be saved in the scene");
                EditorGUILayout.LabelField("-> then use override to prefab");
                return;
            }

            if (grabbablePose.gameObject != null && PrefabStageUtility.GetPrefabStage(grabbablePose.gameObject) == null)
            {
                EditorUtility.SetDirty(grabbablePose);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                grabbablePose.editorHand = (Hand)EditorGUILayout.ObjectField("Editor Hand", grabbablePose.editorHand, typeof(Hand), true);
                grabbablePose.useEditorAutoGrab = EditorGUILayout.Toggle("Use Editor Auto Grab", grabbablePose.useEditorAutoGrab);

                var rect = EditorGUILayout.GetControlRect();
                if (grabbablePose.rightPoseSet)
                    EditorGUI.DrawRect(rect, Color.green);
                else
                    EditorGUI.DrawRect(rect, Color.red);

                rect.width -= 4;
                rect.height -= 2;
                rect.x += 2;
                rect.y += 1;

                if (GUI.Button(rect, "Save Right Pose"))
                    grabbablePose.EditorSaveGrabPose(grabbablePose.editorHand, false);


                rect = EditorGUILayout.GetControlRect();
                if (grabbablePose.leftPoseSet)
                    EditorGUI.DrawRect(rect, Color.green);
                else
                    EditorGUI.DrawRect(rect, Color.red);

                rect.x += 2;
                rect.y += 1;
                rect.width -= 4;
                rect.height -= 2;

                if (GUI.Button(rect, "Save Left Pose"))
                    grabbablePose.EditorSaveGrabPose(grabbablePose.editorHand, true);




                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUILayout.Label(new GUIContent("-------- For tweaking poses --------"), new GUIStyle() { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
                GUILayout.Label(new GUIContent("This will create a copy that should be deleted"), new GUIStyle() { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });

                if (GUILayout.Button("Create Copy - Set Pose")) {
                    grabbablePose.EditorCreateCopySetPose(grabbablePose.editorHand, grabbablePose.centerObject ? grabbablePose.centerObject : grabbablePose.transform);
                    EditorGUIUtility.PingObject(grabbablePose.editorHand);
                }
                if(GUILayout.Button("Select Copy")) { 
                    Selection.activeGameObject = grabbablePose.editorHand.gameObject;
                }

                if (GUILayout.Button("Reset Hand"))
                    grabbablePose.editorHand.RelaxHand();

                EditorGUILayout.Space();
                rect = EditorGUILayout.GetControlRect();
                EditorGUI.DrawRect(rect, Color.red);

                if (GUILayout.Button("Delete Copy")) {
                    if (string.Equals(grabbablePose.editorHand.transform.parent.name, "HAND COPY CONTAINER DELETE"))
                        DestroyImmediate(grabbablePose.editorHand.transform.parent.gameObject);
                    else
                        Debug.LogError("Not a copy - Will not delete");
                }
                if (GUILayout.Button("Clear Poses")) {
                    grabbablePose.EditorClearPoses();
                }
            }
            else {
                GUILayout.Label(new GUIContent(" - This will not work in prefab mode - "), new GUIStyle() { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
                GUILayout.Label(new GUIContent("Use scene to create poses"), new GUIStyle() { alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}
