using UnityEngine;
using System.Collections;
using Lockstep;
using Lockstep.Data;
public class FPSInterfacingHelper : InterfacingHelper
{
    LSAgent playerAgent;

    protected override void OnInitialize()
    {
        base.OnInitialize();
    }

    protected override void OnSimulate()
    {
    }
        
    private Vector2 lastInput;

    protected override void OnVisualize()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (input != Vector2.zero)
        {
            if (input != lastInput)
            {
                lastInput = input;
                Command com = playerAgent.GetAbility<FPSMove>().GenerateMoveCommand(input);
                CommandManager.SendCommand(com);
            }
        }

    }
}
