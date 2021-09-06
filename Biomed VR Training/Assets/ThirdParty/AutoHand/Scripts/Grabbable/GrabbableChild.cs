using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Autohand{
    /// <summary>
    /// THIS SCRIPT CAN BE ATTACHED TO A COLLIDER OBJECT TO REFERENCE A GRABBABLE BODY
    /// </summary>
    [DefaultExecutionOrder(1)]
    public class GrabbableChild : MonoBehaviour{
        public Grabbable grabParent;

        private void Start() {
            grabParent.SetGrabbableChild(this);
            //Delete these layer setters if you want to use your own custom layer set
            gameObject.layer = grabParent.gameObject.layer;
        }
    }
}
