using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmsAimPosition : MonoBehaviour
{
    [SerializeField] Transform pistolAim;
    [SerializeField] Transform rifleAim;

    public GameObject GetAimTransform(Weapon.WeaponType weaponType)
    {
        switch (weaponType)
        {
            case Weapon.WeaponType.pistol: return pistolAim.gameObject;
            case Weapon.WeaponType.rifle: return rifleAim.gameObject;
            default: return pistolAim.gameObject;
        }
    }
}
