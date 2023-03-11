using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickItemsMenu : MonoBehaviour
{
    //mozna initializovat pri awake jako list o deseti prvcich, zni less bug prone
    public List<GameObject> quickItemObjects;
    public List<Transform> quickItemsPositions;
    // mozna neni nutno, nevim este zatim
    public bool isWeapon = false;
    //public bool weaponDrawn = false;
    public int activeWeaponIndex;

    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
    }
    private void SelectQI(int indexObject)
    {
        if (playerController.isAiming || playerController.eda_block) return;
        if(quickItemObjects[indexObject] != null)
        {
            if (indexObject == activeWeaponIndex) return;
            //todle mozna as potom, co ho ucini ditetem posice v pripade validniho pos type
            quickItemObjects[indexObject].SetActive(true);

            if(quickItemObjects[indexObject].TryGetComponent(out Weapon weapon))
            {
                isWeapon = true;
                int indexPosition = ((int)weapon.positionType);
                DeactivateWpChildern(indexPosition);
                if (playerController.weaponDrawn)
                {
                    StartCoroutine(playerController.ChangeDrawnWeaponCoroutine(weapon.weaponType));
                    //StartCoroutine(DeactivateWpChildAfterEdaCoroutine(indexPosition));
                }
                //DeactivateWpChild(indexPosition);

                SetWpChild(indexObject, indexPosition);
                activeWeaponIndex = indexObject;
                playerController.activeWeaponType = weapon.weaponType;
                playerController.activeWeaponTransform = quickItemObjects[indexObject].transform;
                playerController.activeWeaponUnequipedTransform = quickItemsPositions[indexPosition];

                
            }
            else
            {
                //Debug.Log("QI is not a weapon");
                isWeapon = false;
            }
        }
        //else Debug.Log("QI is null");
    }

    void OnQI_1() => SelectQI(0);
    void OnQI_2() => SelectQI(1);
    void OnQI_3() => SelectQI(2);
    void OnQI_4() => SelectQI(3);
    void OnQI_5() => SelectQI(4);
    //kde budou neaktivni zbrane, inventory items??? v podzemi???? destroy and instantiate???
    void DeactivateWpChildern(int indexPosition)
    {
        int childernAmount = quickItemsPositions[indexPosition].childCount;
        if (childernAmount > 0)
        {
            for(int i = 0; i < childernAmount; i++)
            {
                Transform child = quickItemsPositions[indexPosition].GetChild(i);
                child.gameObject.SetActive(false);
            }
            quickItemsPositions[indexPosition].DetachChildren();
            //se budou jako deaktivovany valet po svete? asi jo...

            //quickItemsPositions[indexPosition].DetachChildren();
            //child.gameObject.SetActive(false);
            //List<Transform> childern = quickItemsPositions[indexPosition].;
            //quickItemsPositions[indexPosition].DetachChildren();
            //child.gameObject.SetActive(false);
        }
        
    }
    void SetWpChild(int indexObject, int indexPosition)
    {
        GameObject item = quickItemObjects[indexObject];
        item.transform.SetParent(quickItemsPositions[indexPosition]);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        item.SetActive(true);
        
    }

    IEnumerator DeactivateWpChildAfterEdaCoroutine(int indexPosition)
    {
        yield return new WaitForSeconds(playerController.eda_blockDuration +1);
        DeactivateWpChildern(indexPosition);
    }

}
