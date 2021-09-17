using System.Collections;
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

    int counter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "gasket")
        {
            this.transform.parent.GetChild(counter).gameObject.SetActive(true); //activate old gasket visual
            gasketIn = true;
            obj = other.gameObject;
            obj.transform.parent.GetComponent<Gasket>().DropGasket();
            audio.Play();
            onGasketIn.Invoke();

            counter++;
            if (counter == 4)
                allGasketsIn = true;

            //Destroy(other.GetComponent<Follow>());
            //Destroy(other.GetComponent<Follow>());
            //other.GetComponent<Rigidbody>().isKinematic = false;
            //other.GetComponent<Collider>().isTrigger = false;
            //other.transform.position = this.transform.position;
            //other.GetComponent<Collider>().isTrigger = false;
            //Destroy(other.transform.parent.gameObject);
        }
    }

    //IEnumerator DestroyObject()
    //{
        //yield return new WaitForSeconds(0.5f);
        //obj.GetComponent<Rigidbody>().isKinematic = true;
    //}

}
