using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Lockstep
{
    public class FPSShoot : ActiveAbility
    {

        [SerializeField,FrameCount(true)]
        private int _firePeriod;

        public int FirePeriod { get { return _firePeriod; } }

        [SerializeField]
        private ShootType _shootType;

        public ShootType ShootType { get { return _shootType; } }

        [SerializeField,DataCode("Projectiles")]
        private string _projCode;
        public string ProjCode {get {return _projCode;}}

        #region Lockstep

        [Lockstep (true)]
        public bool IsFiring { get; private set; }

        #endregion

        #region Cache

        public FPSTurn Turner { get; private set; }

        public ushort StartShootCode { get; private set; }

        public ushort EndShootCode { get; private set; }

        #endregion

        int PassedFrames {get; set;}

        protected override void OnSetup()
        {
            FireCount = FirePeriod;
            Turner = Agent.GetAbility<FPSTurn>();
            base.DoRawExecute = true;
            StartShootCode = InputCodeManager.GetCodeID("StartShoot");
            EndShootCode = InputCodeManager.GetCodeID("EndShoot");
            IsFiring = false;
        }

        protected override void OnInitialize()
        {
            this.CurrentShotID = 0;
        }

        protected override void OnRawExecute(Command com)
        {
            if (com.InputCode == StartShootCode || com.InputCode == EndShootCode)
            {
                
                if (com.InputCode == StartShootCode)
                {

                    int comFrame = (int)com.GetData<DefaultData>().Value;
                    PassedFrames = LockstepManager.FrameCount - comFrame;
                    uint currentShot = (uint)com.GetData<DefaultData>(1).Value;
                    if (this.LatencyProjectiles.ContainsKey(currentShot)) {
                        ProjectileManager.EndProjectile(this.LatencyProjectiles[currentShot]);
                    }

                    this.IsFiring = true;
                    this.CheckFire();

                }
                else if (com.InputCode == EndShootCode)
                    this.IsFiring = false;
            }
        }

        private Command _GenerateFireCommand(bool startShoot)
        {
            Command com = new Command(startShoot ? StartShootCode : EndShootCode, this.Agent.Controller.ControllerID);
            if (startShoot) {
                com.Add<DefaultData> (new DefaultData(DataType.Int,LockstepManager.FrameCount));
            }
            return com;
        }
        Vector3d ProjectileStartPosition {
            get {
                return new Vector3d(0,0,this.Turner.CameraHeight);
            }
        }
        uint CurrentShotID {get; set;}
        Dictionary<uint,LSProjectile> LatencyProjectiles = new Dictionary<uint, LSProjectile>();
        public bool GenerateFireCommand(bool startShoot, out Command com)
        {
            if (startShoot)
            {
                com = _GenerateFireCommand(true);
                com.Add<DefaultData> (new DefaultData(DataType.UInt,CurrentShotID));
                LSProjectile projectile = ProjectileManager.NDCreateAndFire (
                    this.ProjCode,
                    ProjectileStartPosition,
                    Turner.GenerateForwards(Turner.CameraController.transform.rotation));

                LatencyProjectiles.Add(CurrentShotID,projectile);
                CurrentShotID++;
                return true;
            } else
            {
                if (ShootType == ShootType.Chain)
                {
                    com = _GenerateFireCommand(false);
                    return true;
                }
            }
            com = null;

            return false;
        }

        [Lockstep]
        private int FireCount { get; set; }

        protected override void OnSimulate()
        {
            FireCount--;
            this.CheckFire();

        }
        private void CheckFire () {
            if (IsFiring)
            {
                if (FireCount <= 0)
                {

                    this.Fire();
                }
            }
        }


        private void Fire()
        {
            if (this.ShootType == ShootType.Single)
                this.IsFiring = false;
            this.FireCount = FirePeriod;

            FPSTurn turn = Agent.GetAbility<FPSTurn>();
            int bodyID = Agent.Body.ID;
            LSProjectile projectile = ProjectileManager.Create (this.ProjCode,Agent,ProjectileStartPosition,AllegianceType.All,(agent)=>true,(agent)=>agent.Body.TestFlash());
            projectile.InitializeFree(turn.Forward,(body) => body.ID != bodyID);
            ProjectileManager.Fire(projectile);

            if (this.PassedFrames > 0) {
                Vector3d delta = projectile.Velocity;
                delta.Mul((int)PassedFrames);
                projectile.RaycastMove(delta);

                PassedFrames = 0;

            }
        }

        protected virtual void OnFire()
        {

        }

    }
}