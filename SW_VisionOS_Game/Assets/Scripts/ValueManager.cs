using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ValueManager : MonoBehaviour
{
    private static ValueManager instance;
    public static ValueManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ValueManager>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<ValueManager>();
                    singletonObject.name = typeof(ValueManager).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    public int Game_Difficulty;
    public float Brightness_Value;
    public int Check_shutButton;

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

    public void Set_Game_Difficulty(int value)
    {
        Game_Difficulty = value;
        Debug.Log("Difficulty : " + Game_Difficulty);
    }

    public void Set_Brightness_Value(float value)
    {
        Brightness_Value = value;
        Debug.Log("Brightness : " + Brightness_Value);
    }

    public void Set_Check_shutButton(int value)
    {
        Check_shutButton = value;
        Debug.Log("ButtonPress : " + Check_shutButton);
    }
}
