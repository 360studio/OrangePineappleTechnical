using UnityEngine;
using System.Collections;
using Lockstep;

namespace OrangePineapple
{
    public class Shoot : Ability
    {
        [SerializeField,FixedNumber]
        private long _firePeriod;
        protected override void OnSimulate()
        {
            base.OnSimulate();
        }
        protected override void OnVisualize()
        {
            if (Input.GetMouseButtonDown(0)) {
            }
        }
    }
}