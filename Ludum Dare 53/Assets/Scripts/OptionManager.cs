using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionManager : MonoBehaviour
{

    public SettingsData settingsData;

    public AudioMixer mixer;

    public Slider musicVolume;
    public Slider effectVolume;
    public Slider mouseSensitivity;

    private const string MIXER_MUSIC = "MusicVolume";
    private const string MIXER_EFFECTS = "EffectsVolume";
    
    private void Awake()
    {
        musicVolume.value = settingsData.musicVolume;
        effectVolume.value = settingsData.effectVolume;
        mouseSensitivity.value = settingsData.mouseSensitivity;
    }

    // Start is called before the first frame update
    void Start()
    {
        musicVolume.onValueChanged.AddListener((value) =>
        {
            settingsData.musicVolume = value;
            mixer.SetFloat(MIXER_MUSIC, Mathf.Log10(value) * 20);
        });
        
        effectVolume.onValueChanged.AddListener((value) =>
        {
            settingsData.effectVolume = value;
            mixer.SetFloat(MIXER_EFFECTS, Mathf.Log10(value) * 20);
        });
    }
}
