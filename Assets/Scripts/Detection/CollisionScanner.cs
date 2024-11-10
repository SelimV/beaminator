using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class CollisionScanner : MonoBehaviour
{
    public UnityEvent OnScanningEnd = new UnityEvent();
    private List<CollisionDetector> coreParts = new List<CollisionDetector>();


    void Awake()
    {
        this.enabled = false;
    }

    public void ScanCoreParts(List<CollisionDetector> parts)
    {
        coreParts = new List<CollisionDetector>(parts);
        this.enabled = true;
        if (coreParts.Count == 0) { this.enabled = false; }
    }

    void FixedUpdate()
    {
        coreParts[coreParts.Count - 1].Collide();
        coreParts.RemoveAt(coreParts.Count - 1);
        if (coreParts.Count == 0) { this.enabled = false;  OnScanningEnd.Invoke(); }
    }
}
