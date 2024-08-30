using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
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
    public float Lightness_Value;
    public int Check_shutButton;
    public int Check_lightButton;
    public float Check_Lpwr;

    private string filePath;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.persistentDataPath, "debugData.json");
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
        SaveDebugDataToJson();
    }
    public void Set_Lightness_Value(float value)
    {
        Lightness_Value = value;
        if(value < 1000)
        {
            Lightness_Value = 0.5f;
        }
        else if(value < 2000)
        {
            Lightness_Value = 0.7f;
        }
        Debug.Log("Lightness : " + Lightness_Value);
        SaveDebugDataToJson();
    }
    public void Set_Check_shutButton(int value)
    {
        Check_shutButton = value;
        Debug.Log("ButtonPress : " + Check_shutButton);
        SaveDebugDataToJson();
    }
    public void Set_Check_lightButton(int value)
    {
        Check_lightButton = value;
        Debug.Log("LightButtonPress : " + Check_lightButton);
        SaveDebugDataToJson();
    }
    public void Set_Check_Lpwr(float value)
    {
        Check_Lpwr = value;
        Debug.Log("Lpwr : " + Check_Lpwr);
        SaveDebugDataToJson();
    }

    private void SaveDebugDataToJson()
    {
        DebugData data = new DebugData(Game_Difficulty, Lightness_Value, Check_shutButton, Check_lightButton, Check_Lpwr);
        string jsonData = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log("Debug data saved to : " + filePath);
    }
    public string LoadDebugData()
    {
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }
        else
        {
            Debug.Log("No debug data found.");
            return null;
        }
    }
}

[System.Serializable]
public class DebugData
{
    public int Game_Difficulty;
    public float Lightness_Value;
    public int Check_shutButton;
    public int Check_lightButton;
    public float Check_Lpwr;

    public DebugData(int Game_Difficulty, float Lightness_Value, int Check_shutButton, int Check_lightButton, float Check_Lpwr)
    {
        this.Game_Difficulty = Game_Difficulty;
        this.Lightness_Value = Lightness_Value;
        this.Check_lightButton = Check_lightButton;
        this.Check_Lpwr = Check_Lpwr;
    }
}