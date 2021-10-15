using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    Material mat;
    Color startColor;

    private void Start()
    {
        mat = this.GetComponent<Renderer>().material;
        startColor = mat.color;
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
            mat.color = Color.Lerp(startColor, Color.clear, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        if (time >= duration)
            this.GetComponent<Renderer>().enabled = false;
    }
}
