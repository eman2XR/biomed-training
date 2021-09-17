using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return new WaitForSeconds(4);
        UserProgress.loadingScreenSeen = true;
        ChangeScene();
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene("MAIN");
    }
}
