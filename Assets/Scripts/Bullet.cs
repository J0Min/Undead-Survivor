using UnityEngine;

public class Bullet : MonoBehaviour
{
    //적에게 줄 데미지
    public float damage;
    //관통횟수(근접무기는 -1로 무한 관통)
    public int penetration;

    public void Init(float damage, int penetration)
    {
        this.damage = damage;
        this.penetration = penetration;
    }
}
