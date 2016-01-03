using UnityEngine;
using System.Collections;
using Lockstep;
using Lockstep.NetworkHelpers;
namespace OrangePineapple
{
    [RequireComponent (typeof (FPSHelper))]
    public class OrangePineappleGameManager : GameManager
    {
      
        private InterfacingHelper _interfacingHelper = new FPSInterfacingHelper();
        public override InterfacingHelper MainInterfacingHelper
        {
            get
            {
                return _interfacingHelper;
            }
        }
    }
}