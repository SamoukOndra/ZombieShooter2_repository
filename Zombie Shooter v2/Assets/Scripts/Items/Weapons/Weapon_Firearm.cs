using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Firearm : Weapon
{
    public GunScriptableObject gunSO;
    [SerializeField] protected Transform bulletSpawnPoint;
    /*[SerializeField]*/ protected Transform firearmAimTransform;
    private Vector3 baseGunLocalPos;
    private Quaternion baseGunLocalRot;
    private Vector3 lastGunLocalPos;
    private Quaternion lastGunLocalRot;
    private Vector3 endGunLocalPos;
    private Quaternion endGunLocalRot;

    public int bulletsInMagazine { get; private set; }
    private bool magEmpty;

    private bool isReloading;
    //private bool isShooting;
    private bool canShoot = true;
    //private float timer;

    AudioSource audioSource;
    ParticleSystem muzzleFlash;
    FirearmsAimPosition firearmsAimPosition;

    private void Start()
    {
        muzzleFlash = Instantiate(gunSO.vfx_muzzleFlash, bulletSpawnPoint);
        //muzzleFlash.transform.localEulerAngles = new Vector3(0f, -90, 0f); setnutý v rotate muzzleflash
    }
    //call pri equipu v QI
    public override void SetWeaponProperties()
    {
        firearmsAimPosition = GetComponentInParent<FirearmsAimPosition>();
        firearmAimTransform = firearmsAimPosition.GetAimTransform(weaponType);
        audioSource = GetComponentInParent<AudioSource>();
        baseGunLocalPos = firearmAimTransform.localPosition;
        baseGunLocalRot = firearmAimTransform.localRotation;
        lastGunLocalPos = baseGunLocalPos;
        lastGunLocalRot = baseGunLocalRot;

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
        if(isAttacking && canShoot && !magEmpty)
        {
            StopAllCoroutines();
            //StopCoroutine(ShotCoroutine());
            StartCoroutine(ShotCoroutine());
            StartCoroutine(WaitBetweenShotsCoroutine());
        }
    }
    //player input component prenesu do playerController
    public override void Reload()
    {
        if (!isReloading && bulletsInMagazine != gunSO.magazineVolume) StartCoroutine(ReloadCoroutine());
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
    void RotateMuzzleFlash(string xyzAxe, float minRot, float maxRot, Vector3 defaultRot = default(Vector3))
    {
        float fRot = Random.Range(minRot, maxRot);
        switch (xyzAxe)
        {
            case "x": defaultRot.x += fRot;
                break;
            case "y": defaultRot.y += fRot;
                break;
            case "z": defaultRot.z += fRot;
                break;
            default: Debug.Log("Invalid axe");
                break;
        }
        muzzleFlash.transform.localRotation = Quaternion.Euler(defaultRot);
    }
    Vector3 GetAfterKickBackPos()
    {
        if (lastGunLocalPos.z - gunSO.kickBackZ < -gunSO.maxKickBackZ)
        {
            //Debug.Log("MAX kickback");
            return new Vector3(lastGunLocalPos.x, lastGunLocalPos.y, -gunSO.maxKickBackZ);
        }
            
        else
        {
            //Debug.Log("norm kickback");
            return new Vector3(lastGunLocalPos.x, lastGunLocalPos.y, lastGunLocalPos.z - gunSO.kickBackZ);
        }
    }

    //nahrazuje rotaci, nepricita!!!!
    Quaternion GetAfterKickBackRotation()
    {
        float xRot = Random.Range(gunSO.minRotX, gunSO.maxRotX);
        float yRot = Random.Range(gunSO.minRotY, gunSO.maxRotY);
        float zRot = Random.Range(gunSO.minRotZ, gunSO.maxRotZ);
        return Quaternion.Euler(xRot, yRot, zRot);
    }
    IEnumerator ReloadCoroutine()
    {
        StopCoroutine(WaitBetweenShotsCoroutine());
        canShoot = false;
        //isReloading = true;
        yield return new WaitForSeconds(gunSO.reloadDuration);
        bulletsInMagazine = gunSO.magazineVolume;
        //isReloading = false;
        UpdateBulletsInMagazine(0);
        canShoot = true;
    }
    IEnumerator WaitBetweenShotsCoroutine()
    {
        canShoot = false;
        yield return new WaitForSeconds(gunSO.timeBetweenShots);
        canShoot = true;
    }
    IEnumerator ShotCoroutine()
    {
        
        lastGunLocalPos = firearmAimTransform.localPosition;
        lastGunLocalRot = firearmAimTransform.localRotation;
        //Debug.Log(lastGunLocalPos);
        float timer = 0f;
        float lerp = 0f;
        SpawnBullet();
        UpdateBulletsInMagazine(1);
        //Debug.Log("befor FX");
        audioSource.PlayOneShot(gunSO.sfx_shot);
        RotateMuzzleFlash("x", 0f, 180f, new Vector3(0f, -90f,0));
        muzzleFlash.Play();
        //Debug.Log("after FX");
        //FXs asi musi bejt decka
        endGunLocalPos = GetAfterKickBackPos();
        endGunLocalRot = lastGunLocalRot * GetAfterKickBackRotation();
        while (timer < gunSO.kickDuration + gunSO.returnDuration)
        {
            timer += Time.deltaTime;
            //Debug.Log(lerp);
            //Debug.Log(timer);
            if (timer < gunSO.kickDuration)
            {
                //Debug.Log("lerp kick");
                lerp = timer / gunSO.kickDuration;
                firearmAimTransform.localPosition = Vector3.Lerp(lastGunLocalPos, endGunLocalPos, lerp);
                firearmAimTransform.localRotation = Quaternion.Lerp(lastGunLocalRot, endGunLocalRot, lerp);
            }
            else
            {
                //Debug.Log("lerp return");
                lerp = (timer - gunSO.kickDuration)/gunSO.returnDuration;
                firearmAimTransform.transform.localPosition = Vector3.Lerp(endGunLocalPos, baseGunLocalPos, lerp);
                firearmAimTransform.transform.localRotation = Quaternion.Lerp(endGunLocalRot, baseGunLocalRot, lerp);
            }
            yield return null;
        }
    }

}
