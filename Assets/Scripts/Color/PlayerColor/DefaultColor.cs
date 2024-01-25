using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DefaultColor : IColorState
{
    public float JumpForce { get { return 850f; } }
    public int Damage { get { return 10; } }
    public bool WallSliding { get { return false; } }
    public float CoolTime { get { return 0.5f; } }


    //Temporal Setting : Default Color Attack -> Sword
    public void Attack(PlayerController player)
    {
        //if (player.doAttack && player.canAttack)
        //{
            Debug.Log("Attak");
            player.canAttack = false;
            player.animator.SetBool("IsAttacking", true);
            //player.UpdateCanAttack();
            player.CallOnDelay(CoolTime, () =>
            {
                player.canAttack = true;
            });
            
        //}
    }


}
