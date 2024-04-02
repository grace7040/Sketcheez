using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedColor :  IColorState
{
    public int Damage { get { return 50; } }
    public bool WallSliding { get { return false; } }
    public float CoolTime { get { return 0.35f; } }


    //Temporal Setting : Red Color Attack -> Throw obj
    public void Attack(PlayerController player)
    {
        //Debug.Log("Attak");
        player.canAttack = false;
        player.animator.SetBool("IsRedAttacking", true);
        AudioManager.Instacne.PlaySFX("Red");
        //player.UpdateCanAttack();
        player.CallOnDelay(CoolTime, () =>
            {
                player.canAttack = true;
            });
    }
}
