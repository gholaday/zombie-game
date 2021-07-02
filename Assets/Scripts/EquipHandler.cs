using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZombieGame
{
    public class EquipHandler : MonoBehaviour
    {
        public Animator rigAnimator;
        public Transform weaponParent;

        public Weapon equippedWeapon;

        public Weapon tempWeapon;
        public Weapon tempWeapon2;

        private PlayerLocomotion playerState; // TODO: Replace this with state machine class

        private void Awake()
        {
            playerState = gameObject.GetComponent<PlayerLocomotion>();

            tempWeapon = Instantiate(tempWeapon);
            tempWeapon2 = Instantiate(tempWeapon2);
        }

        private void Update()
        {
            rigAnimator.SetBool("isAiming", playerState.isAiming);

            if (equippedWeapon != null)
            {
                if (!playerState.isAiming)
                {
                    equippedWeapon.runtime?.weaponHook.SetCanShoot(false);
                }
            }
        }

        private void OnShoot(InputValue value)
        {
            if (!playerState.isAiming)
            {
                return;
            }

            if (equippedWeapon != null)
            {
                if (value.isPressed)
                {
                    equippedWeapon.runtime?.weaponHook.SetCanShoot(true);
                }
                else
                {
                    equippedWeapon.runtime?.weaponHook.SetCanShoot(false);
                }
            }
        }

        public void OnEquipPrimary()
        {
            EquipWeapon(tempWeapon);
        }

        public void OnEquipSecondary()
        {
            EquipWeapon(tempWeapon2);
        }

        public void OnReload()
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.runtime.weaponHook.Reload();
            }
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.UnEquip();
            }

            if (equippedWeapon == weapon)
            {
                equippedWeapon = null;
                rigAnimator.Play("weapon_unarmed", 0, .1f);
            }
            else
            {
                rigAnimator.Play(weapon.animationIdleName);
                StartCoroutine(SpawnWeapon(weapon));
                equippedWeapon = weapon;
            }
        }

        private IEnumerator SpawnWeapon(Weapon weapon)
        {
            yield return new WaitForSeconds(.1f);

            weapon.Equip(weaponParent);
        }
    }
}