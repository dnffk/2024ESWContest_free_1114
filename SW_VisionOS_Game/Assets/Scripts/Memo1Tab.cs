using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Memo1Tab : MonoBehaviour
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
        if (MemoManager.Instance.Memo1Tab == 0)
            MemoManager.Instance.Memo1TabBool(1);   
        else
            MemoManager.Instance.Memo1TabBool(0);
    }
    void Update()
    {
        if (MemoManager.Instance.Memo1Tab == 0)
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
