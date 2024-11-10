using UnityEngine;

public class ObjectScaler : MonoBehaviour
{
    [SerializeField] private Transform handA;
    [SerializeField] private Transform handB;
    [SerializeField] private Transform targetObject;
    [SerializeField] private float scalingSpeed = 1f;
    public Transform TargetObject { get => targetObject;  set => targetObject = value;  }
    private void Awake()
    {
        this.enabled = false;
    }
    public void EnableScaling()
    {
        previousDistance = Vector3.Distance(handA.position, handB.position);
        newDistance = previousDistance;
        scaleAmount = 0f;
        this.enabled = true;
    }
    public void DisableScaling()
    {
        this.enabled = false;
    }

    private float previousDistance = 0f;
    private float newDistance = 0f;
    private float scaleAmount = 0f;
    private void Update()
    {
        newDistance = Vector3.Distance(handA.position, handB.position);
        scaleAmount = targetObject.transform.localScale.x + ((newDistance - previousDistance) * Time.deltaTime * scalingSpeed);
        TargetObject.transform.localScale = Vector3.one * scaleAmount;
        previousDistance = newDistance;
    }
}