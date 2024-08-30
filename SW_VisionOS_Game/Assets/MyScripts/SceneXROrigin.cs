using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneXROrigin : MonoBehaviour
{
    private Vector3 xrOriginPosition;
    private Quaternion xrOriginRotation;

    private void SaveXROriginState()
    {
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin != null)
        {
            xrOriginPosition = xrOrigin.transform.position;
            xrOriginRotation = xrOrigin.transform.rotation;
        }
    }

    private void RestoreXROriginState()
    {
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin != null)
        {
            xrOrigin.transform.position = xrOriginPosition;
            xrOrigin.transform.rotation = xrOriginRotation;
        }
    }

    public void TransitionToScene(string sceneName)
    {
        SaveXROriginState();
        SceneManager.LoadScene(sceneName);
        RestoreXROriginState();
    }
}

