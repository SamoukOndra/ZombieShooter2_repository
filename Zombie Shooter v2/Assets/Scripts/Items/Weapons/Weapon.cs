using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Weapon : MonoBehaviour
{

    public WeaponType weaponType;
    public PositionType positionType;

    protected bool isAttacking;
    public virtual void SetWeaponProperties() { }
    public void StartAttack() { isAttacking = true; }
    public void StopAttack() { isAttacking = false; }
    public virtual void Reload() { }

    public enum WeaponType
    {
        melee_1h,
        melee_2h,
        pistol,
        rifle,
        shotgun,
        granade
    }

    public enum PositionType
    {
        hip_left_0,
        hip_right_1,
        back_2
    }
}
