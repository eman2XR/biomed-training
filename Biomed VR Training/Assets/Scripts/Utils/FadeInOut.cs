using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    Material mat;

    private void Start()
    {
        mat = this.GetComponent<Renderer>().material;
    }

    public void FadeOut()
    {
        StartCoroutine(StartFadeOut());
    }

    IEnumerator StartFadeOut()
    {
        float duration = 4;
        float time = 0;

        while (time < duration) 
        {
            mat.color = Color.Lerp(Color.white, Color.clear, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        if (time >= duration)
            this.GetComponent<Renderer>().enabled = false;
    }
}
