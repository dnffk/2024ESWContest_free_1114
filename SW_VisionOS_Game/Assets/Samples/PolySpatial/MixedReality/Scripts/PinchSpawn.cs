using UnityEngine;
#if UNITY_INCLUDE_XR_HANDS
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
#endif

public class PinchSpawn : MonoBehaviour
{
    [SerializeField]
    GameObject m_Canvas;

    [SerializeField]
    Transform m_PolySpatialCameraTransform;

    [SerializeField]
    float m_HandDistanceThreshold = 1.0f; // 손과 HMD 사이의 거리 임계값

    [SerializeField]
    float m_PoseGripThreshold = 0.1f; // 손의 잡는 포즈 거리 임계값

    [SerializeField]
    float m_HandHeightThresholdMin = 1.2f; // 손의 최소 높이 임계값

    [SerializeField]
    float m_HandHeightThresholdMax = 1.5f; // 손의 최대 높이 임계값

#if UNITY_INCLUDE_XR_HANDS
    XRHandSubsystem m_HandSubsystem;
    XRHandJoint m_RightIndexTipJoint;
    XRHandJoint m_RightThumbTipJoint;
    XRHandJoint m_RightMiddleTipJoint;
    XRHandJoint m_LeftIndexTipJoint;
    XRHandJoint m_LeftThumbTipJoint;
    XRHandJoint m_LeftMiddleTipJoint;

    bool m_RightHandPoseDetected;
    bool m_LeftHandPoseDetected;
    float m_PoseDetectionTimer;
    const float k_PoseStableDuration = 0.5f; // 포즈가 안정된 것으로 간주하는 시간

    void Start()
    {
        GetHandSubsystem();
    }

    void Update()
    {
        if (!CheckHandSubsystem())
            return;

        var updateSuccessFlags = m_HandSubsystem.TryUpdateHands(XRHandSubsystem.UpdateType.Dynamic);

        // 오른손 업데이트
        if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose) != 0)
        {
            m_RightIndexTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.IndexTip);
            m_RightThumbTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.ThumbTip);
            m_RightMiddleTipJoint = m_HandSubsystem.rightHand.GetJoint(XRHandJointID.MiddleTip);

            m_RightHandPoseDetected = DetectPose(m_RightIndexTipJoint, m_RightThumbTipJoint, m_RightMiddleTipJoint);
        }

        // 왼손 업데이트
        if ((updateSuccessFlags & XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose) != 0)
        {
            m_LeftIndexTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.IndexTip);
            m_LeftThumbTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.ThumbTip);
            m_LeftMiddleTipJoint = m_HandSubsystem.leftHand.GetJoint(XRHandJointID.MiddleTip);

            m_LeftHandPoseDetected = DetectPose(m_LeftIndexTipJoint, m_LeftThumbTipJoint, m_LeftMiddleTipJoint);
        }

        // 양손 모두 포즈를 취하고 있는지 확인
        if (m_RightHandPoseDetected && m_LeftHandPoseDetected)
        {
            if (Time.time - m_PoseDetectionTimer >= k_PoseStableDuration)
            {
                m_Canvas.SetActive(true);
            }
        }
        else
        {
            m_Canvas.SetActive(false);
            m_PoseDetectionTimer = Time.time; // 포즈를 감지하지 않으면 타이머 리셋
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

    bool DetectPose(XRHandJoint index, XRHandJoint thumb, XRHandJoint middle)
    {
        if (index.trackingState != XRHandJointTrackingState.None &&
            thumb.trackingState != XRHandJointTrackingState.None &&
            middle.trackingState != XRHandJointTrackingState.None)
        {
            if (index.TryGetPose(out Pose indexPose) &&
                thumb.TryGetPose(out Pose thumbPose) &&
                middle.TryGetPose(out Pose middlePose))
            {
                // 관절 간 거리 계산
                float thumbToIndex = Vector3.Distance(thumbPose.position, indexPose.position);
                float thumbToMiddle = Vector3.Distance(thumbPose.position, middlePose.position);

                // 손의 높이 확인
                float handHeight = thumbPose.position.y;
                float hmdHeight = m_PolySpatialCameraTransform.position.y;
                bool isHandHeightInRange = handHeight > hmdHeight - m_HandHeightThresholdMin && handHeight < hmdHeight + m_HandHeightThresholdMax;

                // 포즈 인식 및 거리 확인
                bool isPoseInRange = thumbToIndex < m_PoseGripThreshold && thumbToMiddle < m_PoseGripThreshold;
                bool isHandCloseToHmd = isHandHeightInRange;

                if (isPoseInRange && isHandCloseToHmd)
                {
                    if (m_PoseDetectionTimer == 0) // 처음으로 포즈 감지 시작
                    {
                        m_PoseDetectionTimer = Time.time; // 타이머 시작
                    }
                    return true;
                }
            }
        }
        m_PoseDetectionTimer = 0; // 포즈가 감지되지 않으면 타이머 초기화
        return false;
    }
#endif
}
