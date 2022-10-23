using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class summon_skill : MonoBehaviour
{
    SpriteRenderer spr;
    Color col;
    WaitForSeconds wait;
    WaitForSeconds die;
    player_movement player;
    BoxCollider2D box;
    bool fadeDone = false;
    bool stop_trans = false;
    public int damage;
    public float x;
    public float up;
    public bool shakeable;
    public GameObject hit1;
    public GameObject hit2;
    public GameObject slash;
    public bool isSlash;
    public float speed;
    public float summon;
    public float live_time;
    public int type;
    public bool breakable;
    Vector2 horming_pos;
    // Start is called before the first frame update
    void Start()
    {
        wait = new WaitForSeconds(0.04f);
        die = new WaitForSeconds(live_time);
        player = FindObjectOfType<player_movement>();
        spr = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        if (type != 8)
        {
            StartCoroutine(fadeIn());
            if (transform.localRotation.eulerAngles.y==180) { speed = -speed; }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeDone) { 
            switch (type)
             {
                case 1:
                    if (!stop_trans) { transform.position = new Vector2(transform.position.x - speed, transform.position.y); }
                    break;
                case 2:
                    transform.position = new Vector2(transform.position.x , transform.position.y + Mathf.Abs(speed));
                    break;
                 case 3:
                    transform.position = new Vector2(transform.position.x, transform.position.y - Mathf.Abs(speed));
                    break;
                case 4:
                    transform.position = new Vector2(transform.position.x - Mathf.Abs(speed), transform.position.y + Mathf.Abs(speed) * 0.2f);
                    break;
                case 5:
                    transform.position = new Vector2(transform.position.x - Mathf.Abs(speed), transform.position.y - Mathf.Abs(speed) * 0.2f);
                    break;
                case 6:
                    transform.position = new Vector2(transform.position.x + horming_pos.x * Mathf.Abs(speed), transform.position.y + horming_pos.y * Mathf.Abs(speed));
                    break;
                case 7:
                    horming_pos = new Vector2(player.transform.position.x - transform.position.x, (player.transform.position.y + 2.5f) - transform.position.y);
                    spr.transform.eulerAngles = new Vector3(0, 0, 180 + Mathf.Atan2(horming_pos.y, horming_pos.x) * Mathf.Rad2Deg);
                    horming_pos = horming_pos.normalized;
                    transform.position = new Vector2(transform.position.x + horming_pos.x * Mathf.Abs(speed), transform.position.y + horming_pos.y * Mathf.Abs(speed));
                    break;
                case 8:
                    break;
              }  
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!player.inv_check()) {
                if (!player.animator.GetCurrentAnimatorStateInfo(0).IsTag("guard") || (type!=3&&((player.transform.localScale.x==1&&player.transform.position.x<=transform.position.x)||
                                                                                       (player.transform.localScale.x == -1 && player.transform.position.x >= transform.position.x))))
                {
                    if (player.animator.GetCurrentAnimatorStateInfo(0).IsTag("guard"))
                    {
                        player.transform.localScale = new Vector3(player.transform.localScale.x * -1, 1, 1);
                    }
                    player.get_damage(damage, x, up, isSlash,shakeable, hit1, hit2, slash);
                }
                else { 
                    player.push_X(-4);
                    Instantiate(player.shield, new Vector2(Random.Range(player.transform.position.x - 0.1f, player.transform.position.x + 0.1f),
                                                        Random.Range(player.transform.position.y + 1.5f, player.transform.position.y + 2.5f))
                                    , Quaternion.identity);
                }
                if (shakeable) { cam_manager.cam.shake(0.5f, 3f); }
            }
            box.enabled = false;
            if (spr.gameObject.name == "Energy_ball(Clone)") { effect_for_env_ball(); }
            else if (type == 6 || type == 7) { Destroy(this.gameObject); }
        }
        if (collision.tag == "player_attack_range"&&breakable) { 
            Destroy(this.gameObject);
            Instantiate(hit1, transform.position, Quaternion.identity);
        }
    }

    IEnumerator fadeIn()
    {
        col = spr.color;
        while (col.a < 1f)
        {
            col.a += summon;
            spr.color = col;
            yield return wait;
        }
        fadeDone = true;
        if (type == 6) {
            horming_pos = new Vector2(player.transform.position.x - transform.position.x, (player.transform.position.y + 2.5f) - transform.position.y);
            spr.transform.eulerAngles = new Vector3(0, 0, 180 + Mathf.Atan2(horming_pos.y, horming_pos.x) * Mathf.Rad2Deg);
            horming_pos = horming_pos.normalized;
        }
        yield return die;
        Destroy(this.gameObject);
    }
    void effect_for_env_ball()
    {
        Animator ani = GetComponent<Animator>();
        ani.SetBool("isColl", true);
        stop_trans = true;
    }
    void effect_destroy()
    {
        Destroy(this.gameObject);
    }
    void shake()
    {
        cam_manager.cam.shake(0.5f, 3f);
    }
}
