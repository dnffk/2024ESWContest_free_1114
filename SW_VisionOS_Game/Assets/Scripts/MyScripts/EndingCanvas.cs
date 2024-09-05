using UnityEngine;

public class EndingCanvas : MonoBehaviour
{
	public Transform cameraTransform;
	public float distanceFromCamera = 0.5f;

	void Start()
	{
		transform.position = cameraTransform.position + cameraTransform.forward * distanceFromCamera;
		transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
	}
}