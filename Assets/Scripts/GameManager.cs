using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("# Game Objects")]
    public Player player;
    public PoolManager pool;
    public LevelUp uiLevelUP;
    
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
    public int health;
    public int maxHealth = 100;


    
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        isLive = true;
        uiLevelUP.Select(0);
        health = maxHealth;
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
