using System.Collections;
using System.Collections.Generic;
using Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public GameObject PausePanel;
    public static GameUI Instance;

    public bool IsPause
    {
        get
        {
            return this.isPause;
        }
    }

    private bool isPause;

    // Use this for initialization
    void Start()
    {

    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (isPause)
                {
                    isPause = false;
                    PausePanel.SetActive(false);
                }
                else
                {
                    isPause = true;
                    PausePanel.SetActive(true);
                }
            }
        }
    }

    public void Back()
    {
        isPause = false;
        PausePanel.SetActive(false);
    }

    public void BackToMenu()
    {
        int highscore = ProfileManager.Current.CurrentPlayer.CurrentHighscore;
        if (highscore != 0)
        {
            ProfileManager.Current.CurrentPlayer.Highscores.Add(highscore);
        }
        DestroyImmediate(GameObject.FindGameObjectWithTag("KI"), true);
        GameInstance.Instance.SetMenuClip();
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
