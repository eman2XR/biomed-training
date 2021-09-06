using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gasket : MonoBehaviour
{
    public List<Follow> linePoints = new List<Follow>();
    public GameObject staticGasket;
    public GameObject guide;

    private void Start()
    {
        foreach (Transform trans in this.GetComponentInChildren<Transform>())
            linePoints.Add(trans.GetComponent<Follow>());
    }

    public void GasketHooked()
    {
        guide.SetActive(false);
        staticGasket.SetActive(false);
        this.GetComponent<LineRenderer>().enabled = true;
        StartCoroutine(AdjustFollowSmooth());
    }

    IEnumerator AdjustFollowSmooth()
    {
        float offset = 0;
        yield return new WaitForSeconds(0.6f);
        foreach (Follow linePoint in linePoints)
            linePoint.smoothPos = 0.015f;
        yield return new WaitForSeconds(0.25f);
        foreach (Follow linePoint in linePoints)
        {
            offset -= 0.0015f;
            linePoint.offset = new Vector3(0, offset, 0);
        }
    }

    public void DropGasket()
    {
        Destroy(this.gameObject);
        //foreach(Follow point in linePoints)
        //{
        //    point.enabled = false;
        //    point.gameObject.GetComponent<Follow>().enabled = false;
        //    point.gameObject.GetComponent<Collider>().enabled = true;
        //    point.gameObject.GetComponent<Collider>().isTrigger = false;
        //    point.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //}
    }
}
