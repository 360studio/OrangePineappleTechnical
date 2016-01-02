using UnityEngine;
using System.Collections;
using Lockstep;
public class FPSMove : Lockstep.ActiveAbility {
    protected override void OnExecute(Command com)
    {
        Vector2d moveInput;
        if (com.TryGetData<Vector2d> (out moveInput)) {
            this.Agent.Body.Velocity = moveInput;
        }
    }
    protected override void OnVisualize()
    {

    }


    public Command GenerateMoveCommand (Vector2 input) {
        Command com = new Command(this.Interfacer.ListenInputID,this.Agent.Controller.ControllerID);
        com.Add<Vector2d> (new Vector2d(input));
        return com;
    }
}
