using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDirectionsAndHaptics : MonoBehaviour {

    public AudioSource notificationAudio;
    public GameObject lineDirectionPrefab;
    bool playNotification;
    GameObject lineDirection;

    public void StartHandNotification(Transform targetObject)
    {
        //move audio to the right position and play
        notificationAudio.transform.position = targetObject.transform.position;
        notificationAudio.Play();

        //instantiate the line direction and move each end to the right place
        lineDirection = Instantiate(lineDirectionPrefab, targetObject.position, Quaternion.identity);
        //ExtensionMethods.GetChild("Start", lineDirection.transform).transform.position = VRTK.VRTK_DeviceFinder.GetControllerRightHand().transform.position;
        //ExtensionMethods.GetChild("Start", lineDirection.transform).transform.parent = VRTK.VRTK_DeviceFinder.GetControllerRightHand().transform;
        //ExtensionMethods.GetChild("End", lineDirection.transform).transform.position = targetObject.position;

        playNotification = true;
        HapticLoop();
    }

    public void EndHandNotification()
    {
        Destroy(lineDirection);
        notificationAudio.Stop();
        playNotification = false;
    }

    void HapticLoop()
    {
        if (playNotification)
        {
            //VRTK.VRTK_ControllerHaptics.TriggerHapticPulse(VRTK.VRTK_DeviceFinder.GetControllerReferenceRightHand(), notificationAudio.clip);
            Run.After(0.6f, () =>
            {
                HapticLoop();
            });
        }
    }

}
