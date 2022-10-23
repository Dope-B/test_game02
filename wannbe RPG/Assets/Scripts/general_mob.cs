using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class general_mob : MonoBehaviour
{
    public GameObject DMGfont;
    protected Rigidbody2D rigid;
    protected BoxCollider2D boxC;
    protected Animator ani;
    protected player_movement player;
    public GameObject[] effect;
    public Sprite portrait;
    protected RaycastHit2D trace_ray;
    protected RaycastHit2D wall_ray;
    protected bool trace_mode;
    protected bool attack_mode;
    protected bool timer_on = false;
    public bool is_hurt;
    protected int ran;
    protected bool LOR;
    public bool is_freezeY = false;
    public bool is_ground = true;
    public float speed;
    public float trace_arrange;
    public float attack_arrange;
    public float max_HP;
    public float cur_HP;
    float ray_gap_y;
    public float min_delay;
    public float max_delay;
    public int stuck_line;
    public int attack_way;
    protected int cur_stuck_amount;
    public float hurt_delay;
    protected int cur_state = 1;// 1 ->idle     2 -> attack     3 -> move    4 -> hurt    5->die 
    // Start is called before the first frame update
    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
        player = FindObjectOfType<player_movement>();
        boxC = GetComponent<BoxCollider2D>();
        Random.InitState((int)Time.time);
        cur_HP = max_HP;
        if (is_freezeY) { ray_gap_y = 5.5f; } else { ray_gap_y = 2.5f; }
    }
    protected virtual void Update()
    {
        limitPos();
        die_check();
        if (!is_hurt && !ani.GetCurrentAnimatorStateInfo(0).IsName("die")&&!ani.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            trace_mode_check();
            if (timer_on)// actual behavior
            {
                switch (cur_state)
                {
                    case 1:
                        ani.SetBool("isMoving", false);
                        break;
                    case 2:
                        break;
                    case 3:
                        if (trace_mode || attack_mode)
                        {
                            if (ran <= 80) { move(1); /*Debug.Log("move_1ta");*/ }
                            else if (ran > 80 && ran <= 90) { move(2); /*Debug.Log("move_2ta");*/ }
                            else { move(3); /*Debug.Log("move_3ta");*/ }
                        }
                        else
                        {
                            if (ran <= 20) { move(1); /*Debug.Log("move_1n");*/ }
                            else if (ran > 20 && ran <= 60) { move(2); /*Debug.Log("move_2n");*/ }
                            else { move(3); /*Debug.Log("move_3n");*/ }
                        }
                        break;
                }
            }
            else// set state
            {
                if (is_ground && !ani.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
                {
                    ran = Random.Range(1, 101);
                    timer_on = true;
                    //Debug.Log("pattern_setting");
                    //Debug.Log("pre_pattern:" + cur_state);
                    switch (cur_state)
                    {
                        case 1:
                            idle_pattern();
                            break;
                        case 2:
                            if (attack_mode)
                            {
                                if (ran <= 60) { cur_state = 2; /*Debug.Log("attack to attack 1"); */ }
                                else if (ran > 60 && ran <= 85) { cur_state = 3; /*Debug.Log("attack to move 1"); */ }
                                else { cur_state = 1; /*Debug.Log("attack to idle 1"); */ }
                            }
                            else if (trace_mode)
                            {
                                if (ran <= 20) { cur_state = 2; /*Debug.Log("attack to attack 2"); */ }
                                else if (ran > 20 && ran <= 70) { cur_state = 3; /*Debug.Log("attack to move 2"); */ }
                                else { cur_state = 1; /*Debug.Log("attack to idle 2"); */ }
                            }
                            else
                            {
                                if (ran <= 20) { cur_state = 2; /*Debug.Log("attack to attack");*/ }
                                else if (ran > 20 && ran <= 50) { cur_state = 3; /*Debug.Log("attack to move");*/ }
                                else { cur_state = 1; /*Debug.Log("attack to idle");*/ }
                            }
                            break;
                        case 3:
                            move_pattern();
                            break;
                    }
                    if (cur_state == 2) { attack_pattern(); }
                    else if (cur_state == 3) { focus(); }
                    else if (cur_state == 1) { ani.SetBool("isMoving", false); }
                    //Debug.Log("new pattern:" + cur_state);
                    StartCoroutine(pattern_cool_down());
                }
            }
        }
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (rigid.velocity.y <= 0f)
        {
            if (collision.collider.tag == "ground")
            {
                rigid.velocity = Vector2.zero;
                ani.enabled = true; is_hurt = false; is_ground = true; ani.SetBool("isMoving", false);
            }
        }
    }
    protected void OnCollisionStay2D(Collision2D collision)
    {

    }
    protected void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "player_attack_range")
        {
            mob_hurt(collision.gameObject.GetComponent<attack_range>().damage, collision.gameObject.GetComponent<attack_range>().force_back,
                collision.gameObject.GetComponent<attack_range>().force_up,
                collision.gameObject.GetComponent<attack_range>().isSlash, collision.gameObject.GetComponent<attack_range>().shakeable,
                collision.gameObject.GetComponent<attack_range>().hit1,
                collision.gameObject.GetComponent<attack_range>().hit2, collision.gameObject.GetComponent<attack_range>().slash);
        }

    }
    protected IEnumerator pattern_cool_down()
    {
        //Debug.Log("pattern_cooling");
        if (cur_state == 2) { yield return new WaitForSeconds(Random.Range(ani.GetCurrentAnimatorStateInfo(0).length + (min_delay * 0.15f), ani.GetCurrentAnimatorStateInfo(0).length + (max_delay + 1) * 0.2f)); }
        else
        {
            if (attack_mode) { yield return new WaitForSeconds(Random.Range(min_delay * 0.65f, (max_delay + 1) * 0.65f)); }
            else if (trace_mode) { yield return new WaitForSeconds(Random.Range((min_delay * 0.8f), (max_delay + 1) * 0.8f)); }
            else { yield return new WaitForSeconds(Random.Range(min_delay, max_delay + 1)); }
        }
        timer_on = false; /*Debug.Log("pattern_cooling_done");*/
    }

    IEnumerator hurt_cool_down()
    {
        yield return new WaitForSeconds(hurt_delay);
        if (is_ground)
        {
            is_hurt = false;
            ani.enabled = true;
            timer_on = false;
        }
    }
    protected virtual void idle_pattern() { }
    protected virtual void attack_pattern()
    {
        focus();
        ani.SetBool("isMoving", false);
        ran = Random.Range(1, (100 / attack_way) * attack_way);
        for (int i = 1; i <= attack_way; i++)
        {
            if (ran >= (100 / attack_way) * (i - 1) && ran < (100 / attack_way) * i)
            {
                ani.SetTrigger("attack" + i.ToString());
                break;
            }
        }
        ran = Random.Range(0, 101);
    }
    protected virtual void move_pattern() { }
    public void mob_hurt(int damage, float back, float up, bool hit_type,bool shake, GameObject hit1, GameObject hit2, GameObject slash)
    {
        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("die"))
        {
            focus();
            if (shake) { StartCoroutine(cam_manager.cam.shake(0.2f, 1.5f)); }
            if (up == 0 || is_freezeY)
            {
                if (!is_ground) { push_up(5); }
                else
                {
                    cur_stuck_amount += damage;
                    if (cur_stuck_amount >= stuck_line)
                    {
                        cur_stuck_amount = 0;
                        ani.SetTrigger("hurt");
                        rigid.velocity = Vector2.zero;
                        is_hurt = true;
                        push_X(-back);
                        StartCoroutine(hurt_cool_down());
                    }
                }
            }
            else
            {
                cur_stuck_amount += damage;
                if (ani.GetCurrentAnimatorStateInfo(0).IsName("idle") ||
              ani.GetCurrentAnimatorStateInfo(0).IsName("move") ||
              ani.GetCurrentAnimatorStateInfo(0).IsName("hurt") ||
                (cur_stuck_amount >= stuck_line))
                {
                    cur_stuck_amount = 0;
                    if (!ani.GetCurrentAnimatorStateInfo(0).IsName("hurt") && ani.enabled)
                    {
                        ani.SetTrigger("hurt");
                        is_hurt = true;
                    }
                    if (is_ground)
                    {
                        rigid.velocity = Vector2.zero;
                        push_X(-back);
                        push_up(up);
                        StopCoroutine(pattern_cool_down());
                    }
                    else { push_up(up / 2); }
                }
            }
            if (hit_type)
            {
                Instantiate(slash, new Vector2(Random.Range(transform.position.x - 0.2f, transform.position.x + 0.2f),
                                              Random.Range(transform.position.y + (boxC.size.y / 2) - 0.3f, transform.position.y + (boxC.size.y / 2) + 0.3f)),
                                              Quaternion.Euler(0, 0, Random.Range(0, 180)));
            }
            else
            {
                int i = Random.Range(0, 2);
                if (i == 0)
                {
                    Instantiate(hit1, new Vector2(Random.Range(transform.position.x - 0.2f, transform.position.x + 0.2f),
                                  Random.Range(transform.position.y + (boxC.size.y / 2) - 0.2f, transform.position.y + (boxC.size.y / 2) + 0.2f)),
                                  Quaternion.identity);
                }
                else
                {
                    Instantiate(hit2, new Vector2(Random.Range(transform.position.x - 0.2f, transform.position.x + 0.2f),
                                         Random.Range(transform.position.y + (boxC.size.y / 2) - 0.2f, transform.position.y + (boxC.size.y / 2) + 0.2f)),
                                         Quaternion.identity);
                }
            }
            GameObject DF = Instantiate(DMGfont, new Vector2(transform.position.x, transform.position.y + boxC.size.y + 0.8f), Quaternion.identity);
            DF.GetComponent<floating_text>().damage = damage.ToString();
            DF.GetComponent<floating_text>().type = false;
            cur_HP -= damage;
            die_check();
            if (!cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>(true).gameObject.activeInHierarchy)
            {
                cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>(true).gameObject.SetActive(true);
            }
            if (cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>().mob != this)
            {
                cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>().mob = this;
                cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>().hp_bar.fillAmount = this.cur_HP / this.max_HP;
                cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>().de_hp_bar.fillAmount = this.cur_HP / this.max_HP;
                cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>().img.sprite = portrait;
            }
            cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>().timer = 5f;
        }
    }
    void die_check()
    {
        if (cur_HP <= 0) { 
            if (!ani.enabled) { ani.enabled = true; }
            if (!ani.GetCurrentAnimatorStateInfo(0).IsName("die")) { ani.SetTrigger("isDie"); }
            
        }
    }
    protected void focus()
    {
        if (transform.position.x <= player.transform.position.x) { LOR = true; transform.localScale = new Vector3(-1, 1, 1); }
        else { LOR = false; transform.localScale = new Vector3(1, 1, 1); }
    }
    protected void trace_mode_check()
    {
        if (transform.localScale.x == -1)
        {
            trace_ray = Physics2D.Raycast(new Vector2(transform.position.x - attack_arrange, transform.position.y + boxC.size.y / ray_gap_y), Vector2.right, trace_arrange + attack_arrange, 1 << 9);
            //Debug.DrawRay(new Vector2(transform.position.x - attack_arrange, transform.position.y + boxC.size.y / ray_gap_y), Vector2.right * (trace_arrange + attack_arrange), Color.red, 0.1f);
            //Debug.DrawRay(new Vector2(transform.position.x - attack_arrange, transform.position.y + boxC.size.y / ray_gap_y), Vector2.right * (attack_arrange + attack_arrange), Color.blue, 0.1f);

        }
        else
        {
            trace_ray = Physics2D.Raycast(new Vector2(transform.position.x + attack_arrange, transform.position.y + boxC.size.y / ray_gap_y), Vector2.left, trace_arrange + attack_arrange, 1 << 9);
            //Debug.DrawRay(new Vector2(transform.position.x + attack_arrange, transform.position.y + boxC.size.y / ray_gap_y), Vector2.left * (trace_arrange + attack_arrange), Color.red, 0.1f);
            //Debug.DrawRay(new Vector2(transform.position.x + attack_arrange, transform.position.y + boxC.size.y / ray_gap_y), Vector2.left * (attack_arrange + attack_arrange), Color.blue, 0.1f);
        }
        if (trace_ray)
        {
            if (trace_ray.distance < attack_arrange * 2)
            {
                if (!attack_mode)
                {
                    attack_mode = true; trace_mode = false; cur_state = 2;
                    StopCoroutine(pattern_cool_down()); timer_on = false;
                }
            }
            else { if (!trace_mode) { attack_mode = false; trace_mode = true; ran = Random.Range(1, 101); } }
        }
        else { trace_mode = false; attack_mode = false; }
    }
    protected void move(int type)
    {
        if (!ani.GetCurrentAnimatorStateInfo(0).IsTag("attack") && ani.enabled)
        {
            ani.SetBool("isMoving", true);
            switch (type)
            {
                case 1:
                    if (LOR) { transform.position += new Vector3(speed, 0) * Time.deltaTime; }
                    else { transform.position += new Vector3(-speed, 0) * Time.deltaTime; }
                    break;
                case 2:
                    
                    transform.localScale = new Vector3(-1, 1, 1);
                    transform.position += new Vector3(speed, 0) * Time.deltaTime;
                    break;
                default:
                    transform.localScale = new Vector3(1, 1, 1);
                    transform.position += new Vector3(-speed, 0) * Time.deltaTime;
                    break;
            }
        }
        else { ani.SetBool("isMoving", false); }
    }
    public void limitPos()
    {
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, cam_manager.cam.bound.bounds.min.x + 1f, cam_manager.cam.bound.bounds.max.x - 1f),
                                    Mathf.Clamp(transform.position.y, cam_manager.cam.bound.bounds.min.y, cam_manager.cam.bound.bounds.max.y));
    }
    bool wall_check(bool LR)
    {
        if (LR) {  wall_ray = Physics2D.Raycast(new Vector2(transform.position.x + (boxC.size.x / 2), transform.position.y + (boxC.size.y / 2)), Vector2.right, 1f, 1 << 10); }
        else { wall_ray = Physics2D.Raycast(new Vector2(transform.position.x - (boxC.size.x / 2), transform.position.y + (boxC.size.y / 2)), Vector2.left, 1f, 1 << 10); }
        if (wall_ray) { return true; }
        else { return false; }
    }
    void push_up(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * power, ForceMode2D.Impulse);
        is_ground = false;
    }
    void push_down(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.down * power, ForceMode2D.Impulse);
    }
    void push_X(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        if (transform.localScale.x == -1)
        { rigid.AddForce(Vector2.right * power, ForceMode2D.Impulse); }
        else { rigid.AddForce(Vector2.left * power, ForceMode2D.Impulse); }
    }
    void velo_reset(int i)
    {
        if (i == 0) { rigid.velocity = Vector2.zero; }
        else if (i == 1) { rigid.velocity = new Vector2(rigid.velocity.x * 0.3f, rigid.velocity.y); }
        else if (i == 2) { rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y * 0.3f); }
        else if (i == 3) { rigid.velocity = new Vector2(rigid.velocity.x * 0.3f, rigid.velocity.y * 0.3f); }
    }
    void ani_stop() { ani.enabled = false; }
    void cast_effect(int type)
    {
        switch (type)
        {
            case 1:
                if (transform.localScale.x == -1) { Instantiate(effect[0], new Vector2(transform.position.x, transform.position.y + 1f), Quaternion.Euler(0, 180, 0)); }
                else { Instantiate(effect[0], new Vector2(transform.position.x, transform.position.y + 1f), Quaternion.Euler(0, 0, 0)); }
                break;
            case 2:
                if (transform.localScale.x == -1) { Instantiate(effect[1], new Vector2(transform.position.x - 5f, transform.position.y + 5f), Quaternion.Euler(0, 180, 0)); }
                else { Instantiate(effect[1], new Vector2(transform.position.x + 5f, transform.position.y + 5f), Quaternion.Euler(0, 0, 0)); }
                break;
            case 3:
                if (transform.localScale.x == -1) { Instantiate(effect[2], new Vector2(transform.position.x, transform.position.y + 8f), Quaternion.Euler(0, 180, 0)); }
                else { Instantiate(effect[2], new Vector2(transform.position.x, transform.position.y + 4f), Quaternion.Euler(0, 0, 0)); }
                break;
            case 4:
                if (transform.localScale.x == -1) { Instantiate(effect[3], new Vector2(transform.position.x - 5f, transform.position.y + 5f), Quaternion.Euler(0, 180, 0)); }
                else { Instantiate(effect[3], new Vector2(transform.position.x + 5f, transform.position.y + 5f), Quaternion.Euler(0, 0, 0)); }
                Instantiate(effect[4], new Vector2(player.transform.position.x, player.transform.position.y + 25f), Quaternion.Euler(0, 0, 90));
                break;
            case 5:
                if (transform.localScale.x == -1) { Instantiate(effect[0], new Vector2(transform.position.x, transform.position.y + 3.8f), Quaternion.Euler(0, 180, 0)); }
                else { Instantiate(effect[0], new Vector2(transform.position.x, transform.position.y + 3.8f), Quaternion.Euler(0, 0, 0)); }
                break;
            case 6:
                if (transform.localScale.x == -1) { Instantiate(effect[0], new Vector2(transform.position.x, transform.position.y + 3f), Quaternion.Euler(0, 180, 0)); }
                else { Instantiate(effect[0], new Vector2(transform.position.x, transform.position.y + 3f), Quaternion.Euler(0, 0, 0)); }
                break;
            case 7:
                Instantiate(effect[0], new Vector2(player.transform.position.x, transform.position.y + 20f), Quaternion.identity);
                break;
            default:
                StartCoroutine(effect_random_summon());
                break;
        }
    }
    IEnumerator effect_random_summon()
    {
        for (int i = 0; i < Random.Range(2, 7); i++)
        {
            if (transform.localScale.x == -1) { Instantiate(effect[5], new Vector2(transform.position.x - Random.Range(1, 7), transform.position.y + Random.Range(3, 10)), Quaternion.Euler(0, 180, 0)); }
            else { Instantiate(effect[5], new Vector2(transform.position.x + Random.Range(1, 7), transform.position.y + Random.Range(3, 10)), Quaternion.Euler(0, 0, 0)); }
            yield return new WaitForSeconds(0.25f);
        }
    }
    IEnumerator effect_lighting()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(effect[0], new Vector2(player.transform.position.x, transform.position.y + 20f), Quaternion.identity);
            yield return new WaitForSeconds(0.8f);
        }

    }

    void destroy_ob() { Destroy(this.gameObject); }
    void cam_shake(float mag) { StartCoroutine(cam_manager.cam.shake(0.25f, mag)); }
}
