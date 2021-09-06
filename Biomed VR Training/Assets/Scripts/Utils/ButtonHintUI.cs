using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonHintUI : MonoBehaviour {

    LineRenderer lineRenderer;
    public Transform targetObject;
    Transform start;
    Transform end;

	void Awake () {
        lineRenderer = this.transform.GetChild(0).GetComponent<LineRenderer>();
        start = transform.GetChild(1);
        end = transform.GetChild(2);
        lineRenderer.SetPosition(0, start.position);
        lineRenderer.SetPosition(1, end.position);
    }

    void Update () {
            lineRenderer.SetPosition(0, start.position);
            lineRenderer.SetPosition(1, end.position);

        if (targetObject)
        {
            end.transform.position = targetObject.position;
        }
    }
}
