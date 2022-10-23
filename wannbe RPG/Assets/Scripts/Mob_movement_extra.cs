using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_movement_extra : general_mob
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
        if (!jump_attack_mode) {base.Update(); }     
    }
    override protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (rigid.velocity.y <= 0f)
        {
            if (collision.collider.tag == "ground")
            {
                rigid.velocity = Vector2.zero;
                ani.enabled = true; is_hurt = false; is_ground = true; timer_on = false; cur_state = 2;StopCoroutine(pattern_cool_down());
            }
        }
    }
    override protected void idle_pattern()
    {
        ran = Random.Range(1, 101);
        if (attack_mode)
        {
            if (ran <= 20) { cur_state = 3; /*Debug.Log("idle to move");*/ }//move
            else if (ran > 20 && ran <= 80) { cur_state = 2; /*Debug.Log("idle to attack");*/ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle");*/ }//idle

        }
        else if (trace_mode)
        {
            if (ran <= 40) { cur_state = 3; /*Debug.Log("idle to move");*/ }//move
            else if (ran > 40 && ran <= 75) { jump_attack_mode = true; cur_state = 2; /*Debug.Log("idle to jump_attack");*/}
            else if (ran > 75 && ran <= 90) { cur_state = 2; /*Debug.Log("idle to attack");*/ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle");*/ }//idle
        }
        else
        {
            if (ran <= 40) { cur_state = 3; /*Debug.Log("idle to move"); */ }//move
            else if (ran > 40 && ran <= 45) { cur_state = 2; /*Debug.Log("idle to attack"); */ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle"); */ }//idle
        }
        ran = Random.Range(1, 101);
    }
    override protected void move_pattern()
    {
        ran = Random.Range(1, 101);
        if (attack_mode)
        {
            if (ran <= 20) { cur_state = 3; /*Debug.Log("move to move"); */ }
            else if (ran > 20 && ran <= 30) { cur_state = 1; /*Debug.Log("move to idle"); */ }
            else { cur_state = 2; /*Debug.Log("move to attack"); */ }
        }
        else if (trace_mode)
        {
            if (ran <= 15) { cur_state = 3; /*Debug.Log("move to move"); */ }
            else if (ran > 15 && ran <= 25) { cur_state = 1; /*Debug.Log("move to idle"); */ }
            else if (ran > 25 && ran <= 45) { cur_state = 2; /*Debug.Log("move to attack"); */ }
            else { jump_attack_mode = true; cur_state = 2; /*Debug.Log("move to jump_attack"); */  }
        }
        else
        {
            if (ran <= 40) { cur_state = 3; /*Debug.Log("move to move"); */ }
            else if (ran > 40 && ran <= 85) { cur_state = 1; /*Debug.Log("move to idle"); */ }
            else { cur_state = 2; /*Debug.Log("move to attack"); */}
        }
    }
    override protected void attack_pattern()
    {
        focus();
        ani.SetBool("isMoving", false);
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
        else { ani.SetTrigger("jumpAttack");}
        ran = Random.Range(0, 101);
    }
    void target_setting()
    {
        Vector3 start_pos = transform.position;
        if (player.transform.position.x >= transform.position.x) { target_pos_x = transform.position.x - Vector3.Lerp(new Vector2(transform.position.x - land_x, transform.position.y), player.transform.position, 0.05f).x; }
        else { target_pos_x = transform.position.x - Vector3.Lerp(new Vector2(transform.position.x + land_x, transform.position.y), player.transform.position, 0.05f).x; }
    }
    IEnumerator jump()
    {
        //Debug.Log("jump");
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
}
