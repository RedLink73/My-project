using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    private GameObject entity;
    private Camera mainCamera;

    public CameraHandler(GameObject entity)
    {
        this.entity = entity;
        mainCamera = Camera.main;
       
    }

    public void Update()
    {
        if (entity == null !)
            return;
        
        FollowTarget(entity.transform);
    }

    public void FollowTarget(Transform target)
    {
        Vector3 targetPosition = target.position;
        targetPosition.z = -10f; // Set a fixed z position for the camera
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position,
            targetPosition, Time.deltaTime * 5f);
    }
}