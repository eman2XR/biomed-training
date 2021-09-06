using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Autohand{


#if UNITY_EDITOR
    [CanEditMultipleObjects]
#endif
    [RequireComponent(typeof(Grabbable))]
    public class GrabbablePose : MonoBehaviour{
        [Header("Pose")]
        [Tooltip("Purely for orginizational purposes in the editor")]
        public string poseName = "";
        public int poseIndex = 0;
#if UNITY_EDITOR
        [Header("Editor")]
        [Tooltip("Used to pose for the grabbable")]
        [HideInInspector]
        public Hand editorHand;
        [Tooltip("This will be set false when setting already saved poses, you can turn it on manually through the hand")]
        [HideInInspector]
        public bool useEditorAutoGrab = true;
#endif
        [HideInInspector]
        public HandPoseData rightPose;
        [HideInInspector]
        public bool rightPoseSet = false;
        [HideInInspector]
        public HandPoseData leftPose;
        [HideInInspector]
        public bool leftPoseSet = false;


        Grabbable grabbable;

        public void Start() {
            OnStart();
        }

        protected virtual void OnStart() {


            grabbable = GetComponent<Grabbable>();
            if (!leftPoseSet && !rightPoseSet) {
                Debug.LogError("Grabbable Pose has not been set for either hand", this);
                grabbable.enabled = false;
            }
            else if (!leftPoseSet && rightPoseSet && !(grabbable.handType == HandType.right)) {
                Debug.Log("Setting Grabbable to right hand only because left handed pose not set", this);
                grabbable.handType = HandType.right;
            }
            else if (leftPoseSet && !rightPoseSet && !(grabbable.handType == HandType.left)) {
                Debug.Log("Setting Grabbable to left hand only because right handed pose not set", this);
                grabbable.handType = HandType.left;
            }

            var poses = GetComponents<GrabbablePose>();
            if(poses.Length > 0){
                if(gameObject.CanGetComponent(out GrabbablePoseCombiner combiner)) {
                    combiner.poses = poses;
                }
                else{
                    gameObject.AddComponent<GrabbablePoseCombiner>().poses = poses;
                }

            }
        }

        public bool CanSetPose(Hand hand) {
            if(hand.poseIndex != poseIndex)
                return false;
            if(hand.left && !leftPoseSet)
                return false;
            if(!hand.left && !rightPoseSet)
                return false;
            return true;
        }

        public virtual HandPoseData GetHandPoseData(Hand hand) {
            return (hand.left) ? leftPose : rightPose;
        }


        public virtual void SetHandPose(Hand hand) {
            GetHandPoseData(hand).SetPose(hand, transform);
        }

        public HandPoseData GetNewPoseData(Hand hand) {
            var pose = new HandPoseData();

            var posePositionsList = new List<Vector3>();
            var poseRotationsList = new List<Quaternion>();

            var handParent = hand.transform.parent;
            hand.transform.parent = transform;
            pose.handOffset = hand.transform.localPosition;
            pose.localQuaternionOffset = hand.transform.localRotation;
            hand.transform.parent = handParent;

            foreach (var finger in hand.fingers) {
                AssignChildrenPose(finger.transform);
            }

            void AssignChildrenPose(Transform obj) {
                AddPoint(obj.localPosition, obj.localRotation);
                for (int j = 0; j < obj.childCount; j++) {
                    AssignChildrenPose(obj.GetChild(j));
                }
            }

            void AddPoint(Vector3 pos, Quaternion rot) {
                posePositionsList.Add(pos);
                poseRotationsList.Add(rot);
            }

            pose.posePositions = new Vector3[posePositionsList.Count];
            pose.poseRotations = new Quaternion[posePositionsList.Count];
            for (int i = 0; i < posePositionsList.Count; i++) {
                pose.posePositions[i] = posePositionsList[i];
                pose.poseRotations[i] = poseRotationsList[i];
            }

            return pose;
        }
        
#if UNITY_EDITOR
        //This is because parenting is used at runtime, but cannot be used on prefabs in editor so a copy is required
        public void EditorCreateCopySetPose(Hand hand, Transform relativeTo){
            var useEditorGrab = useEditorAutoGrab;

            
            Hand handCopy;
            if (hand.name != "HAND COPY DELETE")
                handCopy = Instantiate(hand, relativeTo.transform.position, hand.transform.rotation);
            else
                handCopy = hand;

            handCopy.name = "HAND COPY DELETE";
            editorHand = handCopy;

            if(hand.left && leftPoseSet){
                useEditorGrab = false;
                leftPose.SetPose(handCopy, transform);
            }
            else if(!hand.left && rightPoseSet){
                useEditorGrab = false;
                rightPose.SetPose(handCopy, transform);
            }
            else
            {
                handCopy.transform.position = relativeTo.transform.position; 
                editorHand.RelaxHand();
            }
            
            handCopy.editorAutoGrab = useEditorGrab;

            var contrainer = new GameObject();
            contrainer.name = "HAND COPY CONTAINER DELETE";
            contrainer.transform.position = relativeTo.transform.position;
            contrainer.transform.rotation = relativeTo.transform.rotation;
            handCopy.transform.parent = contrainer.transform;
            EditorGUIUtility.PingObject(handCopy);
        }

        public void EditorSaveGrabPose(Hand hand, bool left){
            var pose = new HandPoseData();
            
            hand.left = left;

            var posePositionsList = new List<Vector3>();
            var poseRotationsList = new List<Quaternion>();
            
            var handCopy = Instantiate(hand, hand.transform.position, hand.transform.rotation);
            handCopy.transform.parent = transform;
            pose.handOffset = handCopy.transform.localPosition;
            pose.localQuaternionOffset = handCopy.transform.localRotation;
            DestroyImmediate(handCopy.gameObject);

            foreach(var finger in hand.fingers) {
                AssignChildrenPose(finger.transform);
            }

            void AssignChildrenPose(Transform obj) {
                AddPoint(obj.localPosition, obj.localRotation);
                for(int j = 0; j < obj.childCount; j++) {
                    AssignChildrenPose(obj.GetChild(j));
                }
            }

            void AddPoint(Vector3 pos, Quaternion rot) {
                posePositionsList.Add(pos);
                poseRotationsList.Add(rot);
            }
            
            pose.posePositions = new Vector3[posePositionsList.Count];
            pose.poseRotations = new Quaternion[posePositionsList.Count];
            for(int i = 0; i < posePositionsList.Count; i++) {
                pose.posePositions[i] = posePositionsList[i];
                pose.poseRotations[i] = poseRotationsList[i];
            }

            if(left){
                leftPose = pose;
                leftPoseSet = true;
                Debug.Log("Pose Saved - Left");
            }
            else{
                rightPose = pose;
                rightPoseSet = true;
                Debug.Log("Pose Saved - Right");
            }
        }
        
        public void EditorClearPoses() {
            leftPoseSet = false;
            leftPose = new HandPoseData();
            rightPoseSet = false;
            rightPose = new HandPoseData();
        }
#endif

        public bool HasPose(bool left) {
            return left ? leftPoseSet : rightPoseSet;
        }
    }
}
