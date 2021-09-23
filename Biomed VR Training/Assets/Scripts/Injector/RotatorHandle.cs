using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorHandle : MonoBehaviour
{
    public Animator animator;
    public Transform leftHand;
    public Transform rightHand;
    public float activationDis = 0.2f;

    public bool isExpanded;

    void Update()
    {
        if(Vector3.Distance(leftHand.position, this.transform.position) < activationDis)
        {
            if (!isExpanded)
            {
                isExpanded = true;
                animator.SetTrigger("expand");
                animator.ResetTrigger("shrink");
            }
        }
        else if (Vector3.Distance(rightHand.position, this.transform.position) < activationDis)
        {
            if (!isExpanded)
            {
                isExpanded = true;
                animator.SetTrigger("expand");
                animator.ResetTrigger("shrink");
            }
        }
        else if (Vector3.Distance(rightHand.position, this.transform.position) > activationDis)
        {
            if (isExpanded)
            {
                isExpanded = false;
                animator.ResetTrigger("expand");
                animator.SetTrigger("shrink");
            }
        }
        else if (Vector3.Distance(leftHand.position, this.transform.position) > activationDis)
        {
            if (isExpanded)
            {
                isExpanded = false;
                animator.ResetTrigger("expand");
                animator.SetTrigger("shrink");
            }
        }
    }
}
