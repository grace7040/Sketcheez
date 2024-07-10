using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(MonController monster) : base(monster) { }

    public override void Enter()
    {
        monster.rb.velocity = Vector2.zero;
        monster.animator.SetBool("IsAttacking", true);
        monster.StartCoroutine(AttackRoutine());
    }

    public override void Update()
    {
        //if (monster.canflip)
        //{
        //    monster.isFlip = monster.player.position.x < monster.transform.position.x;
        //    monster.monsterBody.rotation = monster.isFlip ? Quaternion.identity : monster.flipQuaternion;
        //}
        monster.isFlip = monster.player.position.x < monster.transform.position.x;
        monster.monsterBody.rotation = monster.isFlip ? Quaternion.identity : monster.flipQuaternion;
    }

    public override void Exit()
    {
        monster.animator.SetBool("IsAttacking", false);
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(1f);
        if (!monster.isDie)
        {
            monster.Attack();
            yield return new WaitForSeconds(2f);
            monster.ChangeState(new IdleState(monster));
        }
    }
}