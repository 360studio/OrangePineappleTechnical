using UnityEngine;
using System.Collections;

namespace Lockstep
{
    public class FPSTurn : ActiveAbility
    {
        [SerializeField]
        private GameObject _cameraController;

        public GameObject CameraController { get { return _cameraController; } }

        [SerializeField]
        private Transform _head;
        public Transform Head {get {return _head;}}
        public Vector3 HeadPosition {get; private set;}

        public Vector2dHeight Forward {get; private set;}
        public long Slope {get; private set;}

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
                        EndControl ();
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
            HeadPosition = Head.localPosition;
        }

        protected override void OnInitialize()
        {
            IsControlling = false;
            _cameraController.SetActive(false);
        }

        protected override void OnVisualize()
        {
            if (this.IsControlling) {
            
            }
            else {

            }
        }
        protected override void OnExecute(Command com)
        {
            Vector2dHeight forward = com.GetData<Vector2dHeight> ();
            Forward = forward;
            this.CalculateRotationValues();
        }
        private void CalculateRotationValues () 
        {
            Vector2d newRot = Forward.ToVector2d();
            long newRotMag;
            newRot.Normalize(out newRotMag);
            Slope = Forward.Height.Div(newRotMag);
            Agent.Body.Rotation = newRot;
        }
        private void StartControl ()
        {
            _cameraController.SetActive(true);
            Head.SetParent(Agent.Body.PositionalTransform, true);

        }
        private void EndControl ()
        {
            _cameraController.SetActive(false);
            Head.SetParent(Agent.Body.RotationalTransform, true);
            Head.localPosition = HeadPosition;
            Head.localRotation = Quaternion.identity;
        }

        protected override void OnSimulate()
        {
            base.OnSimulate();
        }

        public Command GenerateTurnCommand (Vector3 forwards) {
            Command com = new Command(this.Interfacer.ListenInputID,this.Agent.Controller.ControllerID);
            Vector2dHeight vecHeight = new Vector2dHeight(forwards);
            com.Add<Vector2dHeight> (vecHeight);
            return com;

        }
    }
}