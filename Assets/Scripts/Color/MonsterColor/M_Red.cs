using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M_Red : MonsterController
{
    public GameObject fireObject;
    // Update is called once per frame

    void Update()
    {
        base.Update();
        if (isDie) return;
        if (distanceX <= detectionRange && distanceY <= 1.0f)
        {
            // Attack
            if (distanceX <= attackRange && distanceY <= 1.0f)
            {
                if (!CheckGround()) { }
                else
                {
                    if (canAttack)
                    {
                        animator.SetBool("IsWalking", false);
                        rb.velocity = Vector2.zero;
                        animator.SetBool("IsAttacking", true);
                        //Color.Attack(this);
                        Attack();

                        //StartCoroutine(AttackCooldown_R());
                        canAttack = false;
                        //canflip = false;
                        rb.velocity = Vector2.zero;

                        this.CallOnDelay(2f, () => {
                            animator.SetBool("IsAttacking", false);
                            canAttack = true;
                        });
                        //yield return new WaitForSeconds(2.0f);
                        //animator.SetBool("IsAttacking", false);
                        //canAttack = true;
                        //canflip = true;
                    }
                }
            }
            else
            {
                if (!CheckGround())
                {
                    rb.velocity += Time.deltaTime * Vector2.down;
                }
                else
                {
                    if (canAttack)
                    {
                        currentWaypoint = player.position;
                        // Move
                        animator.SetBool("IsWalking", true);
                        Vector2 moveDirection = new Vector2(player.position.x - transform.position.x, 0).normalized;
                        rb.velocity = moveDirection * moveSpeed;
                    }
                }
            }
        }
        else 
            Move();
    }

    void Attack()
    {
        fireObject.SetActive(true);
        this.CallOnDelay(1f, () => { fireObject.SetActive(false); });
        //var fire = ObjectPoolManager.Instance.GetGo("Fire");

        //// ���� ó��
        //SpriteRenderer monsterSpriteRenderer = GetComponent<SpriteRenderer>();
        //if (monsterSpriteRenderer.flipX)
        //{
        //    fire.transform.localRotation = Quaternion.Euler(0, 0, 90);
        //}
        //else
        //{
        //    fire.transform.localRotation = Quaternion.Euler(0, 0, -90);
        //}

        //Rigidbody2D rb = GetComponent<Rigidbody2D>();
        //fire.transform.SetParent(rb.transform);
        //fire.transform.localPosition = Vector3.zero;
    }
}
