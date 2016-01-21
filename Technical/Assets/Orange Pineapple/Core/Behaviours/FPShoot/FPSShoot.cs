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

        protected override void OnRawExecute(Command com)
        {
            if (com.InputCode == StartShootCode || com.InputCode == EndShootCode)
            {
                
                if (com.InputCode == StartShootCode)
                {
                    this.CheckFire();
                    this.IsFiring = true;
                    int comFrame = (int)com.GetData<DefaultData>().Value;
                    PassedFrames = LockstepManager.FrameCount - comFrame;

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
            
        public bool GenerateFireCommand(bool startShoot, out Command com)
        {
            if (startShoot)
            {
                com = _GenerateFireCommand(true);
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
            LSProjectile projectile = ProjectileManager.Create (this.ProjCode,Agent,new Vector3d(0,0,turn.CameraHeight + this.Agent.Body.HeightPos),AllegianceType.All,(agent)=>true,(agent)=>agent.Body.TestFlash());
            projectile.InitializeFree(turn.Forward,(body) => body.ID != bodyID);
            ProjectileManager.Fire(projectile);

            if (this.PassedFrames > 0) {
                long compensation = FixedMath.Create((long)PassedFrames,32);
                PassedFrames = 0;

                Vector3d delta = this.Turner.Forward;
                delta.Mul(compensation);
                projectile.RaycastMove(delta);
            }
        }

        protected virtual void OnFire()
        {

        }

    }
}