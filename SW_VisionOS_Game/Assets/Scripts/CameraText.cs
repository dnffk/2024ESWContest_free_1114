using UnityEngine;
using TMPro;
using System;

public class CameraText : MonoBehaviour
{
    public TMP_Text dayText;
    public TMP_Text FilmText;
    public static int Flim = 5;
    public TMP_Text GhostPhotoCountText;

    public static int visiblePhotoCount = 0;

    void Start()
    {
        DateTime currentDate = DateTime.Now;
        string formattedDate = currentDate.ToString("MM/dd/yyyy");
        dayText.text = formattedDate;
    }

    void Update()
    {
        FilmText.text = Flim.ToString() + "/5";
        GhostPhotoCountText.text = visiblePhotoCount.ToString() + "/3";  // visiblePhotoCount 바로 표시
    }
}