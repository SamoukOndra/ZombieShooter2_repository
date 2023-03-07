using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum weaponType
    {
        melee_1h,
        melee_2h,
        pistol,
        rifle,
        shotgun,
        granade
    }

    public enum positionType
    {
        hip_left_0,
        hip_right_1,
        back_2
    }

    protected virtual void OnEnable() { }
}
