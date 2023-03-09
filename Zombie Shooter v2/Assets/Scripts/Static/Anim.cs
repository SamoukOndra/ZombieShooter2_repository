using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Anim
{
    public static IEnumerator SetParentAndLocalPosRotCoroutine(Transform child, Transform newParent, float transferTime, float delay = 0f, Vector3 endLocalPosition = default(Vector3), Quaternion endLocalRotation = default(Quaternion))
    {
        //to be override
        Vector3 startLocalPosition = Vector3.zero;
        Quaternion startLocalRotation = Quaternion.identity;
        //
        bool parentChanged = false;
        float duration = delay + transferTime;
        float timer = 0;
        while(timer < duration)
        {
            timer += Time.deltaTime;
            if (timer < delay) continue;
            if (!parentChanged)
            {
                child.SetParent(newParent);
                parentChanged = true;
                startLocalPosition = child.localPosition;
                startLocalRotation = child.localRotation;
            }
            child.localPosition = Vector3.Lerp(startLocalPosition, endLocalPosition, timer - delay / transferTime);
            child.localRotation = Quaternion.Lerp(startLocalRotation, endLocalRotation, timer - delay / transferTime);
            yield return null;
        }
        
    }
}
