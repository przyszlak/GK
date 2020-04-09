﻿using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{

    public int selectedWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.gameIsPaused && Player.instance.isAlive)
        {
            int previousSelectedWeapon = selectedWeapon;
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                if (selectedWeapon >= transform.childCount - 1)
                    selectedWeapon = 0;
                else
                    selectedWeapon++;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                if (selectedWeapon <= 0)
                    selectedWeapon = transform.childCount - 1;
                else
                    selectedWeapon--;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
                selectedWeapon = 0;
            else if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
                selectedWeapon = 1;
            else if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
                selectedWeapon = 2;
            else if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
                selectedWeapon = 3;
            else if (Input.GetKeyDown(KeyCode.Alpha5) && transform.childCount >= 5)
                selectedWeapon = 4;

            if (previousSelectedWeapon != selectedWeapon)
                SelectWeapon();
        }
    }

    void SelectWeapon()
    {
        int i = 0;
        foreach(Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
