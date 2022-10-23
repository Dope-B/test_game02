using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class map_manage : MonoBehaviour
{
    static public map_manage map_manager;
    public string currentMap;
    public string preMap;
    private void Awake()
    {
        if (map_manager == null) { map_manager = this;DontDestroyOnLoad(this.gameObject); currentMap = "Title"; }
        else { Destroy(this.gameObject); }
    }
}
