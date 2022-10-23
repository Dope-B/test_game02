using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class start_point : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (map_manage.map_manager.preMap == "Title") { player_movement.player.transform.position = this.transform.position; }
    }


}
