using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DropDownController : MonoBehaviour
{
    string DROPDOWN_KEY = "DROPDOWN_KEY";

    int currentOption;
    TMP_Dropdown options;

    void Awake()
    {
        if (PlayerPrefs.HasKey(DROPDOWN_KEY) == false) currentOption = 0;
        else currentOption = PlayerPrefs.GetInt(DROPDOWN_KEY);
    }

    // Start is called before the first frame update
    void Start()
    {
        options = this.GetComponent<TMP_Dropdown>();
        options.value = currentOption;

        options.onValueChanged.AddListener(delegate {setDropDown(options.value);});
        setDropDown(currentOption);
    }

    void setDropDown(int option)
    {
        PlayerPrefs.SetInt(DROPDOWN_KEY, option);

        ValueManager.Instance.Set_Game_Difficulty(option);

        Debug.Log("Difficulty : " + ValueManager.Instance.Game_Difficulty);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
