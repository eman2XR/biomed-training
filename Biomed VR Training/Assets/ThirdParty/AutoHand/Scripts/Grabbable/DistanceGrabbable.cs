using CustomAttributes;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Autohand{
    [RequireComponent(typeof(Grabbable))]
    public class DistanceGrabbable : MonoBehaviour{
        
        
        [Header("Pull")]
        public bool instantPull = false;

        [Header("Velocity Shoot")]
        [Range(0.4f, 1.1f)]
        [Tooltip("Use this to adjust the angle of the arch that the gameobject follows while shooting towards your hand.")]
        [HideIf("instantPull")]
        public float archMultiplier = .6f;

        [Tooltip("Slow down or speed up gravitation to your liking.")]
        [HideIf("instantPull")]
        public float gravitationVelocity = 1f;



        [Header("Rotation")]
        [Tooltip("This enables rotation which makes the gameobject orient to the rotation of you hand as it moves through the air. All below rotation variables have no use when this is false.")]
        [HideIf("instantPull")]
        public bool rotate = true;

        [Tooltip("Speed that the object orients to the rotation of your hand.")]
        [HideIf("instantPull")]
        public float rotationSpeed = 1;
        
        [Header("Highlight Options")]
        [Space, Tooltip("Whether or not to ignore all highlights include default highlights on HandPointGrab")]
        public bool ignoreHighlights = false;
        [HideIf("ignoreHighlights"), Tooltip("Highlight targeted material to use - defaults to HandPointGrab materials if none")]
        public Material targetedMaterial;
        [HideIf("ignoreHighlights"), Tooltip("Highlight selected material to use - defaults to HandPointGrab materials if none")]
        public Material selectedMaterial;

        [Header("Events")]
        public bool showEvents = true;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent OnPull;
        [Space]
        [Tooltip("Called when the object has been targeted/aimed at by the pointer")]
        [ShowIf("showEvents")]
        public UnityHandGrabEvent StartTargeting;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent StopTargeting;
        [Space]
        [Tooltip("Called when the object has been selected before being pulled or flicked")]
        [ShowIf("showEvents")]
        public UnityHandGrabEvent StartSelecting;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent StopSelecting;

        public HandGrabEvent OnPullCanceled;

        internal Grabbable grabbable;
    

        private Transform Target;
        private Vector3 calculatedNecessaryVelocity;
        private bool gravitationEnabled;
        private bool gravitationMethodBegun;
        private bool pullStarted;
        private Rigidbody body;
        float timePassedSincePull;

        private void Start() {
            grabbable = GetComponent<Grabbable>();
            body = grabbable.body;
        }
    
        void FixedUpdate(){
            if(!instantPull){
                if (Target == null)
                    return;

                InitialVelocityPushToHand();
                if(rotate)
                    FollowHandRotation();
                if (gravitationEnabled)
                    GravitateTowardsHand();
                timePassedSincePull += Time.deltaTime;
            }
        }


        private void FollowHandRotation(){
            transform.rotation = Quaternion.Slerp(transform.rotation, Target.rotation, rotationSpeed * Time.fixedDeltaTime); 
        }

        Vector3 lastGravitationVelocity;
        private void GravitateTowardsHand(){
            if (gravitationEnabled){

                if (!gravitationMethodBegun){
                    gravitationMethodBegun = true;
                }
                    
                lastGravitationVelocity = (Target.position- transform.position).normalized*Time.fixedDeltaTime*gravitationVelocity;
                body.velocity += lastGravitationVelocity*10;
            }
            else{
                gravitationMethodBegun = false;
            }
        }


        private void InitialVelocityPushToHand(){
            //This way I can ensure that the initial shot with velocity is only shot once
            if (pullStarted){
                if(archMultiplier > 0)
                    calculatedNecessaryVelocity = CalculateTrajectoryVelocity(transform.position, Target.transform.position, archMultiplier);

                timePassedSincePull = 0;
                body.velocity = calculatedNecessaryVelocity;
                gravitationEnabled = true;
                pullStarted = false;
            }
        }

        private void OnCollisionEnter(Collision collision){
            if (timePassedSincePull > 0.5f)
            {
                pullStarted = false;
                gravitationEnabled = false;
                CancelTarget();
            }
        }


        Vector3 CalculateTrajectoryVelocity(Vector3 origin, Vector3 target, float t){
            float vx = (target.x - origin.x) / t;
            float vz = (target.z - origin.z) / t;
            float vy = ((target.y - origin.y) - 0.5f * Physics.gravity.y * t * t) / t;
            return new Vector3(vx, vy, vz);
        }

        public void SetTarget(Transform theObject) { Target = theObject; pullStarted = true; }
        public void CancelTarget() { Target = null; pullStarted = false; }
    }
}
