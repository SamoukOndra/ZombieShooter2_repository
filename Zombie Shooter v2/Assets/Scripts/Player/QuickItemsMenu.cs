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
        if(quickItemObjects[indexObject] != null)
        {
            //todle mozna as potom, co ho ucini ditetem posice v pripade validniho pos type
            quickItemObjects[indexObject].SetActive(true);
            //Debug.Log("QI set active");
            //spis rovnou misto weaponTag plnohodnotnej script, zjisti weapon type a priradi mu pozici z qiposition(alt: wepon type separatly pro animace, pro pozice je pos type)
            //ulozi posledni wepon index, pokud posledni index stejnou qiposition jako soucasnej, deaktivuje posledni
            //if (TryGetComponent<Weapon>(out Weapon weapon))
            if(quickItemObjects[indexObject].TryGetComponent(out Weapon weapon))
            {
                //Debug.Log("QI is weapon");
                isWeapon = true;
                int indexPosition =((int)weapon.positionType);
                DeactivateWpChild(indexPosition);
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
    void DeactivateWpChild(int indexPosition)
    {
        if(quickItemsPositions[indexPosition].childCount > 0)
        {
            Transform child = quickItemsPositions[indexPosition].GetChild(0);
            quickItemsPositions[indexPosition].DetachChildren();
            child.gameObject.SetActive(false);
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

}
