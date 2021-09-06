using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand{
    public class GrabbablePoseCombiner : MonoBehaviour{
        public float positionWeight = 1;
        public float rotationWeight = 1;
        public GrabbablePose[] poses;

        public void Start() {
            if(poses.Length == 0)
                poses = GetComponents<GrabbablePose>();
        }

        public bool CanSetPose(Hand hand) {
            foreach(var pose in poses) {
                if(pose.CanSetPose(hand))
                    return true;
            }
            return false;
        }

        public GrabbablePose GetClosestPose(Hand hand, Grabbable grab){
            if(this.poses.Length == 0)
                Debug.LogError("AUTO HAND: No poses connected to multi pose", gameObject);

            List<GrabbablePose> poses = new List<GrabbablePose>();
            foreach(var handPose in this.poses)
                if(handPose.CanSetPose(hand))
                    poses.Add(handPose);
            
            var closestValue = float.MaxValue;
            int closestIndex = 0;

            var handParent = hand.transform.parent;
            hand.transform.parent = grab.transform;
            Quaternion localRotation = hand.transform.localRotation;
            Vector3 localPosition = hand.transform.localPosition;
            hand.transform.parent = handParent;

            var startPose = hand.GetHandPose();

            HandPoseData pose;
            for(int i = 0; i < poses.Count; i++){
                startPose.SetPose(hand);

                pose = poses[i].GetHandPoseData(hand);
                pose.SetPose(hand);

                var distance = Vector3.Distance(pose.handOffset, localPosition);

                localRotation = Quaternion.Inverse(grab.transform.rotation) * hand.transform.rotation;
                var angleDistance = Quaternion.Angle(pose.localQuaternionOffset, localRotation);

                var closenessValue = distance * positionWeight + angleDistance * rotationWeight;
                if(closenessValue < closestValue) {
                    closestIndex = i;
                    closestValue = closenessValue;
                }
                
            }

            return poses[closestIndex];
        }
    }
}
