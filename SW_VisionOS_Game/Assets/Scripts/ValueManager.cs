using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.IO;
using System;
public class ValueManager : MonoBehaviour
{
    private static readonly object fileLock = new object();
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
    public Vector3 playerTransform;
    public int endingNum;
    public string sceneName;
    public string currentSceneName;
    public bool serverState;
    public int shutterCounter;
    public int completeShutterCounter;
    public Quaternion cameraRotation;

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

    public void SetPlayerTransform(Vector3 value)
    {
        this.playerTransform = value;
        SaveDebugDataToJson();
    }
    public void SetEnding(int value)
    {
        this.endingNum = value;
        SaveDebugDataToJson();
    }
    public void ChangeScene(string sceneName, string currentSceneName)
    {
        this.sceneName = sceneName;
        this.currentSceneName = currentSceneName;
        SaveDebugDataToJson();
    }
    public void CheckServerState(bool value)
    {
        this.serverState = value;
        SaveDebugDataToJson();
    }
    public void ShutterCounter(int value)
    {
        if (value == 1)
            shutterCounter++;
        else
            shutterCounter = 0;
        SaveDebugDataToJson();
    }
    public void CompletedShutterCounter(int value)
    {
        if (value == 1)
            completeShutterCounter++;
        else
            completeShutterCounter = 0;
        SaveDebugDataToJson();
    }
    public void CheckCameraRotation(Quaternion value)
    {
        this.cameraRotation = value;
        SaveDebugDataToJson();
    }

    private void SaveDebugDataToJson()
    {
        try
        {
            lock(fileLock)
            {
                DebugData data = new DebugData(serverState, Game_Difficulty, Lightness_Value, playerTransform, endingNum, sceneName, currentSceneName, shutterCounter, completeShutterCounter, cameraRotation);

                string jsonData = JsonUtility.ToJson(data, true);
                File.WriteAllText(filePath, jsonData);
                Debug.Log("Debug data saved to : " + filePath);

            }
        }
        catch (Exception e)
        {
            Debug.Log("File write operation failed " + e.Message);
        }
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
    void Update()
    {
        Vector3 playerPosition = Camera.main.transform.position;
        SetPlayerTransform(playerPosition);
    }
}



[System.Serializable]
public class DebugData
{
    public bool serverState;
    public int Game_Difficulty;
    public float Lightness_Value;
    public Vector3 playerTransform;
    public Quaternion cameraRotation;
    public string sceneName;
    public string currentSceneName;
    public int shutterCounter;
    public int completeShutterCounter;
    public int endingNum;

    public DebugData(bool serverState, int Game_Difficulty, float Lightness_Value, Vector3 playerTransform, int endingNum, string sceneName, string currentSceneName, int shutterCounter, int completeShutterCounter, Quaternion cameraRotation)
    {
        this.Game_Difficulty = Game_Difficulty;
        this.Lightness_Value = Lightness_Value;
        this.serverState = serverState;
        this.playerTransform = playerTransform;
        this.endingNum = endingNum;
        this.sceneName = sceneName;
        this.currentSceneName = currentSceneName;
        this.shutterCounter = shutterCounter;
        this.completeShutterCounter = completeShutterCounter;
        this.cameraRotation = cameraRotation;
    }
}