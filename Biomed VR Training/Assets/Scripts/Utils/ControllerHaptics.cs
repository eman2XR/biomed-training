using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHaptics : MonoBehaviour
{
    public static ControllerHaptics instance;

    static bool vibrating;

    private void Awake()
    {
        instance = this;
    }

    public void CreateVibrateTime(int iteration, int frequency, int strength, OVRInput.Controller controller, float time)
    {
        StartCoroutine(DoCreateVibrateTime(iteration, frequency, strength, controller, time));
    }

    IEnumerator DoCreateVibrateTime(int iteration, int frequency, int strength, OVRInput.Controller controller, float time)
    {
        if (vibrating)
        {
            yield break;
        }

        vibrating = true;
        var channel = OVRHaptics.RightChannel;

        switch (controller)
        {
            case OVRInput.Controller.LTouch:
                channel = OVRHaptics.LeftChannel;
                break;
            case OVRInput.Controller.RTouch:
                channel = OVRHaptics.RightChannel;
                break;
            default:
                break;
        }

        OVRHapticsClip createdClip;
        createdClip = new OVRHapticsClip(iteration);

        for (int i = 0; i < iteration; i++)
        {
            createdClip.Samples[i] = i % frequency == 0 ? (byte)0 : (byte)strength;
        }

        createdClip = new OVRHapticsClip(createdClip.Samples, createdClip.Samples.Length);

        for (float t = 0; t <= time; t += Time.deltaTime)
        {
            //Debug.Log("Play vib");
            channel.Queue(createdClip);
        }
        yield return new WaitForSeconds(time);
        channel.Clear();
        vibrating = false;
        yield return null;
    }
}
