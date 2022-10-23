using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fade_manage : MonoBehaviour
{
    public static fade_manage fade_manager;
    public SpriteRenderer black;
    private Color color;
    private WaitForSecondsRealtime wait;
    public bool fadeDone = false;
    // Start is called before the first frame update
    private void Awake()
    {
        if (fade_manager == null) { fade_manager = this; DontDestroyOnLoad(this.gameObject); }
        else { Destroy(this.gameObject); }
    }
    void Start()
    {
        wait = new WaitForSecondsRealtime(0.002f);
    }
    public void FadeOut(float speed = 0.04f)
    {
        fadeDone = false;
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(speed));

    }
    IEnumerator FadeOutCoroutine(float speed)
    {
        color = black.color;
        while (color.a < 1f)
        {
            color.a += speed;
            black.color = color;
            yield return wait;
        }
        fadeDone = true;
    }
    public void FadeIn(float speed = 0.04f)
    {
        fadeDone = false;
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(speed));
    }
    IEnumerator FadeInCoroutine(float speed)
    {
        color = black.color;
        while (color.a > 0f)
        {
            color.a -= speed;
            black.color = color;
            yield return wait;
        }
        fadeDone = true;
    }
}
