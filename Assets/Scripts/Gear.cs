using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;  //기어 종류
    public float rate;              //강화 비율(연사력, 이동속도 증가율)

    public void Init(ItemData data)
    {
        //기본 정보
        name = "Gear " + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;
        
        //능력치
        type = data.type;
        rate = data.damages[0]; //0레벨 증가율(기어는 damages 강화능력치 비율로 사용중)

        ApplyGear();
    }

    public void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    //레벨업 호출 시 rate 갱신
    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }
    
    //장갑: 플레이어가 가진 무기 공격 속도 강화
    public void RateUp()
    {
        //부모(플레이어)의 모든 Weapon 가져와야 함
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0://근접무기 강화 시, -150 시계방향, 더 빠른 속도 = 더 큰 음수
                    weapon.speed = -150 + ((-150) * rate);
                    break;
                default://id=1 원거리 무기 강화 시, 연사 시간 간격, 더 빠른 연사 = 작은 값
                    weapon.speed = .5f * (1 - rate);
                    break;
            }
        }
    }
    //신발: 플레이어의 이동 속도(기본 3에 비율만큼 더함)
    public void SpeedUp()
    {
        float speed = 3 + 3 * rate;
        GameManager.instance.player.speed = speed;
    }
}
