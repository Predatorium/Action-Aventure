using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiLifePlayer : MonoBehaviour
{
    [SerializeField] private Image barreLife = null;
    [SerializeField] private Entity character = null;

    // Update is called once per frame
    void Update()
    {
        if (character)
            barreLife.fillAmount = character.GetPourcentageLife();
    }
}
