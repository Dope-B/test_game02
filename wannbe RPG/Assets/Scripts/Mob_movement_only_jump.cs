using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_movement_only_jump : general_mob
{
    public bool jump_attack_mode;
    float target_pos_x;
    public float jump_time;
    public float land_x;
    override protected void Start()
    {
        base.Start();
    }

    override protected void Update()
    {
        limitPos();
        if (!is_hurt&&!jump_attack_mode && !ani.GetCurrentAnimatorStateInfo(0).IsName("die") && !ani.GetCurrentAnimatorStateInfo(0).IsTag("attack"))
        {
            trace_mode_check();
            if (!timer_on&&is_ground)// actual behavior
            {
                ran = Random.Range(1, 101);
                timer_on = true;
                switch (cur_state)
                {
                    case 1:
                        idle_pattern();
                        break;
                    case 2:
                        if (attack_mode)
                        {
                            if (ran <= 80) { cur_state = 2; /*Debug.Log("attack to attack"); */ }
                            else { cur_state = 1; /*Debug.Log("attack to idle"); */ }
                        }
                        else if (trace_mode)
                        {
                            if (ran <= 20) { cur_state = 2; /*Debug.Log("attack to attack"); */ }
                            else if (ran > 20 && ran <= 70) { jump_attack_mode = true; cur_state = 2; /*Debug.Log("attack to jump_attack"); */ }
                            else { cur_state = 1; /*Debug.Log("attack to idle"); */ }
                        }
                        else
                        {
                            if (ran <= 20) { cur_state = 2; /*Debug.Log("attack to attack"); */ }
                            else if (ran > 20 && ran <= 40) { jump_attack_mode = true; cur_state = 2; /*Debug.Log("attack to jump_attack"); */ }
                            else { cur_state = 1; /*Debug.Log("attack to idle"); */ }
                        }
                        break;
                }
                if (cur_state == 2) { attack_pattern();}
                StartCoroutine(pattern_cool_down());
            }
        }
    }
    override protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (rigid.velocity.y <= 0f)
        {
            if (collision.collider.tag == "ground")
            {
                rigid.velocity = Vector2.zero;
                ani.enabled = true; is_hurt = false; is_ground = true; jump_attack_mode = false;
            }
        }
    }
    override protected void idle_pattern()
    {
        ran = Random.Range(1, 101);
        if (attack_mode)
        {
            if (ran <= 60) { cur_state = 2; /*Debug.Log("idle to attack"); */ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle"); */ }//idle

        }
        else if (trace_mode)
        {
            if (ran <= 65) { jump_attack_mode = true; cur_state = 2; /*Debug.Log("idle to jump_attack"); */ }
            else if (ran > 65 && ran <= 85) { cur_state = 2; /*Debug.Log("idle to attack"); */ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle"); */ }//idle
        }
        else
        {
            if (ran <= 35) { cur_state = 2; /*Debug.Log("idle to attack"); */ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle"); */ }//idle
        }

    }
    override protected void attack_pattern()
    {
        focus();
        if (!jump_attack_mode)
        {
            ran = Random.Range(1, (100 / attack_way) * attack_way);
            for (int i = 1; i <= attack_way; i++)
            {
                if (ran >= (100 / attack_way) * (i - 1) && ran < (100 / attack_way) * i)
                {
                    ani.SetTrigger("attack" + i.ToString());
                    break;
                }
            }
        }
        else { ani.SetTrigger("jumpAttack"); }
        ran = Random.Range(1, 101);
    }

    void target_setting()
    {
        Vector3 start_pos = transform.position;
        if (player.transform.position.x >= transform.position.x) { target_pos_x = transform.position.x - Vector3.Lerp(new Vector2(transform.position.x - land_x, transform.position.y), player.transform.position, 0.05f).x; }
        else { target_pos_x = transform.position.x - Vector3.Lerp(new Vector2(transform.position.x + land_x, transform.position.y), player.transform.position, 0.05f).x; }
    }
    IEnumerator jump()
    {
        if (jump_attack_mode)
        {
            for (int i = 0; i < 20; i++)
            {
                transform.position -= new Vector3(target_pos_x, 0);
                yield return new WaitForSeconds(jump_time / 20);
            }
        }
    }
    void jump_mode_false() { jump_attack_mode = false; }
    void player_trans(float x)
    {
        transform.position = new Vector2(player.transform.position.x-(x*player.transform.localScale.x), player.transform.position.y);
    }
}
