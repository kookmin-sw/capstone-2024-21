using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Crouching", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if(!movement.jumped){
            if(Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Space))
            {   
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
                else {
                    ExitState(movement, movement.Idle);
                    return;
                }
            }
            
            if(Input.GetKey(KeyCode.LeftShift)) {
                movement.currentMoveSpeed = movement.crouchFastBackSpeed;
                return;
            }
            else {
                movement.currentMoveSpeed = movement.crouchBackSpeed;
                return;
            }
        }
    }
    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Crouching", false);
        movement.SwitchState(state);
    }
}
