using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public static class ExtensionMethods
    {
        public static float RemapValue(this float value, float outputLow, float outputHigh, float from2, float to2)
        {
            return (value - outputLow) / (outputHigh - outputLow) * (to2 - from2) + from2;
        }

    /// <summary>
    /// Returns last child in specified parent
    /// </summary>
    /// <param name="parentTransform"></param>
    /// <returns></returns>
    public static Transform LastChid(Transform parentTransform)
    {
        Transform lastChild = parentTransform.GetChild(parentTransform.childCount - 1);
        return lastChild;
    }


    /// <summary>
    /// Returns child by name in a given transform 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parentObject"></param>
    /// <returns></returns>
    public static GameObject GetChild(string name, Transform parentObject)
    {
        //loop through all childern
        Transform[] allChildren = parentObject.GetComponentsInChildren<Transform>(true);
        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in allChildren)
        {
            if (child.name == name)
            { return child.gameObject; }
        }
        return null;
    }
}
