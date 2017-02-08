using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject PausePanel;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                PausePanel.SetActive(true);
            }
        }
    }

    public void Back()
    {
        PausePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        GameInstance.Instance.MenuAudioSource.Play();
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
