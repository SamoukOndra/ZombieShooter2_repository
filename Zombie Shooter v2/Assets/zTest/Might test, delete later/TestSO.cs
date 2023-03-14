using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSO : MonoBehaviour
{
    public ScriptableObject scriptableObject;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(scriptableObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
