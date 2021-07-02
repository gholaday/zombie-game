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

        public Weapon tempWeapon;
        public Weapon tempWeapon2;

        private PlayerLocomotion playerState; // TODO: Replace this with state machine class
        private Weapon equippedWeapon;

        private void Awake()
        {
            playerState = gameObject.GetComponent<PlayerLocomotion>();
        }

        private void Update()
        {
            rigAnimator.SetBool("isAiming", playerState.isAiming);

            if (equippedWeapon != null)
            {
                if (!playerState.isAiming)
                {
                    equippedWeapon.runtime.weaponHook.SetCanShoot(false);
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
                    equippedWeapon.runtime.weaponHook.SetCanShoot(true);
                }
                else
                {
                    equippedWeapon.runtime.weaponHook.SetCanShoot(false);
                }
            }
        }

        public void OnEquipPrimary()
        {
            EquipWeapon(tempWeapon);
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (equippedWeapon != null)
            {
                equippedWeapon.DestroyRuntime();
                rigAnimator.Play("weapon_unarmed", 0, .1f);

                equippedWeapon = null;
            }
            else
            {
                rigAnimator.Play(weapon.animationIdleName);
                weapon.Init(weaponParent, Vector3.zero, Quaternion.identity);

                equippedWeapon = weapon;
            }
        }
    }
}