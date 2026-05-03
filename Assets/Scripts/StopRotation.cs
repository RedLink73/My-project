using UnityEngine;

public class StopRotation : MonoBehaviour
{
 // Source - https://stackoverflow.com/a/54463641
// Posted by derHugo, modified by community. See post 'Timeline' for change history
// Retrieved 2026-05-03, License - CC BY-SA 4.0

[SerializeField] private float rotationspeed;

private void FixedUpdate()
{
    // rotate around global world Y
    transform.Rotate(Input.GetAxis("Mouse X") * rotationspeed * Time.deltaTime, 0, 0, Space.World);

    // rotate around local X
    transform.Rotate(0, -Input.GetAxis("Mouse Y") * rotationspeed * Time.deltaTime, 0);
}

}
