using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lockstep
{
    public class FPSTurn : ActiveAbility
    {
        [SerializeField]
        private GameObject _cameraController;

        public GameObject CameraController { get { return _cameraController; } }

        [SerializeField]
        private Transform _head;

        public Transform VisualHead { get { return _head; } }

        [SerializeField]
        private Transform _body;

        public Transform VisualBody { get { return _body; } }

        [SerializeField,FixedNumber]
        private long _baseCameraHeight = FixedMath.One * 2;

        private long _cameraHeight;

        [Lockstep(true)]
        public long CameraHeight
        {
            get { return _cameraHeight; }
            set
            {
                if (_cameraHeight != value)
                {
                    _cameraHeight = value;

                    Vector3 camPos = CameraController.transform.localPosition;
                    camPos.y = value.ToFloat();
                    CameraController.transform.localPosition = camPos;
                }
            }
        }


        public Vector2dHeight Forward { get; private set; }

        public Vector2d ForwardDirection { get; private set; }

        public long Slope { get; private set; }


        public Quaternion TargetVisualRotation { get; private set; }

        public Quaternion CurRotation { get; private set; }


        private bool _isControlling;

        public bool IsControlling
        {
            get
            {
                return _isControlling;
            }
            set
            {
                if (_isControlling != value)
                {
                    if (value == false)
                    {
                        EndControl();
                    } else
                    {
                        StartControl();
                    }
                    _isControlling = value;
                }
            }
        }

        protected override void OnSetup()
        {
            CameraHeight = _baseCameraHeight;
        }

        protected override void OnInitialize()
        {
            CurRotation = Agent.Body.RotationalTransform.rotation;
            IsControlling = false;
            this.EndControl();
            _cameraController.SetActive(false);
            Agent.Body.CanSetVisualRotation = false;
        }

        protected override void OnVisualize()
        {


            if (this.IsControlling)
            {
                CurRotation = CameraController.transform.rotation;
            } else
            {
                float lerpAmount = 10f;
                CurRotation = Quaternion.Slerp(CurRotation, this.TargetVisualRotation, lerpAmount * Time.deltaTime);
            }

            Vector3 curRotationEulers = CurRotation.eulerAngles;
            VisualBody.transform.eulerAngles = new Vector3(0, curRotationEulers.y, 0);
            VisualHead.transform.localRotation = Quaternion.Euler(curRotationEulers.x, curRotationEulers.y, 0);
        }

        protected override void OnExecute(Command com)
        {
            Vector2dHeight forward = com.GetData<Vector2dHeight>();
            Forward = forward;
            this.CalculateRotationValues();
        }

        private void CalculateRotationValues()
        {
            Vector2d newRot = Forward.ToVector2d().ToRotation();
            long newRotMag;
            newRot.Normalize(out newRotMag);
            this.ForwardDirection = newRot.ToDirection();
            Slope = Forward.Height.Div(newRotMag);
            this.Agent.Body.Rotation = newRot;
            this.TargetVisualRotation = Quaternion.LookRotation(Forward.ToVector3());
        }

        private void StartControl()
        {
            _cameraController.SetActive(true);
        }

        private void EndControl()
        {
            _cameraController.SetActive(false);
        }


        protected override void OnSimulate()
        {
            base.OnSimulate();
        }

        public Command GenerateTurnCommand(Vector3 forwards)
        {
            Command com = new Command(this.Interfacer.ListenInputID, this.Agent.Controller.ControllerID);
            Vector2dHeight vecHeight = new Vector2dHeight(forwards);
            com.Add<Vector2dHeight>(vecHeight);
            return com;

        }

        public IEnumerable<LSBody> GetBodiesInLine(long range)
        {
            Vector2d start = this.Agent.Body.Position;
            Vector2d end = start + this.ForwardDirection * range;
            foreach (LSBody body in Lockstep.Raycaster.RaycastAll (start,end,Agent.Body.HeightPos + CameraHeight,Slope))
            {
                if (body.ID == Agent.Body.ID)
                {
                    continue;
                }
                yield return body;
            }
        }

        void Reset()
        {
            this._body = this.transform.FindChild("Body").transform;
            this._head = this.transform.FindChild("Head").transform;
        }
    }
}