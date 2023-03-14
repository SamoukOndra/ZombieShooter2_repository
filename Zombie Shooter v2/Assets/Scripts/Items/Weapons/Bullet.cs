using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody rb;

    public float shotForce = 600.0f;
    [SerializeField] float duration = 1.0f;
    //provizorní hitpoint system
    [SerializeField] GameObject hitPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rb.AddForce(transform.forward * shotForce, ForceMode.Impulse);
        StartCoroutine(Duration());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        Instantiate(hitPoint, transform.position, transform.rotation);
        gameObject.SetActive(false);
    }

    IEnumerator Duration()
    {
        yield return new WaitForSeconds(duration);
        gameObject.SetActive(false);
    }
}
