using UnityEngine;
using UnityEngine.InputSystem;
using static DefaultControls;

namespace Invector.vCharacterController
{
    public class vThirdPersonInput : MonoBehaviour, IDefaultMappingActions
    {
        [HideInInspector] public vThirdPersonController cc;
        [HideInInspector] public vThirdPersonCamera tpCamera;
        [HideInInspector] public Camera cameraMain;

        private PlayerInput playerInput;
        private DefaultControls defaultControls;
        private Vector2 cameraValue;

        public void OnEnable()
        {
            if (defaultControls == null)
            {
                defaultControls = new DefaultControls();
                defaultControls.@DefaultMapping.SetCallbacks(this);
            }

            defaultControls.Enable();
        }

        public void OnDisable()
        {
            defaultControls.Disable();
        }

        protected virtual void Start()
        {
            InitilizeController();
            InitializeTpCamera();
        }

        protected virtual void FixedUpdate()
        {
            cc.UpdateMotor();               // updates the ThirdPersonMotor methods
            cc.ControlLocomotionType();     // handle the controller locomotion type and movespeed
            cc.ControlRotationType();       // handle the controller rotation type
        }

        protected virtual void Update()
        {
            if (GameManager.Instance.IsDead)
                return;

            InputHandle();
            CameraInput();
            // update the input methods
            cc.UpdateAnimator();             // updates the Animator Parameters
        }

        public virtual void OnAnimatorMove()
        {
            cc.ControlAnimatorRootMotion(); // handle root motion animations 
        }

        #region Basic Locomotion Inputs

        protected virtual void InitilizeController()
        {
            cc = GetComponent<vThirdPersonController>();

            if (cc != null)
                cc.Init();
        }

        protected virtual void InitializeTpCamera()
        {
            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vThirdPersonCamera>();
                if (tpCamera == null)
                    return;
                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }
        }

        protected virtual void InputHandle()
        {
            CameraInput();
            /*SprintInput();
            StrafeInput();
            JumpInput();*/
        }

        protected virtual void CameraInput()
        {
            if (!cameraMain)
            {
                if (!Camera.main) Debug.Log("Missing a Camera with the tag MainCamera, please add one.");
                else
                {
                    cameraMain = Camera.main;
                    cc.rotateTarget = cameraMain.transform;
                }
            }

            if (cameraMain)
            {
                cc.UpdateMoveDirection(cameraMain.transform);
            }

            if (tpCamera == null)
                return;

            tpCamera.RotateCamera(cameraValue.x, cameraValue.y);

            /*var Y = Input.GetAxis(rotateCameraYInput);
            var X = Input.GetAxis(rotateCameraXInput);*/

            //tpCamera.RotateCamera(X, Y);
        }

        protected virtual void StrafeInput()
        {
            //if (Input.GetKeyDown(strafeInput))
                cc.Strafe();
        }

        protected virtual void SprintInput()
        {
            //if (Input.GetKeyDown(sprintInput))
                cc.Sprint(true);
            //else if (Input.GetKeyUp(sprintInput))
                cc.Sprint(false);
        }

        /// <summary>
        /// Conditions to trigger the Jump animation & behavior
        /// </summary>
        /// <returns></returns>
        protected virtual bool JumpConditions()
        {
            return cc.isGrounded && cc.GroundAngle() < cc.slopeLimit && !cc.isJumping && !cc.stopMove;
        }

        /// <summary>
        /// Input to trigger the Jump 
        /// </summary>
        protected virtual void JumpInput()
        {
            //if (Input.GetKeyDown(jumpInput) && JumpConditions())
                cc.Jump();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            cc.input = new Vector3(value.x, cc.input.y, value.y);
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (!JumpConditions())
                return;

            cc.Jump();
        }

        public void OnCamera(InputAction.CallbackContext context)
        {
            cameraValue = context.ReadValue<Vector2>();            
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            cc.Sprint(context.ReadValue<float>() > 0);
        }

        #endregion
    }
}