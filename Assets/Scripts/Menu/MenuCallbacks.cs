using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuCallbacks : MonoBehaviour
{
    public void Start()
    {
        sfx = GameObject.Find("SFX").GetComponent<SFX>();
    }

    public void LaunchGame()
    {
        SceneManager.LoadScene("BaseScene");
        sfx.PlayerOnShot(0);
    }

    public void Quit()
    {
        Application.Quit();
        sfx.PlayerOnShot(0);
    }

    SFX sfx;
}
