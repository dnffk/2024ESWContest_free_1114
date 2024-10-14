using UnityEngine;
#if UNITY_INCLUDE_XR_HANDS
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
#endif

public class PinchSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject m_Canvas;

#if UNITY_INCLUDE_XR_HANDS
    XRHandSubsystem m_HandSubsystem;

    void Start()
    {
        GetHandSubsystem();
    }

    void Update()
    {
        if (!CheckHandSubsystem())
            return;

        var updateSuccessFlags = m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);

        // 오른손 또는 왼손이 감지되면 캔버스 활성화
        if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0 ||
            (updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
        {
            Debug.Log("손 인식성공 ");
            m_Canvas.SetActive(true);
        }
        else
        {
            Debug.Log("손 인식실패 ");
            m_Canvas.SetActive(false);
        }
    }

    void GetHandSubsystem()
    {
        var xrGeneralSettings = XRGeneralSettings.Instance;
        if (xrGeneralSettings == null)
        {
            Debug.LogError("XR general settings not set");
        }

        var manager = xrGeneralSettings.Manager;
        if (manager != null)
        {
            var loader = manager.activeLoader;
            if (loader != null)
            {
                m_HandSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                if (!CheckHandSubsystem())
                    return;

                m_HandSubsystem.Start();
            }
        }
    }

    bool CheckHandSubsystem()
    {
        if (m_HandSubsystem == null)
        {
#if !UNITY_EDITOR
            Debug.LogError("Could not find Hand Subsystem");
#endif
            enabled = false;
            return false;
        }

        return true;
    }
#endif
}