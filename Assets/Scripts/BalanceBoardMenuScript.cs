using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Wii.Exceptions;

public class BalanceBoardMenuScript : MonoBehaviour
{
    public GameObject LoadImage;
    public GameObject ConnectingImage;
    public Text InfoText;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Connect()
    {
        ConnectingImage.SetActive(true);

        BalanceBoardInstance.Instance.Connect();

        if (BalanceBoardInstance.Instance.IsBalanceBoardConnected)
        {
            InfoText.text = "BalanceBoard found";
        }
        else
        {
            InfoText.text = "BalanceBoard not found";
        }
        ConnectingImage.SetActive(false);
    }

    public void Back()
    {
        LoadImage.SetActive(true);
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }
}
