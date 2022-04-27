using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles += new Vector3(Input.GetAxis("Rot Y"), Input.GetAxis("Rot X"), 0f) * Time.deltaTime * GameManager.current.settings.sensibility;
        Vector3 angle = transform.eulerAngles;

        if (angle.x > 180 && angle.x < 340)
        {
            angle.x = 340;
        }
        if (angle.x < 180 && angle.x > 40)
        {
            angle.x = 40;
        }

        if (angle.z != 0f)
            angle.z = 0f;

        transform.eulerAngles = angle;
    }
}
