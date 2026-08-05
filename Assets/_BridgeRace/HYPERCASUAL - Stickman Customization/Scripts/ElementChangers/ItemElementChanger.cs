using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemElementChanger : ElementChanger
{
    public GameObject hand;

    private GameObject _prefab;

    public GameObject[] weapons = new GameObject[10];

    public override void Start()
    {
        base.Start();
        ChangeWeaponElements();
    }

    public override void GetNextButton()
    {
        base.GetNextButton();
        ChangeWeaponElements();
    }

    public override void GetPreviousButton()
    {
        base.GetPreviousButton();
        ChangeWeaponElements();
    }

    private void ChangeWeaponElements()
    {
        if (elementIndex == meshElements.Count)
        {
            Destroy(_prefab);
        }
        else
        {
            Destroy(_prefab);
            GameObject newWeapon = (GameObject)Instantiate(weapons[elementIndex]);

            newWeapon.transform.SetParent(hand.transform);
            newWeapon.transform.position = hand.transform.position;
            newWeapon.transform.localRotation = hand.transform.localRotation;

            _prefab = newWeapon;
        }
    }
}
