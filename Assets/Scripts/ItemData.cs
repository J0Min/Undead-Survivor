using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
//fileName : 새로 만들때 기본 파일 이름
//menuName : Create 메뉴 내의 표시된 경로
public class ItemData : ScriptableObject
{
    //아이템 경로
    public enum ItemType{Melee, Range, Glove, Shoe, Heal}

    public ItemType type; //선택된 아이템의 종류

}
