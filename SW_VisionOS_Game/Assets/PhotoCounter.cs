using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhotoCounter : MonoBehaviour
{
    private TextMeshProUGUI countText;
    // Start is called before the first frame update
    void Start()
    {
        countText = GetComponent<TextMeshProUGUI>();
        int value = ValueManager.Instance.photoCount;
        countText.text = $"( {value} / 3 )";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}