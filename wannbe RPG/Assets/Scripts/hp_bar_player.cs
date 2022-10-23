using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class hp_bar_player : MonoBehaviour
{
    public Image hp_bar;
    public Image de_hp_bar;
    public Image img;
    // Start is called before the first frame update
    void Start()
    {
        hp_bar.fillAmount = 1f;
        de_hp_bar.fillAmount = 1f;
        img.sprite = player_movement.player.portrait;
    }

    // Update is called once per frame
    void Update()
    {
        hp_bar.fillAmount = player_movement.player.curHP / player_movement.player.maxHP;
        if (de_hp_bar.fillAmount > hp_bar.fillAmount)
        {
            de_hp_bar.fillAmount -= 0.005f;
        }
    }
}
