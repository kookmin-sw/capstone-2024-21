using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.SFX_move_walk);

        movement.currentMoveSpeed = movement.walkSpeed;
        movement.anim.SetBool("Walking", true);
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
            if (Input.GetKey(KeyCode.LeftShift)) {
                ExitState(movement, movement.Run);
                return;
            }
            else if (Input.GetKeyDown(KeyCode.C)) {
                ExitState(movement, movement.Crouch);
                return;
            }
            else if (movement.moveDir.magnitude < 0.1f) {
                ExitState(movement, movement.Idle);
                return;
            }

            if (movement.zAxis < 0) movement.currentMoveSpeed = movement.walkBackSpeed;
            else movement.currentMoveSpeed = movement.walkSpeed;
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Walking", false);
        movement.SwitchState(state);
    }
}
