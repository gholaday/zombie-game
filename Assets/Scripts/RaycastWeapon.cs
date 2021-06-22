using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    public ParticleSystem[] weaponParticles = new ParticleSystem[0];
    public Transform raycastOrigin;
    public GameObject bulletHolePrefab;

    // TODO: Move this out to abstract Gun class or SO

    public int fireRate = 25;

    private float accumulatedTime;

    private bool isFiring = false;

    public void StartFiring()
    {
        isFiring = true;
        accumulatedTime = 0;
        FireBullet();
    }

    public void UpdateFiring(float deltaTime)
    {
        if (isFiring)
        {
            accumulatedTime += deltaTime;
            float fireInterval = 1.0f / fireRate;
            while (accumulatedTime >= 0.0f)
            {
                FireBullet();
                accumulatedTime -= fireInterval;
            }
        }
    }

    private void FireBullet()
    {
        foreach (ParticleSystem ps in weaponParticles)
        {
            ps.Emit(1);
        }

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

    public void StopFiring()
    {
        isFiring = false;
    }

    public void DrawBulletHole(RaycastHit hitInfo)
    {
        if (bulletHolePrefab == null)
        {
            return;
        }

        GameObject hole = Instantiate(bulletHolePrefab, hitInfo.point, Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
    }
}