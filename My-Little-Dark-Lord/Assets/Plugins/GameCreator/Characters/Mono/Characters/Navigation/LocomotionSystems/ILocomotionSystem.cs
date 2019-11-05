namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    public abstract class ILocomotionSystem
    {
        public class TargetRotation
        {
            public bool hasRotation;
            public Quaternion rotation;

            public TargetRotation(bool hasRotation = false, Vector3 direction = default(Vector3))
            {
                this.hasRotation = hasRotation;
				this.rotation = hasRotation && direction != Vector3.zero
                    ? Quaternion.LookRotation(direction)
                    : Quaternion.identity;
            }
        }

        protected class DirectionData
        {
            public CharacterLocomotion.FACE_DIRECTION direction;
            public TargetPosition target;

            public DirectionData(CharacterLocomotion.FACE_DIRECTION direction, TargetPosition target)
            {
                this.direction = direction;
                this.target = target;
            }
        }

        // CONSTANT PROPERTIES: -------------------------------------------------------------------

		protected static readonly Vector3 HORIZONTAL_PLANE = new Vector3(1,0,1);
        protected const float SLOW_THRESHOLD = 1.0f;
        protected const float STOP_THRESHOLD = 0.05f;
        protected const float SLIDING_SMOOTH = 0.35f;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected CharacterLocomotion characterLocomotion;
        public Vector3 aimDirection;
        public Vector3 movementDirection;
        public float pivotSpeed;

        public bool isSliding = false;
        protected Vector3 slideDirection = Vector3.zero;

        public bool isDashing { private set; get; }
        protected Vector3 dashVelocity = Vector3.zero;

        private float dashStartTime = -100f;
        private float dashDuration = 0f;
        private float dashDrag = 10f;

        private float slidingValue = 0f;
        private float slidingSpeed = 0f;

        private DirectionData cacheDirectionData = new DirectionData(
            CharacterLocomotion.FACE_DIRECTION.MovementDirection,
            new TargetPosition(TargetPosition.Target.Player)
        );

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Setup(CharacterLocomotion characterLocomotion)
        {
            this.characterLocomotion = characterLocomotion;
        }

        public void Dash(Vector3 direction, float impulse, float duration, float drag)
        {
            this.isDashing = true;
            this.dashStartTime = Time.time;
            this.dashDuration = duration;
            this.dashDrag = drag;

            this.dashVelocity = direction.normalized * (
                impulse * Mathf.Log(1f / (Time.deltaTime * this.dashDrag + 1)) / -Time.deltaTime
            );
        }

        // ABSTRACT & VIRTUAL METHODS: ------------------------------------------------------------

        public virtual CharacterLocomotion.LOCOMOTION_SYSTEM Update()
        {
            if (this.isDashing)
            {
                if (Time.time >= this.dashStartTime + this.dashDuration)
                {
                    this.dashVelocity /= 1 + this.dashDrag * Time.deltaTime;
                }

                if (this.dashVelocity.magnitude < this.characterLocomotion.runSpeed)
                {
                    this.isDashing = false;
                }
            }

            return CharacterLocomotion.LOCOMOTION_SYSTEM.CharacterController;
        }

        public abstract void OnDestroy();

        // CHARACTER CONTROLLER METHODS: ----------------------------------------------------------

        protected Quaternion UpdateRotation(Vector3 targetDirection)
        {
            Quaternion targetRotation = this.characterLocomotion.character.transform.rotation;
            this.aimDirection = this.characterLocomotion.character.transform.forward;
            this.movementDirection = (targetDirection == Vector3.zero
                ? this.aimDirection
                : targetDirection.normalized
            );

            DirectionData faceDirection = this.GetFaceDirection();

            if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.MovementDirection &&
                targetDirection != Vector3.zero)
            {
                Quaternion srcRotation = this.characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(targetDirection);
                this.aimDirection = dstRotation * Vector3.forward;

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * this.characterLocomotion.angularSpeed
                );
            }
            else if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.CameraDirection &&
                HookCamera.Instance != null)
            {
                Vector3 camDirection = HookCamera.Instance.transform.TransformDirection(Vector3.forward);
                this.aimDirection = camDirection;

                camDirection.Scale(new Vector3(1, 0, 1));

                Quaternion srcRotation = this.characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(camDirection);

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * this.characterLocomotion.angularSpeed
                );
            }
            else if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.Target)
            {
                Vector3 target = faceDirection.target.GetPosition(this.characterLocomotion.character.gameObject);
                Vector3 direction = target - this.characterLocomotion.character.transform.position;
                this.aimDirection = direction;

                direction.Scale(new Vector3(1, 0, 1));

                Quaternion srcRotation = this.characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(direction);

                targetRotation = Quaternion.RotateTowards(
                    srcRotation,
                    dstRotation,
                    Time.deltaTime * this.characterLocomotion.angularSpeed
                );
            }
            else if (faceDirection.direction == CharacterLocomotion.FACE_DIRECTION.GroundPlaneCursor)
            {
                Camera camera = null;
                if (camera == null)
                {
                    if (HookCamera.Instance != null) camera = HookCamera.Instance.Get<Camera>();
                    if (camera == null && Camera.main != null) camera = Camera.main;
                }

                Ray cameraRay = camera.ScreenPointToRay(Input.mousePosition);
                Transform character = this.characterLocomotion.character.transform;

                Plane plane = new Plane(Vector3.up, character.position);
                float rayDistance = 0.0f;

                if (plane.Raycast(cameraRay, out rayDistance))
                {
                    Vector3 cursor = cameraRay.GetPoint(rayDistance);
                    Vector3 target = Vector3.MoveTowards(character.position, cursor, 1f);
                    Vector3 direction = target - this.characterLocomotion.character.transform.position;
                    direction.Scale(new Vector3(1, 0, 1));

                    Quaternion srcRotation = character.rotation;
                    Quaternion dstRotation = Quaternion.LookRotation(direction);
                    this.aimDirection = dstRotation * Vector3.forward;

                    targetRotation = Quaternion.RotateTowards(
                        srcRotation,
                        dstRotation,
                        Time.deltaTime * this.characterLocomotion.angularSpeed
                    );
                }
            }

            return targetRotation;
        }

        protected float CalculateSpeed(Vector3 targetDirection, bool isGrounded)
        {
            float speed = (this.characterLocomotion.canRun
                ? this.characterLocomotion.runSpeed
                : this.characterLocomotion.runSpeed / 2.0f
            );

            DirectionData direction = this.GetFaceDirection();

            if (direction.direction == CharacterLocomotion.FACE_DIRECTION.MovementDirection &&
                targetDirection != Vector3.zero)
            {
                Quaternion srcRotation = this.characterLocomotion.character.transform.rotation;
                Quaternion dstRotation = Quaternion.LookRotation(targetDirection);
                float angle = Quaternion.Angle(srcRotation, dstRotation) / 180.0f;
                float speedDampening = Mathf.Clamp(1.0f - angle, 0.5f, 1.0f);
                speed *= speedDampening;
            }

            return speed;
        }

        protected virtual void UpdateAnimationConstraints(ref Vector3 targetDirection, ref Quaternion targetRotation)
        {
            if (this.characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_MOVEMENT)
            {
                if (targetDirection == Vector3.zero)
                {
                    targetDirection = this.characterLocomotion.characterController.transform.forward;
                }
            }

            if (this.characterLocomotion.animatorConstraint == CharacterLocomotion.ANIM_CONSTRAINT.KEEP_POSITION)
            {
                targetDirection = Vector3.zero;
                targetRotation = this.characterLocomotion.characterController.transform.rotation;
            }
        }

        protected virtual void UpdateSliding()
        {
            float slopeAngle = Vector3.Angle(Vector3.up, this.characterLocomotion.terrainNormal);
            bool frameSliding = (
                this.characterLocomotion.character.IsGrounded() &&
                slopeAngle > this.characterLocomotion.characterController.slopeLimit
            );

            this.slidingValue = Mathf.SmoothDamp(
                this.slidingValue,
                frameSliding ? 1f : 0f,
                ref this.slidingSpeed,
                SLIDING_SMOOTH
            );

            this.isSliding = this.slidingValue > 0.5f;
            if (this.isSliding)
            {
                this.isSliding = true;
                this.slideDirection = Vector3.Reflect(
                    Vector3.down, this.characterLocomotion.terrainNormal
                ) * this.characterLocomotion.runSpeed;
            }
            else
            {
                this.slideDirection = Vector3.zero;
            }
        }

        protected DirectionData GetFaceDirection()
        {
            CharacterLocomotion.FACE_DIRECTION direction = this.characterLocomotion.faceDirection;
            TargetPosition target = this.characterLocomotion.faceDirectionTarget;

            if (this.characterLocomotion.overrideFaceDirection != CharacterLocomotion.OVERRIDE_FACE_DIRECTION.None)
            {
                direction = (CharacterLocomotion.FACE_DIRECTION)this.characterLocomotion.overrideFaceDirection;
                target = this.characterLocomotion.overrideFaceDirectionTarget;
            }

            this.cacheDirectionData.direction = direction;
            this.cacheDirectionData.target = target;

            return this.cacheDirectionData;
        }
    }
}