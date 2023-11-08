using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackColor : IColorState
{
    public float JumpForce { get { return 800f; } }
    public int Damage { get { return 15; } }
    public bool WallSliding { get { return false; } }

    ////Temporal Setting : Red Color Attack -> Throw obj
    //public void Attack(PlayerController player)
    //{
    //    GameObject throwableWeapon = Instantiate(Resources.Load("Projectile"),
    //        player.transform.position + new Vector3(player.transform.localScale.x * 0.5f, -0.2f),
    //        Quaternion.identity) as GameObject;
    //    throwableWeapon.GetComponent<SpriteRenderer>().sprite = this.sprite;
    //    Vector2 direction = new Vector2(player.transform.localScale.x, 0);
    //    throwableWeapon.GetComponent<ThrowableWeapon>().direction = direction;
    //    throwableWeapon.name = "ThrowableWeapon";
    //}

    public float pullForce = 1f; // 끌어당기는 힘 조절용 변수
    public float throwForce = 20f; // 던지는 힘 조절용 변수
    public bool isHoldingEnemy = false; // 적을 가지고 있는지 여부
    private Rigidbody2D heldEnemyRigidbody; // 가지고 있는 적의 Rigidbody2D
    private Transform playerTransform; // 플레이어의 Transform
    private GameObject Enemy;

    // 플레이어와 충돌했을 때 호출되는 함수
    public void Attack(PlayerController player)
    {
        if (isHoldingEnemy)
        {
            // 이미 가지고 있는 적을 던집니다.
            ThrowHeldEnemy();
        }
        else
        {
            // 플레이어와 충돌했을 때, 적을 가지게 합니다.
            playerTransform = player.transform; // 플레이어의 Transform 얻기
            PullClosestEnemy(playerTransform);
        }
    }

    // 가장 가까운 적을 끌어당김
    private void PullClosestEnemy(Transform playerTransform)
    {
        if (!isHoldingEnemy)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float closestDistance = Mathf.Infinity;
            Transform closestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector2.Distance(playerTransform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }

            if (closestEnemy != null)
            {
                isHoldingEnemy = true; // 타이밍이 문제
                heldEnemyRigidbody = closestEnemy.GetComponent<Rigidbody2D>();
                closestEnemy.GetComponent<CapsuleCollider>().enabled = false;
                Enemy = closestEnemy.gameObject;

                closestEnemy.GetComponent<Animator>().enabled = false;
                closestEnemy.GetComponent<MonsterController>().enabled = false;
                closestEnemy.GetComponent<Rigidbody2D>().mass = 0.1f;
                //heldEnemyRigidbody.isKinematic = true;
                //heldEnemyRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;

                //Debug.Log(playerTransform.position);
                //Enemy.transform.Translate(playerTransform.position * pullForce * Time.deltaTime);
                //Debug.Log(Enemy.transform.position);

                Vector2 throwDirection = (playerTransform.position - heldEnemyRigidbody.transform.position).normalized;
                heldEnemyRigidbody.AddForce(throwDirection * pullForce, ForceMode2D.Impulse);
                ;            }
        }
    }

    // 가지고 있는 적을 던집니다.
    private void ThrowHeldEnemy()
    {
     Rigidbody2D rb = Enemy.AddComponent<Rigidbody2D>();
        if (rb != null)
        {
            // 하위 객체(child)의 Transform을 얻어옵니다.
            Transform childTransform = Enemy.gameObject.transform;

            childTransform.GetComponent<CapsuleCollider>().enabled = true;

            // 부모-자식 관계를 해제합니다.
            childTransform.SetParent(null);

            isHoldingEnemy = false;
            Vector2 throwDirection = (rb.transform.position - playerTransform.position).normalized;
            rb.velocity = throwDirection * throwForce;
            rb.gameObject.tag = "WeaponB"; // 태그 변경
            heldEnemyRigidbody = null;
        }
    }
}
