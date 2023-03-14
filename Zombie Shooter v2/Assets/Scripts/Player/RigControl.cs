using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class RigControl : MonoBehaviour
{
    RigBuilder rigBuilder;
    [SerializeField] Rig pistolRig;
    [SerializeField] Rig rifleRig;
    // ? [SerializeField] Rig granadeRig; ...


    private void Awake()
    {
        rigBuilder = GetComponentInChildren<RigBuilder>();
    }

    public Rig GetActiveRig(Weapon.WeaponType activeWeaponType)
    {
        switch (activeWeaponType)
        {
            case Weapon.WeaponType.pistol:
                return pistolRig;

            case Weapon.WeaponType.rifle:
                return rifleRig;

            //dokoncit ostatni rigs
            default: return pistolRig;

        }
    }

}
