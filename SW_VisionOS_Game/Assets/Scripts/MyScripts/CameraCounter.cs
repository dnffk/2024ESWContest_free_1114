using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraCounter : MonoBehaviour
{
    public Camera CountCamera;
    public string targetTag;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
        int count = 0;

        foreach (GameObject obj in objects)
        {
            if (CheckObjectsInCamera(CountCamera, obj))
            {
                count ++;
            }
        }
        Debug.Log(count);
    }

    public bool CheckObjectsInCamera(Camera cam, GameObject obj)
    {
        Vector3 screenPoint = cam.WorldToViewportPoint(obj.transform.position);
        bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
        return onScreen;
    }
}
