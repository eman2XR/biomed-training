using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AutoExtensions{
    /// <summary>Autohand extension method, used so I can use TryGetComponent for newer versions and GetComponent for older versions</summary>
    public static bool CanGetComponent<T>(this Component componentClass, out T component) {
#if UNITY_2019_1 || UNITY_2018 || UNITY_2017
       var tempComponent = componentClass.GetComponent<T>();
        if(tempComponent != null){
            component = tempComponent;
            return true;
        }
        else {
            component = tempComponent;
            return false;
        }
#else
        var value = componentClass.TryGetComponent(out component);
        return value;
#endif
    }

    /// <summary>Autohand extension method, used so I can use TryGetComponent for newer versions and GetComponent for older versions</summary>
    public static bool CanGetComponent<T>(this GameObject componentClass, out T component) {
#if UNITY_2019_1 || UNITY_2018 || UNITY_2017
       var tempComponent = componentClass.GetComponent<T>();
        if(tempComponent != null){
            component = tempComponent;
            return true;
        }
        else {
            component = tempComponent;
            return false;
        }
#else
        var value = componentClass.TryGetComponent(out component);
        return value;
#endif
    }
}
