using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightButtonManager : MonoBehaviour
{
    public GameObject vignette;

    void Update()
    {
        int dd = ValueManager.Instance.Check_lightButton;

        if (dd == 1)
        {
            vignette.SetActive(true);
        }
        else if (dd == 0)
        {
            vignette.SetActive(false);
        }
    }
}