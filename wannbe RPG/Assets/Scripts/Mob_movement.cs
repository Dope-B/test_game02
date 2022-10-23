using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob_movement : general_mob
{
    override protected void Start()
    {
        base.Start();
    }
    override protected void Update()
    {
        base.Update();
    }
    override protected void idle_pattern()
    {
        ran = Random.Range(1, 101);
        if (attack_mode)
        {
            if (ran <= 15) { cur_state = 3; /*Debug.Log("idle to move1");*/ }//move
            else if (ran > 15 && ran <= 70) { cur_state = 2; /*Debug.Log("idle to attack1"); */ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle1"); */ }//idle
        }
        else if (trace_mode)
        {
            if (ran <= 50) { cur_state = 3; /*Debug.Log("idle to move2"); */ }//move
            else if (ran > 75 && ran <= 90) { cur_state = 2; /*Debug.Log("idle to attack2"); */ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle2"); */ }//idle
        }
        else
        {
            if (ran <= 40) { cur_state = 3; /*Debug.Log("idle to move2"); */ }//move
            else if (ran > 40 && ran <= 45) { cur_state = 2; /*Debug.Log("idle to attack2"); */ }//attack
            else { cur_state = 1; /*Debug.Log("idle to idle2"); */ }//idle
        }
        ran = Random.Range(1, 101);
    }
     override protected void move_pattern()
        {
            ran = Random.Range(1, 101);
            if (attack_mode)
            {
                if (ran <= 10) { cur_state = 3; /*Debug.Log("move to move1"); */ }
                else if (ran > 10 && ran <= 40) { cur_state = 1; /*Debug.Log("move to idle1"); */ }
                else { cur_state = 2; /*Debug.Log("move to attack1"); */ }
            }
            else if (trace_mode)
            {
                if (ran <= 50) { cur_state = 3; /*Debug.Log("move to move2"); */ }
                else if (ran > 50 && ran <= 75) { cur_state = 1; /*Debug.Log("move to idle2"); */ }
                else { cur_state = 2; /*Debug.Log("move to attack2"); */ }

            }
            else
            {
                if (ran <= 40) { cur_state = 3; /*Debug.Log("move to move3");*/ }
                else if (ran > 40 && ran <= 90) { cur_state = 1; /*Debug.Log("move to idle3");*/ }
                else { cur_state = 2; /*Debug.Log("move to attack3");*/ }
            }
             ran = Random.Range(1, 101);
    }
}
