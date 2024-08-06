using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIMenuManager : MonoBehaviour
{
    public Transform head;
    //public GameObject menu;
    public GameObject menu2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    //    menu.transform.LookAt(new Vector3 (head.position.x, menu.transform.position.y, head.position.z));
    //    menu.transform.forward *= 1;
        menu2.transform.LookAt(new Vector3 (head.position.x, menu2.transform.position.y, head.position.z));
        menu2.transform.forward *= -1;
    }
}
