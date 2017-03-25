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
    public Slider MusicVolumeSlider;
    public Toggle MusicMuteToggle;
    public Slider EffectsVolumeSlider;
    public Toggle EffectsMuteToggle;
    public Toggle WeightUnitToggle;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        MusicVolumeSlider.value = ConfigManager.Current.GameConfig.MusicVolumeLevel;
        EffectsVolumeSlider.value = ConfigManager.Current.GameConfig.EffectsVolumeLevel;
        MusicMuteToggle.isOn = ConfigManager.Current.GameConfig.MusicMute;
        EffectsMuteToggle.isOn = ConfigManager.Current.GameConfig.EffectsMute;
        WeightUnitToggle.isOn = ConfigManager.Current.GameConfig.WeightUnitLb;

        audioSource = GameInstance.Instance.AudioSource;
        if (audioSource != null)
        {
            audioSource.mute = MusicMuteToggle.isOn;
            audioSource.volume = MusicVolumeSlider.value;
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
            audioSource.mute = ConfigManager.Current.GameConfig.MusicMute;
            audioSource.volume = ConfigManager.Current.GameConfig.MusicVolumeLevel;
        }
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }

    public void Save()
    {
        LoadImage.SetActive(true);
        ConfigManager.Current.GameConfig.MusicMute = MusicMuteToggle.isOn;
        ConfigManager.Current.GameConfig.MusicVolumeLevel = MusicVolumeSlider.value;
        ConfigManager.Current.GameConfig.EffectsMute = EffectsMuteToggle.isOn;
        ConfigManager.Current.GameConfig.EffectsVolumeLevel = EffectsVolumeSlider.value;
        ConfigManager.Current.GameConfig.WeightUnitLb = WeightUnitToggle.isOn;
        ConfigManager.Current.SaveGameConfig();
        SceneManager.LoadSceneAsync("MainMenu", LoadSceneMode.Single);
    }

    public void OnMusicVolumeChanged()
    {
        if (audioSource != null)
        {
            audioSource.volume = MusicVolumeSlider.value;
        }

        if (MusicVolumeSlider.value == 0)
        {
            MusicMuteToggle.isOn = true;
        }
        else
        {
            MusicMuteToggle.isOn = false;
        }
    }

    public void OnMusicMuteChanged()
    {
        if (audioSource != null)
        {
            audioSource.mute = MusicMuteToggle.isOn;
        }
    }

    public void OnEffectsVolumeChanged()
    {
        if (EffectsVolumeSlider.value == 0)
        {
            EffectsMuteToggle.isOn = true;
        }
        else
        {
            EffectsMuteToggle.isOn = false;
        }
    }

    public void OnEffectsMuteChanged()
    {

    }
}
