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

        #region Lockstep

        [Lockstep (true)]
        public bool IsFiring { get; private set; }

        #endregion

        #region Cache

        public FPSTurn Turner { get; private set; }

        public ushort StartShootCode { get; private set; }

        public ushort EndShootCode { get; private set; }

        #endregion

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
                    this.IsFiring = true;
                else if (com.InputCode == EndShootCode)
                    this.IsFiring = false;
            }
        }

        private Command _GenerateFireCommand(bool startShoot)
        {
            Command com = new Command(startShoot ? StartShootCode : EndShootCode, this.Agent.Controller.ControllerID);
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
            foreach (var body in Turner.GetBodiesInLine(FixedMath.Create(1000)))
            {
                if (body.ID != this.Agent.Body.ID)
                body.TestFlash();
            }
        }

        protected virtual void OnFire()
        {

        }

    }
}