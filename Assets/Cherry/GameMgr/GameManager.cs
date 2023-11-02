using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    public bool isGameOver;
    public int playerHP = 100;
    public int playerMAXHP = 100;

    void Start()
    {
        isGameOver = false;
    }

    void Update()
    {
        // 게임오버 확인 (ex. 플레이어의 체력)
    }

    public float HPBar()
    {
        return (float)playerHP / playerMAXHP;
    }

    public void PauseGame()
    {
        
        // 게임 일시정지
        Managers.UI.ClosePopupUI();
        print("일시정지");
    }

    public void ResumeGame()
    {
        Managers.UI.ShowPopupUI<UI_InGame>();
        print("계속하자");
    }

    public void RetryGame()
    {
        // 게임 재시작
        Managers.UI.ShowPopupUI<UI_InGame>();
        print("재시작");
    }

    public void GameOver()
    {
        
        // 패배 UI 띄우기
        isGameOver = true;

        // 왜 안띄ㅜ어질까?
        Managers.UI.ShowPopupUI<UI_GameOver>();
        print("GameOver");
        PauseGame();
       
    }

    public void GameWin()
    {
        // 승리 UI 띄우기
        isGameOver = true;
        PauseGame();
}

}
