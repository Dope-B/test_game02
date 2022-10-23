using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_movement : MonoBehaviour
{
    static public player_movement player;
    Rigidbody2D rigid;
    public Animator animator;
    public GameObject shield;
    public GameObject DMGfont;
    BoxCollider2D box;
    public GameObject dash_dust;
    public GameObject jump_dust;
    public GameObject slash_effect;
    public GameObject fire_col;
    public Sprite portrait;
    bool super_armor = false;
    bool is_inv;
    public float maxHP;
    public float curHP;
    public float move_power = 1f;
    public float jump_power = 1f;
    // Start is called before the first frame update
    void Awake()
    {
        if (player == null)
        {
            player = this;
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            box = GetComponent<BoxCollider2D>();
            DontDestroyOnLoad(this.gameObject);
            curHP = maxHP;
        }
        else { Destroy(this.gameObject); }
    }
    private void Update()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("hurt_out_2")) { button_check(); }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        falling_check();
        limitPos();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rigid.velocity.y <= 0f)
        {
            if (collision.collider.tag == "ground")
            {
                animator.SetBool("isFalling", false);
                animator.SetBool("isJumping", false);
                rigid.velocity = Vector2.zero;
            }
        }

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "ground" && rigid.velocity.y <= 0f)
        {
            if (animator.GetBool("isFalling")) { animator.SetBool("isFalling", false); }
            if (animator.GetBool("isJumping")) { animator.SetBool("isJumping", false); }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "mob_attack_range"&& !animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt"))
        {
            if (!inv_check())
            {
                if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("guard") || transform.localScale.x * collision.GetComponentInParent<general_mob>().transform.localScale.x == 1)
                {
                    if (transform.localScale.x * collision.gameObject.transform.localScale.x == 1)
                    {
                        transform.localScale = new Vector3(transform.localScale.x * -1, 1, 1);
                    }
                    get_damage(collision.gameObject.GetComponent<attack_range>().damage,
                                 collision.gameObject.GetComponent<attack_range>().force_back,
                                 collision.gameObject.GetComponent<attack_range>().force_up,
                                 collision.gameObject.GetComponent<attack_range>().isSlash,
                                 collision.gameObject.GetComponent<attack_range>().shakeable,
                                 collision.gameObject.GetComponent<attack_range>().hit1,
                                 collision.gameObject.GetComponent<attack_range>().hit2,
                                 collision.gameObject.GetComponent<attack_range>().slash);
                }
                else
                {
                    push_X(-4);
                    Instantiate(shield, new Vector2(Random.Range(transform.position.x - 0.1f, transform.position.x + 0.1f),
                                                    Random.Range(transform.position.y + (box.size.y / 2) - 0.1f, transform.position.y + (box.size.y / 2) + 0.1f))
                                , Quaternion.identity);
                }
            }

        }
    }
    IEnumerator slash_skill()
    {
        float t = transform.localScale.x;
        for (int i = 0; i < 10; i++)
        {
            Instantiate(slash_effect, new Vector2(transform.position.x + (6f * -t), transform.position.y + 2.5f), Quaternion.Euler(0, 0, Random.Range(0, 180)));
            yield return new WaitForSeconds(0.1f);
        }

    }
    IEnumerator fire_skill()
    {
        float t = transform.localScale.x;
        for (int i = 0; i < 4; i++)
        {
            Instantiate(fire_col, new Vector2(transform.position.x + (2f * i * -t), transform.position.y + 2.5f), Quaternion.identity);
            Instantiate(fire_col, new Vector2(transform.position.x - (2f * i * -t), transform.position.y + 2.5f), Quaternion.identity);
            yield return new WaitForSeconds(0.15f);
        }
    }
    void button_check()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && ani_check()) { animator.SetTrigger("dash"); }
        if (Input.GetKey(KeyCode.DownArrow) && (ani_check() ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("guard"))) { animator.SetBool("isSitting", true); }
        else if (Input.GetKeyUp(KeyCode.DownArrow)) { animator.SetBool("isSitting", false); }
        if (Input.GetKey(KeyCode.C) && (ani_check() ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("jump_up") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("jump_down") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("sit"))) { animator.SetBool("isGuarding", true); }
        else if (Input.GetKeyUp(KeyCode.C)) { animator.SetBool("isGuarding", false); }
        attack();
        if (!disable_move()) { move(); }
        if (!disable_jump()) { jump(); }
    }
    void move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        Vector3 move_velocity = Vector3.zero;
        if (h != 0)
        {
            if (h < 0) { move_velocity = Vector3.left; transform.localScale = new Vector3(1, 1, 1); }
            else if (h > 0) { move_velocity = Vector3.right; transform.localScale = new Vector3(-1, 1, 1); }
            if (Input.GetKey(KeyCode.LeftShift) && !animator.GetBool("isJumping") && !animator.GetBool("isFalling"))
            {
                if (!animator.GetBool("isRunning"))
                {
                    animator.SetBool("isRunning", true);
                    animator.SetBool("isWalking", false);

                }
                transform.position += move_velocity * move_power * 3 * Time.deltaTime;
            }
            else
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isRunning", false);
                transform.position += move_velocity * move_power * Time.deltaTime;
            }

        }
        else
        { animator.SetBool("isWalking", false); animator.SetBool("isRunning", false); }

    }
    void jump()
    {
        if (!animator.GetBool("isJumping") && !animator.GetBool("isFalling"))
        {
            if (Input.GetButtonDown("Jump"))
            {
                rigid.AddForce(Vector2.up * jump_power, ForceMode2D.Impulse);
                Instantiate(jump_dust, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity);
            }
        }
    }
    bool ani_check()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("idle") || animator.GetCurrentAnimatorStateInfo(0).IsName("walk") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("run") || animator.GetCurrentAnimatorStateInfo(0).IsName("dash")) { return true; }
        else { return false; }
    }
    bool disable_move()
    {
        if (animator.GetBool("isSitting") ||
            animator.GetBool("isGuarding") ||
            attack_check() ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("dash") ||
            animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt")
            ) { return true; }
        else { return false; }
    }
    bool disable_jump()
    {
        if (attack_check()
            || animator.GetCurrentAnimatorStateInfo(0).IsName("dash") ||
            animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt")
            ) { return true; }
        else { return false; }
    }
    void falling_check()
    {
        if (rigid.velocity.y < 0f && !animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt") && !animator.GetCurrentAnimatorStateInfo(0).IsName("sit")) { animator.SetBool("isFalling", true); }
        else if (rigid.velocity.y > 0f && !animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt")) { animator.SetBool("isJumping", true); }
    }
    void attack()
    {
        if (Input.GetKeyDown(KeyCode.Z) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt")) { animator.SetTrigger("attack1"); }
        else if (Input.GetKeyDown(KeyCode.X) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt")) { animator.SetTrigger("attack2"); }
        else if (Input.GetKeyDown(KeyCode.A) && ani_check()) { animator.SetTrigger("attack3"); }
        else if (Input.GetKeyDown(KeyCode.S) && ani_check() && !animator.GetCurrentAnimatorStateInfo(0).IsName("jump_down")) { animator.SetTrigger("attack4"); }
        else if (Input.GetKeyDown(KeyCode.D) && ani_check()) { animator.SetTrigger("attack5"); }

    }
    bool attack_check()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("attack")) { return true; }
        else { return false; }
    }
    public bool inv_check()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("hurt") || animator.GetCurrentAnimatorStateInfo(0).IsName("dash")
            || animator.GetCurrentAnimatorStateInfo(0).IsName("attack_skill_dash")||is_inv) { return true; }
        else { return false; }
    }
    void limitPos()
    {
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, cam_manager.cam.bound.bounds.min.x + 1f, cam_manager.cam.bound.bounds.max.x - 1f),
                                    Mathf.Clamp(transform.position.y, cam_manager.cam.bound.bounds.min.y, cam_manager.cam.bound.bounds.max.y));
    }
    void push_up(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.up * power, ForceMode2D.Impulse);
    }
    void push_down(float power)
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);
        rigid.AddForce(Vector2.down * power, ForceMode2D.Impulse);
    }
    public void push_X(float power)
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
    void resetTrigger(int i)
    {
        if (i == 1) { animator.ResetTrigger("attack1"); }
        else if (i == 2) { animator.ResetTrigger("attack2"); }
        else if (i == 3) { animator.ResetTrigger("attack3"); }
        else if (i == 4) { animator.ResetTrigger("attack4"); }
        else if (i == 5) { animator.ResetTrigger("attack5"); }
    }
    void dash_dust_on()
    {
        if (transform.localScale.x == -1) { Instantiate(dash_dust, new Vector2(transform.position.x - 1f, transform.position.y + 0.3f), Quaternion.Euler(0, 180, 0)); }
        else { Instantiate(dash_dust, new Vector2(transform.position.x + 1f, transform.position.y + 0.3f), Quaternion.Euler(0, 0, 0)); }
    }
    void jump_dust_on() { Instantiate(jump_dust, new Vector2(transform.position.x, transform.position.y + 0.5f), Quaternion.identity); }
    public void get_damage(int damage, float x, float up, bool hit_type, bool shake, GameObject hit1, GameObject hit2, GameObject slash)
    {
        if (shake) { StartCoroutine(cam_manager.cam.shake(0.2f, 1.8f)); }
        int i = Random.Range(0, 2);
        if (!super_armor)
        {
            if (animator.GetBool("isJumping") || animator.GetBool("isFalling")) { animator.SetTrigger("isHurtOut"); }
            else
            {
                if (i == 1) { animator.SetBool("Hurt2", true); }
                else { animator.SetBool("Hurt2", false); }
                animator.SetTrigger("isHurt");
                rigid.velocity = Vector2.zero;
                push_X(-x);
            }
        }
        if (hit_type)
        {
            Instantiate(slash, new Vector2(Random.Range(transform.position.x - 0.1f, transform.position.x + 0.1f),
                                          Random.Range(transform.position.y + (box.size.y / 2) - 0.1f, transform.position.y + (box.size.y / 2) + 0.1f)),
                                          Quaternion.Euler(0, 0, Random.Range(0, 180)));
        }
        else
        {
            i = Random.Range(0, 2);
            if (i == 0)
            {
                Instantiate(hit1, new Vector2(Random.Range(transform.position.x - 0.1f, transform.position.x + 0.1f),
                                Random.Range(transform.position.y + (box.size.y / 2) - 0.1f, transform.position.y + (box.size.y / 2) + 0.1f)),
                                Quaternion.identity);
            }
            else
            {
                Instantiate(hit2, new Vector2(Random.Range(transform.position.x - 0.1f, transform.position.x + 0.1f),
                             Random.Range(transform.position.y + (box.size.y / 2) - 0.1f, transform.position.y + (box.size.y / 2) + 0.1f)),
                             Quaternion.identity);
            }
        }
        GameObject DF = Instantiate(DMGfont, new Vector2(transform.position.x, transform.position.y + box.size.y + 0.8f), Quaternion.identity);
        DF.GetComponent<floating_text>().damage = damage.ToString();
        DF.GetComponent<floating_text>().type = true;
        curHP -= damage;
        if (curHP <= 0) { animator.SetBool("isDie", true); }
    }
    void trans_to_enemy()
    {
        RaycastHit2D ray;
        ray = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + box.size.y / 2),new Vector2(-1,0)*transform.localScale.x, 20f, 1 << 8);
        //Debug.DrawRay(new Vector2(transform.position.x, transform.position.y + box.size.y / 2), new Vector2(-1, 0) * transform.localScale.x*20f, Color.green, 0.1f);
        if (ray)
        {
            transform.position = new Vector2(ray.transform.position.x + (ray.transform.localScale.x * 3f), transform.position.y);
            transform.localScale = ray.transform.localScale;
        }
    }
    void cour_slash_skill() { StartCoroutine(slash_skill()); }
    void cour_fire_skill() { StartCoroutine(fire_skill()); }
    void destroy_ob() { Destroy(this.gameObject); }
    void cam_shake(float mag) { StartCoroutine(cam_manager.cam.shake(0.2f, mag)); }
    void die_check() { if (curHP <= 0) { animator.SetTrigger("isDie"); } }
    IEnumerator char_revive()
    {
        fade_manage.fade_manager.black.transform.position = new Vector3(cam_manager.cam.transform.position.x, cam_manager.cam.transform.position.y, this.transform.position.z);
        if (cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>(true).gameObject.activeInHierarchy)
        {
            cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>(true).gameObject.SetActive(false);
        }
        Time.timeScale = 0f;
        fade_manage.fade_manager.FadeOut();
        yield return new WaitUntil(() => fade_manage.fade_manager.fadeDone);
        Destroy(this.gameObject);
        cam_manager.cam.TheCamera.orthographicSize = 20f;
        cam_manager.cam.TheCamera.transform.position = new Vector3(0, 0, cam_manager.cam.TheCamera.transform.position.z);
        SceneManager.LoadScene("Title");
        map_manage.map_manager.preMap = null;
        map_manage.map_manager.currentMap = "Title";
        fade_manage.fade_manager.FadeIn();
        Time.timeScale = 1f;
    }
    void is_sm_check(int i) { if (i == 0) { super_armor = false; } else { super_armor = true; } }
    void is_inv_check(int i) { if (i == 0) { is_inv = false; } else { is_inv = true; } }
}
