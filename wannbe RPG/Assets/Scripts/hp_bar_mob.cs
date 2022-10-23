using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hp_bar_mob : MonoBehaviour
{
    public Image img;
    public Image hp_bar;
    public Image de_hp_bar;
    public general_mob mob;
    public float timer=5f;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timer <= 0) { timer = 0;  this.gameObject.SetActive(false); }
        else { timer -= Time.deltaTime; }
        hp_bar.fillAmount = (mob.cur_HP / mob.max_HP);
        if (de_hp_bar.fillAmount > hp_bar.fillAmount)
        {
            de_hp_bar.fillAmount -= 0.002f;
        }
        if (de_hp_bar.fillAmount <= 0f && hp_bar.fillAmount <= 0) { this.gameObject.SetActive(false); }
    }
}
