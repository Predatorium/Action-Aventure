using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private Settings settings = null;
    [SerializeField] private TextMeshProUGUI text = null;
    [SerializeField] private Slider slider = null;
    [SerializeField] private Toggle toggle = null;
    [SerializeField] private EventSystem system = null;

    private void Awake()
    {
        slider.value = settings.sensibility / 40f;
        text.text = (settings.sensibility / 40f).ToString();

        toggle.isOn = settings.fullscreen;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        settings.Save();
    }

    private void OnEnable()
    {
        system.SetSelectedGameObject(slider.gameObject);
        settings.Load();
    }

    public void ChangeSensibility(float value)
    {
        settings.sensibility = value * 40;
        text.text = value.ToString();
    }

    public void FullScreen(bool isFull)
    {
        settings.fullscreen = isFull;
        Screen.fullScreen = settings.fullscreen;
        Screen.SetResolution(1920, 1080, settings.fullscreen);
    }
}
