using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Settings", menuName = "ScriptableObjects/Settings")]
public class Settings : ScriptableObject
{
    public float sensibility = 200f;
    public bool fullscreen = true;

    public void Save()
    {
        PlayerPrefs.SetFloat("Sensibility", sensibility);
        PlayerPrefs.SetInt("FullScreen", fullscreen ? 1 : 0);
    }

    public void Load()
    {
        sensibility = PlayerPrefs.GetFloat("Sensibility", 200);
        fullscreen = PlayerPrefs.GetInt("FullScreen", 1) == 1 ? true : false;
        Screen.fullScreen = fullscreen;
    }
}
