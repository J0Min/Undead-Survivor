using UnityEngine;
using UnityEngine.PlayerLoop;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public PoolManager pool;
    //흐르는 게임 시간 - 난이도 계산용
    public float gameTime;
    //최대 게임 시간 - 난이도 증가 기준
    public float maxGameTime = 10f;
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
}
