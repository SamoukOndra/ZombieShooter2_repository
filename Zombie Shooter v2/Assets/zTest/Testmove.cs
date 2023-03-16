using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testmove : MonoBehaviour
{
    public float multiplier = 1f;
    private void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            gameObject.transform.localPosition += new Vector3(0f, multiplier * Time.deltaTime, 0f);
        }
    }
}
