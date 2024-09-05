using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class JY_LoadPhotoEnding1 : MonoBehaviour
{
    public RawImage screenshotDisplay1;
    public RawImage screenshotDisplay2;
    public RawImage screenshotDisplay3;

    void Start()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "noCapture");

        if (Directory.Exists(folderPath))
        {
            // 폴더 내의 모든 파일을 생성 날짜에 따라 내림차순 정렬
            var files = new DirectoryInfo(folderPath).GetFiles()
                .OrderByDescending(f => f.CreationTime)
                .Take(3) // 가장 최근의 3개 파일을 선택
                .ToList();

            if (files.Count > 0)
            {
                if (files.Count > 0) LoadScreenshot(files[0].FullName, screenshotDisplay1);
                if (files.Count > 1) LoadScreenshot(files[1].FullName, screenshotDisplay2);
                if (files.Count > 2) LoadScreenshot(files[2].FullName, screenshotDisplay3);
            }
            else
            {
                Debug.LogError("No screenshots found in the specified folder.");
            }
        }
        else
        {
            Debug.LogError("Screenshot folder not found.");
        }
    }

    public void LoadScreenshot(string filePath, RawImage display)
    {
        byte[] fileData = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData);

        display.texture = texture;
        display.gameObject.SetActive(true);
    }
}
