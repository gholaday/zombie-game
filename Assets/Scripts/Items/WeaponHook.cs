using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieGame
{
    public class WeaponHook : MonoBehaviour
    {
        public Transform raycastOrigin;

        [HideInInspector]
        public Weapon weaponData;

        private ParticleSystem[] weaponParticles;
        private CinemachineImpulseSource recoilImpulse;
        private Animator rigAnimator;

        private float accumulatedTime;
        private bool canShoot = false;
        private bool isReloading = false;

        private int totalAmmoCount = 100;

        public void Init()
        {
            weaponParticles = GetComponentsInChildren<ParticleSystem>();
            recoilImpulse = GetComponent<CinemachineImpulseSource>();
            rigAnimator = GetComponentInParent<Animator>();
        }

        public void Update()
        {
            if (accumulatedTime >= 0)
            {
                accumulatedTime -= Time.deltaTime;
            }

            if (canShoot)
            {
                float fireInterval = 1.0f / weaponData.fireRate;

                while (accumulatedTime <= 0.0f && !isReloading && weaponData.currentAmmoInMagazine > 0)
                {
                    FireBullet();
                    accumulatedTime += fireInterval;
                }
            }
        }

        public void SetCanShoot(bool _canShoot)
        {
            canShoot = _canShoot;

            if (canShoot && !weaponData.fixedFireRate) // If we tap trigger, reset timer
            {
                accumulatedTime = 0;
            }
        }

        private void FireBullet()
        {
            weaponData.currentAmmoInMagazine--;

            foreach (ParticleSystem ps in weaponParticles)
            {
                ps.Play();
            }

            Recoil();

            RaycastHit hitInfo;

            // Draw ray from camera to get point where crosshair touches
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, Mathf.Infinity))
            {
                // Then draw ray from gun to crosshair point to determine if we hit it
                if (Physics.Raycast(raycastOrigin.position, hitInfo.point - raycastOrigin.position, out hitInfo, Mathf.Infinity))
                {
                    //Damageable damageable = hitInfo.collider.gameObject.GetComponentInParent<Damageable>();

                    //if (damageable != null)
                    //{
                    //    damageable.TakeDamage(damagePerShot, hitInfo);
                    //}

                    // TODO: Make this more robust

                    if (hitInfo.transform.gameObject.layer == 6) // We hit the "Environment" layer
                    {
                        DrawBulletHole(hitInfo);
                    }
                }
            }
        }

        public void Reload()
        {
            StartCoroutine(DoReload());
        }

        private IEnumerator DoReload()
        {
            if (isReloading || weaponData.currentAmmoInMagazine == weaponData.magazineSize)
            {
                yield break;
            }

            isReloading = true;
            accumulatedTime = 0;

            if (totalAmmoCount > 0 && weaponData.currentAmmoInMagazine < weaponData.magazineSize)
            {
                weaponData.reloadEvent?.Raise(weaponData.reloadTime);

                yield return new WaitForSeconds(weaponData.reloadTime);

                int ammoNeeded = weaponData.magazineSize - weaponData.currentAmmoInMagazine;

                totalAmmoCount -= ammoNeeded;

                if (totalAmmoCount < 0)
                {
                    weaponData.currentAmmoInMagazine = ammoNeeded + totalAmmoCount; //totalAmmoCount will be NEGATIVE
                    totalAmmoCount = 0;
                }
                else
                {
                    weaponData.currentAmmoInMagazine = weaponData.magazineSize;
                }
            }

            isReloading = false;
        }

        private void Recoil()
        {
            if (recoilImpulse != null)
            {
                recoilImpulse.GenerateImpulse(Camera.main.transform.forward);
            }

            if (rigAnimator != null)
            {
                rigAnimator.Play(weaponData.animationRecoilName, 1, 0.0f);
            }
        }

        private void DrawBulletHole(RaycastHit hitInfo)
        {
            if (weaponData.bulletHolePrefab == null)
            {
                return;
            }

            GameObject hole = Instantiate(weaponData.bulletHolePrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal), hitInfo.transform);
        }
    }
}