using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //적 이동 속도
    public float speed;
    //추적할 대상 물리 컴포넌트 target
    public Rigidbody2D target;
    //적의 생존여부
    private bool isAlive;
    //Enemy 본인 물리 컴포넌트
    Rigidbody2D rigid;
    //flipX를 통한 적 좌우 반전용
    SpriteRenderer spriter;

    private void Awake()
    {
        //미리 컴포넌트를 로드하여 메모리에 캐싱
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        //1 방향 구하기(목표위치 - 내위치) -> 플레이어쪽을 바라보는 벡터
        Vector2 dirVec = target.position - rigid.position;
        //2 이동량 구하기(방향 * 속도 * 프레임시간) - 정규화를 통해 거리와 관계없이 일정한 속도를 냄
        Vector2 nextVec = dirVec.normalized * (speed * Time.fixedDeltaTime);
        //3 객체 이동(현재 위치 + 이동량)
        rigid.MovePosition(rigid.position + nextVec);
        //4 잔여 속도 제거 - 플레이어랑 충돌하면, 물리 속도를 0으로
        rigid.linearVelocity = Vector2.zero;
    }
}
