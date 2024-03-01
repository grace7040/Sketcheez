using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class UI_Game : UI_Scene
{
    public GameObject player;
    public TextMeshProUGUI Score;
   // public int mapNum;
    Colors player_color = Colors.def;

    // Button
    GameObject jump;
    GameObject dash;
    GameObject attack;

    // Image
    Image hpBar;
    Image Red_IMG;
    Image Yellow_IMG;
    Image Blue_IMG;
    Image Attack_Cool_Time_IMG;
    Image PotalArr_IMG;

    public float hpBarMAX;
    public VariableJoystick joystick;

    //player
    private float attack_cool_time = 0f;
    private float attack_cool_count = 0f;
    private bool canAttack;

    //Potal
    private float angle;
    private Vector2 potalVec;
    private Vector2 playerVec;

    enum Buttons
    {
        SettingBtn,
        Palette,
        Attack,
        Jump,
        Dash,
        Joystick,
    }

    enum Images
    {
        HP,
        Red,
        Yellow,
        Blue,
        Attack_Cool_Time,
        PotalArrow,
    }

    enum Texts
    {
        StarCount,
    }

    public void Start()
    {
        Init();
        ObjectManager.Instance.UI_InGame_Ready = true;
        //GameManager.Instance.currentMapNum = mapNum;
    }

    private void FixedUpdate()
    {
        // 플레이어 체력바
        hpBar.fillAmount = GameManager.Instance.HPBar();

        // 별
        int star = 10 - GameManager.Instance.starCount;
        Score.text = star + "/10";
        //GetText((int)Texts.StarCount).text = 
    }

    public void SetPalette()
    {
        Blue_IMG.gameObject.SetActive(ColorManager.Instance.HasBlue);
        Red_IMG.gameObject.SetActive(ColorManager.Instance.HasRed);
        Yellow_IMG.gameObject.SetActive(ColorManager.Instance.HasYellow);

        Colors current_color32 = GameManager.Instance.playerColor;

        // 플레이어 state에 따른 색 변경
        if (player_color != current_color32)
        {
            player_color = current_color32;

            Color32 color32 = ColorManager.Instance.GetColor(player_color);
            Color32 alpha_color32_1 = new Color32(color32.r, color32.g, color32.b, 150);
            Color32 alpha_color32_2 = new Color32(color32.r, color32.g, color32.b, 200);

            hpBar.color = alpha_color32_2;
            GetButton((int)Buttons.Attack).gameObject.GetComponent<Image>().color = alpha_color32_2;
            jump.GetComponent<Image>().color = alpha_color32_1;
            dash.GetComponent<Image>().color = alpha_color32_1;

        }
        if(player_color == Colors.yellow)
            GetImage((int)Images.Attack_Cool_Time).gameObject.SetActive(true);
        else
            GetImage((int)Images.Attack_Cool_Time).gameObject.SetActive(false);

    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<Text>(typeof(Texts));

        GetButton((int)Buttons.SettingBtn).gameObject.BindEvent(OnSettingBtnClicked);
        GetButton((int)Buttons.Palette).gameObject.BindEvent(PaletteBtnClicked);
        //  GetButton((int)Buttons.Attack).gameObject.BindEvent(AttackBtnClicked);
        hpBar = GetImage((int)Images.HP);


        jump = GetButton((int)Buttons.Jump).gameObject;
        dash = GetButton((int)Buttons.Dash).gameObject;
        attack = GetButton((int)Buttons.Attack).gameObject;

        BindEvent(jump, JumpBtnClickedDown, Define.UIEvent.DownClick);
        BindEvent(jump, JumpBtnClickedUp, Define.UIEvent.UpClick);
        BindEvent(dash, DashBtnClickedDown, Define.UIEvent.DownClick);
        BindEvent(dash, DashBtnClickedUp, Define.UIEvent.UpClick);
        BindEvent(attack, AttackBtnClickedDown, Define.UIEvent.DownClick);
        BindEvent(attack, AttackBtnClickedUp, Define.UIEvent.UpClick);


        // 물감 오브젝트 받아두기 + ColorManger에 저장된 색으로 변경
        Red_IMG = GetImage((int)Images.Red);
        Red_IMG.color = ColorManager.Instance.GetColor(Colors.red);

        Yellow_IMG = GetImage((int)Images.Yellow);
        Yellow_IMG.color = ColorManager.Instance.GetColor(Colors.yellow);


        Blue_IMG = GetImage((int)Images.Blue);
        Blue_IMG.color = ColorManager.Instance.GetColor(Colors.blue);

        Attack_Cool_Time_IMG = GetImage((int)Images.Attack_Cool_Time);


        // Palette 세팅
        SetPalette();
        ColorManager.Instance.OnSetColor += SetPalette;

        GameManager.Instance.SetJoystick = () => {
            FindObjectOfType<CharacterMove>().joystick = joystick;
        };


        // hpBar 길이 받아두기
        hpBarMAX = hpBar.gameObject.GetComponent<RectTransform>().rect.width;

        // Potal
        PotalArr_IMG = GetImage((int)Images.PotalArrow);
        if(GameManager.Instance.Potal != null)
            potalVec = GameManager.Instance.Potal.transform.position;


        GetImage((int)Images.Attack_Cool_Time).gameObject.SetActive(false);

    }

    public void OnSettingBtnClicked(PointerEventData data) // 설정 버튼 눌렀을 때
    {
        // 게임 일시정지 후 설정UI 띄우기

        GameManager.Instance.PauseGame();
        Managers.UI.ShowPopupUI<UI_Setting>();
    }

    public void PaletteBtnClicked(PointerEventData data)
    {
        // 게임 일시정지 후 설정UI 띄우기

        GameManager.Instance.PauseGame();
        Managers.UI.ShowPopupUI<UI_Palette>();
        //  Managers.UI.ShowPopupUI<UI_Palette>();

    }


    // 플레이어 컨트롤

    //공격 버튼이 눌렸을 때
    public void AttackBtnClickedDown(PointerEventData data)
    {
        player.GetComponent<PlayerController>().AttackDown();
        attack_cool_time = player.GetComponent<PlayerController>().coolTime;
        attack_cool_count = attack_cool_time;

        GetImage((int)Images.Attack_Cool_Time).gameObject.SetActive(true);
        canAttack = true;
    }
    
    public void AttackBtnClickedUp(PointerEventData data)
    {
        player.GetComponent<PlayerController>().AttackUp();
    }

    public void JumpBtnClickedDown(PointerEventData data)
    {
        player.GetComponent<CharacterMove>().JumpDown();
    }

    public void JumpBtnClickedUp(PointerEventData data)
    {
        player.GetComponent<CharacterMove>().JumpUp();
    }

    public void DashBtnClickedDown(PointerEventData data)
    {
        player.GetComponent<CharacterMove>().DashDown();
    }

    public void DashBtnClickedUp(PointerEventData data)
    {
        player.GetComponent<CharacterMove>().DashUp();
    }

    private void OnDestroy()
    {
        if (ColorManager.Instance != null)
            ColorManager.Instance.OnSetColor -= SetPalette;
    }

    private void Update()
    {
        // print(player.GetComponent<PlayerController>().coolTime);
        if (canAttack)
        {
            StartCoroutine("SkillTimeChk");
        }

        // Potal
        playerVec = player.transform.position;
        angle = Mathf.Atan2(potalVec.y - playerVec.y, potalVec.x - playerVec.x) * Mathf.Rad2Deg;
        PotalArr_IMG.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    IEnumerator SkillTimeChk()
    {
        yield return null;
        if (attack_cool_count > 0)
        {
            attack_cool_count -= Time.deltaTime;

            if (attack_cool_count < 0)
            {
                print("hi");

                attack_cool_count = 0;
                canAttack = false;
                GetImage((int)Images.Attack_Cool_Time).gameObject.SetActive(false);

                attack_cool_count = player.GetComponent<PlayerController>().coolTime;

            }
            float time = attack_cool_count / player.GetComponent<PlayerController>().coolTime;
            GetImage((int)Images.Attack_Cool_Time).fillAmount = time;

        }

    }


}
