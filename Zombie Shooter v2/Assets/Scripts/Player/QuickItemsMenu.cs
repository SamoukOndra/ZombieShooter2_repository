using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickItemsMenu : MonoBehaviour
{
    //mozna initializovat pri awake jako list o deseti prvcich, zni less bug prone
    public List<GameObject> quickItemObjects;
    public List<Transform> quickItemsPositions;
    public bool isWeapon = false;
    public bool weaponDrawn = false;
    public int activeWeaponIndex; // use!!!!

    private void SelectQI(int index)
    {
        if(quickItemObjects[index] != null)
        {
            //todle mozna as potom, co ho ucini ditetem posice v pripade validniho pos type
            quickItemObjects[index].SetActive(true);
            //spis rovnou misto weaponTag plnohodnotnej script, zjisti weapon type a priradi mu pozici z qiposition(alt: wepon type separatly pro animace, pro pozice je pos type)
            //ulozi posledni wepon index, pokud posledni index stejnou qiposition jako soucasnej, deaktivuje posledni
            if (TryGetComponent<WeaponTag>(out WeaponTag weaponTag))
            {
                isWeapon = true;
            }
            else isWeapon = false;
        }
    }

    void OnQI_1() => SelectQI(0);
    void OnQI_2() => SelectQI(1);
    void OnQI_3() => SelectQI(2);
    void OnQI_4() => SelectQI(3);
    void OnQI_5() => SelectQI(4);

}
