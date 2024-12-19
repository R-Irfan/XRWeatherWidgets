using UnityEngine;

public class CanvasLookAtCamera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 dirToTarget = (Camera.main.transform.position - transform.position).normalized;
        transform.LookAt(transform.position - dirToTarget, Vector3.up);
    }
}
