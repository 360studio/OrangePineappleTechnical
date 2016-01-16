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
    private float lastHorInput;
    Quaternion lastRotation;

    double accumulator;
    int comCount = 101;

    int comHash { get { return (ClientManager.ClientID ^ comCount++); } }

    private FPSMove _mover;

    FPSMove Mover
    {
        get { return _mover ?? (_mover = FPSHelper.Instance.FPSAgent.GetAbility<FPSMove>()); }
    }

    LSAgent _agent;

    LSAgent Agent
    {
        get { return _agent ?? (_agent = FPSHelper.Instance.FPSAgent); }
    }

    FPSTurn _turner;

    FPSTurn Turner
    {
        get { return _turner ?? (_turner = FPSHelper.Instance.FPSAgent.GetAbility<FPSTurn>()); }
    }

    FPSShoot _shooter;

    FPSShoot Shooter
    {
        get { return _shooter ?? (_shooter = FPSHelper.Instance.FPSAgent.GetAbility<FPSShoot>()); }
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
                /*
                Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (input != lastInput)
                {
                    lastInput = input;
                    Command com = Mover.GenerateMoveCommand(input);
                    if (Agent.Controller.SelectionChanged)
                    {
                        com.Add(new Selection(Agent.Controller.SelectedAgents));
                    }
                    SendCommand(com);
                }
                */

                float horInput = Input.GetAxis("Horizontal");
                if (horInput != lastHorInput)
                {
                    Command com = Agent.GetAbility<CoverMove>().GenerateMovementCommand(horInput);

                    if (com != null)
                    {
                        lastHorInput = horInput;

                        SendCommand(com);
                    }
                }

                Quaternion curRotation = FPSHelper.Instance.FPSAgent.GetAbility<FPSTurn>().CameraController.transform.rotation;
                if (lastRotation != curRotation)
                {
                    Vector3 forward = curRotation * Vector3.forward;
                    Command com = FPSHelper.Instance.FPSAgent.GetAbility<FPSTurn>().GenerateTurnCommand(forward);

                    SendCommand(com);
                    lastRotation = curRotation;
                }
            }
        }

        #if true
        if (Input.GetButtonDown("Fire1"))
        {
            Command com;
            if (Shooter.GenerateFireCommand(true, out com))
            {
                SendCommand(com, true);
            }
        }
        if (Input.GetButtonUp("Fire1"))
        {
            Command com;
            if (Shooter.GenerateFireCommand(false, out com))
            {
                SendCommand(com, true);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SendCommand(Agent.GetAbility<CoverMove>().GenerateTransitionCommand(), true);
        }
        if (Input.GetButtonDown("Jump"))
        {
            SendCommand(Agent.GetAbility<FPSCrouch>().GenerateCrouchCommand(true), true);
        }
        if (Input.GetButtonUp("Jump")) {
            SendCommand(Agent.GetAbility<FPSCrouch>().GenerateCrouchCommand(false),true);
        }
        #endif

        if (sendOut) {
            CommandManager.SendOut();
        }
    }
    bool sendOut;
    public void SendCommand(Command com, bool immediate = false)
    {
        if (com == null)
            return;
        if (Agent.Controller.SelectionChanged)
            com.Add(new Selection(Agent.Controller.SelectedAgents));
        CommandManager.SendCommand(com, false);
        if (immediate) sendOut = true;
    }
}
