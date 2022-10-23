using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class map_transfer : MonoBehaviour
{
    public string depart;
    public string dest;
    // Start is called before the first frame update
    void Start()
    {
        if (map_manage.map_manager.preMap == dest && map_manage.map_manager.currentMap == depart)
        {
            cam_manager.cam.transform.position = new Vector3(transform.position.x,transform.position.y, cam_manager.cam.transform.position.z);
            player_movement.player.transform.position = new Vector2(transform.position.x, transform.position.y-0.5f);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player" && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(trans());
        }
    }
    IEnumerator trans()
    {
        fade_manage.fade_manager.black.transform.position = new Vector3(cam_manager.cam.transform.position.x, cam_manager.cam.transform.position.y, this.transform.position.z);
        if (cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>(true).gameObject.activeInHierarchy)
        {
            cam_manager.cam.gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<hp_bar_mob>(true).gameObject.SetActive(false);
        }
        Time.timeScale = 0f;
        fade_manage.fade_manager.FadeOut();
        yield return new WaitUntil(() => fade_manage.fade_manager.fadeDone);
        SceneManager.LoadScene(dest);
        map_manage.map_manager.preMap = depart;
        map_manage.map_manager.currentMap = dest;
        player_movement.player.curHP = player_movement.player.maxHP;
        if (map_manage.map_manager.currentMap == "Map_6") { cam_manager.cam.TheCamera.orthographicSize = 12f; cam_manager.cam.cam_player_gap = 10f; cam_manager.cam.setSize(); }
        else { cam_manager.cam.TheCamera.orthographicSize = 7.5f; cam_manager.cam.cam_player_gap = 5f; cam_manager.cam.setSize(); }
        fade_manage.fade_manager.FadeIn();
        Time.timeScale = 1f;
    }
   
}
