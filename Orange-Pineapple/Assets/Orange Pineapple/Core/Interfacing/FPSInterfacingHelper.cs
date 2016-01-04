using UnityEngine;
using System.Collections;
using Lockstep;
using Lockstep.Data;

public class FPSInterfacingHelper : InterfacingHelper
{

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    protected override void OnSimulate()
    {
    }

    private Vector2 lastInput;
    Quaternion lastRotation;

    protected override void OnVisualize()
    {
        if (FPSHelper.Instance.FPSAgent != null)
        {
            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (input != lastInput)
            {
                lastInput = input;
                Command com = FPSHelper.Instance.FPSAgent.GetAbility<FPSMove>().GenerateMoveCommand(input);
                PlayerManager.SendCommand(com);
            }

            Quaternion curRotation = FPSHelper.Instance.FPSAgent.GetAbility<FPSTurn>().CameraController.transform.rotation;
            if (lastRotation != curRotation)
            {
                Vector3 forward = curRotation * Vector3.forward;
                PlayerManager.SendCommand(FPSHelper.Instance.FPSAgent.GetAbility<FPSTurn>().GenerateTurnCommand(forward));
                lastRotation = curRotation;
            }
        }
    }
}
