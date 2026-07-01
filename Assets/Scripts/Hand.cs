using System;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public bool isLeft;//왼손 여부(왼손-근접 / 오른손-원거리)
    public SpriteRenderer spriter;//이 손의 스프라이트

    private SpriteRenderer player;//플레이어 스프라이트(반전 여부, FlipX 확인용)
    
    //오른손(원거리)는 위치로 표현 - 정상/반전 두가지
    Vector3 rightPos = new Vector3(0.35f, -0.015f, 0);
    Vector3 rightPosReverse = new  Vector3(-0.15f, -0.015f, 0);
    
    //왼손(근접)은 회전으로 표현 - 정상/반전 두가지
    Quaternion leftRot = Quaternion.Euler(0, 0, -35);
    Quaternion leftRotReverse = Quaternion.Euler(0, 0, -130);

    private void Awake()
    {
        //부모 방향으로 SpriteRenderer를 가져오면 [0]내손, [1]부모(플레이어)
        //플레이어의 반전 상태를 가져와야 함
        player = GetComponentsInParent<SpriteRenderer>()[1];
    }

    private void LateUpdate()
    {
        //플레이어 캐릭터의 이동, 반전 처리가 끝나면 손의 위치, 회전을 보강
        bool isReverse = player.flipX;//왼쪽이 True
        if (isLeft)
        {
            //왼손(근접 무기) - 회전으로 자세 표현
            transform.localRotation = isReverse ? leftRotReverse : leftRot;
            spriter.flipY = isReverse;//플레이어 반전에 따라서 Y축 반전
            spriter.sortingOrder = isReverse ? 4 : 6;//화면에 보이는 우선순위(반전 상태 캐릭터 5 _ 뒤, 정상시 6 _ 앞)
        }
        else
        {
            //오른손 - 원거리 무기 : 위치로 자세 표현
            transform.localPosition = isReverse? rightPosReverse : rightPos;
            spriter.flipX = isReverse;//플레이어 반전에 따라서 X축 반전
            spriter.sortingOrder = isReverse ? 6 : 4;
        }
    }
}
