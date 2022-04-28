using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[System.Serializable]
public class QTE
{
    public GameObject UI = null;
    public string inputName = "";
}

public class QTEManager : MonoBehaviour
{
    [SerializeField] private QTE[] qtes = null;
    private QTE currentQTE = null;
    [SerializeField] private float intervalQTE = 0f;
    [SerializeField] private int numberQTE = 2;
    private int successQTE = 0;
    private float timeQTE = 0f;

    [SerializeField] private Image image = null;

    // Update is called once per frame
    void Update()
    {
        image.fillAmount = (intervalQTE - timeQTE) / intervalQTE;

        if (currentQTE == null)
        {
            if (successQTE < numberQTE)
            {
                currentQTE = qtes[Random.Range(0, qtes.Length)];
                currentQTE.UI.SetActive(true);
            }
            else if (successQTE == numberQTE)
            {
                GameManager.current.WinQTE();
                successQTE = 0;
            }
        }
        else
        {
            timeQTE += Time.deltaTime;

            if (Input.GetButtonDown(currentQTE.inputName))
            {
                currentQTE.UI.gameObject.SetActive(false);
                currentQTE = null;
                successQTE++;
                timeQTE = 0f;
            }

            if (timeQTE >= intervalQTE)
            {
                currentQTE.UI.gameObject.SetActive(false);
                timeQTE = 0f;
                successQTE = 0;
                currentQTE = null;
                GameManager.current.LooseQTE();
            }
        }
    }
}
