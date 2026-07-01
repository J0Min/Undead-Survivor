using System;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData data;   //이 버튼이 담당할 아이템 데이터
    public int level;       //현재 아이템 강화 레벨
    public Weapon weapon;   //아이템이 만들거나 관리하는 무기
    public Gear gear;       //아이템을 관리하는 기어

    private Image icon;     //버튼에 표시할 아이템 아이콘
    private Text textLevel; //버튼에 표시할 아이템 레벨 텍스트
    private Text textName;  //버튼에 표시할 아이템 이름 텍스트
    private Text textDesc;  //버튼에 표시할 아이템 설명 텍스트

    void Awake()
    {
        //[0] - 버튼 자기 자신(버튼도 image)
        //[1] - 자식 아이콘
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;    //데이터의 아이콘 그림으로 세팅
        //버튼 속에 있는 Text들을 배열로 가져올 계층 구조 순서대로 0 레벨 1 이름 2 설명
        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
        
        //이름은 한번 정해지면 안바뀌므로 Awake에서 1회만 세팅
        textName.text = data.itemName;
    }

    //OnEnable 오브젝트가 활성화 되었을 때, 자동 호출 = 레벨업 창이 뜰때만 갱신
    private void OnEnable()
    {
        textLevel.text = "Lv. " + level;
        switch (data.type)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                //무기 - 0 데미지%, 1 갯수
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100, data.counts[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                //기어 - 0 증가율%
                textDesc.text = string.Format(data.itemDesc, data.damages[level] * 100);
                break;
            case ItemData.ItemType.Heal:
                //회복 - 인자 없음
                textDesc.text = string.Format(data.itemDesc);
                break;
        }
    }

    public void Onclick()
    {
        //아이템 종류별로 분기
        switch (data.type)
        {
            //근접/원거리는 같은 무기 로직 처리
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    //첫 선택: Weapon 컴포넌트를 붙여 무기를 설정해줘야 함
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(data);
                }
                else
                {
                    //재 선택: 다음 레벨 수치 계산 후 무기에 반영(강화)
                    //강화된 데미지(Damages) = 기본값 + (기본값 * 레벨별 증가율)
                    float nextDamage = data.baseDamage;
                    nextDamage += data.baseDamage * data.damages[level];
                    //갯수(Counts) = 레벨별 증가량
                    int nextCount = data.counts[level];
                    nextDamage += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    //재선택: 다음 레벨 비율로 장비아이템에 반영(강화)
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                break;
            case ItemData.ItemType.Heal:
                //회복: 체력 최대체력 회복(간단구현)
                GameManager.instance.health = GameManager.instance.maxHealth;
                break;
        }

        //회복은 일회성아이템으로 레벨업 제외
        if (data.type != ItemData.ItemType.Heal)
        {
            //아이템 레벨 증가
            level++;
        
            //최대 레벨에 도달시 (레벨 == 데이터 배열의 최대 길이) -> 더 못올리도록 비활성화
            if (level == data.damages.Length)
            {
                GetComponent<Button>().enabled = false;
            }
        }
    }
}
