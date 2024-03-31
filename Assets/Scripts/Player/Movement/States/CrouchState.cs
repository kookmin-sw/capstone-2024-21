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
        if(Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Space))
        {   
            if(movement.moveDir.magnitude > 0.1f){
                if (Input.GetKey(KeyCode.LeftShift)) ExitState(movement, movement.Run);
                else ExitState(movement, movement.Walk);
            }
            else ExitState(movement, movement.Idle);
        }
        
        if(Input.GetKey(KeyCode.LeftShift)) movement.currentMoveSpeed = movement.crouchFastBackSpeed;
        else movement.currentMoveSpeed = movement.crouchBackSpeed;
    }
    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Crouching", false);
        movement.SwitchState(state);
    }
}
