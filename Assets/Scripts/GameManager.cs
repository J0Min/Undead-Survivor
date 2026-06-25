using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    [Header("# Game Objects")]
    public Player player;
    public PoolManager pool;
    
    [Header("# Game Controls")]
    public float gameTime;//흐르는 게임 시간 - 난이도 계산용
    public float maxGameTime; //최대 게임 시간 - 난이도 증가 기준

    [Header("# Player Data")]
    public int level;
    public int kill;
    public int exp;
    public int[] nextExp = { 3, 5, 10, 20, 150, 210, 280, 360, 450, 600 };
    
    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
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
        }
    }
}
