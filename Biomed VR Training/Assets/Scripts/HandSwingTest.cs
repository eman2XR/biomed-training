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

    void Start()
    {
        initRotation = this.transform.localEulerAngles;
    }

    private void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            //Unparent();
        }
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

    public void IsUsingScrew(Transform target)
    {
        transform.position = target.GetChild(0).position;
        transform.eulerAngles = new Vector3(target.GetChild(0).eulerAngles.x, target.GetChild(0).eulerAngles.y, target.GetChild(0).eulerAngles.z);
        transform.parent = target;
        //allowRotation = true;
    }

    public void ParentBack()
    {
        print("parent back");
        followRotation = false;
        transform.parent = parent;
        transform.localPosition = Vector3.zero;
        transform.localEulerAngles = initRotation;
    }
}
