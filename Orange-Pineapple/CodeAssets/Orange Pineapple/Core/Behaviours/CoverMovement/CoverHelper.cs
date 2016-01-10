using UnityEngine;
using System.Collections;
using Lockstep;
public class CoverHelper : BehaviourHelper {
    protected override void OnInitialize()
    {
        CoverManager.Initialize();
    }

    protected override void OnSimulate()
    {
        CoverManager.Simulate();
    }
    protected override void OnDeactivate()
    {
        CoverManager.Deactivate();
    }
}
