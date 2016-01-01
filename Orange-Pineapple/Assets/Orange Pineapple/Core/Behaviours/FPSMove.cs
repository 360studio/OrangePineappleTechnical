using UnityEngine;
using System.Collections;
using Lockstep;
public class FPSMove : Lockstep.ActiveAbility {
    protected override void OnExecute(Command com)
    {
        
    }
    protected override void OnVisualize()
    {
        if (PlayerManager.ContainsController(this.Agent.Controller)) {
            float vertical = Input.GetAxis("Vertical");
            float horizontal = Input.GetAxis("Horizontal");
            if (vertical != 0 || horizontal != 0) {
                Vector2d moveVec = new Vector2d (horizontal,vertical);
                Command com = new Command(InputCodeManager.GetCodeID("Movement"), this.Agent.Controller.ControllerID);
                com.Add<Vector2d> (moveVec);
            }
        }
    }
}
