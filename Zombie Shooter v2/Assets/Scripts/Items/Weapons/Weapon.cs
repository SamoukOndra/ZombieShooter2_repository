using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Weapon : MonoBehaviour
{
    //private Animator animator;
    private Rig rig;

    public WeaponType weaponType;
    public PositionType positionType;

    public GunScriptableObject gunScriptableObject;

    private bool canShoot;
    private float timer;


    private void OnEnable()
    {
        //animator = GetComponentInParent<Animator>();
    }
    private void Update()
    {
        
    }



    //v playercontroller setnout adekvatni rig





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
