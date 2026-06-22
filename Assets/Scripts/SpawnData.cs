using UnityEngine;
[System.Serializable]
//이 클래스를 직렬화 대상으로 표시
//spawner의 배열로 인스펙터에 값을 입력 가능하도록 하기 위함
public class SpawnData
{
    //몬스터 종류
    public int spriteType;
    //스폰 간격(작을수록 자주 등장)
    public float spawnTime;
    //몬스터 체력
    public float health;
    //몬스터 이동속도
    public float speed;
}
