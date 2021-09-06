using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

namespace Autohand {
    public class Grabbable : MonoBehaviour {


        [Header("Holding Settings")]

        [Tooltip("The physics body to connect this colliders grab to - if left empty will default to local body")]
        public Rigidbody body;

        [Tooltip("Which hand this can be held by")]
        public HandType handType = HandType.both;
        
        [Tooltip("Experimental - ignores weight of held object while held")]
        public bool ignoreWeight = false;

        [Tooltip("Creates an offset an grab so the hand will not return to the hand on grab - Good for statically jointed grabbable objects")]
        public bool maintainGrabOffset = false;

        

        [Header("Grab Settings")]

        [Tooltip("Whether or not this can be grabbed with more than one hand")]
        public bool singleHandOnly = false;

        [ShowIf("singleHandOnly")]
        [Tooltip("if false single handed items cannot be passes back and forth on grab")]
        public bool allowHeldSwapping = true;

        [Tooltip("Will the item automatically return the hand on grab - good for saved poses, bad for heavy things")]
        public bool instantGrab = false;


        [Tooltip("This will NOT parent the object under the hands on grab. This will parent the object to the parents of the hand, which allow you to move the hand parent object smoothly while holding an item, but will also allow you to move items that are very heavy - recommended for all objects that aren't very heavy or jointed to other rigidbodies")]
        public bool parentOnGrab = true;




        [Header("Release Settings")]
        [Tooltip("How much to multiply throw by for this grabbable when releasing - 0-1 for no or reduced throw strength")]
        public float throwMultiplyer = 1;




        [Header("Highlight Settings")]
        [Tooltip("Multiplies look assist speed")]
        public float lookAssistMultiplyer = 1f;
        [Tooltip("A copy of the mesh will be created and slighly scaled and this material will be applied to create a highlight effect with options")]
        public Material hightlightMaterial;



        [Header("Break Settings")]
        [Tooltip("The required force to break the fixedJoint\n " +
                 "Turn this to \"infinity\" to disable (Might cause jitter)\n" +
                "Ideal value depends on hand mass and velocity settings")]
        public float jointBreakForce = 2500;
        

        [Header("Advanced Settings")]
        public bool showAdvancedSettings = true;
        [Space]



        [ShowIf("showAdvancedSettings")]
        [Tooltip("Lock hand in place on grab")]
        public bool lockHandOnGrab = false;

        [ShowIf("showAdvancedSettings")]
        [Tooltip("Adds and links a GrabbableChild to each child with a collider on start - So the hand can grab them")]
        public bool makeChildrenGrabbable = true;

        [ShowIf("showAdvancedSettings")]
        [Tooltip("For the special use case of having grabbable objects with physics jointed peices move properly while being held")]
        public List<Rigidbody> jointedBodies = new List<Rigidbody>();

        [ShowIf("showAdvancedSettings")]
        [Tooltip("For the special use case of having grabbable objects that the hand should ignore")]
        public List<Collider> handIgnoreColliders = new List<Collider>();


        [Tooltip("The number of seconds that the hand collision should ignore the released object\n (Good for increased placement precision and resolves clipping errors)"), Min(0)]
        [ShowIf("showAdvancedSettings")]
        public float ignoreReleaseTime = 0.25f;

        [Tooltip("I.E. Grab Prioirty - SMALLER IS BETTER - Multiplys highlight distance by this when calculating which object to grab. Hands always grab closest object to palm")]
        [Min(0)]
        [ShowIf("showAdvancedSettings")]
        public float grabDistancePriority = 1;

        [Tooltip("Offsets the grabbable by this much when being held")]
        [ShowIf("showAdvancedSettings")]
        public Vector3 heldPositionOffset;

        [Tooltip("Offsets the grabbable by this many degrees when being held")]
        [ShowIf("showAdvancedSettings")]
        public Vector3 heldRotationOffset;



        [Header("Events")]
        public bool showEvents = true;
        [Space]
        [ShowIf("showEvents")]
        public UnityHandGrabEvent onGrab;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent onRelease;

        [ShowIf("showEvents")]
        [Space, Space]
        public UnityHandGrabEvent onSqueeze;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent onUnsqueeze;

        [Space, Space]
        [ShowIf("showEvents")]
        public UnityHandGrabEvent onHighlight;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent onUnhighlight;
        [Space, Space]

        [Tooltip("Whether or not the break call made only when holding with multiple hands - if this is false the break event can be called by forcing an object into a static collider")]
        [ShowIf("showEvents")]
        public bool pullApartBreakOnly = true;
        [ShowIf("showEvents")]
        public UnityHandGrabEvent OnJointBreak;



        //For programmers <3
        public HandGrabEvent OnBeforeGrabEvent;
        public HandGrabEvent OnGrabEvent;
        public HandGrabEvent OnReleaseEvent;

        public HandGrabEvent OnForceReleaseEvent;
        public HandGrabEvent OnJointBreakEvent;

        public HandGrabEvent OnSqueezeEvent;
        public HandGrabEvent OnUnsqueezeEvent;

        public HandGrabEvent OnHighlightEvent;
        public HandGrabEvent OnUnhighlightEvent;

        [HideInInspector]
        public bool isGrabbable = true;
        
        [HideInInspector]
        public bool hideEvents = false;

        protected bool beingHeld = false;

        protected List<Hand> heldBy = new List<Hand>();
        protected bool throwing;
        protected bool hightlighting;
        protected GameObject highlightObj;
        protected PlacePoint placePoint = null;
        protected PlacePoint lastPlacePoint = null;

        Transform originalParent;
        Vector3 lastCenterOfMassPos;
        Quaternion lastCenterOfMassRot;
        CollisionDetectionMode detectionMode;

        bool heldBodyJointed = false;
        internal bool beingGrabbed = false;
        bool wasIsGrabbable = false;
        bool beingDestroyed = false;
        int originalLayer;
        Coroutine resetLayerRoutine = null;
        List<GrabbableChild> grabChildren = new List<GrabbableChild>();
        List<Transform> jointedParents = new List<Transform>();
        List<Collision> collisions = new List<Collision>();

        protected void Awake() {
            OnAwake();
        }

        /// <summary>Virtual substitute for Awake()</summary>
        public virtual void OnAwake() {

            //Delete these layer setters if you want to use your own custom layer set
            if(gameObject.layer == LayerMask.NameToLayer("Default") || LayerMask.LayerToName(gameObject.layer) == "")
                gameObject.layer = LayerMask.NameToLayer(Hand.grabbableLayerNameDefault);
            
            if(makeChildrenGrabbable)
                MakeChildrenGrabbable();


            if(heldBy == null)
                heldBy = new List<Hand>();

            if(body == null){
                if(GetComponent<Rigidbody>())
                    body = GetComponent<Rigidbody>();
                else
                    Debug.LogError("RIGIDBODY MISSING FROM GRABBABLE: " + transform.name + " \nPlease add/attach a rigidbody", this);
            }

            originalLayer = gameObject.layer;
            originalParent = body.transform.parent;
            for (int i = 0; i < jointedBodies.Count; i++)
                jointedParents.Add(jointedBodies[i].transform.parent ?? null);
            detectionMode = body.collisionDetectionMode;
            
        }
        

        protected void FixedUpdate() {
            if(beingHeld) {
                lastCenterOfMassRot = body.transform.rotation;
                lastCenterOfMassPos = body.transform.position;
            }

            if(wasIsGrabbable && !isGrabbable) {
                ForceHandsRelease();
            }
            wasIsGrabbable = isGrabbable;
        }

        private void OnDisable(){
            if (resetLayerRoutine != null){
                StopCoroutine(resetLayerRoutine);
                resetLayerRoutine = null;
            }
        }

        /// <summary>Called when the hand starts aiming at this item for pickup</summary>
        public virtual void Highlight(Hand hand, Material customMat = null) {
            if(!hightlighting){
                hightlighting = true;
                onHighlight?.Invoke(hand, this);
                OnHighlightEvent?.Invoke(hand, this);
                var highlightMat = customMat != null ? customMat : hightlightMaterial;
                if(highlightMat != null){
                    if(!gameObject.CanGetComponent<MeshRenderer>(out _)) {
                        return;
                    }

                    //Creates a slightly larger copy of the mesh and sets its material to highlight material
                    highlightObj = new GameObject();
                    highlightObj.transform.parent = transform;
                    highlightObj.transform.localPosition = Vector3.zero;
                    highlightObj.transform.localRotation = Quaternion.identity;
                    highlightObj.transform.localScale = Vector3.one * 1.001f;

                    highlightObj.AddComponent<MeshFilter>().sharedMesh = GetComponent<MeshFilter>().sharedMesh;
                    highlightObj.AddComponent<MeshRenderer>().materials = new Material[GetComponent<MeshRenderer>().materials.Length];
                    var mats = new Material[GetComponent<MeshRenderer>().materials.Length];
                    for(int i = 0; i < mats.Length; i++) {
                        mats[i] = highlightMat;
                    }
                    highlightObj.GetComponent<MeshRenderer>().materials = mats;
                }
            }
        }
        
        /// <summary>Called when the hand stops aiming at this item</summary>
        public virtual void Unhighlight(Hand hand) {
            if(hightlighting){
                onUnhighlight?.Invoke(hand, this);
                OnUnhighlightEvent?.Invoke(hand, this);
                hightlighting = false;
                if(highlightObj != null)
                    Destroy(highlightObj);
            }
        }



        /// <summary>Called by the hands Squeeze() function is called and this item is being held</summary>
        public virtual void OnSqueeze(Hand hand) {
            OnSqueezeEvent?.Invoke(hand, this);
            onSqueeze?.Invoke(hand, this);
        }
        
        /// <summary>Called by the hands Unsqueeze() function is called and this item is being held</summary>
        public virtual void OnUnsqueeze(Hand hand) {
            OnUnsqueezeEvent?.Invoke(hand, this);
            onUnsqueeze?.Invoke(hand, this);
        }

        public virtual void OnBeforeGrab(Hand hand) {
            OnBeforeGrabEvent?.Invoke(hand, this);
            beingGrabbed = true;
            if (resetLayerRoutine != null){
                StopCoroutine(resetLayerRoutine);
                resetLayerRoutine = null;
            }

        }

        /// <summary>Called by the hand whenever this item is grabbed</summary>
        public virtual void OnGrab(Hand hand) {
            placePoint?.Remove(this);
            placePoint = null;
            if(lockHandOnGrab)
                hand.body.isKinematic = true;
            
            if(!body.isKinematic)
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            else
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            if (!beingDestroyed) {
                if (resetLayerRoutine != null){
                    StopCoroutine(resetLayerRoutine);
                    resetLayerRoutine = null;
                }

                resetLayerRoutine = StartCoroutine(ResetLayer(0.1f, LayerMask.NameToLayer(Hand.grabbingLayerName)));
            }

            if (parentOnGrab) {
                body.transform.parent = hand.transform.parent;
                foreach (var jointedBody in jointedBodies){
                    jointedBody.transform.parent = hand.transform.parent;
                    if(Hand.HasGrabbable(jointedBody.gameObject, out var grab))
                        grab.heldBodyJointed = true;
                }
            }

            heldBy?.Add(hand);
            throwing = false;
            beingHeld = true;
            beingGrabbed = false;
            onGrab?.Invoke(hand, this);
            OnGrabEvent?.Invoke(hand, this);
        }
        
        /// <summary>Called by the hand whenever this item is release</summary>
        public virtual void OnRelease(Hand hand, bool thrown) {
            if(beingHeld) {
                if(!heldBy.Remove(hand))
                    return;

                if(lockHandOnGrab)
                    hand.body.isKinematic = false;

                if(heldBy.Count == 0){
                    beingHeld = false;
                    SetOriginalParentAndLayer();
                }

                OnReleaseEvent?.Invoke(hand, this);
                onRelease?.Invoke(hand, this);

                CheckPlacePoint(hand);

                if(body != null) {
                    if(!beingHeld && thrown && !throwing) {
                        throwing = true;
                        body.velocity = hand.ThrowVelocity() * throwMultiplyer;
                        try {
                            body.angularVelocity = hand.ThrowAngularVelocity();
                        }
                        catch { }
                    }
                    if(!thrown) {
                        body.velocity = Vector3.zero;
                        body.angularVelocity = Vector3.zero;
                    }
                }
            }
        }

        void CheckPlacePoint(Hand hand){
            if(placePoint != null){
                if(placePoint.CanPlace(transform))
                    placePoint.Place(this);

                Unhighlight(hand);

                placePoint.StopHighlight(this);
            }
        }


        /// <summary>Tells each hand holding this object to release</summary>
        public virtual void HandRelease() {
            for(int i = heldBy.Count - 1; i >= 0; i--)
                heldBy[i].ReleaseGrabLock();
        }

        /// <summary>Forces all the hands on this object to relese without applying throw force or calling OnRelease event</summary>
        public virtual void ForceHandsRelease() {
            for (int i = heldBy.Count - 1; i >= 0; i--)
                heldBy[i].ForceReleaseGrab();
        }

        /// <summary>Forces all the hands on this object to relese without applying throw force or calling OnRelease event</summary>
        public virtual void ForceHandRelease(Hand hand) {
            if (heldBy.Contains(hand)) {
                hand.ForceReleaseGrab();

                if(lockHandOnGrab)
                    hand.body.isKinematic = false;
                    
                heldBy.Remove(hand);
                if(heldBy.Count == 0){
                    beingHeld = false;
                    SetOriginalParentAndLayer();
                }
            }
        }
        
        /// <summary>Helps keep track of hand collisions, used to help create extra stability</summary>
        protected void OnCollisionEnter(Collision collision) {
            if(heldBy.Count > 0) {
                collisions.Add(collision);
            }
        }

        /// <summary>Helps keep track of hand collisions, used to help create extra stability</summary>
        protected void OnCollisionExit(Collision collision) {
            if(heldBy.Count > 0) {
                collisions.Remove(collision);
            }

            if(throwing && (collision.gameObject.layer != (collision.gameObject.layer | (1 << Hand.GetHandsLayerMask())))) {
                Invoke("ResetThrowing", Time.fixedDeltaTime);
            }
        }


        private void OnDestroy() {
            beingDestroyed = true;
            ForceHandsRelease();
            MakeChildrenUngrabbable();
            if (resetLayerRoutine != null){
                StopCoroutine(resetLayerRoutine);
                resetLayerRoutine = null;
            }
        }

        /// <summary>Called when the joint between the hand and this item is broken\n - Works to simulate pulling item apart event</summary>
        public virtual void OnHandJointBreak(Hand hand) {
            body.WakeUp();
            body.velocity /= 1000;
            body.angularVelocity /= 1000;

            if(!pullApartBreakOnly){
                OnJointBreakEvent?.Invoke(hand, this);
                OnJointBreak?.Invoke(hand, this);
            }
            if (pullApartBreakOnly && heldBy.Count > 1){
                OnJointBreakEvent?.Invoke(hand, this);
                OnJointBreak?.Invoke(hand, this);
            }

            ForceHandsRelease();
            SetOriginalParentAndLayer();
        }

        
        public void OnTriggerEnter(Collider other) {
            PlacePoint otherPoint;
            if(other.CanGetComponent(out otherPoint)) {
                if(heldBy.Count == 0 && !otherPoint.onlyPlaceWhileHolding) return;
                if(otherPoint == null) return;

                if(placePoint != null && placePoint.GetPlacedObject() != null)
                    return;

                if(placePoint == null){
                    if(otherPoint.CanPlace(transform)){
                        placePoint = otherPoint;
                        if(placePoint.forcePlace){
                            if(lastPlacePoint == null || (lastPlacePoint != null && !lastPlacePoint.Equals(placePoint))){
                                ForceHandsRelease();
                                placePoint.Place(this);
                                lastPlacePoint = placePoint;
                            }
                        }
                        else{
                            placePoint.Highlight(this);
                        }
                    }
                }
            }
        }
        
        public void OnTriggerExit(Collider other){
            PlacePoint otherPoint;
            if(!other.CanGetComponent(out otherPoint))
                return;

            if(placePoint != null && placePoint.Equals(otherPoint) && placePoint.Distance(transform) > 0.01f) {
                placePoint.StopHighlight(this);
                placePoint = null;
            }

            if(lastPlacePoint != null && lastPlacePoint.Equals(otherPoint) && Vector3.Distance(lastPlacePoint.transform.position, transform.position) > lastPlacePoint.placeRadius){
                lastPlacePoint = null;
            }
        }


        public bool IsThrowing(){
            return throwing;
        }

        public Vector3 GetVelocity(){
            return lastCenterOfMassPos - transform.position;
        }
        
        public Vector3 GetAngularVelocity(){
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastCenterOfMassRot);
            deltaRotation.ToAngleAxis(out var angle, out var axis);
            angle *= Mathf.Deg2Rad;
            return (1.0f / Time.fixedDeltaTime) * angle/1.2f * axis ;
        }


        public List<Hand> GetHeldBy() {
            return heldBy;
        }

        public int HeldCount() {
            return heldBy.Count;
        }

        public bool IsHeld() {
            return beingHeld;
        }

        /// <summary>Returns true during hand grabbing coroutine</summary>
        public bool BeingGrabbed() {
            return beingGrabbed;
        }
        

        internal void SetPlacePoint(PlacePoint point) {
            placePoint = point;
        }

        public void SetGrabbableChild(GrabbableChild child) {
            if(!grabChildren.Contains(child))
                grabChildren.Add(child);
        }

        internal void SetLayerRecursive(Transform obj, int oldLayer, int newLayer) {
            for(int i = 0; i < grabChildren.Count; i++) {
                if(grabChildren[i].gameObject.layer == oldLayer)
                    grabChildren[i].gameObject.layer = newLayer;
            }
            SetChildrenLayers(obj);

            void SetChildrenLayers(Transform obj1){
                if(obj1.gameObject.layer == oldLayer)
                    obj1.gameObject.layer = newLayer;
                for(int i = 0; i < obj1.childCount; i++) {
                    SetChildrenLayers(obj1.GetChild(i));
                }
            }
        }
        
        internal void SetOriginalParentAndLayer(){
            if (resetLayerRoutine != null){
                StopCoroutine(resetLayerRoutine);
                resetLayerRoutine = null;
            }

            if (!beingDestroyed) {
                SetLayerRecursive(transform, gameObject.layer, LayerMask.NameToLayer(Hand.releasingLayerName));
                resetLayerRoutine = StartCoroutine(ResetLayer(ignoreReleaseTime, LayerMask.NameToLayer(Hand.releasingLayerName)));
                if (!heldBodyJointed)
                    body.transform.parent = originalParent;

                for (int i = 0; i < jointedBodies.Count; i++){
                    if (Hand.HasGrabbable(jointedBodies[i].gameObject, out var grab)){
                        if (grab.HeldCount() == 0)
                            grab.transform.parent = grab.originalParent;
                        grab.heldBodyJointed = false;
                    }
                    else if(!heldBodyJointed)
                        jointedBodies[i].transform.parent = jointedParents[i];
                }
            }
        }

        
        public void AddJointedBody(Rigidbody body){
            Grabbable grab;
            jointedBodies.Add(body);

            if (Hand.HasGrabbable(body.gameObject, out grab))
                jointedParents.Add(grab.originalParent);
            else
                jointedParents.Add(body.transform.parent);

            if (transform.parent != originalParent){
                if(grab != null) {
                    if (grab.HeldCount() == 0)
                        grab.transform.parent = transform.parent;
                    grab.heldBodyJointed = true;
                }
                else
                    grab.transform.parent = transform.parent;
            }
        }
        public void RemoveJointedBody(Rigidbody body){
            var i = jointedBodies.IndexOf(body);
            if (Hand.HasGrabbable(jointedBodies[i].gameObject, out var grab)){
                if (grab.HeldCount() == 0)
                    grab.transform.parent = grab.originalParent;
                grab.heldBodyJointed = false;
            }
            else
                jointedBodies[i].transform.parent = jointedParents[i];

            jointedBodies.RemoveAt(i);
            jointedParents.RemoveAt(i);
        }

        //Adds a reference script to child colliders so they can be grabbed
        void MakeChildrenGrabbable() {
            for(int i = 0; i < transform.childCount; i++) {
                AddChildGrabbableRecursive(transform.GetChild(i));
            }

            void AddChildGrabbableRecursive(Transform obj) {
                if(obj.CanGetComponent(out Collider col) && col.isTrigger == false && !obj.CanGetComponent<Grabbable>(out _) && !obj.CanGetComponent<GrabbableChild>(out _) && !obj.CanGetComponent<PlacePoint>(out _)){
                    var child = obj.gameObject.AddComponent<GrabbableChild>();
                    child.gameObject.layer = originalLayer;
                    child.grabParent = this;
                }
                for(int i = 0; i < obj.childCount; i++){
                    if(!obj.CanGetComponent<Grabbable>(out _))
                        AddChildGrabbableRecursive(obj.GetChild(i));
                }
            }
        }


        //Adds a reference script to child colliders so they can be grabbed
        void MakeChildrenUngrabbable() {
            for(int i = 0; i < transform.childCount; i++) {
                RemoveChildGrabbableRecursive(transform.GetChild(i));
            }

            void RemoveChildGrabbableRecursive(Transform obj) {
                if(obj.GetComponent<GrabbableChild>() && obj.GetComponent<GrabbableChild>().grabParent == this){
                    Destroy(obj.gameObject.GetComponent<GrabbableChild>());
                }
                for(int i = 0; i < obj.childCount; i++){
                    RemoveChildGrabbableRecursive(obj.GetChild(i));
                }
            }
        }


        public int GetOriginalLayer(){
            return originalLayer;
        }

        public void DoDestroy(){
            Destroy(gameObject);
        }


        //Invoked one fixedupdate after impact
        protected void ResetThrowing(){
            throwing = false;
        }

        //Invoked a quatersecond after releasing
        protected IEnumerator ResetLayer(float delay, int fromLayer) {
            yield return new WaitForSeconds(delay);
            body.WakeUp();
            if(gameObject.layer == fromLayer){
                SetLayerRecursive(transform, fromLayer, originalLayer);
            }
            OriginalCollisionDetection();
            resetLayerRoutine = null;

            collisions.Clear();
        }
        
        //Resets to original collision dection
        protected void OriginalCollisionDetection() {
            if(body != null && gameObject.layer == originalLayer)
                body.collisionDetectionMode = detectionMode;
        }

        public int HeldCollisions() {
            return collisions.Count;
        }

        public List<Collision> HeldCollisionList(){
            return collisions;
        }
    }
}
