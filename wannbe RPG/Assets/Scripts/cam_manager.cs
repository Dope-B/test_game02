using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam_manager : MonoBehaviour
{
    static public cam_manager cam;
    public float speed;
    public Camera TheCamera;
    public BoxCollider2D bound;
    private Vector3 minBound;
    private Vector3 maxBound;
    private Vector3 targetPos;
    private float halfWidth;
    private float halfHeight;
    public float cam_player_gap;
    private float clampX;
    private float clampY;
    private void Awake()
    {
        if (cam == null) { cam = this; DontDestroyOnLoad(this.gameObject); }
        else { Destroy(this.gameObject); }
    }

    void Start()
    {
        TheCamera = GetComponent<Camera>();
        halfHeight = TheCamera.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;
        if (map_manage.map_manager.currentMap != "Title")
        {
            transform.position = new Vector3(player_movement.player.transform.position.x, player_movement.player.transform.position.y + cam_player_gap, this.transform.position.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (map_manage.map_manager.currentMap != "Title")
        {
            if (player_movement.player.gameObject != null)
            {
                targetPos.Set(player_movement.player.transform.position.x, player_movement.player.transform.position.y + cam_player_gap, this.transform.position.z);
                this.transform.position = Vector3.Lerp(this.transform.position, targetPos, speed * Time.deltaTime);
                clampX = Mathf.Clamp(this.transform.position.x, minBound.x + halfWidth, maxBound.x - halfWidth);
                if (bound.size.x <= halfWidth * 2) { clampX = (minBound.x + maxBound.x) * 0.5f; }
                clampY = Mathf.Clamp(this.transform.position.y, minBound.y + halfHeight, maxBound.y - halfHeight);
                if (bound.size.y <= halfHeight * 2) { clampY = (minBound.y + maxBound.y) * 0.5f; }
                this.transform.position = new Vector3(clampX, clampY, this.transform.position.z);
            }
        }
       
    }
    public void setBound(BoxCollider2D box)
    {
        bound = box;
        minBound = bound.bounds.min;
        maxBound = bound.bounds.max;
    }
    public void setSize()
    {
        halfHeight = TheCamera.orthographicSize;
        halfWidth = halfHeight * Screen.width / Screen.height;
    }
    public IEnumerator shake(float duration, float mag)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-0.1f, 0.1f) * mag;
            float y = Random.Range(-0.1f, 0.1f) * mag;
            transform.position = new Vector3(transform.position.x+ x,transform.position.y+ y, transform.position.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        this.transform.position = new Vector3(clampX, clampY, this.transform.position.z);
    }
}
