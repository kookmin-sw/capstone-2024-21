using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Idle", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if(!movement.jumped){
            if (Input.GetKeyDown(KeyCode.Space) && movement.staminaManager.staminaBar.value >= movement.staminaManager.jumpValue)
            {
                movement.previousState = this;
                ExitState(movement, movement.Jump);
                return;
            }
            if(movement.moveDir.magnitude > 0.1f){
                if (Input.GetKey(KeyCode.LeftShift)) {
                    ExitState(movement, movement.Run);
                    return;
                }
                else {
                    ExitState(movement, movement.Walk);
                    return;
                }
            }
            if(Input.GetKeyDown(KeyCode.C)) {
                ExitState(movement, movement.Crouch);
                return;
            }    
        }
    }
    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Idle", false);
        movement.SwitchState(state);
    }
    
}