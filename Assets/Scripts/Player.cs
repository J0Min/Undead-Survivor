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
    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();
    }
    
    //물리이동은 FixedUpdate에서 진행
    private void FixedUpdate()
    {
        //다음에 이동할 양 = 방향 * 속도 * 프레임시간
        Vector2 nextVec = inputVec.normalized * (speed * Time.fixedDeltaTime);
        //현재위지 + 이동방향
        rigid.MovePosition(rigid.position + nextVec);
    }
    
    //모든 업데이트가 끝난 뒤 수행 하는 LateUpdate, 방향 전환같은 후처리에 적합
    void LateUpdate()
    {
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
