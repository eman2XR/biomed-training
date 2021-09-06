using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Autohand{
    public class HandPublicEvents : MonoBehaviour{
        public Hand hand;
        public UnityHandGrabEvent OnGrab;
        public UnityHandGrabEvent OnRelease;
        public UnityHandGrabEvent OnForceRelease;
        public UnityHandGrabEvent OnSqueeze;
        public UnityHandGrabEvent OnUnsqueeze;

        void Start(){
            hand.OnGrabbed += OnGrabEvent;
            hand.OnReleased += OnReleaseEvent;
            hand.OnForcedRelease += OnForceReleaseEvent;
            hand.OnSqueezed += OnSqueezeEvent;
            hand.OnUnsqueezed += OnUnsqueezeEvent;
        }

        public void OnGrabEvent(Hand hand, Grabbable grab) {
            OnGrab?.Invoke(hand, grab);
        }

        public void OnReleaseEvent(Hand hand, Grabbable grab) {
            OnRelease?.Invoke(hand, grab);
        }

        public void OnSqueezeEvent(Hand hand, Grabbable grab) {
            OnSqueeze?.Invoke(hand, grab);
        }

        public void OnUnsqueezeEvent(Hand hand, Grabbable grab) {
            OnUnsqueeze?.Invoke(hand, grab);
        }

        public void OnForceReleaseEvent(Hand hand, Grabbable grab) {
            OnForceRelease?.Invoke(hand, grab);
        }

        private void OnDrawGizmosSelected() {
            if(hand == null && GetComponent<Hand>())
                hand = GetComponent<Hand>();
        }
    }
}
