using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand{
    [DefaultExecutionOrder(1)]
    public class HandTeleportGuard : MonoBehaviour{
        [Header("Helps prevent hand from passing through static collision boundries")]
        public Hand hand;
        [Tooltip("The amount of distance change required to activate")]
        public float buffer = 0.1f;
        [Tooltip("Whether this should always run or only run when activated by the teleporter")]
        public bool alwaysRun = false;
        [Tooltip("If strict that hands wont teleport return when past the max distance, if something is in the way")]
        public bool strict = false;
        
        
        Vector3 deltaHandPos;
        int mask;
        void Awake(){
            if(hand == null && GetComponent<Hand>())
                hand = GetComponent<Hand>();
            
            mask = LayerMask.GetMask(Hand.grabbableLayerNameDefault, Hand.grabbingLayerName, Hand.releasingLayerName, Hand.rightHandLayerName, Hand.leftHandLayerName, "HandPlayer");
        }

        void FixedUpdate(){
            if (hand == null || hand.gameObject.activeInHierarchy)
                return;

            if (alwaysRun){
                var distance = Vector3.Distance(hand.body.position, deltaHandPos);
                if(strict || (!strict && distance < hand.maxFollowDistance)){
                    if(distance > buffer)
                        TeleportProtection(deltaHandPos, hand.body.position);
                }
                deltaHandPos = hand.body.position;
            }
        }

        /// <summary>Should be called just after a teleportation</summary>
        public void TeleportProtection(Vector3 fromPos, Vector3 toPos) {
            if (hand == null || hand.body == null)
                return;

            RaycastHit[] hits = Physics.RaycastAll(fromPos, hand.body.position-fromPos, Vector3.Distance(fromPos, hand.body.position), ~mask);
            Vector3 handPos = -Vector3.one;
            foreach(var hit in hits) {
                if(hit.transform != hand.transform && hit.transform.CanGetComponent<Rigidbody>(out _)) {
                    handPos = Vector3.MoveTowards(hit.point, hand.body.position, buffer);
                }
            }
            if(handPos != -Vector3.one)
                hand.SetHandLocation(handPos, hand.body.rotation);
        }
    }
}
