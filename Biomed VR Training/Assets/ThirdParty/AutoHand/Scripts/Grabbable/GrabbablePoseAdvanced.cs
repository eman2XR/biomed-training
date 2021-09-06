using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Autohand {
    public class GrabbablePoseAdvanced : GrabbablePose{
        [Header("Advanced Pose")]
        [Tooltip("Usually this can be left empty, used to create a different center point if the objects transform isn't ceneterd for the prefered rotation/movement axis")]
        public Transform centerObject;
        [Tooltip("You want this set so the disc gizmo is around the axis you want the hand to rotate, or that the line is straight through the axis you want to move")]
        public Vector3 up = Vector3.up;
        [Space]
        public int minAngle;
        public int maxAngle = 360;
        public float maxRange = 0;
        public float minRange = 0;

        [Header("Editor Testing - Requires Gizmos Enabled")]
        [Tooltip("Helps test pose by setting the angle of the editor hand, REQUIRES GIZMOS ENABLED")]
        public int testAngle = 0;
        [Tooltip("Helps test pose by setting the range position of the editor hand, REQUIRES GIZMOS ENABLED")]
        public float testRange = 0;
        int lastAngle = 0;
        float lastRange = 0;
        [Space]
        public bool showGizmos = true;

        new public void Start() {
            base.OnStart();
            if (minAngle > maxAngle) {
                var tempAngle = minAngle;
                minAngle = maxAngle;
                maxAngle = tempAngle;
            }
            if (minRange > maxRange) {
                var temp = minRange;
                minRange = maxRange;
                maxRange = temp;
            }
        }

        public override HandPoseData GetHandPoseData(Hand hand) {
            Vector3 pregrabPos = hand.transform.position;
            Quaternion pregrabRot = hand.transform.rotation;

            base.GetHandPoseData(hand).SetPose(hand, transform);

            var handParent = hand.transform.parent;
            var tempContainer = new GameObject().transform;
            var getTransform = GetTransform();
            tempContainer.position = getTransform.position;
            hand.transform.parent = tempContainer;
            tempContainer.rotation = getTransform.rotation;


            float closestDistance = float.MaxValue;
            Quaternion closestRotation = tempContainer.rotation;

            for (int i = minAngle; i <= maxAngle; i++) {
                tempContainer.eulerAngles = getTransform.rotation * up;
                tempContainer.RotateAround(tempContainer.transform.position, GetTransform().rotation * up, i);
                
                var distance = Quaternion.Angle(hand.transform.rotation, pregrabRot);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestRotation = tempContainer.rotation;
                }
            }
            closestDistance = float.MaxValue;
            Vector3 closestPosition = tempContainer.position;

            var minRangeVec = getTransform.position + getTransform.rotation * up * minRange;
            var maxRangeVec = getTransform.position + getTransform.rotation * up * maxRange;
            for(int i = 0; i < 20; i++) {
                tempContainer.transform.position = Vector3.Lerp(minRangeVec, maxRangeVec, i/20f);
                var distance = Vector3.Distance(hand.transform.position, pregrabPos);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closestPosition = tempContainer.position;
                }
            }
            
            tempContainer.transform.rotation = closestRotation;
            tempContainer.transform.position = closestPosition;

            hand.transform.parent = handParent;

            Destroy(tempContainer.gameObject);
            var pose = base.GetNewPoseData(hand);

            hand.transform.position = pregrabPos;
            hand.transform.rotation = pregrabRot;

            return pose;
        }

        Vector3 lastPos = Vector3.zero;
        public HandPoseData GetHandPoseData(Hand hand, int angle, float range) {
            Vector3 pregrabPos = hand.transform.position;

            base.GetHandPoseData(hand).SetPose(hand, transform);

            var handParent = hand.transform.parent;
            var tempContainer = new GameObject().transform;
            tempContainer.position = GetTransform().position;
            hand.transform.parent = tempContainer;
            tempContainer.rotation = GetTransform().rotation;
            
            tempContainer.eulerAngles = GetTransform().rotation * up;
            tempContainer.RotateAround(tempContainer.transform.position, GetTransform().rotation * up, angle);
            tempContainer.transform.position = GetTransform().position +  GetTransform().rotation * up * range;
            
            lastPos = tempContainer.transform.position;
            hand.transform.parent = handParent;
            DestroyImmediate(tempContainer.gameObject);

            return base.GetNewPoseData(hand);
        }

        Transform GetTransform() {
            return centerObject != null ? centerObject : transform;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected() {
            if(showGizmos){
                var usingTransform = GetTransform();
                var radius = 0.1f;
            
                var pose = HasPose(false) ? rightPose : leftPose;

                var handDir = Quaternion.AngleAxis(minAngle, usingTransform.rotation * up) * pose.handOffset.normalized;
                Handles.DrawWireArc(usingTransform.position, usingTransform.rotation * up, handDir, maxAngle-minAngle, radius);
            
                var minRangeVec = usingTransform.position + usingTransform.rotation * up * minRange;
                var maxRangeVec = usingTransform.position + usingTransform.rotation * up * maxRange;
                Gizmos.DrawLine(usingTransform.position, minRangeVec);
                Gizmos.DrawLine(usingTransform.position, maxRangeVec);
            }
            if (editorHand != null && (testAngle != lastAngle || testRange != lastRange)) {
                testAngle = Mathf.Clamp(testAngle, minAngle, maxAngle);
                testRange = Mathf.Clamp(testRange, minRange, maxRange);

                if (minAngle > maxAngle) {
                    var temp = minAngle;
                    minAngle = maxAngle;
                    maxAngle = temp;
                }
                if (minRange > maxRange) {
                    var temp = minRange;
                    minRange = maxRange;
                    maxRange = temp;
                }
                
                lastAngle = testAngle;
                lastRange = testRange;
                if(CanSetPose(editorHand))
                    GetHandPoseData(editorHand, testAngle, testRange).SetPose(editorHand, transform);
            }
        }
#endif
    }
}
