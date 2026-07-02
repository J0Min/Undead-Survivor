using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    //방향 키 입력 받는 벡터 (x좌우 ,y상하)
    public Vector2 inputVec;
    
    //이동 속도
    public float speed;
    
    //가장 가까운 적을 찾는 스캐너
    public Scanner scanner;
    
    //객체가 가진 물리 컴포넌트
    Rigidbody2D rigid;
    
    //스프라이트 렌더러 컴포넌트
    private SpriteRenderer sprite;
    
    //애니메이션 상태 제어 컴포넌트
    Animator anim;
    
    //무기를 드는 양손(왼/오)
    public Hand[] hands;
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
        //자식의 Hand들을 가져올때, true-> 비활성 손도 포함해서 가져오게 됨
        //손은 평소엔 비활성 상태였다가 무기가 생기면 활성, 비활성 상태로 미리 배열에 담기 위함
        hands = GetComponentsInChildren<Hand>(true);
    }
    
    //물리이동은 FixedUpdate에서 진행
    private void FixedUpdate()
    {
        if(!GameManager.instance.isLive)//일시정지 샅애에서 이동 중단
            return;
        
        //다음에 이동할 양 = 방향 * 속도 * 프레임시간
        Vector2 nextVec = inputVec.normalized * (speed * Time.fixedDeltaTime);
        //현재위지 + 이동방향
        rigid.MovePosition(rigid.position + nextVec);
    }
    
    //모든 업데이트가 끝난 뒤 수행 하는 LateUpdate, 방향 전환같은 후처리에 적합
    void LateUpdate()
    {
        if(!GameManager.instance.isLive)
            return;
        
        if (inputVec.x != 0)
        {
            sprite.flipX = inputVec.x < 0;
        }
        anim.SetFloat("Speed", inputVec.magnitude);//입력 벡터의 크기
    }
    
    void OnMove(InputValue value)
    {
        inputVec = value.Get<Vector2>();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), inputVec.ToString());
    }

    //OnCollisonStay2D - 충돌이 지속되는 동안 매 프레임 호출
    private void OnCollisionStay2D(Collision2D other)
    {
        if (!GameManager.instance.isLive) //일시 정지 상태에는 중단
            return;
        //프레임과 무관하게 1초당 일정량 감소하도록
        GameManager.instance.health -= 10 * Time.deltaTime;

        if (GameManager.instance.health <= 0)
        {
            //자식 오브젝트를 돌면서 2번부터 비활성화
            for (int index = 2; index < transform.childCount; index++)
            {
                transform.GetChild(index).gameObject.SetActive(false);
            }
            
            anim.SetTrigger("Dead");//묘비 애니메이션 수행
            GameManager.instance.GameOver();
        }
    }
}
