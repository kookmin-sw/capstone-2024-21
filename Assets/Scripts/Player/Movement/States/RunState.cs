using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.audioState((int)AudioManager.Sfx.SFX_move_run);      

        movement.anim.SetBool("Running", true);
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
            if (!Input.GetKey(KeyCode.LeftShift)) {
                ExitState(movement, movement.Walk);
                return;
            }
            else if (movement.moveDir.magnitude < 0.1f) {
                ExitState(movement, movement.Idle);
                return;
            }
 
            if (Input.GetKeyDown(KeyCode.C)) {
                ExitState(movement, movement.Crouch);
                return;
            }

            if (movement.zAxis < 0) movement.currentMoveSpeed = movement.runBackSpeed;
            else movement.currentMoveSpeed = movement.runSpeed;
        }
    }

    public void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Running", false);
        movement.SwitchState(state);
    }
}
