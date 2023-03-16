using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmsAimPosition : MonoBehaviour
{
    [SerializeField] Transform pistolAim;
    [SerializeField] Transform rifleAim;

    public Transform GetAimTransform(Weapon.WeaponType weaponType)
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.pistol: return pistolAim;
            case Weapon.WeaponType.rifle: return rifleAim;
            default: return pistolAim;
        }
    }
}
