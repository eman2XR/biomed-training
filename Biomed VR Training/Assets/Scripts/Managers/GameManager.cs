using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject logoReveal;

    private void Awake()
    {
        //if (!UserProgress.loadingScreenSeen)
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Intro");
    }

    private void Start()
    {
        logoReveal.SetActive(true);
    }

    void FocusLost()
    {

    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MAIN");
    }
}
