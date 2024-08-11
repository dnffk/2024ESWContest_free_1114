using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NewsTab : MonoBehaviour
{
    public Button btn;
    public GameObject Stacker;
    public GameObject Icon;

    void Start()
    {
        btn.onClick.AddListener(ChangVal);
    }
    void ChangVal()
    {
        if (MemoManager.Instance.NewsTab == 0)
            MemoManager.Instance.NewsTabBool(1);   
        else
            MemoManager.Instance.NewsTabBool(0);
    }
    void Update()
    {
        if (MemoManager.Instance.NewsTab == 0)
        {
            Icon.SetActive(true);
            Stacker.SetActive(false);
        }
        else
        {
            Icon.SetActive(false);
            Stacker.SetActive(true);
        }
    }
}
