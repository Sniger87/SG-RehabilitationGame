using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configurations;
using Profiles;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject LoadImage;
    public Slider VolumeSlider;
    public Toggle MuteToggle;
    public Toggle WeightUnitToggle;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        MuteToggle.isOn = ConfigManager.Current.GameConfig.Mute;
        WeightUnitToggle.isOn = ConfigManager.Current.GameConfig.WeightUnitLb;
        VolumeSlider.value = ConfigManager.Current.GameConfig.VolumeLevel;

        audioSource = GameInstance.Instance.MenuAudioSource;
        if (audioSource != null)
        {
            audioSource.mute = MuteToggle.isOn;
            audioSource.volume = VolumeSlider.value;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Cancel()
    {
        LoadImage.SetActive(true);
        ConfigManager.Current.LoadGameConfig();
        if (audioSource != null)
        {
            audioSource.mute = ConfigManager.Current.GameConfig.Mute;
            audioSource.volume = ConfigManager.Current.GameConfig.VolumeLevel;
        }
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }

    public void Save()
    {
        LoadImage.SetActive(true);
        ConfigManager.Current.GameConfig.Mute = MuteToggle.isOn;
        ConfigManager.Current.GameConfig.VolumeLevel = VolumeSlider.value;
        ConfigManager.Current.GameConfig.WeightUnitLb = WeightUnitToggle.isOn;
        ConfigManager.Current.SaveGameConfig();
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }

    public void OnVolumeChanged()
    {
        if (audioSource != null)
        {
            audioSource.volume = VolumeSlider.value;
        }

        if (VolumeSlider.value == 0)
        {
            MuteToggle.isOn = true;
        }
        else
        {
            MuteToggle.isOn = false;
        }
    }

    public void OnMuteChanged()
    {
        if (audioSource != null)
        {
            audioSource.mute = MuteToggle.isOn;
        }
    }
}
