using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon_Firearm : Weapon
{
    public GunScriptableObject gunSO;
    [SerializeField] protected Transform bulletSpawnPoint;
    /*[SerializeField]*/ protected Transform firearmAimTransform;
    private Transform baseGunAimTransform;
    private Vector3 lastGunLocalPos;
    private Quaternion lastGunLocalRot;
    private Vector3 endGunLocalPos;
    private Quaternion endGunLocalRot;

    public int bulletsInMagazine { get; private set; }
    private bool magEmpty;

    private bool isReloading;
    private bool isShooting;
    private bool canShoot = true;
    //private float timer;

    AudioSource audioSource;
    FirearmsAimPosition firearmsAimPosition;

    private void Start()
    {
        //
        //
    }
    //call pri equipu v QI
    public override void SetWeaponProperties()
    {
        firearmsAimPosition = GetComponentInParent<FirearmsAimPosition>();
        firearmAimTransform = firearmsAimPosition.GetAimTransform(weaponType);
        audioSource = GetComponentInParent<AudioSource>();
        baseGunAimTransform = firearmAimTransform;
        bulletsInMagazine = gunSO.magazineVolume;
        UpdateBulletsInMagazine(0);
    }
    /*private void OnEnable()
    {
        firearmsAimPosition = GetComponentInParent<FirearmsAimPosition>();
        if(firearmsAimPosition != null)
        {
            firearmAimTransform = firearmsAimPosition.GetAimTransform(weaponType);
            audioSource = GetComponentInParent<AudioSource>();
            baseGunAimTransform = firearmAimTransform;
        }
        //startovni bulletsInMag urco casem pozmenim
        bulletsInMagazine = gunSO.magazineVolume;
        UpdateBulletsInMagazine(0);
    }*/
    private void Update()
    {
        if(isShooting && canShoot && !magEmpty)
        {
            StopCoroutine(ShotCoroutine());
            StartCoroutine(ShotCoroutine());
            StartCoroutine(WaitBetweenShotsCoroutine());
        }
    }
    void OnReload()
    {
        if (!isReloading && bulletsInMagazine != gunSO.magazineVolume) StartCoroutine(ReloadCoroutine());
    }
    void OnFire(InputValue value)
    {
        isShooting = value.isPressed;
        if (isShooting)
        {
            lastGunLocalPos = firearmAimTransform.localPosition;
            lastGunLocalRot = firearmAimTransform.localRotation;
        }
    }
    void SpawnBullet()
    {
        GameObject bullet = BulletPool.SharedInstance.GetPooledBullet();
        if (bullet != null)
        {
            bullet.transform.position = bulletSpawnPoint.transform.position;
            bullet.transform.rotation = bulletSpawnPoint.transform.rotation;
            bullet.SetActive(true);
        }
    }
    void UpdateBulletsInMagazine(int reduceBy)
    {
        bulletsInMagazine -= reduceBy;
        magEmpty = (bulletsInMagazine == 0) ? true : false;
    }
    Vector3 GetAfterKickBackPosition()
    {
        if (lastGunLocalPos.z - gunSO.kickBackZ < -gunSO.maxKickBackZ)
            return new Vector3(0, 0, -gunSO.maxKickBackZ);
        else return new Vector3(0, 0, lastGunLocalPos.z - gunSO.kickBackZ);
    }
    Quaternion GetAfterKickBackRotation()
    {
        float xRot = Random.Range(gunSO.minRotX, gunSO.maxRotX);
        float yRot = Random.Range(gunSO.minRotY, gunSO.maxRotY);
        float zRot = Random.Range(gunSO.minRotZ, gunSO.maxRotZ);
        return Quaternion.Euler(xRot, yRot, zRot);
    }
    IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(gunSO.reloadDuration);
        bulletsInMagazine = gunSO.magazineVolume;
        isReloading = false;
        UpdateBulletsInMagazine(0);
    }
    IEnumerator WaitBetweenShotsCoroutine()
    {
        canShoot = false;
        yield return new WaitForSeconds(gunSO.timeBetweenShots);
        canShoot = true;
    }
    IEnumerator ShotCoroutine()
    {
        float timer = 0f;
        SpawnBullet();
        UpdateBulletsInMagazine(1);
        Debug.Log("befor FX");
        audioSource.PlayOneShot(gunSO.sfx_shot);

        gunSO.vfx_muzzleFlash.Play();
        Debug.Log("after FX");
        //FXs asi musi bejt decka
        endGunLocalPos = GetAfterKickBackPosition();
        endGunLocalRot = GetAfterKickBackRotation();
        while (timer < gunSO.kickDuration + gunSO.returnDuration)
        {
            if (timer < gunSO.kickDuration)
            {
                float lerp = timer / gunSO.kickDuration;
                firearmAimTransform.localPosition = Vector3.Lerp(lastGunLocalPos, endGunLocalPos, lerp);
                firearmAimTransform.localRotation = Quaternion.Lerp(lastGunLocalRot, endGunLocalRot, lerp);
            }
            else
            {
                float lerp = (timer - gunSO.kickDuration)/gunSO.returnDuration;
                firearmAimTransform.localPosition = Vector3.Lerp(endGunLocalPos, baseGunAimTransform.localPosition, lerp);
                firearmAimTransform.localRotation = Quaternion.Lerp(endGunLocalRot, baseGunAimTransform.localRotation, lerp);
            }
            yield return null;
        }
    }

}
