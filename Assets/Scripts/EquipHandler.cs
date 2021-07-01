using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipHandler : MonoBehaviour
{
    public Animator rigAnimator;

    public Transform weaponParent;
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
            Destroy(equippedWeapon.gameObject);
            rigAnimator.Play("weapon_unarmed", 0, .1f);
        }
        else
        {
            rigAnimator.Play("weapon_equip_idle_rifle");
            //Time.timeScale = .1f;

            StartCoroutine(SpawnWeapon());
        }

        isEquipped = !isEquipped;
    }

    private IEnumerator SpawnWeapon()
    {
        yield return new WaitForSeconds(.1f);

        RaycastWeapon weapon = Instantiate(weaponPrefab, weaponParent).GetComponent<RaycastWeapon>();

        //LeanTween.alpha(weapon.gameObject, 0, 0);
        //LeanTween.alpha(weapon.gameObject, 1, .1f);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        equippedWeapon = weapon;
    }
}