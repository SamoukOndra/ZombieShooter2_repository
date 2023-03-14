using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRay : MonoBehaviour
{
    [SerializeField] Transform target;

    private void FixedUpdate()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            target.position = hit.point;
        }
         
    }
}
