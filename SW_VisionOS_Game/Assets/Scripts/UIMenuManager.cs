using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class UIMenuManager : MonoBehaviour
{
    public Transform head;
    //public GameObject menu;
    public GameObject menu2;
    // Start is called before the first frame update
    private Vector3 initialPosition;
    private Vector3 finalPosition;
    private bool isFinalScene;
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //transform.position = new Vector3(transform.position.x, head.position.y, transform.position.z) + menu2.transform.forward * 2;
    }
    // Update is called once per frame
    void Update()
    {
        //    menu.transform.LookAt(new Vector3 (head.position.x, menu.transform.position.y, head.position.z));
        //    menu.transform.forward *= 1;
        //menu2.transform.LookAt(new Vector3(head.position.x, menu2.transform.position.y, head.position.z));
        //menu2.transform.forward *= -1;
        PositionUIEnding();
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        /*
        XROrigin xrOrigin = FindObjectOfType<XROrigin>();
        if (xrOrigin != null)
        {
            head = xrOrigin.Camera.transform; // XROrigin의 카메라를 참조
            //PositionUIEnding();
        }
        else
        {
            Debug.LogWarning("XROrigin을 찾을 수 없습니다.");
        }

        if (scene.name == "IntroScene")
        {
            Debug.Log("IntroScene Start");
            initialPosition = transform.position;
            Vector3 distanceMoved = initialPosition - finalPosition;
            menu2.transform.position += distanceMoved;
        }
        if (scene.name == "Ending1")
            isFinalScene = true;
        else if (scene.name == "Ending2")
            isFinalScene = true;
        else if (scene.name == "Ending3")
            isFinalScene = true;
        if (isFinalScene)
        {
            Debug.Log("EndingScene Start");
            finalPosition = transform.position;
            Vector3 distanceMoved = finalPosition - initialPosition;
            menu2.transform.position += distanceMoved;
            isFinalScene = false;
        }
        */
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void PositionUIEnding()
    {
        menu2.transform.LookAt(new Vector3(head.position.x, menu2.transform.position.y, head.position.z));
        menu2.transform.forward *= -1;
    }
}









