using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class floating_text : MonoBehaviour
{
    public string damage;
    public bool type;
    TextMeshPro tex;
    private void Awake()
    {
        
    }
    void Start()
    {
        Destroy(this.gameObject, 0.2f);
        tex = GetComponent<TextMeshPro>();
        tex.text = damage;
        if (type) { tex.color = new Color32(112, 112, 255,255); }
        else { tex.color = new Color32(255, 112, 112,255); }
    }
    private void Update()
    {
        transform.Translate(new Vector3(0, 5f * Time.deltaTime, 0));
    }
}
//new Color(112, 112, 255); new Color(255, 112, 112);