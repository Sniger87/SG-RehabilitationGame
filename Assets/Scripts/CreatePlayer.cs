using System.Collections;
using System.Collections.Generic;
using Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreatePlayer : MonoBehaviour
{
    public InputField Name;
    public GameObject LoadImage;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Create()
    {
        if (Name != null && !string.IsNullOrEmpty(Name.text))
        {
            LoadImage.SetActive(true);
            ProfileManager.Current.CreatePlayer(Name.text);
            SceneManager.LoadSceneAsync("Login", LoadSceneMode.Single);
        }
    }

    public void Cancel()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("Login", LoadSceneMode.Single);
    }
}
