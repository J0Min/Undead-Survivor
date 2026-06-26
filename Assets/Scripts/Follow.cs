using System;
using UnityEngine;

public class Follow : MonoBehaviour
{
    private RectTransform rect;//UI 오브젝트 전용 위치, 크기 컴포넌트 (Transform UI 버전)

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Update()
    {
        //UI의 Canvas 스크린 좌표계를 사용, 따라서 변환 필요
        //Camera.main.WorldToScreenPoint
        rect.position = Camera.main.WorldToScreenPoint(GameManager.instance.player.transform.position);
    }
}
