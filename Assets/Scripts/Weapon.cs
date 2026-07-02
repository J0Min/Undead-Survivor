using System;
using Mono.Cecil;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;//무기 종류(0근접1원거리)
    public int prefabId;//풀매니저에 등록된 무기 인덱스(외형)
    public float damage;//데미지
    public int count;//칼날 갯수//근접 칼날 개수 / 원거리 관통 호횟수
    public float speed;//회전 속도//공격속도 회전속도
    
    private float timer; // 원거리 발사 타이머
    private Player player; // player.Scanner 대상에 접근하기 위함
    void Awake()
    {
        // 부모 객체인 Player 컴포넌트 가져오기 (Weapon은 Player 자식)
        player = GameManager.instance.player;
    }

    //Update - 매 프레임당 호출 - 입력, 회전 등 일반 로직
    //FixedUpdate - 물리 갱신 프레임당 호출 - Rigidbody 물리 이동 전용
    //LateUpdate - 위 업데이트들이 모두 실행 후 호출 - 카메라,방향 보정 등 후처리
    void Update()
    {
        if(!GameManager.instance.isLive)
            return;
        
        //무기 종류별 동작 분기
        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.forward * (speed * Time.deltaTime));
                break;
            case 1:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0;
                    Fire();
                }
                break;
            default: break;
        }
    }
    
    public void Init(ItemData data)
    {
        // 기본 정보 셋팅
        name = "Weapon " + data.itemId;         // 오브젝트 이름 (Weapon 1 이런식으로 나타나게)
        transform.parent = player.transform;    // 플레이어 자식으로 등록(따라다니게)
        transform.localPosition = Vector3.zero; // 플레이어 기준 원점 배치

        // 능력치 셋팅
        id = data.itemId;
        damage = data.baseDamage;
        count = data.baseCount;

        // 투사체 프리팹
        // 데이터안의 투사체 프리팹이 풀의 몇번째 인덱스인지 지정
        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }
        }
        
        // 무기 종류별 초기 세팅
        switch (id)
        {
            case 0:
                speed = -150; // (음수 = 시계방향)
                Arrange(); // 칼날 원형 배치
                break;
            case 1:
                speed = 0.3f; // 연사속도 (0.3초마다 발사)
                break;
            default:
                break;
        }
        
        //새 무기가 생성될 때, 이미 강화된 기어 효과(공속 등)가 이 무기에도 적용되도록 메시지 전달
        //받을 함수가 없는 경우 오류를 반환을 안하도록 DontRequireReceiver
        player.BroadcastMessage("ApplyDamage", SendMessageOptions.DontRequireReceiver);
        
        //무기 종류에 맞는 손을 켜고, 그 무기 스프라이트를 적용
        Hand hand = player.hands[(int)data.type];
        hand.spriter.sprite = data.hand;//손에 무기 모양 스프라이트를 입힘
        hand.gameObject.SetActive(true);//평소에 꺼저있는 손을 켬
    }

    public void LevelUp(float nextDamage, int nextCount)
    {
        this.damage = nextDamage;
        this.count = nextCount;
        //근접 무기는 칼날 개수 재배치
        if (id == 0)
        {
            Arrange();
        }
        //강화됙 무기 위에 기어 효과를 다시 입힘.
        player.BroadcastMessage("ApplyDamage", SendMessageOptions.DontRequireReceiver);
        
    }
    
    //칼날을 풀에서 꺼내서 플레이어 주위에 원형으로 균형 배치
    void Arrange()
    {
        
        //칼날 count 개를 풀에서 꺼냄
        for (int index = 0; index < count; index++)
        {
            //풀에서 칼날(Bullet) 꺼내어 Transform 확보
            Transform bullet;
            if (index < transform.childCount)
            {
                bullet = transform.GetChild(index);
            }
            else
            {
                bullet = GameManager.instance.pool.Get(prefabId).transform;//새 칼날
                bullet.parent = transform;//Weapon 자식 등록

            }
            
            //Weapon의 자식으로 설정하여 플레이어를 따라다니면서 회전
            bullet.parent = transform;
            //부모 기준 위치, 회전 초기화(재사용 시 이전값 제거)
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;
            //index마다 균등 각도로 회전(360/count 간격)
            Vector3 rotVec = Vector3.forward * 360 * index / count;
            bullet.Rotate(rotVec);
            //회전된 위 방향으로 1.5 바깥으로 위치(월드기준)
            bullet.Translate(bullet.up * 1.5f,Space.World);
            bullet.GetComponent<Bullet>().Init(damage, -1, Vector3.zero);
        }
    }
    //가장 가까운 적을 정해서 총알 1개 발사
    void Fire()
    {
        //조준 대상이 없는 경우를 필터링
        if (player.scanner.nearestTarget == null)
            return;
        //대상 방향을 계산
        Vector3 targetPos = player.scanner.nearestTarget.position;
        Vector3 dir = (targetPos - transform.position).normalized;
        //풀 매니저에서 총아르 꺼내 위치, 회전 세팅(대상을 바라보도록)
        Transform bullet = GameManager.instance.pool.Get(prefabId).transform;
        bullet.position = transform.position;
        bullet.rotation = Quaternion.FromToRotation(Vector3.up, dir); //A파라미터의 방향을 B방향으로 회전값 -> 총알의 up(위쪽) 방향이 적을 향하게 함
        
        //Init 값 주입
        bullet.GetComponent<Bullet>().Init(damage, count, dir);
    }
}
