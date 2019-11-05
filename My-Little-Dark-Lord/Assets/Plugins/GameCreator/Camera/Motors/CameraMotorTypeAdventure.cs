﻿namespace GameCreator.Camera
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;
    using GameCreator.Characters;

    [AddComponentMenu("")]
    [System.Serializable]
    public class CameraMotorTypeAdventure : ICameraMotorType
    {
        public enum OrbitInput
        {
            MouseMove,
            HoldLeftMouse,
            HoldRightMouse,
            HoldMiddleMouse
        }

        // PROPERTIES: ----------------------------------------------------------------------------

        private const string INPUT_MOUSE_X = "Mouse X";
        private const string INPUT_MOUSE_Y = "Mouse Y";
        private const string INPUT_MOUSE_W = "Mouse ScrollWheel";

        public static new string NAME = "Adventure Camera";
        public static Rect MOBILE_RECT = new Rect(0.5f, 0.0f, 0.5f, 1.0f);

        // PROPERTIES: ----------------------------------------------------------------------------

        private GameObject pivot;
        private bool motorEnabled = false;
        private float targetRotationX = 0.0f;
        private float targetRotationY = 0.0f;
        private Vector3 _velocityPosition = Vector3.zero;

        public TargetGameObject target = new TargetGameObject(TargetGameObject.Target.Player);
        public Vector3 targetOffset = Vector3.up;
        public Vector3 pivotOffset = Vector3.zero;

        public bool allowOrbitInput = true;
        public OrbitInput orbitInput = OrbitInput.MouseMove;
        public float orbitSpeed = 25.0f;
        private float orbitInputTime = -1000f;

        [Range(0.0f, 180f)] public float maxPitch = 120f;
        public Vector2 sensitivity = new Vector2(10f, 10f);

        public bool allowZoom = true;
        public float zoomSpeed = 25.0f;
        public float initialZoom = 3.0f;
        [Range(1f, 20f)]
        public float zoomSensitivity = 5.0f;
        public Vector2 zoomLimits = new Vector2(1f, 10f);

        private float wallConstrainZoom = 0.0f;
        private float desiredZoom = 0.0f;
        private float currentZoom = 0.0f;
        private float targetZoom = 0.0f;

        public bool avoidWallClip = true;
        public float wallClipRadius = 0.4f;
        public LayerMask wallClipLayerMask = ~4;

        public bool autoRepositionBehind = true;
        public float autoRepositionTimeout = 1.5f;
        public float autoRepositionSpeed = 2.5f;
        private float recoverSpeedX = 0f;
        private float recoverSpeedY = 0f;

        private RaycastHit[] hitsBuffer = new RaycastHit[20];

        // INITIALIZERS: --------------------------------------------------------------------------

        private void Awake()
        {
            this.pivot = new GameObject(gameObject.name + " Pivot");
            this.pivot.transform.SetParent(transform);

            Vector3 pivotPosition = (Vector3.forward * this.initialZoom) + this.pivotOffset;
            this.pivot.transform.localPosition = pivotPosition;
        }

        public override void EnableMotor()
        {
            GameObject targetGo = this.target.GetGameObject(gameObject);
            if (targetGo != null)
            {
                Transform targetTransform = targetGo.transform;

                this.targetRotationX = targetTransform.rotation.eulerAngles.y + 180f;
                this.targetRotationY = targetTransform.rotation.eulerAngles.x;

                transform.position = targetTransform.position + this.targetOffset;
                transform.rotation = Quaternion.Euler(
                    this.targetRotationY,
                    this.targetRotationX,
                    0f
                );
            }

            this.desiredZoom = this.initialZoom;
            this.currentZoom = this.initialZoom;
            this.wallConstrainZoom = this.zoomLimits.y;

            this.motorEnabled = true;
        }

        public override void DisableMotor()
        {
            this.motorEnabled = false;
        }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void UpdateMotor()
        {
            float rotationX = 0.0f;
            float rotationY = 0.0f;

            if (this.allowOrbitInput) this.RotationInput(ref rotationX, ref rotationY);

            this.targetRotationX += rotationX;
            this.targetRotationY += rotationY;

            if (this.autoRepositionBehind &&
                this.orbitInputTime + this.autoRepositionTimeout < Time.time)
            {
                this.RotationRecover();
            }

            this.targetRotationX %= 360f;
            this.targetRotationY %= 360f;

            this.targetRotationY = Mathf.Clamp(
                this.targetRotationY,
                -this.maxPitch / 2.0f,
                this.maxPitch / 2.0f
            );

            float smoothTime = (HookCamera.Instance != null
                ? HookCamera.Instance.Get<CameraController>().cameraSmoothTime.positionDuration
                : 0.1f
            );

            
            Transform targetTransform = this.target.GetTransform(gameObject);

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetTransform.TransformPoint(this.targetOffset),
                ref this._velocityPosition,
                smoothTime
            );

            Quaternion targetRotation = Quaternion.Euler(
                this.targetRotationY,
                this.targetRotationX,
                0f
            );

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * this.orbitSpeed
            );

            if (this.allowZoom)
            {
                this.desiredZoom = Mathf.Clamp(
                    this.desiredZoom - Input.GetAxis(INPUT_MOUSE_W) * this.zoomSensitivity,
                    this.zoomLimits.x, this.zoomLimits.y
                );
            }

            this.currentZoom = Mathf.Max(this.zoomLimits.x, this.desiredZoom);
            this.currentZoom = Mathf.Min(this.currentZoom, this.wallConstrainZoom);
            this.targetZoom = Mathf.Lerp(this.targetZoom, this.currentZoom, Time.deltaTime * this.zoomSpeed);

            Vector3 pivotPosition = (Vector3.forward * this.targetZoom) + this.pivotOffset;
            this.pivot.transform.localPosition = pivotPosition;
        }

        public override Vector3 GetPosition(CameraController camera)
        {
            return this.pivot.transform.position;
        }

        public override Vector3 GetDirection(CameraController camera)
        {
            return transform.TransformDirection(-Vector3.forward);
        }

        public override bool UseSmoothPosition()
        {
            return false;
        }

        public override bool UseSmoothRotation()
        {
            return false;
        }

        public void AddRotation(float yaw, float pitch)
        {
            this.targetRotationY += yaw;
            this.targetRotationX += pitch;
        }

        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void FixedUpdate()
        {
            this.wallConstrainZoom = this.zoomLimits.y;
            if (!this.motorEnabled || !this.avoidWallClip) return;

            if (this.avoidWallClip && HookCamera.Instance != null)
            {
                Vector3 direction = (this.pivot.transform.position - transform.position).normalized;
                QueryTriggerInteraction queryTrigger = QueryTriggerInteraction.Ignore;

                int hitCount = Physics.SphereCastNonAlloc(
                    transform.position + (direction * this.wallClipRadius), 
                    this.wallClipRadius, direction, this.hitsBuffer, this.zoomLimits.y,
                    this.wallClipLayerMask, queryTrigger
                );

                float minDistance = this.zoomLimits.y;
                for (int i = 0; i < hitCount; ++i)
                {
                    float hitDistance = this.hitsBuffer[i].distance + this.wallClipRadius;
                    Transform targetTransform = this.target.GetTransform(gameObject);
                    Transform bufferTransform = this.hitsBuffer[i].collider.transform;

                    bool childOfTarget = (
                        bufferTransform.GetInstanceID() == targetTransform.GetInstanceID() ||
                        bufferTransform.IsChildOf(targetTransform)
                    );

                    if (hitDistance <= minDistance && !childOfTarget)
                    {
                        minDistance = hitDistance;
                        this.wallConstrainZoom = Mathf.Clamp(
                            minDistance, 
                            this.zoomLimits.x, 
                            this.zoomLimits.y
                        );
                    }
                }
            }
        }

        private void RotationInput(ref float rotationX, ref float rotationY)
        {
            if (Application.isMobilePlatform)
            {
                Rect screenRect = new Rect(
                    Screen.width * MOBILE_RECT.x,
                    Screen.height * MOBILE_RECT.y,
                    Screen.width * MOBILE_RECT.width,
                    Screen.height * MOBILE_RECT.height
                );

                for (int i = 0; i < Input.touchCount; ++i)
                {
                    Touch touch = Input.touches[i];
                    if (touch.phase == TouchPhase.Moved && screenRect.Contains(touch.position))
                    {
                        rotationX = (touch.deltaPosition.x / Screen.width) * this.sensitivity.x * 10f * Time.timeScale;
                        rotationY = (touch.deltaPosition.y / Screen.height) * this.sensitivity.y * 10f * Time.timeScale;
                        orbitInputTime = Time.time;
                        this.recoverSpeedX = 0f;
                        this.recoverSpeedY = 0f;
                        break;
                    }
                }
            }
            else
            {
                bool inputConditions;
                if (this.orbitInput == OrbitInput.MouseMove) inputConditions = true;
                else if (this.orbitInput == OrbitInput.HoldLeftMouse && Input.GetMouseButton(0)) inputConditions = true;
                else if (this.orbitInput == OrbitInput.HoldRightMouse && Input.GetMouseButton(1)) inputConditions = true;
                else if (this.orbitInput == OrbitInput.HoldMiddleMouse && Input.GetMouseButton(2)) inputConditions = true;
                else inputConditions = false;

                if (inputConditions)
                {
                    float axisX = Input.GetAxis(INPUT_MOUSE_X);
                    float axisY = Input.GetAxis(INPUT_MOUSE_Y);

                    rotationX = axisX * this.sensitivity.x * Time.timeScale;
                    rotationY = axisY * this.sensitivity.y * Time.timeScale;

                    if (!Mathf.Approximately(axisX, 0f) || !Mathf.Approximately(axisY, 0f))
                    {
                        orbitInputTime = Time.time;
                        this.recoverSpeedX = 0f;
                        this.recoverSpeedY = 0f;
                    }
                }
            }
        }

        private void RotationRecover()
        {
            Transform targetTransform = this.target.GetTransform(gameObject);
            if (targetTransform == null) return;

            float targetRecoverX = targetTransform.eulerAngles.y + 180f;
            float targetRecoverY = targetTransform.eulerAngles.x;

            this.targetRotationX = Mathf.SmoothDampAngle(
                this.targetRotationX, targetRecoverX,
                ref this.recoverSpeedX, this.autoRepositionSpeed
            );

            this.targetRotationY = Mathf.SmoothDampAngle(
                this.targetRotationY, targetRecoverY,
                ref this.recoverSpeedY, this.autoRepositionSpeed
            );
        }
    }
}