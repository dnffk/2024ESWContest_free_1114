using UnityEngine;
using Unity.PolySpatial;

public class BatchModeUpdateRenderer : MonoBehaviour
{
    Camera m_Camera;

    void Start()
    {
        m_Camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (Application.isBatchMode && m_Camera)
            m_Camera.Render();
    }
}