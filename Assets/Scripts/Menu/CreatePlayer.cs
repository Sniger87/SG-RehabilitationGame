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
    public Button CreateButton;

    // Use this for initialization
    void Start()
    {
        CheckInputField();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputField();
    }

    private void CheckInputField()
    {
        if (Name != null && !string.IsNullOrEmpty(Name.text))
        {
            CreateButton.interactable = true;
        }
        else
        {
            CreateButton.interactable = false;
        }
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
