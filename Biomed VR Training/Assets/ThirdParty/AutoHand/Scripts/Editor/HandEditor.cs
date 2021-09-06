using UnityEditor;
using UnityEngine;

namespace Autohand {
    //[CustomEditor(typeof(Hand)), CanEditMultipleObjects]
    public class HandEditor : Editor {
        Hand hand;
        float lastOffset;
        Vector3 lastHandPos;
        Quaternion lastHandRot;

        private void OnEnable() {
            hand = target as Hand;
            hand.SetPalmRays();
            lastOffset = hand.gripOffset;
        }

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            EditorUtility.SetDirty(hand);

            EditorGUILayout.Space();
            var rect = EditorGUILayout.GetControlRect();
            rect.y += rect.height * 1.1f;
            rect.height *= 2.2f;
            rect.x -= 5;
            rect.y -= 6f;
            rect.width += 10;
            rect.height += 18;
            EditorGUI.DrawRect(rect, Color.gray);

            if(hand.gripOffset != lastOffset) {
                lastOffset = hand.gripOffset;
                hand.RelaxHand();
            }
            
            if(GUILayout.Button("Save Opened Hand")) {
                if(hand.fingers == null)
                    Debug.LogError("Fingers not set");
                else{
                    foreach(var finger in hand.fingers) {
                        EditorUtility.SetDirty(finger);
                        finger.SetMinPose();
                    }
                }
                Debug.Log("Open Pose Set");
            }
            EditorGUILayout.Space();
            if(GUILayout.Button("Save Closed Hand")) {
                if(hand.fingers == null)
                    Debug.LogError("Fingers not set");
                else{
                    foreach(var finger in hand.fingers) {
                        EditorUtility.SetDirty(finger);
                        finger.SetMaxPose();
                    }
                    Debug.Log("Closed Pose Set");
                }
            }

            if(hand.editorAutoGrab && (hand.transform.position != lastHandPos || hand.transform.rotation != lastHandRot)){
                hand.ProceduralFingerBend();
                lastHandRot = hand.transform.rotation;
                lastHandPos = hand.transform.position;
            }
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            Repaint();
        }
    }
}
