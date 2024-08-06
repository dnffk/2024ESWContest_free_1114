using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIMenuManager2 : MonoBehaviour
{
    public Transform head;
    public GameObject menu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        menu.transform.LookAt(new Vector3 (head.position.x, menu.transform.position.y, head.position.z));
        menu.transform.forward *= 1;
    }
}
