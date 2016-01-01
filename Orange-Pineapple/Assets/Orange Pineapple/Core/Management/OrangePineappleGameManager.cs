using UnityEngine;
using System.Collections;
using Lockstep;

namespace OrangePineapple
{
    public class OrangePineappleGameManager : GameManager
    {
        private NetworkHelper _networkHelper = new Lockstep.Example.ExampleNetworkHelper ();
        public override NetworkHelper MainNetworkHelper
        {
            get
            {
                return _networkHelper;
            }
        }
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