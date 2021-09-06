using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Autohand{
    [RequireComponent(typeof(Rigidbody))]
    public class HeadPhysicsFollower : MonoBehaviour{

        [Header("References")]
        public Camera headCamera;
        public Transform trackingContainer;
        public Transform followBody;

        [Header("Follow Settings")]
        public float followStrength = 50f;
        [Tooltip("The maximum allowed distance from the body for the headCamera to still move")]
        public float maxBodyDistance = 0.5f;

        internal SphereCollider headCollider;
        Vector3 startHeadPos;
        Vector3 startedHeadPos;
        bool started;
        
        List<GameObject> collisions;
        Transform moveTo;
        internal Rigidbody body;

        public void Start() {
            if(moveTo == null){
                moveTo = new GameObject().transform;
                moveTo.name = gameObject.name + " FOLLOW POINT";
                moveTo.parent = headCamera.transform.parent;
                moveTo.position = headCamera.transform.transform.position;
                moveTo.rotation = headCamera.transform.transform.rotation;
                body = GetComponent<Rigidbody>();
            }
            
            collisions = new List<GameObject>();
            gameObject.layer = LayerMask.NameToLayer("HandPlayer");
            transform.position = headCamera.transform.position;
            transform.rotation = headCamera.transform.rotation;
            headCollider = GetComponent<SphereCollider>();
            startHeadPos = headCamera.transform.position;
        }

        protected void FixedUpdate() {
            moveTo.position = headCamera.transform.position;

            if(startHeadPos.y != headCamera.transform.position.y && !started) {
                started = true;
                body.position = headCamera.transform.position;
            }

            if(!started)
                return;
            
            
            moveTo.position = headCamera.transform.position;
            MoveTo();
        }

        public bool Started() {
            return started;
        }
        
        internal virtual void MoveTo() {
            var movePos = moveTo.position;
            var distance = Vector3.Distance(movePos, transform.position);
            
            //Sets velocity linearly based on distance from hand
            var vel = (movePos - transform.position).normalized * followStrength * distance;
            body.velocity = vel;
        }

        private void OnCollisionEnter(Collision collision) {
            if(!collisions.Contains(collision.gameObject)) {
                collisions.Add(collision.gameObject);
            }
        }

        private void OnCollisionExit(Collision collision) {
            if(collisions.Contains(collision.gameObject)) {
                collisions.Remove(collision.gameObject);
            }
        }
        
        public int CollisionCount() {
            return collisions.Count;
        }

    }
}
