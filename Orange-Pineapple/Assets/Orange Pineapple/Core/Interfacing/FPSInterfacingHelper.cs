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

    private Vector2 lastInput;
    Quaternion lastRotation;

    double accumulator;
    int comCount = 101;
    int comHash {get {return (ClientManager.ClientID ^ comCount++);}}

    private FPSMove _mover;
    FPSMove Mover {
        get {return _mover??(_mover = FPSHelper.Instance.FPSAgent.GetAbility<FPSMove>());}
    }
    LSAgent _agent;
    LSAgent Agent {
        get {return _agent ?? (_agent = FPSHelper.Instance.FPSAgent);}
    }
    FPSTurn _turner;
    FPSTurn Turner {
        get {return _turner??(_turner = FPSHelper.Instance.FPSAgent.GetAbility<FPSTurn>());}
    }

    protected override void OnVisualize()
    {
        double sendRate = 1d / (LockstepManager.FrameCount / LockstepManager.InfluenceResolution);
        accumulator += (double)Time.deltaTime;
        if (accumulator >= sendRate)
        {
            accumulator %= sendRate;
            if (FPSHelper.Instance.FPSAgent != null)
            {
                Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (input != lastInput)
                {
                    lastInput = input;
                    Command com = Mover.GenerateMoveCommand(input);
                    if (Agent.Controller.SelectionChanged) {
                        com.Add (new Selection(Agent.Controller.SelectedAgents));
                    }
                    CommandManager.SendCommand(com);
                }

                Quaternion curRotation = FPSHelper.Instance.FPSAgent.GetAbility<FPSTurn>().CameraController.transform.rotation;
                if (lastRotation != curRotation)
                {
                    Vector3 forward = curRotation * Vector3.forward;
                    Command com = FPSHelper.Instance.FPSAgent.GetAbility<FPSTurn>().GenerateTurnCommand(forward);
                    if (Agent.Controller.SelectionChanged)
                        com.Add(new Selection(Agent.Controller.SelectedAgents));
                    CommandManager.SendCommand(com);
                    lastRotation = curRotation;
                }
            }
        }
    }
}
