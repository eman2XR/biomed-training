﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WasteBin : MonoBehaviour
{
    GameObject obj;
    public AudioSource audio;
    public bool gasketIn;
    public UnityEvent onGasketIn;
    public bool allGasketsIn;
    public bool finalGasketIn;
    bool waited = true;
    int counter;

    private void OnTriggerEnter(Collider other)
    {
        List<GameObject> gasketsIn = new List<GameObject>();

        if (other.tag == "gasket" && waited)
        {
            if (gasketsIn.Contains(other.transform.parent.gameObject))
                return; //prevent the same gasket from triggering

            gasketsIn.Add(other.transform.parent.gameObject);
            //StartCoroutine(Delay());
            this.transform.parent.GetChild(counter).gameObject.SetActive(true); //activate old gasket visual
            gasketIn = true;
            obj = other.gameObject;
            obj.transform.parent.GetComponent<Gasket>().DropGasket();
            audio.Play();
            onGasketIn.Invoke();

            counter++;
            if (counter == 4)
                allGasketsIn = true;

            if (counter == 5)
                finalGasketIn = true;
            //Destroy(other.GetComponent<Follow>());
            //Destroy(other.GetComponent<Follow>());
            //other.GetComponent<Rigidbody>().isKinematic = false;
            //other.GetComponent<Collider>().isTrigger = false;
            //other.transform.position = this.transform.position;
            //other.GetComponent<Collider>().isTrigger = false;
            //Destroy(other.transform.parent.gameObject);
        }
    }

    IEnumerator Delay()
    {
        waited = false;
        yield return new WaitForSeconds(1f);
        waited = true;
    }

}
