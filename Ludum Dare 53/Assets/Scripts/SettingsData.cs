using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[CreateAssetMenu(menuName = "SettingsData" ,fileName = "SettingsData")]
public class SettingsData : ScriptableObject
{
    [Range(0,1)]
    public float musicVolume = 0.6f;
    
    [Range(0,1)]
    public float effectVolume = 0.6f;
    
    [Range(0, 2)]
    public float mouseSensitivity = 0.4f;
}
