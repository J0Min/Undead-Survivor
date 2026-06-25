using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    //적 이동 속도
    public float speed;
    //현재 체력, 최대 체력
    public float health;
    public float maxHealth;
    
    //몬스터 종류별 애니메이션 컨트롤러 배열(Init 함수에서 spriteType으로 갈아끼움)
    public RuntimeAnimatorController[] animCon;
    //애니메이터
    Animator anim;
    //추적할 대상 물리 컴포넌트 target
    public Rigidbody2D target;
    //충돌 콜라이더 컴포넌트
    private Collider2D coll;
    //적의 생존여부
    private bool isAlive;
    //Enemy 본인 물리 컴포넌트
    Rigidbody2D rigid;
    //flipX를 통한 적 좌우 반전용
    SpriteRenderer spriter;
    //코루틴에서 쓸 "물리 프레임 대기"
    private WaitForFixedUpdate wait;
    
    
    private void Awake()
    {
        //미리 컴포넌트를 로드하여 메모리에 캐싱
        rigid = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }
    //OnEanble: 객체가 활성될 때마다 호출
    private void OnEnable()
    {
        //프리팹은 Player를 참조(활동)하지 못하도록
        //GameManager를 통해 매번 플레이어를 타겟으로 할당
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();
        
        //풀에서 다시 사용될때(죽음->재스폰)
        isAlive = true;
        health = maxHealth;
        //사망에서 바뀐 상태를 원복(재사용을 위함)
        isAlive = true;//이동, 반응 정지(FixedUpdate 필터용)
        coll.enabled = true;//시체 충돌 제거
        rigid.simulated = true;//물리 정지(밀리기 움직임 정지)
        spriter.sortingOrder = 2;//정렬을 내림
        anim.SetBool("Dead", false);//사망 애니메이션 재생을 위한 파라미터값 전달
    }
    private void FixedUpdate()
    {
        //죽은 상태(isAlive) 히트 상태(넉백 중)인 상태에는 아래 추적 이동을 멈춤
        if (!isAlive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))
        {
            return;
        }
        
        //1 방향 구하기(목표위치 - 내위치) -> 플레이어쪽을 바라보는 벡터
        Vector2 dirVec = target.position - rigid.position;
        //2 이동량 구하기(방향 * 속도 * 프레임시간) - 정규화를 통해 거리와 관계없이 일정한 속도를 냄
        Vector2 nextVec = dirVec.normalized * (speed * Time.fixedDeltaTime);
        //3 객체 이동(현재 위치 + 이동량)
        rigid.MovePosition(rigid.position + nextVec);
        //4 잔여 속도 제거 - 플레이어랑 충돌하면, 물리 속도를 0으로
        rigid.linearVelocity = Vector2.zero;
    }

    void LateUpdate()
    {
        spriter.flipX = target.position.x < transform.position.x;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //총알(Bullet 태그) 에 맞은 경우에 처리하기 위한 필터(벽,플레이어 등 무시)
        if (!collision.CompareTag("Bullet"))
        {
            return;
        }
        //충돌된 bullet 컴포넌트의 데미지만큼 체력 감소
        health -= collision.gameObject.GetComponent<Bullet>().damage;

        if (health > 0)
        {
            //피격 반응 추가
            anim.SetTrigger("Hit");//animator의 hit발동 -> hit 애니메이션 클립 재생
            StartCoroutine(KnockBack());
        }
        else
        {
            //Dead();
            isAlive = false;//이동, 반응 정지(FixedUpdate 필터용)
            coll.enabled = false;//시체 충돌 제거
            rigid.simulated = false;//물리 정지(밀리기 움직임 정지)
            spriter.sortingOrder = 1;//정렬을 내림
            anim.SetBool("Dead", true);//사망 애니메이션 재생을 위한 파라미터값 전달
        }
    }
    
    //적 사망 객체 Destroy가 아니라 오브젝트 풀에서 재사용을 위해 비활성화 처리
    void Dead()
    {
        gameObject.SetActive(false);
    }
    
    //난이도 데이터를 파라미터로 전달받아 적 객체 초기화
    public void Init(SpawnData data)
    {
        //종류에 맞는 애니메이션 교체
        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.speed;
        health = data.health;
        maxHealth = data.health;;
    }
    
    //KnockBack - 맞는 순간 물리적으로 밀려나는 코루틴
    IEnumerator KnockBack()
    {
        yield return wait;//다음 물리 프레임까지 1프레임을 대기
        //플레이어 반대 방향으로 = 내 위치(Enemy) - 플레이어 위치
        Vector3 playerPos = GameManager.instance.player.transform.position;
        Vector3 dirVec = transform.position - playerPos;
        //그 방향으로 순간 물리 충격(Impulse)를 가함
        rigid.AddForce(dirVec.normalized * 3,ForceMode2D.Impulse);
    }
}
