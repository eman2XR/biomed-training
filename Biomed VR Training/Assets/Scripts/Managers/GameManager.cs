using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject focusNotice;
    public GameObject logoReveal;

    private void Awake()
    {
        //if (!UserProgress.loadingScreenSeen)
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Intro");
    }

    private void Start()
    {
        logoReveal.SetActive(true);
        OVRManager.InputFocusLost += LostFocus;
        OVRManager.InputFocusAcquired += AcquiredFocus;
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MAIN");
    }
    
    void LostFocus()
    {
        focusNotice.SetActive(true);
    }

    void AcquiredFocus()
    {
        focusNotice.SetActive(false);
    }

}
