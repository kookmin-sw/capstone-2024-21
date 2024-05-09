using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitingState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Exiting", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if(movement.moveDir.magnitude > 0.1f){
            if (Input.GetKey(KeyCode.LeftShift)) ExitState(movement, movement.Run);
            else ExitState(movement, movement.Walk);
        }
        if (Input.GetKeyDown(KeyCode.Space) && movement.staminaManager.staminaBar.value >= movement.staminaManager.jumpValue)
        {
            movement.previousState = this;
            ExitState(movement, movement.Jump);
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Exiting", false);
        movement.SwitchState(state);
    }
}
