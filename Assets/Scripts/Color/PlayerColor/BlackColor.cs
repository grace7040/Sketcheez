using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackColor : IColorState
{
    public float JumpForce { get { return 800f; } }
    public int Damage { get { return 10; } }
    public bool WallSliding { get { return false; } }

    public GameObject ThrowableObject { get; set; }
    public GameObject CustomObject { get; set; }

    public Sprite Sprite { get; set; }

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

    public float pullForce = 20f; // 끌어당기는 힘 조절용 변수
    public float throwForce = 20f; // 던지는 힘 조절용 변수
    private bool isHoldingEnemy = false; // 적을 가지고 있는지 여부
    private Rigidbody2D heldEnemyRigidbody; // 가지고 있는 적의 Rigidbody2D
    private Transform playerTransform; // 플레이어의 Transform

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
            PullClosestEnemy(player.transform);
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
                isHoldingEnemy = true;
                heldEnemyRigidbody = closestEnemy.GetComponent<Rigidbody2D>();

                closestEnemy.GetComponent<Animator>().enabled = false;
                closestEnemy.GetComponent<MonsterController>().enabled = false;
                
                // Player가 데미지도 안받고, 넉백도 안되게 해야함. Collision 끄면 될듯.
                // 그리고 몬스터가 플레이어한테 붙어있어야함.
                // 점프 상태에서도 잘 끌고 와야 함.
                // 원하는 방향으로 날리고 싶음.

                Vector2 throwDirection = (playerTransform.position - heldEnemyRigidbody.transform.position).normalized;
                heldEnemyRigidbody.AddForce(throwDirection * pullForce, ForceMode2D.Impulse);
;            }
        }
    }

    // 가지고 있는 적을 던집니다.
    private void ThrowHeldEnemy()
    {
        if (heldEnemyRigidbody != null)
        {
            isHoldingEnemy = false;
            Vector2 throwDirection = (heldEnemyRigidbody.transform.position - playerTransform.position).normalized;
            heldEnemyRigidbody.velocity = throwDirection * throwForce;
            heldEnemyRigidbody = null;
        }
    }
}
