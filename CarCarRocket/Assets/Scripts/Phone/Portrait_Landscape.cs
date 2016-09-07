using UnityEngine;
using System.Collections;

public class Portrait_Landscape : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (Input.deviceOrientation == DeviceOrientation.Portrait)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 90); 
        }
    }
}
