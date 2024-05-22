 using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;

 public class JumpState : MovementBaseState
 {
     public override void EnterState(MovementStateManager movement)
     {

       if (movement.previousState == movement.Run || movement.previousState == movement.Walk || movement.previousState == movement.Idle) movement.anim.SetTrigger("Jump");

        AudioManager.instance.PlaySfx(AudioManager.Sfx.SFX_move_jumpstart);
    }

     public override void UpdateState(MovementStateManager movement)
     {
        if(movement.jumped == true && movement.IsGrounded())
        {
            movement.jumped = false;

            AudioManager.instance.PlaySfx(AudioManager.Sfx.SFX_move_jumpend);

            if (movement.moveDir.magnitude > 0.1f){
              if (Input.GetKey(KeyCode.LeftShift)) movement.SwitchState(movement.Run);
              else  movement.SwitchState(movement.Walk);
            }
            else movement.SwitchState(movement.Idle);
        }
     }
 }
