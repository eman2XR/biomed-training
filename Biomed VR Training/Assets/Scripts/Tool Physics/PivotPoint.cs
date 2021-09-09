using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotPoint : MonoBehaviour
{
    Rigidbody rb;
    public Transform hand;
    public bool move;
    public HandSwingTest handSwing;
    //public Transform offset;
    public bool isScrew;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isScrew)
        {
            if (other.tag == "phillipsScrewdriver")
            {
                move = true;
                handSwing.IsUsingScrew(this.transform);
                StartCoroutine(Reset());
            }
        }
        else
        {
            if (other.tag == "screwdriver")
            {
                move = true;
                handSwing.IsUsingScrew(this.transform);
                StartCoroutine(Reset());
            }
        }
    }

    private void Update()
    {
        if (move)
            this.transform.LookAt(hand);
        //rb.MovePosition(hand.position);
        //
        //
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(1.5f);
        handSwing.ParentBack();
        this.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        //this.transform.eulerAngles = new Vector3(transform.eulerAngles.x, 0, transform.eulerAngles.y)
    }
}
