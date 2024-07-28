using GooglePlayGames.BasicApi;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{

    [Header("Weapon")]
    public SpriteRenderer[] ColorWeapons;
    public GameObject RedWeapon;
    public GameObject OrangeWeapon;
    public GameObject PurpleWeapon;
    public GameObject GreenWeapon;
    public GameObject BlueWeapon;
    public GameObject BlackWeapon;

    [Header("AttackEffect")]
    public GameObject YellowAttackEffect;
    public GameObject OrangeAttackEffect;
    public GameObject PurpleAttackEffect;

    //Attack
    [HideInInspector] public bool canAttack = true;

    // Black
    public GameObject WeaponPosition;
    bool _isHoldingEnemy = false; // 적을 가지고 있는지 여부
    Rigidbody2D _heldEnemyRigidbody; // 가지고 있는 적의 Rigidbody2D
    GameObject _enemy;
    float _pullForce = 10f; // 끌어당기는 힘 조절용 변수
    float _throwForce = 15f; // 던지는 힘 조절용 변수

    PlayerController _playerController;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        ColorManager.Instance.InitPlayerAttack(this, OnOrangeAttacked, OnYellowAttacked, OnBlackAttacked, OnSetBlackColor);
    }


    public void AttackDown()
    {
        canAttack = false;
        _playerController.Color.Attack(transform.position, transform.localScale.x);
        this.CallOnDelay(_playerController.Color.CoolTime, () => { canAttack = true; });   // ::TODO:: 노랑일 경우 예외처리 해야함
    }

    public void AttackUp()
    {
        //isAttack = false;
    }

    public void SetCustomWeapon()
    {
        ColorWeapons[(int)_playerController.myColor].sprite = DrawManager.Instance.sprites[(int)_playerController.myColor];

        // Yellow 경우, 자식들에도 sprite 할당이 필요함
        if (_playerController.myColor == Colors.Yellow)
        {
            for (int i = 0; i < 4; i++)
            {
                YellowAttackEffect.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = ColorWeapons[(int)Colors.Yellow].sprite;
            }
        }
    }

    public void SetBasicWeapon()
    {
        ColorWeapons[(int)_playerController.myColor].sprite = DrawManager.Instance.Basic_Sprites[(int)_playerController.myColor];

        if (_playerController.myColor == Colors.Yellow)
        {
            for (int i = 0; i < 4; i++)
            {
                YellowAttackEffect.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = ColorWeapons[(int)Colors.Yellow].sprite;
            }
        }
    }

    public void OnPurpleAttacked()
    {
        StartCoroutine(PurpleAttackEffectCo());
    }

    IEnumerator PurpleAttackEffectCo()
    {
        PurpleAttackEffect.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        PurpleAttackEffect.SetActive(false);
    }

    void OnOrangeAttacked(float durationTime)
    {
        gameObject.layer = (int)Layer.OrangeAttack; // layer 변경으로 충돌 처리 막음

        OrangeAttackEffect.SetActive(true);

        var originalMoveSpeed = _playerController.MoveSpeed;
        _playerController.MoveSpeed = 20f;

        this.CallOnDelay(durationTime, () =>
        {
            OrangeAttackEffect.SetActive(false);
            gameObject.layer = (int)Layer.Player;
            _playerController.MoveSpeed = originalMoveSpeed;
        });
    }

    void OnYellowAttacked()
    {
        YellowAttackEffect.SetActive(true);
        YellowAttackEffect.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = ColorWeapons[(int)Colors.Yellow].sprite;
    }

    void OnBlackAttacked()
    {
        if (_isHoldingEnemy)
        {
            BlackThrow();
            AudioManager.Instacne.PlaySFX("BlackRelease");
        }
        else
        {
            BlackPull();
            AudioManager.Instacne.PlaySFX("Black");
        }
    }

    void OnSetBlackColor()
    {
        StartCoroutine(OnSetBlackColorCo());
    }
    IEnumerator OnSetBlackColorCo()
    {
        while (true)
        {
            if (_playerController.myColor == Colors.Black)
            {
                if (_isHoldingEnemy)
                {
                    _enemy.transform.localPosition = new Vector2(0, 0);
                }
            }
            else if (_playerController.myColor != Colors.Black)
            {
                Destroy(_enemy, 0.1f);
                break;
            }

            yield return 1;
        }
    }

    public void BlackPull()
    {
        StartCoroutine(PullCoroutine());
    }

    private IEnumerator PullCoroutine()
    {
        if (!_isHoldingEnemy)
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float closestDistance = 7.5f;
            Transform closestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                MonsterController enemyController = enemy.GetComponent<MonsterController>();

                if (enemyController != null && !enemyController.IsDie)
                {
                    float distance = Vector2.Distance(transform.position, enemy.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy.transform;
                    }
                }
            }

            if (closestEnemy != null)
            {
                _heldEnemyRigidbody = closestEnemy.GetComponent<Rigidbody2D>();
                _enemy = closestEnemy.gameObject;

                _enemy.SendMessage("PulledByBlack");
                //closestEnemy.AddComponent<BloodEffect>();

                float distance = Vector2.Distance(_enemy.transform.position, transform.position);

                while (!_isHoldingEnemy)
                {
                    Vector2 throwDirection = (transform.position - _enemy.transform.position).normalized;
                    _enemy.transform.Translate(_pullForce * Time.deltaTime * throwDirection);

                    yield return null;
                }
            }
        }
        yield return new WaitForSeconds(0f);
    }


    public void BlackThrow()
    {
        Rigidbody2D rb = _enemy.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Transform childTransform = _enemy.gameObject.transform;

            childTransform.SetParent(null);

            _isHoldingEnemy = false;
            _enemy.GetComponent<Rigidbody2D>().gravityScale = 0.0f;

            rb.gameObject.GetComponent<OB_VerticlaMovement>().enabled = false;

            Vector2 throwDirection = (rb.transform.position - transform.position).normalized;
            rb.velocity = throwDirection * _throwForce;
            rb.gameObject.tag = "WeaponB";
            _heldEnemyRigidbody = null;

            _enemy.GetComponent<MonsterController>().Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (_playerController.myColor == Colors.Black)
            {
                if (!collision.gameObject.GetComponent<MonsterController>().isActiveAndEnabled)
                {
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    collision.gameObject.GetComponent<OB_VerticlaMovement>().enabled = true;

                    Transform parentTransform = WeaponPosition.transform;
                    Transform childTransform = collision.gameObject.transform;
                    childTransform.SetParent(parentTransform);

                    _isHoldingEnemy = true;
                }
            }
        }
    }
}