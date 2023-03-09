using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Anim
{
    public static IEnumerator SetParentAndLocalPosRotCoroutine(Transform child, Transform newParent, float transferTime, Vector3 endLocalPosition, Quaternion endLocalRotation, float delay)
    {
        //to be override
        Vector3 startLocalPosition = Vector3.zero;
        Quaternion startLocalRotation = Quaternion.identity;
        //
        bool parentChanged = false;
        float duration = delay + transferTime;
        float timer = 0;
        Debug.Log(endLocalPosition);
        Debug.Log(endLocalRotation);
        while(timer < duration)
        {
            timer += Time.deltaTime;
            if (timer < delay)
            {
                //Debug.Log("delay true");
                //Debug.Log(child.parent.gameObject);
                yield return null;
                continue;
            }
            if (!parentChanged)
            {
                Vector3 pos = child.position;
                Quaternion rot = child.rotation;
                child.SetParent(newParent);
                child.rotation = rot;
                child.position = pos;
                parentChanged = true;
                //Debug.Log("parent changed");
                startLocalPosition = child.localPosition;
                startLocalRotation = child.localRotation;
                //Debug.Log(startLocalPosition);
                //Debug.Log(startLocalRotation);
            }
            if (timer > duration) timer = duration;
            child.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, (timer - delay) / transferTime);
            child.localRotation = Quaternion.Lerp(startLocalRotation, endLocalRotation, (timer - delay) / transferTime);
            //child.localPosition = endLocalPosition;
            //child.localRotation = endLocalRotation;
            Debug.Log((timer - delay) / transferTime);
            yield return null;
        }
        Debug.Log(timer);
        
    }
}
