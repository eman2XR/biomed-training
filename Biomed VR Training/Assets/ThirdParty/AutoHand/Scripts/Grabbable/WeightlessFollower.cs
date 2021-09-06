using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand{
    [RequireComponent(typeof(Rigidbody)), DefaultExecutionOrder(1)]
    public class WeightlessFollower : MonoBehaviour{
        [HideInInspector]
        public Transform follow;
        [HideInInspector]
        public Transform follow1;

        [HideInInspector]
        public float followPositionStrength = 30;
        [HideInInspector]
        public float followRotationStrength = 30;

        [HideInInspector]
        public float maxVelocity = 5;

        [HideInInspector]
        public Grabbable grab;
        
        
        internal Rigidbody body;
        Transform moveTo;

        float startMass;
        float startDrag;
        float startAngleDrag;
        
        public void Start(){
            if (body == null)
                body = GetComponent<Rigidbody>();

            if (startAngleDrag == 0){
                startMass = body.mass;
                startDrag = body.drag;
                startAngleDrag = body.angularDrag;
            }
        }

        public virtual void Set(Hand hand, Grabbable grab) {
            if(body == null)
                body = GetComponent<Rigidbody>();

            if (startAngleDrag == 0){
                startMass = body.mass;
                startDrag = body.drag;
                startAngleDrag = body.angularDrag;
            }

            if (follow == null)
                follow = hand.heldMoveTo;
            else if(follow1 == null)
                follow1 = hand.heldMoveTo;

            body.drag = hand.body.drag;
            body.angularDrag = hand.body.angularDrag;

            followPositionStrength = hand.followPositionStrength;
            followRotationStrength = hand.followRotationStrength;
            maxVelocity = hand.maxVelocity;

            this.grab = grab;

            if(moveTo == null){
                moveTo = new GameObject().transform;
                moveTo.name = gameObject.name + " FOLLOW POINT";
                moveTo.parent = follow.parent;
            }

        }


        public void FixedUpdate() {
            OnFixedUpdate();
        }

        protected virtual void OnFixedUpdate() {
            if(follow == null)
                return;
            
            //Sets [Move To] Object
            if (follow1){
                moveTo.position = Vector3.Lerp(follow.position, follow1.position, 0.5f);
                moveTo.rotation = Quaternion.Lerp(follow.rotation, follow1.rotation, 0.5f);
            }
            else {
                moveTo.position = follow.position;
                moveTo.rotation = follow.rotation;
            }

            //Calls physics movements
            MoveTo();
            TorqueTo();

        }


        /// <summary>Moves the hand to the controller position using physics movement</summary>
        internal virtual void MoveTo() {
            if(followPositionStrength <= 0)
                return;

            var movePos = moveTo.position;
            var distance = Vector3.Distance(movePos, transform.position);
            var velocityClamp = maxVelocity;
            
            
            //Sets velocity linearly based on distance from hand
            var vel = (movePos - transform.position).normalized * followPositionStrength * distance;
            vel.x = Mathf.Clamp(vel.x, -velocityClamp, velocityClamp);
            vel.y = Mathf.Clamp(vel.y, -velocityClamp, velocityClamp);
            vel.z = Mathf.Clamp(vel.z, -velocityClamp, velocityClamp);
            body.velocity = vel;
        }


        /// <summary>Rotates the hand to the controller rotation using physics movement</summary>
        internal virtual void TorqueTo() {
            var toRot = moveTo.rotation;
            float angleDist = Quaternion.Angle(body.rotation, toRot);
            Quaternion desiredRotation = Quaternion.Lerp(body.rotation, toRot, Mathf.Clamp(angleDist, 0, 2) / 4f);

            var kp = 90f * followRotationStrength;
            var kd = 60f;
            Vector3 x;
            float xMag;
            Quaternion q = desiredRotation * Quaternion.Inverse(transform.rotation);
            q.ToAngleAxis(out xMag, out x);
            x.Normalize();
            x *= Mathf.Deg2Rad;
            Vector3 pidv = kp * x * xMag - kd * body.angularVelocity;
            Quaternion rotInertia2World = body.inertiaTensorRotation * transform.rotation;
            pidv = Quaternion.Inverse(rotInertia2World) * pidv;
            pidv.Scale(body.inertiaTensor);
            pidv = rotInertia2World * pidv;
            body.AddTorque(pidv);
        }
        
        public void RemoveFollow(Transform follow){
            if (this.follow == follow)
                this.follow = null;
            if (follow1 == follow)
                follow1 = null;

            if (this.follow == null && follow1 != null){
                this.follow = follow1;
                follow1 = null;
            }

            if(this.follow == null && follow1 == null){
                body.mass = startMass;
                body.drag = startDrag;
                body.angularDrag = startAngleDrag;
                Destroy(this);
            }
        }

        private void OnDestroy(){
            body.mass = startMass;
            body.drag = startDrag;
            body.angularDrag = startAngleDrag;
            Destroy(moveTo.gameObject);
        }
    }

  
}
