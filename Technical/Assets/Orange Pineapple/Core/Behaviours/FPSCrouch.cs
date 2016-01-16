using UnityEngine;
using System.Collections;
using Lockstep;
public class FPSCrouch : ActiveAbility {
    [SerializeField,FixedNumber]
    private long _normalHeight = FixedMath.One * 2;
    public long NormalHeight {get {return _normalHeight;}}
    [SerializeField,FixedNumber]
    private long _crouchHeight = FixedMath.One;
    public long CrouchHeight {get {return _crouchHeight;}}
    private bool _crouched;
    public bool Crouched {
        get {
            return _crouched;
        }
        set {
            if (_crouched != value) {
                _crouched = value;
                if (value)
                    Crouch ();
                else
                    Uncrouch();
            }
        }
    }
    private void Crouch () {
        //TODO: Smoothing
        Agent.Body.SetHeight (CrouchHeight);
        CachedTurn.CameraHeight = CameraCrouchHeight;
    }
    private void Uncrouch () {
        Agent.Body.SetHeight(NormalHeight);
        CachedTurn.CameraHeight = CameraNormalHeight;
    }

    FPSTurn CachedTurn {get; set;}

    long CameraCrouchHeight {get ;set;}
    long CameraNormalHeight {get; set;}

    protected override void OnLateSetup()
    {
        CachedTurn = Agent.GetAbility<FPSTurn> ();
        this.CameraNormalHeight = CachedTurn.CameraHeight;
        this.CameraCrouchHeight = CameraNormalHeight - (NormalHeight - CrouchHeight);
    }
    protected override void OnInitialize()
    {
        this.Uncrouch ();
    }
    protected override void OnSimulate()
    {
        base.OnSimulate();
    }   
    protected override void OnExecute(Command com)
    {
        bool crouch = (bool) com.GetData<DefaultData>().Value;
        this.Crouched = crouch;
    }



    public Command GenerateCrouchCommand (bool crouch) {
        Command com = new Command(this.Interfacer.ListenInputID,Agent.Controller.ControllerID);
        com.Add<DefaultData> (new DefaultData(DataType.Bool,crouch));
        return com;
    }
}
