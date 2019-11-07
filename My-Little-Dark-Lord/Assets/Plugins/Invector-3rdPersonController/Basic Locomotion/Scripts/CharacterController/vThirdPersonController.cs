using UnityEngine;
using System.Collections;

namespace Invector.vCharacterController
{
    [vClassHeader("THIRD PERSON CONTROLLER", iconName = "controllerIcon")]
    public class vThirdPersonController : vThirdPersonAnimator
    {
        #region Variables

        [vHelpBox("Check this option to transfer your character from one scene to another, uncheck if you're planning to use the controller with any kind of Multiplayer local or online")]
        public bool useInstance = true;
        public static vThirdPersonController instance;

        #endregion

        protected virtual void Awake()
        {
            StartCoroutine(UpdateRaycast()); // limit raycasts calls for better performance
        }

        protected override void Start()
        {
            base.Start();
            if (!useInstance) return;

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
                this.gameObject.name = gameObject.name + " Instance";
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }

        #region Locomotion Actions

        public virtual void Sprint(bool value)
        {
            var sprintConditions = (currentStamina > 0 && input.sqrMagnitude > 0.1f && !isCrouching && isGrounded && !customAction && !(isStrafing && !strafeSpeed.walkByDefault && (direction >= 0.5 || direction <= -0.5 || speed <= 0)));
            if (value && sprintConditions)
            {
                if (currentStamina > (finishStaminaOnSprint ? sprintStamina : 0) && input.sqrMagnitude > 0.1f)
                {
                    finishStaminaOnSprint = false;
                    if (isGrounded && !isCrouching && useContinuousSprint)
                    {
                        isSprinting = !isSprinting;
                        OnStartSprinting.Invoke();
                    }
                    else if (!isSprinting)
                    {
                        isSprinting = true;
                    }
                }
                else if (!useContinuousSprint && isSprinting)
                {
                    if (currentStamina <= 0)
                    {
                        finishStaminaOnSprint = true;
                        OnFinishSprintingByStamina.Invoke();
                    }
                    isSprinting = false;
                    OnFinishSprinting.Invoke();
                }
            }
            else if (isSprinting && (!useContinuousSprint || !sprintConditions))
            {
                if (currentStamina <= 0)
                {
                    finishStaminaOnSprint = true;
                    OnFinishSprintingByStamina.Invoke();
                }

                isSprinting = false;
                OnFinishSprinting.Invoke();
            }
        }

        public virtual void Crouch()
        {
            if (isGrounded && !customAction)
            {
                if (isCrouching && CanExitCrouch())
                    isCrouching = false;
                else
                {
                    isCrouching = true;
                }
            }
        }

        public virtual void Strafe()
        {
            isStrafing = !isStrafing;
        }

        public virtual void Jump(bool consumeStamina = false)
        {
            if (customAction || GroundAngle() > slopeLimit) return;

            // know if has enough stamina to make this action
            bool staminaConditions = currentStamina > jumpStamina;
            // conditions to do this action
            bool jumpConditions = !isCrouching && isGrounded && staminaConditions && !isJumping;
            // return if jumpCondigions is false
            if (!jumpConditions) return;
            // trigger jump behaviour
            jumpCounter = jumpTimer;
            isJumping = true;
            OnJump.Invoke();
            // trigger jump animations
            if (input.sqrMagnitude < 0.1f)
                animator.CrossFadeInFixedTime("Jump", 0.1f);
            else
                animator.CrossFadeInFixedTime("JumpMove", .2f);
            // reduce stamina
            if (consumeStamina)
            {
                ReduceStamina(jumpStamina, false);
                currentStaminaRecoveryDelay = 1f;
            }
        }

        public virtual void Roll()
        {
            bool staminaCondition = currentStamina > rollStamina;            
            // general conditions to roll
            bool rollConditions = (input != Vector2.zero || speed > 0.25f) && !customAction && isGrounded && staminaCondition && !isJumping;

            if (!rollConditions || isRolling) return;

            animator.CrossFadeInFixedTime("Roll", 0.1f);
            ReduceStamina(rollStamina, false);
            currentStaminaRecoveryDelay = 2f;
        }

        /// <summary>
        /// Use another transform as  reference to rotate
        /// </summary>
        /// <param name="referenceTransform"></param>
        public virtual void RotateWithAnotherTransform(Transform referenceTransform)
        {
            var newRotation = new Vector3(transform.eulerAngles.x, referenceTransform.eulerAngles.y, transform.eulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(newRotation), strafeSpeed.rotationSpeed * Time.deltaTime);
            targetRotation = transform.rotation;
        }

        #endregion

        #region Check Action Triggers 

        /// <summary>
        /// Call this in OnTriggerEnter or OnTriggerStay to check if enter in triggerActions     
        /// </summary>
        /// <param name="other">collider trigger</param>                         
        protected override void OnTriggerStay(Collider other)
        {
            try
            {
                CheckForAutoCrouch(other);
            }
            catch (UnityException e)
            {
                Debug.LogWarning(e.Message);
            }
            base.OnTriggerStay(other);
        }

        /// <summary>
        /// Call this in OnTriggerExit to check if exit of triggerActions 
        /// </summary>
        /// <param name="other"></param>
        protected override void OnTriggerExit(Collider other)
        {
            AutoCrouchExit(other);
            base.OnTriggerExit(other);
        }

        #region Update Raycasts  

        protected IEnumerator UpdateRaycast()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                AutoCrouch();
                //StopMove();
            }
        }

        #endregion

        #region Crouch Methods

        public virtual void UseAutoCrouch(bool value)
        {
            autoCrouch = value;
        }

        protected virtual void AutoCrouch()
        {
            if (autoCrouch)
                isCrouching = true;

            if (autoCrouch && !inCrouchArea && CanExitCrouch())
            {
                autoCrouch = false;
                isCrouching = false;
            }
        }

        public virtual bool CanExitCrouch()
        {
            // radius of SphereCast
            float radius = _capsuleCollider.radius * 0.9f;
            // Position of SphereCast origin stating in base of capsule
            Vector3 pos = transform.position + Vector3.up * ((colliderHeight * 0.5f) - colliderRadius);
            // ray for SphereCast
            Ray ray2 = new Ray(pos, Vector3.up);

            // sphere cast around the base of capsule for check ground distance
            if (Physics.SphereCast(ray2, radius, out groundHit, headDetect - (colliderRadius * 0.1f), autoCrouchLayer))
                return false;
            else
                return true;
        }

        protected virtual void AutoCrouchExit(Collider other)
        {
            if (other.CompareTag("AutoCrouch"))
            {
                inCrouchArea = false;
            }
        }

        protected virtual void CheckForAutoCrouch(Collider other)
        {
            if (other.gameObject.CompareTag("AutoCrouch"))
            {
                autoCrouch = true;
                inCrouchArea = true;
            }
        }

        #endregion

        #endregion
    }
}