using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashUI : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(GoToLoobyScene), 1.0f);
    }

    public void OnClickStartButton()
    {
        //Invoke(nameof(GoToLoobyScene), 1.0f);
    }

    private void GoToLoobyScene()
    {
        SceneManager.LoadScene("Lobby");
    }
}
