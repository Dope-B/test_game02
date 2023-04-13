# 연속공격 기능 구현용 테스트 게임

#### 사용 기술: Unity(2020.2.2f1)
#### 제작 기간: 2020.02.25~2020.04.27

<p align="center">
<img width="30%" src="https://user-images.githubusercontent.com/33209821/230086327-cbce8a54-8c4f-4a14-b35c-3a5dbc0ff571.png"/>
<img width="30%" src="https://user-images.githubusercontent.com/33209821/230086333-82e8a713-c6df-431d-815f-0d33f4caaae7.png"/>
<img width="30%" src="https://user-images.githubusercontent.com/33209821/230086341-72810531-a471-47c6-bb94-358befbc58ed.png"/>
<img width="30%" src="https://user-images.githubusercontent.com/33209821/230086347-b9afad1c-48dd-4121-8cb6-9ef536cd8633.png"/>
<img width="30%" src="https://user-images.githubusercontent.com/33209821/230086352-3fa39803-8abb-43db-801a-f19b03e82963.png"/>
</p>



#### 설명
- 플로팅 텍스트 출력은 화질깨짐을 방지하기 위해 TextMeshPro를 사용했다.(생성 시 상단으로 이동하며 일정 시간 후 삭제)
- 4 개의 캐릭터 중 하나를 선택 할 수 있다.
<img width="50%" src="https://user-images.githubusercontent.com/33209821/230110480-de0ee3f4-474b-49a0-92f4-ef2d34fe897b.gif"/>
- 플레이어와 몬스터의 애니메이터는 다음과 같다.

<p>
<img width="45%" src="https://user-images.githubusercontent.com/33209821/230091893-801b6d2d-af97-44f8-80a9-4154fae88b65.png"/>
<img width="45%" src="https://user-images.githubusercontent.com/33209821/230091897-62dd47d3-64f8-42fa-9150-a8e79a11fee8.png"/>
</p>

- 연속공격을 실행하기 위해 연속된 모션을 가진 공격 애니메이션 실행 시<br/> 공격 키를 한번 더 누르면 트리거를 발생시켜서 바로 다음 공격 모션으로 이어지게 구현

```C#
void attack()// 피격됐다면 트리거 발동 불가
{
   if (Input.GetKeyDown(KeyCode.Z) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt")) { animator.SetTrigger("attack1"); }
   else if (Input.GetKeyDown(KeyCode.X) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt")) { animator.SetTrigger("attack2"); }
   else if (Input.GetKeyDown(KeyCode.A) && ani_check()) { animator.SetTrigger("attack3"); }
   else if (Input.GetKeyDown(KeyCode.S) && ani_check() && !animator.GetCurrentAnimatorStateInfo(0).IsName("jump_down")) { animator.SetTrigger("attack4"); }
   else if (Input.GetKeyDown(KeyCode.D) && ani_check()) { animator.SetTrigger("attack5"); }
   }

```

- 공중 공격, 대쉬 공격, 앉아있을 때 공격 모션이 있다.

<img width="50%" src="https://user-images.githubusercontent.com/33209821/230109152-d0e44d08-985e-41ec-8f61-a17a16de50fe.gif"/>

- 플레이어 점프와 달리기 시 먼지 이펙트가 출력된다.
- 대쉬, 피격 모션 중 플레이어는 무적 상태가 된다.
- 특정 스킬 사용중에서 플레이어는 데미지는 받지만 피격 상태가 되지 않는다.
- 플레이어 hp가 0 이하로 내려갈 시 캐릭터 선택창으로 맵이 이동된다.

- 모든 타격은 다음과 같은 속성을 가진다.

```C#
public class attack_range:MonoBehaviour
{
    public int damage;// 데미지
    public float force_up;// y축 방향 힘(높을 시 타격 당한다면 띄워진다)
    public float force_back;// x축 방향 힘
    public GameObject hit1;// 타격 이펙트
    public GameObject hit2;
    public GameObject slash;// 베기 전용 타격 이펙트
    public bool isSlash;// 베기 여부
    public bool shakeable;// 타격 시 화면 흔들림 여부
}
```

- 몬스터의 hp바는 타격 시 일정 시간동안 출력된다.

```C#
void FixedUpdate()
    {
        if (timer <= 0) { timer = 0;  this.gameObject.SetActive(false); }// 일정 시간이 지난다면 비활성화
        else { timer -= Time.deltaTime; }
        hp_bar.fillAmount = (mob.cur_HP / mob.max_HP);// 현재 채력 
        if (de_hp_bar.fillAmount > hp_bar.fillAmount)// 채력 소모 시 부드러운 연출을 위함
        {
            de_hp_bar.fillAmount -= 0.002f;// 현재 체력에 맞게 조금씩 줄어듦
        }
        if (de_hp_bar.fillAmount <= 0f && hp_bar.fillAmount <= 0) { this.gameObject.SetActive(false); }// 둘 다 0 이하라면 비활성화
    }

```

- 몬스터 공격 패턴 중 독립 오브젝트를 소환하는 패턴은 페이드 인 효과와 함께 생성되며 플레이어 피격 판정 또한 판별한다.<br/> 플레이어가 오브젝트를 보는 방향으로 가드 상태거나 무적 상태일 시 를 제외하고 데미지를 가한다.(summon_skill에 구현)
- 몬스터 ai는 일반 몹, 이동불가 몹, 추적기능을 가진 몹으로 나뉜다.(2,3번째 ai는 일반몹을 상속받음)

<img width="50%" src="https://user-images.githubusercontent.com/33209821/230109184-2f455153-9f0a-434b-a159-c118c2a70e10.gif"/>

- 몬스터 ai는 기본적으로 여러 패턴들을 만들어둔 뒤 랜덤으로 패턴을 실행하는 형태이다.
- 몬스터의 상태는 총 5개로 idle, trace, attack, hurt, die가 있다.
- idle mode는 플레이어를 인식하지 못 했을 때를 의미하고 trace mode는 플레이어를 인식 했을 때를 의미하고 attack mode는 공격 중 일 때를 의미한다.
- trace mode는 몹이 바라보는 방향으로 일정 거리 이하로 플레이어가 있을 시 활성화 된다.
- 각 모드에 따라 다음에 올 패턴의 확률이 다르다.
<img width="50%" src="https://user-images.githubusercontent.com/33209821/230109161-fd46cd8c-92a8-4e80-9439-b117728dbf86.gif"/>
- 각 패턴이 끝난다면 다음 패턴 사이에 쿨타임이 있는데 쿨타임 또한 모드에 따라 랜덤으로 결정된다.

```C#
if (attack_mode) { yield return new WaitForSeconds(Random.Range(min_delay * 0.65f, (max_delay + 1) * 0.65f)); }
else if (trace_mode) { yield return new WaitForSeconds(Random.Range((min_delay * 0.8f), (max_delay + 1) * 0.8f)); }
else { yield return new WaitForSeconds(Random.Range(min_delay, max_delay + 1)); }

```
- 일반적인 공격 패턴은 다음과 같다.

```C#
ran = Random.Range(1, (100 / attack_way) * attack_way);// 공격 패턴 수에 따라 랜덤한 패턴 실행 
for (int i = 1; i <= attack_way; i++)
    {
        if (ran >= (100 / attack_way) * (i - 1) && ran < (100 / attack_way) * i)
        {
           ani.SetTrigger("attack" + i.ToString());
           break;
        }
     }

```

- 몬스터는 고유의 경직치를 가지고 경직치 이상의 누적 데미지를 받았을 시 피격 모션이 출력된다.

<img width="50%" src="https://user-images.githubusercontent.com/33209821/230109142-b801f22b-c60a-4b91-b6da-d8338433ca40.gif"/>

- 플레이어가 가드 성공 시 x방향으로 -4만큼 힘을 받는다. 그리고 방어 전용 이펙트를 출력한다.(이펙트 출력 위치는 랜덤)

<img width="50%" src="https://user-images.githubusercontent.com/33209821/230109168-ae31ce00-29b4-44db-ba18-d81562980dac.gif"/>

- 플레이어 스킬 중 적을 추적하여 순간이동 후 뒤에서 공격하는 스킬은 레이캐스트를 사용하여 구현하였다.

<img width="50%" src="https://user-images.githubusercontent.com/33209821/230113528-ecb0e71c-6b83-4287-8784-0001fdc9875c.gif"/>

```C#
    void trans_to_enemy()
    {
        RaycastHit2D ray;
        ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + box.size.y / 2),// 플레이어 피벗이 바닥에 위치하기에 가운데로 맞춤
                                new Vector2(-1,0)*transform.localScale.x, 20f, 1 << 8);// 20f 범위 내 적 탐지
        if (ray)
        {
            transform.position = new Vector2(ray.transform.position.x + (ray.transform.localScale.x * 3f),//// 몬스터의 뒤로 이동해야 하기에 몬스터의 localScale 접근
                                              transform.position.y);
            transform.localScale = ray.transform.localScale;// 몬스터 방향으로 플레이어 방향 세팅 후 공
        }
    }

```


#### 피드백 
- 독립 오브젝트 스크립트에 플레이어 피격 판정을 넣어서 코드가 복잡해짐
- 리소스가 너무 많아서 관리가 힘들고 이유는 모르겠지만 빌드가 안 된다.
- 실행에는 문제가 없는 것 같지만 TextMeshPro에 오류가 있는 것 같다.
- 공중 피격 시 애니메이션 출력이 어색한 경우가 있다.


#### 리소스 출처
  https://www.spriters-resource.com/xbox_360/persona4arenaultimax/
