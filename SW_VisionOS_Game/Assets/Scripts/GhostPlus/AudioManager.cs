using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource Whisper;

    // Start is called before the first frame update
    void Start()
    {
        // 처음에 소리를 끔
        Whisper.volume = 0.0f;
        Whisper.Play(); // 소리 재생 시작, 초기 볼륨은 0
    }

    // Update is called once per frame
    void Update()
    {
        float hp = GhostManager.Instance.HP; // 현재 체력 가져오기

        // 체력이 30 이하일 때만 볼륨 조절
        if (hp <= 50)
        {
            // 체력이 30일 때는 최소 볼륨, 0일 때는 최대 볼륨으로 설정
            float volume = Mathf.Lerp(0.2f, 1, (50 - hp) / 50);
            Whisper.volume = volume;

            // 소리가 재생 중이 아닐 때만 재생
            if (!Whisper.isPlaying)
            {
                Whisper.Play();
            }
        }

        else
        {
            // 체력이 30 이상일 때는 음소거
            Whisper.volume = 0.0f;
        }
    }

    public void StopAudio()
    {
        Whisper.Stop();
    }
}
