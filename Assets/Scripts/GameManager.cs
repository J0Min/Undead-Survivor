using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("# Game Objects")]
    public Player player;
    public PoolManager pool;
    public LevelUp uiLevelUP;
    public GameObject uiResult;
    
    [Header("# Game Controls")]
    public float gameTime;//흐르는 게임 시간 - 난이도 계산용
    public float maxGameTime; //최대 게임 시간 - 난이도 증가 기준

    [Header("# Player Data")]
    public int level;
    public int kill;
    public int exp;
    //public int[] nextExp = { 3, 5, 10, 20, 150, 210, 280, 360, 450, 600 };
    public List<int> nextExp = new List<int> { 3, 5, 10, 20, 150, 210, 280, 360, 450, 600 }; // 동적 배열을 위해 일반 배열 대신 리스트 자료구조 변경
    public bool isLive; // 일시적지
    public float health;
    public float maxHealth = 100;


    
    private void Awake()
    {
        instance = this;
        //GameStart();
    }
    //public이어야 버튼 OnClick목록에 보여주기 위함
    public void GameStart()
    {
        isLive = true;
        uiLevelUP.Select(0);
        health = maxHealth;
        Resume();
    }

    //플레이어 사망시 호출
    public void GameOver()
    {
        StartCoroutine(GameOverRoutine());
    }

    IEnumerator GameOverRoutine()
    {
        isLive = false;
        yield return new WaitForSeconds(0.5f);//묘비 애니메이션 재생 시간
        uiResult.SetActive(true);//결과창 켜기
        Stop();//시간정지
    }

    public void GameRetry()
    {
        SceneManager.LoadScene(0);//0번에 등록된 씬
    }
    
    void Update()
    {
        if(!isLive)//일시 정지 상태에서는 시간 누적 중단
            return;
        
        //매 프레임마다 실제 흐른 시간을 누적
        gameTime += Time.deltaTime;
        //최대 시간을 넘지 않도록 고정 (게임 종료 등 처리에 활용)
        if (gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
    }
    
    //경험치 획득 및 레벨업 로직
    public void GetExp()
    {
        exp++;
        if (exp == nextExp[level])
        {
            level++;
            exp = 0;
            uiLevelUP.Show(); //레벨업 - 아이템 선택 UI 표시
            //초기 레벨 이상 초과시, 경험치 테이블을 추가하면서 최대 경험치를 복사
            if (level >= nextExp.Count)
            {
                nextExp.Add(nextExp[nextExp.Count - 1]);
            }
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }
    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }
}
