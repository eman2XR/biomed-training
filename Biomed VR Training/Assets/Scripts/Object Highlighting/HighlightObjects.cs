//=============================================================================
// Purpose: Flashes and Highlights objects by attaching a 
// HighlightObjectInstance script to them
//=============================================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HeavyDutyInspector;

public class HighlightObjects : MonoBehaviour {

    #region Variables
    public static HighlightObjects instance;

    //list to hold objects being highlighed
    List<GameObject> objectsToHighlight = new List<GameObject>();
    #endregion

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Flashes the vrtk highlighter on an object
    /// </summary>
    /// <param name="objectToHighlight"></param>
    /// <param name="parentObject">set as the same object unless grayOut is true</param>
    public void HighlightThisObject(GameObject objectToHighlight)
    {
        //add to the list of objects to highlight
        if (!objectsToHighlight.Contains(objectToHighlight))
            objectsToHighlight.Add(objectToHighlight);

        //start the flashing
        objectToHighlight.AddComponent<HighlightObjectInstance>().Highlight();
        //objectToHighlight.GetComponent<HighlightObjectInstance>().Highlight();
    }

    public void StopAllFlashing()
    {
        foreach (GameObject obj in objectsToHighlight)
            if (obj.GetComponent<HighlightObjectInstance>())
            {
                obj.GetComponent<HighlightObjectInstance>().StopFlashing();
                Destroy(obj.GetComponent<HighlightObjectInstance>());
            }
    }

    public void StopFlashing(GameObject obj)
    {
        if (obj.GetComponent<HighlightObjectInstance>())
        {
            obj.GetComponent<HighlightObjectInstance>().StopFlashing();
            Destroy(obj.GetComponent<HighlightObjectInstance>());
        }
    }

}
