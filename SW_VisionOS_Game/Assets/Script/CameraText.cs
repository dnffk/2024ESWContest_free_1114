using UnityEngine;
using TMPro;
using System;

public class CameraText : MonoBehaviour
{
    public TMP_Text dayText;
    public TMP_Text FilmText;
    public static int Flim = 3;

    void Start()
    {
        DateTime currentDate = DateTime.Now;
        string formattedDate = currentDate.ToString("MM/dd/yyyy");
        dayText.text = formattedDate;
    }

    void Update()
    {
        FilmText.text = Flim.ToString();
    }
}