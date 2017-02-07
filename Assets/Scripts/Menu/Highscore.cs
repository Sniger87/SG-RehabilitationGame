using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Profiles;
using System.Text;
using UnityEngine.SceneManagement;
using System.Linq;

public class Highscore : MonoBehaviour
{
    public GameObject LoadImage;
    public Text HighscoreList;

    // Use this for initialization
    void Start()
    {
        StringBuilder sb = new StringBuilder();
        int j = 1;
        int maxCount = 10;

        List<int> highscores = new List<int>();

        if (ProfileManager.Current.CurrentPlayer != null)
        {
            highscores.AddRange(ProfileManager.Current.CurrentPlayer.Highscores);
        }

        if (!highscores.Any())
        {
            sb.AppendLine("Not Available");
        }
        else
        {
            highscores.Sort();

            for (int i = 0; i < highscores.Count; i++)
            {
                if (j == maxCount)
                {
                    break;
                }
                sb.AppendLine(string.Format("{0}: {}", j, highscores[i]));
                j++;
            }
        }


        HighscoreList.text = sb.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Back()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
