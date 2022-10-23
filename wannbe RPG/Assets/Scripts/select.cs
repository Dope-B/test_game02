using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class select : MonoBehaviour
{
    public SpriteRenderer[] portrait;
    public GameObject[] player;
    Color color;
    int index;
    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        indexCheck();
    }

    // Update is called once per frame
    void Update()
    {
        buttonCheck();
    }
    void buttonCheck()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (index != 0) { index--; } else { index = 3; }
            indexCheck();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (index != 3) { index++; } else { index = 0; }
            indexCheck();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(start());
        }
    }
    void indexCheck()
    {
        for(int i = 0; i < 4; i++)
        {
            color = portrait[i].color;
            if (i == index) { 
                color.a = 1f;
                portrait[i].color = color;
            }
            else {
                color.a = 0.3f;
                portrait[i].color = color;
            }
        }
    }
    IEnumerator start()
    {
        Time.timeScale = 0f;
        fade_manage.fade_manager.FadeOut();
        yield return new WaitUntil(() => fade_manage.fade_manager.fadeDone);
        SceneManager.LoadScene("Map_1");
        map_manage.map_manager.preMap = "Title";
        map_manage.map_manager.currentMap = "Map_1";
        cam_manager.cam.TheCamera.orthographicSize = 7.5f;
        cam_manager.cam.cam_player_gap = 5f; 
        cam_manager.cam.setSize();
        Instantiate(player[index], new Vector2(0, 0), Quaternion.identity);
        cam_manager.cam.gameObject.GetComponentInChildren<hp_bar_player>(true).gameObject.SetActive(true);
        cam_manager.cam.gameObject.GetComponentInChildren<hp_bar_player>(true).img.sprite = player_movement.player.portrait;
        fade_manage.fade_manager.FadeIn();
        Time.timeScale = 1f;
    }
}
