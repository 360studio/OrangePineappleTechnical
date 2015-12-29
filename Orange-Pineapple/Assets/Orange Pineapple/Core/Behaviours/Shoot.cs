using UnityEngine;
using System.Collections;
using Lockstep;

namespace OrangePineapple
{
    public class Shoot : ActiveAbility
    {
        [SerializeField]
        private Camera _trackedCamera;
        public Camera TrackedCamera {get; private set;}

        [Lockstep (true)]
        public Vector2d Rotation {get; private set;}
        [Lockstep(true)]
        public long HeightSlope {get; private set;}

        LineRenderer lineRenderer;

        FastList<LSBody> shotBodies = new FastList<LSBody>();

        protected override void OnSetup()
        {
            lineRenderer = this.GetComponent<LineRenderer> ();
            Rotation = Vector2d.one;
            _trackedCamera = this.GetComponentInChildren<Camera> ();
        }

        protected override void OnSimulate()
        {
            RecalculateShot ();
        }

        protected override void OnExecute(Command com)
        {
            this.Rotation = com.Rotation;
            this.HeightSlope = com.Value;
        }

        void RecalculateShot () {
            
            shotBodies.FastClear();
            foreach (LSBody body in Raycaster.RaycastAll(
                this.Agent.Body._position + this.Rotation,this.Rotation * 500, this.Agent.Body.HeightPos, HeightSlope)) {
                if (body.ID == Agent.Body.ID) continue;
                shotBodies.Add(body);
            }

            Vector3 pos = _trackedCamera.transform.position + Vector3.down * .05f;
            lineRenderer.SetPosition(0,pos);
            pos += this.Rotation.ToVector3(HeightSlope.ToFloat()) * 200;
            lineRenderer.SetPosition (1,pos);
        }

        void OnGUI () {
            GUI.color = Color.black;
            for (int i= 0 ; i < shotBodies.Count; i++) {
                Vector3 screenPos = _trackedCamera.WorldToScreenPoint(shotBodies[i].transform.position);
                screenPos.y = Screen.height - screenPos.y;
                Rect rect = new Rect(screenPos.x + 0,screenPos.y - 25,100,100);
                GUI.Label (rect, "Shot");
            }
        }

        protected override void OnVisualize()
        {
            if (PlayerManager.ContainsController(this.Agent.Controller))
            {
                if (true)//Input.GetMouseButtonDown(0))
                {
                    Command com = new Command(this.Interfacer.ListenInput, Agent.Controller.ControllerID);
                    FastList<LSAgent> selection = new FastList<LSAgent>();
                    selection.Add(this.Agent);
                    com.Select = new Selection(selection);
                    float yAngle = (_trackedCamera.transform.eulerAngles.y) * Mathf.Deg2Rad;
                    com.Rotation = Vector2d.CreateFromAngle(yAngle);
                    float heightAngle = -(_trackedCamera.transform.eulerAngles.x) * Mathf.Deg2Rad;
                    com.Value = FixedMath.Create(Mathf.Sin(heightAngle));
                    CommandManager.SendCommand(com);
                }
            }

        }
    }
}