using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGunScriptableObject", menuName = "ScriptableObjects/GunScriptableObject")]
public class GunScriptableObject : ScriptableObject
{
    public string gunName;
    public float timeBetweenShots;
    public int animatorLayer;

    public float maxKickBackZ;
    public float kickBackZ;
    public float minRotX;
    public float maxRotX;
    public float minRotY;
    public float maxRotY;
    public float minRotZ;
    public float maxRotZ;

    public float kickDuration;
    public float returnDuration;

    public int magazineVolume;
    public float reloadDuration;

    public ParticleSystem vfx_muzzleFlash;
    public AudioClip sfx_shot;

}
