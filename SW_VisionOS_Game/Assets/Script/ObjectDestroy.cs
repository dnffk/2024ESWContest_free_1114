using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.PolySpatial.InputDevices;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.EnhancedTouch;

public class ObjectDestroy : MonoBehaviour
{
    public AudioSource Scream;
    SpatialPointerState touchData;

    private void OnEnable()
    {
        EnhancedTouchSupport.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            foreach (var touch in Touch.activeTouches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    touchData = EnhancedSpatialPointerSupport.GetPointerState(touch);
                    if (touchData.targetObject != null)
                    {
                        RenderManager render = touchData.targetObject.GetComponent<RenderManager>();
                        NavGhost navGhost = touchData.targetObject.GetComponent<NavGhost>();

                        navGhost.canMove = false;
                        render.PlayParticle();
                        Scream.Play();
                        render.StartFadeOut(3f);
                        break;
                    }
                }
            }
        }
    }
}

