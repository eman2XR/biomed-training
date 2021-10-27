using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObjectInstance : MonoBehaviour
{
    //highlighter
    Outline outline;

    private Coroutine flashCoroutine;
    private bool flashing;

    private void Start()
    {
        outline = this.GetComponent<Outline>();
    }

    public void Highlight()
    {
        flashing = true;
        if(this.gameObject.activeSelf)
            StartCoroutine(FlashObject(0));
    }

    public void HighlightWithDelay(float delay)
    {
        flashing = true;
        flashCoroutine = StartCoroutine(FlashObject(delay));
    }

    public void StopFlashing()
    {
        flashing = false;
        StopAllCoroutines();

        if (GetComponent<Outline>())
            GetComponent<Outline>().enabled = false;
    }

    IEnumerator FlashObject(float delay)
    {
        //outline.UpdateRenderers();
        yield return new WaitForSeconds(delay);
        //print("flash");

        while (flashing)
        {
            //print("flashing");

            outline.enabled = true;

            yield return new WaitForSeconds(0.4f);

            outline.enabled = false;

            yield return new WaitForSeconds(0.4f);
        }
    }
}
