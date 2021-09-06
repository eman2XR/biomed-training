using Autohand;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Autohand.Demo {
    [RequireComponent(typeof(Grabbable))]public class GrabbableEventDebugger : MonoBehaviour
    {

        private void OnEnable()
        {
            var grab = GetComponent<Grabbable>();
            grab.OnBeforeGrabEvent += (hand, grabbable) => { Debug.Log("BEFORE GRAB EVENT"); };
            grab.OnGrabEvent += (hand, grabbable) => { Debug.Log("GRAB EVENT"); };
            grab.OnReleaseEvent += (hand, grabbable) => { Debug.Log("RELEASE EVENT"); };
            grab.OnForceReleaseEvent += (hand, grabbable) => { Debug.Log("FORCE RELEASE EVENT"); };
            grab.OnJointBreakEvent += (hand, grabbable) => { Debug.Log("JOINT BREAK EVENT"); };
            grab.OnSqueezeEvent += (hand, grabbable) => { Debug.Log("SQUEEZE EVENT"); };
            grab.OnUnsqueezeEvent += (hand, grabbable) => { Debug.Log("UNSQUEEZE EVENT"); };
            grab.OnHighlightEvent += (hand, grabbable) => { Debug.Log("HIGHLIGHT EVENT"); };
            grab.OnUnhighlightEvent += (hand, grabbable) => { Debug.Log("UNHIGHLIGHT EVENT"); };
        }

        private void OnDisable()
        {
            var grab = GetComponent<Grabbable>();
            grab.OnBeforeGrabEvent -= (hand, grabbable) => { Debug.Log("BEFORE GRAB EVENT"); };
            grab.OnGrabEvent -= (hand, grabbable) => { Debug.Log("GRAB EVENT"); };
            grab.OnReleaseEvent -= (hand, grabbable) => { Debug.Log("RELEASE EVENT"); };
            grab.OnForceReleaseEvent -= (hand, grabbable) => { Debug.Log("FORCE RELEASE EVENT"); };
            grab.OnJointBreakEvent -= (hand, grabbable) => { Debug.Log("JOINT BREAK EVENT"); };
            grab.OnSqueezeEvent -= (hand, grabbable) => { Debug.Log("SQUEEZE EVENT"); };
            grab.OnUnsqueezeEvent -= (hand, grabbable) => { Debug.Log("UNSQUEEZE EVENT"); };
            grab.OnHighlightEvent -= (hand, grabbable) => { Debug.Log("HIGHLIGHT EVENT"); };
            grab.OnUnhighlightEvent -= (hand, grabbable) => { Debug.Log("UNHIGHLIGHT EVENT"); };
        }
    }
}