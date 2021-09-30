using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class Piston : MonoBehaviour
{
    public Knob knob;
    float initialX;
    public float targetPos;
    public UnityEvent onTargetReached;
    int counter = 0;
    public TextMeshProUGUI text;

    void Start()
    {
        initialX = this.transform.localPosition.x;
    }

    void Update()
    {
        //this.transform.localPosition = new Vector3(initialX - ExtensionMethods.RemapValue(knob.outAngle, 0, 100, 0, 0.3f), transform.localPosition.y, transform.localPosition.z);

        if(this.transform.localPosition.x < targetPos)
        {
            onTargetReached.Invoke();
            this.enabled = false;
        }
    }

    public void KnobTurned()
    {
        counter++;
        text.text = (3 - counter).ToString();
        if(counter <= 3)
         StartCoroutine(MoveUp());
    }

    IEnumerator MoveUp()
    {
        float duration = 1f;
        float elapsedTime = 0;
        this.GetComponent<AudioSource>().Play();

        while (elapsedTime < duration)
        {
            this.transform.localPosition = Vector3.Lerp(transform.localPosition, transform.localPosition - new Vector3(0.55f, 0, 0), Time.deltaTime * 1);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
    }
}
