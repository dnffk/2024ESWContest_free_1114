using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MemoManager : MonoBehaviour
{
    private static MemoManager instance;
    public static MemoManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MemoManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<MemoManager>();
                    singletonObject.name = typeof(MemoManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    public int NewsTab = 1;
    public int Memo1Tab = 1;
    public int Memo2Tab = 1;
    public int Memo3Tab = 1;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void NewsTabBool(int value)
    {
        NewsTab = value;
        Debug.Log("News Tab on/off : " + NewsTab);
    }

    public void Memo1TabBool(int value)
    {
        Memo1Tab = value;
        Debug.Log("Memo1 Tab on/off : " + Memo1Tab);
    }
    public void Memo2TabBool(int value)
    {
        Memo2Tab = value;
        Debug.Log("Memo2 Tab on/off : " + Memo2Tab);
    }
    public void Memo3TabBool(int value)
    {
        Memo3Tab = value;
        Debug.Log("Memo3 Tab on/off : " + Memo3Tab);
    }
    
}
