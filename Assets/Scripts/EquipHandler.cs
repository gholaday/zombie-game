using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipHandler : MonoBehaviour
{
    public Animator rigAnimator;

    public Transform weaponPivot;
    public GameObject weaponPrefab;

    private PlayerLocomotion playerState; // TODO: Replace this with state machine class
    private RaycastWeapon equippedWeapon;

    private bool isEquipped = false;

    private void Awake()
    {
        playerState = gameObject.GetComponent<PlayerLocomotion>();
    }

    private void Update()
    {
        rigAnimator.SetBool("isAiming", playerState.isAiming);
    }

    public void OnEquipPrimary()
    {
        if (isEquipped)
        {
            Destroy(equippedWeapon.gameObject); ;
            rigAnimator.Play("weapon_unarmed");
        }
        else
        {
            RaycastWeapon weapon = Instantiate(weaponPrefab, weaponPivot).GetComponent<RaycastWeapon>();
            equippedWeapon = weapon;
            rigAnimator.Play("weapon_equip_idle_rifle");
        }

        isEquipped = !isEquipped;
    }
}