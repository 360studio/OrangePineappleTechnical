using UnityEngine;
using System.Collections;
using Lockstep;

public class FPSMove : Lockstep.ActiveAbility
{
    //asdfasdawey23
    [SerializeField,FixedNumber(true)]
    private long _speed = FixedMath.One * 5 / LockstepManager.FrameRate;

    [SerializeField, FixedNumber]
    private long _acceleration = FixedMath.One / LockstepManager.FrameRate;

    public long Speed { get { return _speed; } }

    public long Acceleration { get { return _acceleration; } }

    public Vector2d MoveInput {get; private set;}
    //TODO: Make sure resetting works
    [Lockstep(true)]
    private Vector2d TargetVelocity { get; set; }

    [Lockstep(true)]
    private bool TargetVelocityReached { get; set; }

    protected override void OnSetup()
    {
        this.TargetVelocityReached = true;
    }

    protected override void OnExecute(Command com)
    {
        Vector2d moveInput;

        if (com.TryGetData<Vector2d>(out moveInput))
        {
            if (moveInput.FastMagnitude() > (FixedMath.One + FixedMath.One / 8) * (FixedMath.One + FixedMath.One / 8))
                Debug.LogError("Hax movement input: " + moveInput);
            MoveInput = moveInput;
            CalculateTargetVelocity ();
        }
    }

    private void CalculateTargetVelocity () {
        TargetVelocity = MoveInput;
        TargetVelocity = TargetVelocity.Rotated(Agent.Body._rotation.y,Agent.Body._rotation.x);

        TargetVelocity *= Speed;
        TargetVelocityReached = false;
    }
    protected override void OnSimulate()
    {

        if (Agent.Body.RotationChangedBuffer) {
            CalculateTargetVelocity();
        }
        if (!this.TargetVelocityReached) {

            Vector2d steering = TargetVelocity - Agent.Body._velocity;
            steering *= Acceleration;
            Agent.Body._velocity += steering;
            Agent.Body.VelocityChanged = true;
            if (Agent.Body._velocity == TargetVelocity)
                TargetVelocityReached = true;
        }
    }


    public Command GenerateMoveCommand(Vector2 input)
    {
        Command com = new Command(this.Data.ListenInputID, this.Agent.Controller.ControllerID);
        Vector2d vec = (new Vector2d(input));
        if (vec.FastMagnitude () > FixedMath.One * FixedMath.One) {
            vec.Normalize();
        }
        com.Add<Vector2d>(vec);
        return com;
    }
}
