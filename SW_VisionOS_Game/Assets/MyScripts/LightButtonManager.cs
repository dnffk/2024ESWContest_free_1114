using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LightButtonManager : MonoBehaviour
{
    public GameObject vignette;
    public Image backGroundImage;

    void Update()
    {
        int dd = ValueManager.Instance.Check_lightButton;
        Debug.Log("dd는" + dd);

        if (dd == 1)
        {
            vignette.SetActive(true);
            backGroundImage.enabled = false;
        }
        else if (dd == 0)
        {
            vignette.SetActive(false);
            backGroundImage.enabled = true;
        }
    }
}