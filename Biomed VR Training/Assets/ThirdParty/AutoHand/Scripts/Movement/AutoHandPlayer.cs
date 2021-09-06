using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand.Demo;
using System;
using NaughtyAttributes;

namespace Autohand{
    [RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(CapsuleCollider)), DefaultExecutionOrder(-1)]
    public class AutoHandPlayer : MonoBehaviour{

        [Header("References")]
        [Tooltip("The tracked headCamera object")]
        public Camera headCamera;
        [Tooltip("The object that represents the forward direction movement, usually should be set as the camera or a tracked controller")]
        public Transform forwardFollow;
        [Tooltip("This should NOT be a child of this body. This should be a GameObject that contains all the tracked objects (head/controllers)")]
        public Transform trackingContainer;
        public Hand handRight;
        public Hand handLeft;

        [Header("Movement")]
        [Foldout("Movement")]
        [Tooltip("Movement speed when isGrounded")]
        public float moveSpeed = 2f;
        [Foldout("Movement")]
        [Tooltip("Maximum distance that the head is allowed to be from the body before movement on that axis is stopped")]
        [Min(0.1f)]
        public float maxHeadDistance = 0.3f;

        [Header("Height Settings")]
        [Foldout("Height")]
        public float heightOffset = 0;
        [Tooltip("Whether or not the capsule height should be adjusted to match the headCamera height")]
        [Foldout("Height")]
        public bool autoAdjustColliderHeight = true;
        [ShowIf("autoAdjustColliderHeight")]
        [Tooltip("Minimum and maximum auto adjusted height, to adjust height without auto adjustment change capsule collider height instead")]

        [Foldout("Height")]
        [MinMaxSlider(0, 3)]
        public Vector2 minMaxHeight = new Vector2(0.7f, 2f);



        [Tooltip("Whether or not to use snap turning or smooth turning"), Min(0)]
        [Foldout("Turning")]
        public bool snapTurning = true;
        [Tooltip("turn speed when not using snap turning - if snap turning, represents angle per snap")]
        [ShowIf("snapTurning")]
        [Foldout("Turning")]
        public float snapTurnAngle = 15f;
        [HideIf("snapTurning")]
        [Foldout("Turning")]
        public float smoothTurnSpeed = 10f;

        [Header("Grounding")]
        [Foldout("Grounding")]
        public bool useGrounding = true;
        [Foldout("Grounding"), Tooltip("Maximum height that the body can step up onto"), Min(0)]
        public float maxStepHeight = 0.1f;
        [Foldout("Grounding"), Tooltip("Maximum angle the player can walk on"), Min(0)]
        public float maxStepAngle = 30f;
        [Foldout("Grounding"), Tooltip("The layers that count as ground")]
        public LayerMask groundLayerMask;

        [Header("Crouching")]
        [Foldout("Crouching")]
        public bool crouching = false;
        [Foldout("Crouching")]
        public float crouchHeight = 0.6f;

        [Header("Climbing")]
        [Foldout("Climbing")]
        [Tooltip("Whether or not the player can use Climbable objects  (Objects with the Climbable component)")]
        public bool allowClimbing = true;
        [Tooltip("Whether or not the player move while climbing")]
        [ShowIf("allowClimbing")]
        [Foldout("Climbing")]
        public bool allowClimbingMovement = true;
        [Tooltip("How quickly the player can climb")]
        [ShowIf("allowClimbing")]
        [Foldout("Climbing")]
        public Vector3 climbingStrength = new Vector3(0.5f, 1f, 0.5f);

        [Header("Pushing")]
        [Foldout("Pushing")]
        [Tooltip("Whether or not the player can use Pushable objects (Objects with the Pushable component)")]
        public bool allowBodyPushing = true;
        [Tooltip("How quickly the player can climb")]
        [ShowIf("allowBodyPushing")]
        [Foldout("Pushing")]
        public Vector3 pushingStrength = new Vector3(1f, 1f, 1f);

        [Foldout("Platforms")]
        [Tooltip("Platforms will move the player with them. A platform is an object with the PlayerPlatform component on it")]
        public bool allowPlatforms = true;


        float movementDeadzone = 0.2f;
        float turnDeadzone = 0.4f;
        float turnResetzone = 0.3f;
        float groundedOffset = 0.002f;

        float headFollowSpeed = 4f;
        float groundedDrag = 0.5f;

        HeadPhysicsFollower headPhysicsFollower;
        Rigidbody body;
        CapsuleCollider bodyCapsule;

        Transform moveTo;
        Vector3 moveDirection;
        Vector3 moveVelocity = Vector3.zero;
        List<Vector3> moveDirections = new List<Vector3>();
        Vector3 climbAxis;
        Vector3 adjustedOffset;
        float turningAxis;
        float deltaY;
        bool isGrounded = false;
        bool axisReset = true;
        float playerHeight = 0;
        float lastHeightOffset;
        bool lastCrouching;

        Hand lastRightHand;
        Hand lastLeftHand;
        
        Dictionary<Hand, Climbable> climbing;
        Dictionary<Pushable, Hand> pushRight;
        Dictionary<Pushable, int> pushRightCount;
        Dictionary<Pushable, Hand> pushLeft;
        Dictionary<Pushable, int> pushLeftCount;
        List<GameObject> collisions = new List<GameObject>();
        private Vector3 pushAxis;
        RaycastHit groundHit;

        List<PlayerPlatform> platforms = new List<PlayerPlatform>();
        Dictionary<PlayerPlatform, int> platformsCount = new Dictionary<PlayerPlatform, int>();
        Dictionary<PlayerPlatform, Vector3> platformPositions = new Dictionary<PlayerPlatform, Vector3>();
        Dictionary<PlayerPlatform, Quaternion> platformRotations = new Dictionary<PlayerPlatform, Quaternion>();
        private Quaternion startRot;
        private float headDistance;

        public void Start(){
            startRot = headCamera.transform.rotation;

            gameObject.layer = LayerMask.NameToLayer("HandPlayer");

            bodyCapsule = GetComponent<CapsuleCollider>();

            body = GetComponent<Rigidbody>();

            if(body.collisionDetectionMode == CollisionDetectionMode.Discrete)
                body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            
            if(forwardFollow == null)
                forwardFollow = headCamera.transform;

            body.freezeRotation = true;
            
            climbing = new Dictionary<Hand, Climbable>();
            pushRight = new Dictionary<Pushable, Hand>();
            pushRightCount = new Dictionary<Pushable, int>();
            pushLeft = new Dictionary<Pushable, Hand>();
            pushLeftCount = new Dictionary<Pushable, int>();
            collisions = new List<GameObject>();


            moveTo = new GameObject().transform;
            moveTo.transform.rotation = transform.rotation;
            moveTo.name = "PLAYER FOLLOW POINT";
            

            deltaY = transform.position.y;

            CreateHeadFollower();
        }

        void OnEnable(){
            EnableHand(handRight);
            EnableHand(handLeft);
        }

        void OnDisable(){
            DisableHand(handRight);
            DisableHand(handLeft);
        }



        void CreateHeadFollower() {
            var headFollower = new GameObject().transform;
            headFollower.name = "Head Follower";
            headFollower.parent = transform.parent;

            var col = headFollower.gameObject.AddComponent<SphereCollider>();
            col.material = bodyCapsule.material;
            col.radius = bodyCapsule.radius;
            col.material = bodyCapsule.material;

            var headBody = headFollower.gameObject.AddComponent<Rigidbody>();
            headBody.drag = 10;
            headBody.angularDrag = 5;
            headBody.freezeRotation = false;
            headBody.mass = body.mass/3f;
            headBody.position = new Vector3(transform.position.x, transform.position.y+1, transform.position.z);

            headPhysicsFollower = headFollower.gameObject.AddComponent<HeadPhysicsFollower>();
            headPhysicsFollower.headCamera = headCamera;
            headPhysicsFollower.followBody = transform;
            headPhysicsFollower.trackingContainer = trackingContainer;
            headPhysicsFollower.maxBodyDistance = maxHeadDistance;
        }

        

        void CheckHands() {
            if (lastLeftHand != handLeft) {
                EnableHand(handLeft);
                lastLeftHand = handLeft;
            }

            if (lastRightHand != handRight) {
                EnableHand(handRight);
                lastRightHand = handRight;
            }
        }

        void EnableHand(Hand hand)
        {
            hand.OnReleased += (handValue, grabbable) => { if (grabbable) grabbable.body.velocity += (AlterDirection(moveDirection, Time.fixedDeltaTime) / Time.fixedDeltaTime) / 2f; };
            hand.OnHeldConnectionBreak += (handValue, grabbable) => { if (grabbable && grabbable.HeldCount() == 1) grabbable.body.velocity += (AlterDirection(moveDirection, Time.fixedDeltaTime) / Time.fixedDeltaTime) / 2f; };

            if (allowClimbing){
                hand.OnGrabbed += StartClimb;
                hand.OnHeldConnectionBreak += EndClimb;
            }
            
            if(allowBodyPushing){
                hand.OnGrabbed += StartGrabPush;
                hand.OnHeldConnectionBreak += EndGrabPush;
                hand.OnHandTriggerStart += StartPush;
                hand.OnHandTriggerStop += StopPush;
            }
        }

        void DisableHand(Hand hand) {
            hand.OnReleased -= (handValue, grabbable) => { if (grabbable) grabbable.body.velocity += (AlterDirection(moveDirection, Time.fixedDeltaTime) / Time.fixedDeltaTime) / 2f; };
            hand.OnHeldConnectionBreak -= (handValue, grabbable) => { if (grabbable && grabbable.HeldCount() == 1) grabbable.body.velocity += (AlterDirection(moveDirection, Time.fixedDeltaTime) / Time.fixedDeltaTime) / 2f; };

            if (allowClimbing){
                hand.OnGrabbed -= StartClimb;
                hand.OnHeldConnectionBreak -= EndClimb;
                if(climbing.ContainsKey(hand))
                    climbing.Remove(hand);
            }
            
            if(allowBodyPushing)
            {
                hand.OnGrabbed -= StartGrabPush;
                hand.OnHeldConnectionBreak -= EndGrabPush;
                hand.OnHandTriggerStart -= StartPush;
                hand.OnHandTriggerStop -= StopPush;
                if (hand.left) {
                    pushLeft.Clear();
                    pushLeftCount.Clear();
                }
                else { 
                    pushRight.Clear();
                    pushRightCount.Clear();
                    
                }
            }
        }

        
        /// <summary>Sets move direction, uses move speed multiplyer</summary>
        public void Move(Vector2 axis, bool useDeadzone = false) {
            if(!useDeadzone || Mathf.Abs(axis.x) > movementDeadzone)
                moveDirection.x = axis.x;
            else
                moveDirection.x = 0;

            moveDirection.y = 0;

            if (!useDeadzone || Mathf.Abs(axis.y) > movementDeadzone)
                moveDirection.z = axis.y;
            else
                moveDirection.z = 0;

            moveDirection *= moveSpeed;
        }
        /// <summary>Sets move direction, uses move speed multiplyer</summary>
        public void Move(Vector3 axis, bool useDeadzone = false) {
            if(!useDeadzone || Mathf.Abs(axis.x) > movementDeadzone)
                moveDirection.x = axis.x;
            else
                moveDirection.x = 0;

            if (!useDeadzone || Mathf.Abs(axis.y) > movementDeadzone)
                moveDirection.y = axis.y;
            else
                moveDirection.y = 0;


            if (!useDeadzone || Mathf.Abs(axis.z) > movementDeadzone)
                moveDirection.z = axis.z;
            else
                moveDirection.z = 0;

            moveDirection *= moveSpeed;
        }

        /// <summary>Adds move direction on top of core movement for the next fixed movement update, does not use move speed</summary>
        public void AddMove(Vector3 axis, bool useDeadzone = false) {

            var moveDirection = Vector3.zero;
            if(!useDeadzone || Mathf.Abs(axis.x) > movementDeadzone)
                moveDirection.x = axis.x;
            else
                moveDirection.x = 0;

            if (!useDeadzone || Mathf.Abs(axis.y) > movementDeadzone)
                moveDirection.y = axis.y;
            else
                moveDirection.y = 0;


            if (!useDeadzone || Mathf.Abs(axis.z) > movementDeadzone)
                moveDirection.z = axis.z;
            else
                moveDirection.z = 0;

            moveDirections.Add(moveDirection);
        }

        public void AddVelocity(Vector3 velocity)
        {
            moveVelocity += velocity;
        }
        
        void LateUpdate(){
            if(!headPhysicsFollower.Started())
                return;

            handRight.allowUpdateMovement = handLeft.allowUpdateMovement = !IsPushing() && !IsClimbing();

            if (!UsePhysicsMovement())
            {
                Ground();
                UpdateMove(Time.deltaTime);
            }
        }


        void FixedUpdate(){
            if(!headPhysicsFollower.Started())
                return;

            CheckHands();

            if (UsePhysicsMovement())
            {
                Ground();
                UpdateMove(Time.fixedDeltaTime);
            }

            UpdateTurn();

            UpdatePlayerHeight();
            CheckPlatforms();

        }


        public void Turn(float turnAxis) {
            turningAxis = turnAxis;
        }

        
        void UpdateMove(float deltaTime)
        {
            MoveBody();

            Vector3 move = AlterDirection(moveDirection, deltaTime);

            for (int i = 0; i < moveDirections.Count; i++)
                move += moveDirections[i];

            move += moveVelocity;

            if (allowClimbing && IsClimbing())
            {
                var climbAxis = this.climbAxis;
                climbAxis.y = 0;
                move += climbAxis;
            }

            else if (allowBodyPushing && IsPushing())
            {
                var pushAxis = this.pushAxis;
                pushAxis.y = 0;
                move += pushAxis;
            }


            move.y = 0;

            //Adjusts height to headCamera to match body height movements
            move += new Vector3(0, body.position.y-deltaY, 0);

            var flatPosition = headCamera.transform.position;
            flatPosition.y = 0;
            var flatBodyPosition = body.position+move;
            flatBodyPosition.y = 0;
            headDistance = Vector3.Distance(flatPosition, flatBodyPosition);
            if (headDistance != 0)
            {
                if (headDistance >= maxHeadDistance)
                {
                    var idealPos = (flatBodyPosition - flatPosition) - (flatBodyPosition - flatPosition).normalized * maxHeadDistance;
                    move += idealPos;
                }
            }
            else
                headPhysicsFollower.transform.position = new Vector3(transform.position.x, headPhysicsFollower.transform.position.y, transform.position.z);

            if(move != Vector3.zero){
                MoveTracking();
            }

            ManageHeadOffset();

            deltaY = body.position.y;
            moveVelocity *= (1 - deltaTime * groundedDrag * 10);

            moveDirections.Clear();

            void MoveTracking()
            {
                headPhysicsFollower.body.position += move;
                trackingContainer.position += move;
            }

            void ManageHeadOffset(){
                //Keeps the head down when colliding something above it and manages bouncing back up when not
                if(Vector3.Distance(headCamera.transform.position, headPhysicsFollower.transform.position) > headPhysicsFollower.headCollider.radius/2f) {
                    var idealPos = headPhysicsFollower.transform.position+(headCamera.transform.position - headPhysicsFollower.transform.position).normalized*headPhysicsFollower.headCollider.radius/2f;
                    var offsetPos = headCamera.transform.position - idealPos;
                    trackingContainer.position -= offsetPos;
                    adjustedOffset += offsetPos;
                }
            
                if(headPhysicsFollower.CollisionCount() == 0){
                    var moveAdjustedOffset = Vector3.MoveTowards(adjustedOffset, Vector3.zero, deltaTime);
                    var moveAdjustedOffsetY = moveAdjustedOffset;
                    moveAdjustedOffsetY.x = moveAdjustedOffsetY.z = 0;
                    headPhysicsFollower.body.position += moveAdjustedOffsetY;
                    trackingContainer.position += moveAdjustedOffsetY;
                    adjustedOffset -= moveAdjustedOffset;
                }
            }
        
            void MoveBody(){
                moveTo.position = Vector3.zero;

                var headBodyDifference = headPhysicsFollower.body.position - body.position;
                headBodyDifference.y = 0;

                moveTo.position +=  headBodyDifference*headFollowSpeed*10f;

                if (!useGrounding)
                    moveTo.position += AlterDirection(moveDirection, 1);

                if (allowClimbing && IsClimbing()){
                    ApplyClimbingForce();
                    moveTo.position += climbAxis;
                    isGrounded = false;
                }

                else if (allowBodyPushing && IsPushing()){
                    ApplyPushingForce();
                    moveTo.position += pushAxis;
                    isGrounded = false;
                }

                for (int i = 0; i < moveDirections.Count; i++)
                {
                    var addDir = (moveDirections[i] / (deltaTime));
                    moveTo.position += addDir;
                }

                moveTo.position += moveVelocity;

                var vel = moveTo.position;

                body.constraints = RigidbodyConstraints.None;
                body.freezeRotation = true;

                if (useGrounding){
                    if (pushAxis.y > 0 || IsClimbing()){
                        body.useGravity = false;
                    }
                    else if(isGrounded){
                        vel.y = body.velocity.y;
                        body.useGravity = false;
                        body.constraints = RigidbodyConstraints.FreezePositionY;
                        body.freezeRotation = true;
                    }
                    else{
                        vel.y = body.velocity.y;
                        body.useGravity = true;
                    }
                }
                else{
                    body.useGravity = false;
                    if (isGrounded && vel.y <= 0)
                        vel.y = 0;
                }

                body.velocity = vel; 

            }
        }


        Vector3 AlterDirection(Vector3 moveAxis, float deltaTime) {
            Vector3 holder;

            if (useGrounding) {
                Quaternion forwardAxis = Quaternion.identity;
                if(forwardFollow != null)
                    forwardAxis = Quaternion.AngleAxis(forwardFollow.eulerAngles.y, Vector3.up);
                
                if(isGrounded) {
                    holder = forwardAxis*(new Vector3(moveAxis.x, moveAxis.y, moveAxis.z)) * deltaTime;
                }
                else {
                    holder = forwardAxis*(new Vector3(moveAxis.x, moveAxis.y, moveAxis.z)) * deltaTime;
                    if (isGrounded && holder.y < 0)
                        holder.y = 0;
                }
            }
            else {
                holder = forwardFollow.rotation*(new Vector3(moveAxis.x, moveAxis.y, moveAxis.z)) * deltaTime;
            }

            return holder;
        }



        
        void UpdateTurn(){
            //Snap turning
            if (snapTurning){
                if (turningAxis > turnDeadzone && axisReset){
                    trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, snapTurnAngle);
                    axisReset = false;
                    handRight?.SetHandLocation(handRight.moveTo.position, handRight.moveTo.rotation);
                    handLeft?.SetHandLocation(handLeft.moveTo.position, handLeft.moveTo.rotation);
                }
                else if (turningAxis < -turnDeadzone && axisReset){
                    trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, -snapTurnAngle);
                    axisReset = false;
                    handRight?.SetHandLocation(handRight.moveTo.position, handRight.moveTo.rotation);
                    handLeft?.SetHandLocation(handLeft.moveTo.position, handLeft.moveTo.rotation);
                }
            }
            else if(Mathf.Abs(turningAxis) > turnDeadzone){
                trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, smoothTurnSpeed*turningAxis*Time.fixedDeltaTime);
                handRight?.SetHandLocation(handRight.moveTo.position, handRight.moveTo.rotation);
                handLeft?.SetHandLocation(handLeft.moveTo.position, handLeft.moveTo.rotation);
            }

            if (Mathf.Abs(turningAxis) < turnResetzone)
                axisReset = true;
        }


        public void SetPosition(Vector3 position){
            SetPosition(position, headCamera.transform.rotation);
        }

        public void SetPosition(Vector3 position, Quaternion rotation){
            Vector3 deltaPos = position - transform.position;
            transform.position += deltaPos;
            body.position = transform.position;

            headPhysicsFollower.transform.position += deltaPos;
            headPhysicsFollower.body.position = headPhysicsFollower.transform.position;


            deltaPos.y = 0;
            trackingContainer.position += deltaPos;

            var deltaRot = rotation * Quaternion.Inverse(headCamera.transform.rotation); 
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, deltaRot.eulerAngles.y);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.right, deltaRot.eulerAngles.x);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.forward, deltaRot.eulerAngles.z);
            
            axisReset = false;
            handRight?.SetHandLocation(handRight.moveTo.position, handRight.moveTo.rotation);
            handLeft?.SetHandLocation(handLeft.moveTo.position, handLeft.moveTo.rotation);
        }

        public void SetRotation(Quaternion rotation){
            var deltaRot = rotation * Quaternion.Inverse(headCamera.transform.rotation);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, deltaRot.eulerAngles.y);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.right, deltaRot.eulerAngles.x);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.forward, deltaRot.eulerAngles.z);

            axisReset = false;
            handRight?.SetHandLocation(handRight.moveTo.position, handRight.moveTo.rotation);
            handLeft?.SetHandLocation(handLeft.moveTo.position, handLeft.moveTo.rotation);
        }

        public void AddRotation(Quaternion addRotation){
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, addRotation.eulerAngles.y);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.right, addRotation.eulerAngles.x);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.forward, addRotation.eulerAngles.z);

            axisReset = false;
            handRight?.SetHandLocation(handRight.moveTo.position, handRight.moveTo.rotation);
            handLeft?.SetHandLocation(handLeft.moveTo.position, handLeft.moveTo.rotation);
        }

        public void AddLocalRotation(Quaternion addRotation){
            trackingContainer.RotateAround(headCamera.transform.position, headCamera.transform.up, addRotation.eulerAngles.y);
            trackingContainer.RotateAround(headCamera.transform.position, headCamera.transform.right, addRotation.eulerAngles.x);
            trackingContainer.RotateAround(headCamera.transform.position, headCamera.transform.forward, addRotation.eulerAngles.z);

            axisReset = false;
            handRight?.SetHandLocation(handRight.moveTo.position, handRight.moveTo.rotation);
            handLeft?.SetHandLocation(handLeft.moveTo.position, handLeft.moveTo.rotation);
        }

        public void Recenter(){
            var deltaRot = startRot * Quaternion.Inverse(headCamera.transform.rotation);
            trackingContainer.RotateAround(headCamera.transform.position, Vector3.up, deltaRot.eulerAngles.y);
        }

        
        void Ground(){
            isGrounded = false;

            RaycastHit stepHit;
            var stepPos = transform.position;
            float highestPoint = -1;
            float newHeightPoint = transform.position.y;

            stepPos.y += maxStepHeight;
            if(Physics.Raycast(stepPos, Vector3.down, out stepHit, maxStepHeight + groundedOffset, groundLayerMask)) {
                isGrounded = true;
                var stepAngle = Vector3.Angle(stepHit.normal, Vector3.up);
                if(stepAngle < maxStepAngle && stepHit.point.y - transform.position.y > highestPoint) {
                    groundHit = stepHit;
                    highestPoint = stepHit.point.y - transform.position.y;
                    newHeightPoint = stepHit.point.y;
                }
            }
            
            for(int i = 0; i < 8; i++) {
                stepPos = transform.position;
                stepPos.x += Mathf.Cos(i*Mathf.PI/4f)*(bodyCapsule.radius+0.05f);
                stepPos.z += Mathf.Sin(i*Mathf.PI/4f)*(bodyCapsule.radius+0.05f);
                stepPos.y += maxStepHeight;
                if(Physics.Raycast(stepPos, Vector3.down, out stepHit, maxStepHeight + groundedOffset, groundLayerMask)) {
                    isGrounded = true;
                    var stepAngle = Vector3.Angle(stepHit.normal, Vector3.up);
                    if(stepAngle < maxStepAngle && stepHit.point.y - transform.position.y > highestPoint) {
                        groundHit = stepHit;
                        highestPoint = stepHit.point.y - transform.position.y;
                        newHeightPoint = stepHit.point.y;
                    }
                }
            }
            
            for(int i = 0; i < 8; i++) {
                stepPos = transform.position;
                stepPos.x += Mathf.Cos(i*Mathf.PI/4f)*(bodyCapsule.radius+0.05f)/2f;
                stepPos.z += Mathf.Sin(i*Mathf.PI/4f)*(bodyCapsule.radius+0.05f)/2f;
                stepPos.y += maxStepHeight;
                if(Physics.Raycast(stepPos, Vector3.down, out stepHit, maxStepHeight + groundedOffset, groundLayerMask)) {
                    isGrounded = true;
                    var stepAngle = Vector3.Angle(stepHit.normal, Vector3.up);
                    if(stepAngle < maxStepAngle && stepHit.point.y - transform.position.y > highestPoint) {
                        groundHit = stepHit;
                        highestPoint = stepHit.point.y - transform.position.y;
                        newHeightPoint = stepHit.point.y;
                    }
                }
            }

            if(isGrounded) {
                var newHeight = transform.position;
                newHeight.y = newHeightPoint;
                transform.position = newHeight;
                if (useGrounding)
                {
                    body.constraints = RigidbodyConstraints.FreezePositionY;
                    body.freezeRotation = true;
                    body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
                }
            }
            
        }
        
        public bool IsGrounded(){
            return isGrounded;
        }

        public void ToggleFlying()
        {
            useGrounding = !useGrounding;
        }

        void UpdatePlayerHeight(){
            if (!useGrounding)
                return;

            if (heightOffset != lastHeightOffset) {
                trackingContainer.Translate(new Vector3(0, heightOffset - lastHeightOffset, 0));
                lastHeightOffset = heightOffset;
            }

            if(crouching != lastCrouching)
            {
                var height = crouching ? -crouchHeight : crouchHeight;
                trackingContainer.Translate(new Vector3(0, height, 0));
                lastCrouching = crouching;
            }

            if(autoAdjustColliderHeight){
                playerHeight = Mathf.Clamp(headCamera.transform.position.y - transform.position.y, minMaxHeight.x, minMaxHeight.y);
                bodyCapsule.height = playerHeight;
                var centerHeight = playerHeight/2f > bodyCapsule.radius ? playerHeight/2f : bodyCapsule.radius; 
                bodyCapsule.center = new Vector3(0, centerHeight, 0);
            }
        }
        


        void StartPush(Hand hand, GameObject other) {
            if(!allowBodyPushing || IsClimbing())
                return;

            if(other.CanGetComponent(out Pushable push) && push.enabled) {
                if(hand.left) {
                    if(!pushLeft.ContainsKey(push)){
                        pushLeft.Add(push, hand);
                        pushLeftCount.Add(push, 1);
                    }
                    else {
                        pushLeftCount[push]++;
                    }
                }

                if(!hand.left && !pushRight.ContainsKey(push)) {
                    if(!pushRight.ContainsKey(push)){
                        pushRight.Add(push, hand);
                        pushRightCount.Add(push, 1);
                    }
                    else {
                        pushRightCount[push]++;
                    }
                }
            }
        }
        
        void StopPush(Hand hand, GameObject other) {
            if(!allowBodyPushing)
                return;
            
            if(other.CanGetComponent(out Pushable push)) {
                if(hand.left && pushLeft.ContainsKey(push)) {
                    var count = --pushLeftCount[push];
                    if(count == 0){
                        pushLeft.Remove(push);
                        pushLeftCount.Remove(push);
                    }
                }
                if(!hand.left && pushRight.ContainsKey(push)) {
                    var count = --pushRightCount[push];
                    if(count == 0){
                        pushRight.Remove(push);
                        pushRightCount.Remove(push);
                    }
                }
            }
        }

        void StartGrabPush(Hand hand, Grabbable grab) {
            if(!allowBodyPushing || IsClimbing())
                return;
            try {
                if(grab.CanGetComponent(out Pushable push) && push.enabled) {
                    if(hand.left) {
                        if(!pushLeft.ContainsKey(push)){
                            pushLeft.Add(push, hand);
                            pushLeftCount.Add(push, 1);
                        }
                    }

                    if(!hand.left && !pushRight.ContainsKey(push)) {
                        if(!pushRight.ContainsKey(push)){
                            pushRight.Add(push, hand);
                            pushRightCount.Add(push, 1);
                        }
                    }
                }
            }
            catch { }
        }
        
        void EndGrabPush(Hand hand, Grabbable grab) {
            if(grab != null && grab.CanGetComponent(out Pushable push)) {
                if (hand.left && pushLeft.ContainsKey(push)) {
                    pushLeft.Remove(push);
                    pushLeftCount.Remove(push);
                }
                else if (!hand.left && pushRight.ContainsKey(push)) {
                    pushRight.Remove(push);
                    pushRightCount.Remove(push);
                }
                    
            }
        }
        
        void ApplyPushingForce() {
            pushAxis = Vector3.zero;
            var rightHandCast = Physics.RaycastAll(handRight.transform.position, Vector3.down, 0.1f, ~handRight.handLayers);
            var leftHandCast = Physics.RaycastAll(handLeft.transform.position, Vector3.down, 0.1f, ~handLeft.handLayers);
            List<GameObject> hitObjects = new List<GameObject>();
            foreach (var hit in rightHandCast){
                hitObjects.Add(hit.transform.gameObject);
            }
            foreach (var hit in leftHandCast){
                hitObjects.Add(hit.transform.gameObject);
            }

            foreach (var push in pushRight) {
                if (push.Key.enabled && !push.Value.IsGrabbing()){
                    Vector3 offset = Vector3.zero;
                    var distance = Vector3.Distance(push.Value.body.position, push.Value.moveTo.position);
                    if (distance > 0)
                        offset = Vector3.Scale((push.Value.body.position - push.Value.moveTo.position), push.Key.strengthScale);

                    offset = Vector3.Scale(offset, pushingStrength);
                    if (!hitObjects.Contains(push.Key.transform.gameObject))
                        offset.y = 0;
                    pushAxis += offset/2f;
                }
            }

            foreach(var push in pushLeft) {
                if (push.Key.enabled && !push.Value.IsGrabbing()){
                    Vector3 offset = Vector3.zero;
                    var distance = Vector3.Distance(push.Value.body.position, push.Value.moveTo.position);
                    if(distance > 0)
                        offset = Vector3.Scale((push.Value.body.position-push.Value.moveTo.position), push.Key.strengthScale);

                    offset = Vector3.Scale(offset, pushingStrength);
                    if (!hitObjects.Contains(push.Key.transform.gameObject))
                        offset.y = 0;
                    pushAxis += offset/2f;
                }
            }

        }

        public bool IsPushing() {
            bool isPushing = false;
            foreach (var push in pushRight){
                if (push.Key.enabled)
                    isPushing = true;
            }
            foreach (var push in pushLeft){
                if (push.Key.enabled)
                    isPushing = true;
            }

            return isPushing;
        }



        void StartClimb(Hand hand, Grabbable grab) {
            if(!allowClimbing)
                return;

            try{
                if (!climbing.ContainsKey(hand) && grab != null && grab.CanGetComponent(out Climbable climbbable) && climbbable.enabled){
                    if (climbing.Count == 0){
                        pushRight.Clear();
                        pushRightCount.Clear();

                        pushLeft.Clear();
                        pushLeftCount.Clear();
                    }

                    climbing.Add(hand, climbbable);
                }
            }
            catch { }
        }
        
        void EndClimb(Hand hand, Grabbable grab) {
            if(!allowClimbing)
                return;

            if(climbing.ContainsKey(hand)) {
                var climb = climbing[hand];
                climbing.Remove(hand);
            }
        }

        void ApplyClimbingForce(){
            climbAxis = Vector3.zero;
            if(climbing.Count > 0){
                foreach(var hand in climbing) {
                    if (hand.Value.enabled) {
                        var offset = Vector3.Scale(hand.Key.body.position-hand.Key.moveTo.position, hand.Value.axis);
                        offset = Vector3.Scale(offset, climbingStrength);
                        climbAxis += offset/climbing.Count;
                    }
                }
            }
        }

        public bool IsClimbing() {
            bool isClimbing = false;
            foreach (var climb in climbing){
                if (climb.Value.enabled)
                    isClimbing = true;
            }

            return isClimbing;
        }
        

        void CheckPlatforms(){
            if (!allowPlatforms)
                return;

            foreach (var platform in platforms){
                var deltaPos = platform.transform.position - platformPositions[platform];
                trackingContainer.position += deltaPos;
                body.position += deltaPos;
                platformPositions[platform] = platform.transform.position;

                var deltaRot = (Quaternion.Inverse(platformRotations[platform]) * platform.transform.rotation).eulerAngles;
                trackingContainer.RotateAround(platform.transform.position, Vector3.up, deltaRot.y);
                trackingContainer.RotateAround(platform.transform.position, Vector3.right, deltaRot.x);
                trackingContainer.RotateAround(platform.transform.position, Vector3.forward, deltaRot.z);

                platformRotations[platform] = platform.transform.rotation;
            }
        }
        


        private void OnTriggerEnter(Collider other){
            
            if (!allowPlatforms)
                return;

            if (other.CanGetComponent(out PlayerPlatform platform)){
                if (!platforms.Contains(platform)) {
                    platforms.Add(platform);
                    platformPositions.Add(platform, platform.transform.position);
                    platformRotations.Add(platform, platform.transform.rotation);
                    platformsCount.Add(platform, 1);
                }
                else {
                    platformsCount[platform]++;
                }
            }
        }

        private void OnTriggerExit(Collider other){
            if (!allowPlatforms)
                return;

            if (other.CanGetComponent(out PlayerPlatform platform)){
                if (platforms.Contains(platform)) {
                    if(platformsCount[platform]-1 == 0) {
                        platforms.Remove(platform);
                        platformPositions.Remove(platform);
                        platformRotations.Remove(platform);
                        platformsCount.Remove(platform);
                    }
                    else {
                        platformsCount[platform]--;
                    }
                }
            }
        }

        private bool UsePhysicsMovement()
        {
            var bodyCollision = headDistance*2 > maxHeadDistance;
            var handCollision = ((handLeft.CollisionCount() + handRight.CollisionCount()) > 0) || handLeft.IsGrabbing() || handRight.IsGrabbing();
            var usePhysics = bodyCollision || handCollision || IsClimbing() || IsPushing();
            return usePhysics;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!collisions.Contains(collision.gameObject)) {
                collisions.Add(collision.gameObject);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if(collisions.Contains(collision.gameObject)) {
                collisions.Remove(collision.gameObject);
            }
        }



        public static LayerMask GetPhysicsLayerMask(int currentLayer) {
            int finalMask = 0;
            for(int i = 0; i < 32; i++){
                if(!Physics.GetIgnoreLayerCollision(currentLayer, i))
                    finalMask = finalMask | (1 << i);
            }
            return finalMask;
        }

        private void OnDrawGizmos() {
            if(bodyCapsule == null)
                bodyCapsule = GetComponent<CapsuleCollider>();
            
            if(isGrounded)
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.red;

            var offsetPos = transform.position;
            var offSetEndPos = offsetPos;
            offSetEndPos.y += maxStepHeight;
            Gizmos.DrawLine(offsetPos, offSetEndPos);
            
            for(int i = 0; i < 8; i++) {
                offsetPos = transform.position;
                offsetPos.x += Mathf.Cos(i*Mathf.PI/4f)*(bodyCapsule.radius+0.05f);
                offsetPos.z += Mathf.Sin(i*Mathf.PI/4f)*(bodyCapsule.radius+0.05f);
                offSetEndPos = offsetPos;
                offSetEndPos.y += maxStepHeight;
                Gizmos.DrawLine(offsetPos, offSetEndPos);
            }

            for(int i = 0; i < 8; i++) {
                offsetPos = transform.position;
                offsetPos.x += Mathf.Cos(i*Mathf.PI/4f)*(bodyCapsule.radius+0.05f)/2f;
                offsetPos.z += Mathf.Sin(i*Mathf.PI/4f)*(bodyCapsule.radius+0.05f)/2f;
                offSetEndPos = offsetPos;
                offSetEndPos.y += maxStepHeight;
                Gizmos.DrawLine(offsetPos, offSetEndPos);
            }
            
            if(headCamera == null || forwardFollow == null)
                return;

            Gizmos.color = Color.blue;
            var containerAxis = Quaternion.AngleAxis(forwardFollow.transform.localEulerAngles.y, Vector3.up);
            var forward = Quaternion.AngleAxis(forwardFollow.transform.localEulerAngles.y, Vector3.up);
            Gizmos.DrawRay(transform.position, containerAxis*forward*Vector3.forward);
            
            Gizmos.color = Color.red;
            var right = Quaternion.AngleAxis(forwardFollow.transform.localEulerAngles.y, Vector3.up);
            Gizmos.DrawRay(transform.position, containerAxis*right*Vector3.right);
        }
    }
}
