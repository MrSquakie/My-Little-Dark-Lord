namespace GameCreator.Characters
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.AI;
    using UnityEngine.SceneManagement;
    using GameCreator.Core;
    using GameCreator.Core.Hooks;

    [AddComponentMenu("Game Creator/Characters/Player Character", 100)]
    public class PlayerCharacter : Character
    {
        public enum INPUT_TYPE
        {
            PointAndClick,
            Directional,
            FollowPointer,
            SideScrollX,
            SideScrollZ,
            TankControl
        }

        public enum MOUSE_BUTTON
        {
            LeftClick = 0,
            RightClick = 1,
            MiddleClick = 2
        }

        protected const string AXIS_H = "Horizontal";
        protected const string AXIS_V = "Vertical";

        protected static readonly Vector3 PLANE = new Vector3(1, 0, 1);

        protected const string PLAYER_ID = "player";
        public static OnLoadSceneData ON_LOAD_SCENE_DATA = null;

        // PROPERTIES: ----------------------------------------------------------------------------

        public INPUT_TYPE inputType = INPUT_TYPE.Directional;
        public MOUSE_BUTTON mouseButtonMove = MOUSE_BUTTON.LeftClick;
        public LayerMask mouseLayerMask = ~0;
        public bool invertAxis = false;

        public KeyCode jumpKey = KeyCode.Space;

        protected bool uiConstrained = false;
        protected Camera cacheCamera;

        // INITIALIZERS: --------------------------------------------------------------------------

        protected override void Awake()
        {
            if (!Application.isPlaying) return;
            this.CharacterAwake();

            this.initSaveData = new SaveData()
            {
                position = transform.position,
                rotation = transform.rotation,
            };

            if (this.save)
            {
                SaveLoadManager.Instance.Initialize(
                    this, (int)SaveLoadManager.Priority.Normal, true
                );
            }

            HookPlayer hookPlayer = gameObject.GetComponent<HookPlayer>();
            if (hookPlayer == null) gameObject.AddComponent<HookPlayer>();

            if (ON_LOAD_SCENE_DATA != null && ON_LOAD_SCENE_DATA.active)
            {
                transform.position = ON_LOAD_SCENE_DATA.position;
                transform.rotation = ON_LOAD_SCENE_DATA.rotation;
                ON_LOAD_SCENE_DATA.Consume();
            }
        }

        // UPDATE: --------------------------------------------------------------------------------

        protected virtual void Update()
        {
            if (!Application.isPlaying) return;

            switch (this.inputType)
            {
                case INPUT_TYPE.Directional: this.UpdateInputDirectional(); break;
                case INPUT_TYPE.PointAndClick: this.UpdateInputPointClick(); break;
                case INPUT_TYPE.FollowPointer: this.UpdateInputFollowPointer(); break;
                case INPUT_TYPE.SideScrollX: this.UpdateInputSideScroll(Vector3.right); break;
                case INPUT_TYPE.SideScrollZ: this.UpdateInputSideScroll(Vector3.forward); break;
                case INPUT_TYPE.TankControl: this.UpdateInputTank(); break;
            }

            if (this.IsControllable())
            {
                if (Input.GetKeyDown(this.jumpKey)) this.Jump();
            }

            this.CharacterUpdate();
        }

        protected virtual void UpdateInputDirectional()
        {
            Vector3 direction = Vector3.zero;
            if (!this.IsControllable()) return;

            if (Application.isMobilePlatform || TouchStickManager.FORCE_USAGE)
            {
                Vector2 touchDirection = TouchStickManager.Instance.GetDirection(this);
                direction = new Vector3(touchDirection.x, 0.0f, touchDirection.y);
            }
            else
            {
                direction = new Vector3(
                    Input.GetAxis(AXIS_H),
                    0.0f,
                    Input.GetAxis(AXIS_V)
                );
            }

            Camera maincam = this.GetMainCamera();
            if (maincam == null) return;

            Vector3 moveDirection = maincam.transform.TransformDirection(direction);
            moveDirection.Scale(PLANE);
            moveDirection.Normalize();
            this.characterLocomotion.SetDirectionalDirection(moveDirection);
        }

        protected virtual void UpdateInputTank()
        {
            Vector3 movement = Vector3.zero;
            float rotationY = 0f;

            if (!this.IsControllable()) return;

            if (Application.isMobilePlatform || TouchStickManager.FORCE_USAGE)
            {
                Vector2 touchDirection = TouchStickManager.Instance.GetDirection(this);
                movement = new Vector3(0f, 0.0f, touchDirection.y);
                rotationY = touchDirection.x;
            }
            else
            {
                movement = transform.TransformDirection(new Vector3(
                    0f,
                    0f, 
                    Input.GetAxis(AXIS_V)
                ));
                rotationY = Input.GetAxis(AXIS_H);
            }

            this.characterLocomotion.SetTankDirection(movement, rotationY);
        }

        protected virtual void UpdateInputPointClick()
        {
            if (!this.IsControllable()) return;
            this.UpdateUIConstraints();

            if (Input.GetMouseButtonDown((int)this.mouseButtonMove) && !this.uiConstrained)
            {
                Camera maincam = this.GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);
                this.characterLocomotion.SetTarget(cameraRay, this.mouseLayerMask, null, 0f, null);
            }
        }

        protected virtual void UpdateInputFollowPointer()
        {
            if (!this.IsControllable()) return;
            this.UpdateUIConstraints();

            if (Input.GetMouseButton((int)this.mouseButtonMove) && !this.uiConstrained)
            {
                if (HookPlayer.Instance == null) return;

                Camera maincam = this.GetMainCamera();
                if (maincam == null) return;

                Ray cameraRay = maincam.ScreenPointToRay(Input.mousePosition);

                Transform player = HookPlayer.Instance.transform;
                Plane groundPlane = new Plane(Vector3.up, player.position);

                float rayDistance = 0f;
                if (groundPlane.Raycast(cameraRay, out rayDistance))
                {
                    Vector3 cursor = cameraRay.GetPoint(rayDistance);
                    if (Vector3.Distance(player.position, cursor) >= 0.05f)
                    {
                        Vector3 target = Vector3.MoveTowards(player.position, cursor, 1f);
                        this.characterLocomotion.SetTarget(target, null, 0f, null);
                    }
                }
            }
        }

        protected virtual void UpdateInputSideScroll(Vector3 axis)
        {
            Vector3 direction = Vector3.zero;
            if (!this.IsControllable()) return;

            if (Application.isMobilePlatform || TouchStickManager.FORCE_USAGE)
            {
                Vector2 touchDirection = TouchStickManager.Instance.GetDirection(this);
                direction = axis * touchDirection.x;
            }
            else
            {
                direction = axis * Input.GetAxis(AXIS_H);
            }

            Camera maincam = this.GetMainCamera();
            if (maincam == null) return;

            float invertValue = (this.invertAxis ? -1 : 1);
            direction.Scale(axis * invertValue);
            direction.Normalize();
            this.characterLocomotion.SetDirectionalDirection(direction);
        }

        protected Camera GetMainCamera()
        {
            if (HookCamera.Instance != null) return HookCamera.Instance.Get<Camera>();
            if (this.cacheCamera != null) return this.cacheCamera;

            this.cacheCamera = Camera.main;
            if (this.cacheCamera != null)
            {
                return this.cacheCamera;
            }

            this.cacheCamera = GameObject.FindObjectOfType<Camera>();
            if (this.cacheCamera != null)
            {
                return this.cacheCamera;
            }

            Debug.LogError(ERR_NOCAM, gameObject);
            return null;
        }

        protected void UpdateUIConstraints()
        {
            EventSystemManager.Instance.Wakeup();
            this.uiConstrained = EventSystemManager.Instance.IsPointerOverUI();

            #if UNITY_IOS || UNITY_ANDROID
            for (int i = 0; i < Input.touches.Length; ++i)
            {
                if (Input.GetTouch(i).phase != TouchPhase.Began) continue;

                int fingerID = Input.GetTouch(i).fingerId;
                bool pointerOverUI = EventSystemManager.Instance.IsPointerOverUI(fingerID);
                if (pointerOverUI) this.uiConstrained = true;
            }
            #endif
        }

        // GAME SAVE: -----------------------------------------------------------------------------

        protected override string GetUniqueCharacterID()
        {
            return PLAYER_ID;
        }
    }
}