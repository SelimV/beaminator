using UnityEngine;

public class RotateTowardsCamera : MonoBehaviour
{
    void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 direction = new Vector3(cameraPosition.x - transform.position.x, 0, cameraPosition.z - transform.position.z);
        Quaternion targeRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Euler(0, targeRotation.eulerAngles.y, 0);
    }
}
