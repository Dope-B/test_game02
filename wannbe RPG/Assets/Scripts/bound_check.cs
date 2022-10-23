using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bound_check : MonoBehaviour
{
    BoxCollider2D box;
    // Start is called before the first frame update
    void Awake()
    {
        box = GetComponent<BoxCollider2D>();
        cam_manager.cam.setBound(box);
    }
}
