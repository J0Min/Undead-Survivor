using System;
using UnityEngine;
using Random = UnityEngine.Random;


//레벨업 선택창 컨트롤하는 클래스 - 창 on/off, 랜덤 아이템 배치
public class LevelUp : MonoBehaviour
{
    private RectTransform rect;
    private Item[] items;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    //창 보이기
    public void Show()
    {
        Next();//랜덤 3개 아이템 출제
        rect.localScale = Vector3.one;//1.1.1-> 보임
        GameManager.instance.Stop();//게임 일시정지
    }
    
    //창 숨기기
    public void Hide()
    {
        rect.localScale = Vector3.zero;//1.1.1-> 숨김
        GameManager.instance.Resume();//게임 재게
    }

    //버튼 클릭시 아이템에 위임 -> GameManager가 기본무기 지급에 사용
    public void Select(int index)
    {
        items[index].Onclick();
    }
        
    //강화할 아이템 5개 중 랜덤 3개만 활성화 (중복 없이) / 최대 레벨 도달시 회복 아이템으로 대체
    void Next()
    {
        //1.모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        
        //2.중복 없이 랜덤 아이템 3개 뽑기
        int[] ran = new int[3];
        while (true)
        {
            ran[0] = Random.Range(0, items.Length);
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);
            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])
                break;
        }
        
        //3.중복없이 뽑힌 3개만 켜기
        for (int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];
            
            //뽑힌 아이템 중 최대 레벨에 도달한 아이템은 회복 아이템으로 대체
            if (ranItem.level == ranItem.data.damages.Length)
            {
                items[items.Length - 1].gameObject.SetActive(true);//회복 아이템
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
        }
    }
}
