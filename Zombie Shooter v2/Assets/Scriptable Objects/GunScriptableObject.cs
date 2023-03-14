using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGunScriptableObject", menuName = "ScriptableObjects/GunScriptableObject")]
public class GunScriptableObject : ScriptableObject
{
    public string gunName;
    public float timeBetweenShots;
    public int animatorLAyer;

    public float maxKickBackZ;
    public float maxRotX;
    public float maxRotY;
    public float maxRotZ;

    public float kickDuration;
    public float returnDuration;

    public ParticleSystem vfx_muzzleFlash;
    public AudioClip sfx_shot;

}
