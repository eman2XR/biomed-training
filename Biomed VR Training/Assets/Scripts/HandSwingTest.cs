using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSwingTest : MonoBehaviour
{
    public bool isLeft;
    public Transform parent;
    public Transform pivot;

    float initY;
    bool followRotation;

    bool allowRotation;

    Vector3 initRotation;

    public Renderer handVisual;
    Transform initialParent;
    Vector3 initialPos;
    Quaternion initialRot;

    //public Vector3 leftHandPosOffset;
    //public Vector3 leftHandRotOffset;

    void Start()
    {
        initRotation = this.transform.localEulerAngles;

        initialParent = handVisual.transform.parent;
    }

    private void Update()
    {
        //if (Input.GetKeyDown("p"))
        //{
            //Unparent();
        //}
        if (followRotation)
        {
            if (isLeft)
            {
                this.transform.parent.eulerAngles = new Vector3(this.transform.parent.eulerAngles.x, this.transform.parent.eulerAngles.y, this.transform.parent.eulerAngles.z + ((parent.position.y - initY) * 1));
            }
            else
            {
                this.transform.parent.eulerAngles = new Vector3(this.transform.parent.eulerAngles.x + ((parent.position.y - initY) * 3), this.transform.parent.eulerAngles.y, this.transform.parent.eulerAngles.z);
            }
        }
        if (allowRotation)
        {
            if (isLeft)
            {
                //this.transform.parent.eulerAngles = new Vector3(this.transform.parent.eulerAngles.y + (-(parent.position.y - initY) * 100), this.transform.parent.eulerAngles.y, this.transform.parent.eulerAngles.z);
            }
            else
            {
                this.transform.parent.eulerAngles = new Vector3(this.transform.parent.eulerAngles.x + ((parent.position.y - initY) * 1), this.transform.parent.eulerAngles.y, this.transform.parent.eulerAngles.z);
            }
        }
    }

    public void Unparent(Transform position)
    {
        initY = parent.position.y;
        this.transform.parent = null;
        this.transform.position = position.position;
        this.transform.rotation = position.rotation;
        this.transform.parent = pivot;
        followRotation = true;
    }

    public void IsUsingKnob(Transform target, Transform targetPostion, bool leftOnly)
    {
        if (leftOnly && !isLeft)
            return;

        transform.parent = target;
        transform.localPosition = targetPostion.localPosition;

        if (isLeft)
            transform.localEulerAngles = new Vector3(0, 115f, -10);
        else
            transform.localEulerAngles = new Vector3(0, -115f, 4);
        //allowRotation = true;
    }

    public void IsUsingScrew(Transform target, Transform targetPostion, Transform targetPostionLeft)
    {
        //print("left");
        transform.parent = target;

        if (isLeft)
        {
            transform.localEulerAngles = targetPostionLeft.localEulerAngles;
            transform.localPosition = targetPostionLeft.localPosition;
            //transform.localPosition = transform.localPosition + leftHandOffset;
        }
        else
        {
            transform.localPosition = targetPostion.localPosition;
            transform.localEulerAngles = targetPostion.localEulerAngles;
        }
        //transform.localEulerAngles = new Vector3(0, -115f, 8.5f);
        //allowRotation = true;

        //handVisual.parent = null;
        handVisual.enabled = false;
    }

    public void ParentBack()
    {
        //print("parent back");
        followRotation = false;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = initRotation;

        //handVisual.parent = initialParent;
        handVisual.enabled = true;
        //handVisual.localPosition = initialPos;
        //handVisual.localRotation = initialRot;
    }

}
