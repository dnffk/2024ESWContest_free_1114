using UnityEngine;

public class CanvasFollowCamera : MonoBehaviour
{
	public Transform cameraTransform;
	public float distanceFromCamera = 0.5f;

	void Update()
	{
		transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
		transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
	}
}