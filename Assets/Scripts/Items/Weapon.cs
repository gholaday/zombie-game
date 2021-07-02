using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieGame
{
    [CreateAssetMenu(menuName = "Items/Weapon")]
    [System.Serializable]
    public class Weapon : Item
    {
        [Header("Weapon Data")]
        public int currentAmmoInMagazine = 30;

        public int magazineSize = 30;

        [Tooltip("Bullets fired per second")]
        public float fireRate = 0.2f;

        public bool fixedFireRate = false;
        public float reloadTime = .5f;

        [Header("Animation")]
        public string animationIdleName;

        public string animationRecoilName;

        [Header("GameObjects")]
        public GameObject modelPrefab;

        public GameObject bulletHolePrefab;
        public FloatGameEvent reloadEvent;

        public RuntimeWeapon runtime;

        public void Equip(Transform parent)
        {
            runtime = new RuntimeWeapon();
            runtime.modelInstance = Instantiate(modelPrefab, parent) as GameObject;
            runtime.weaponHook = runtime.modelInstance.GetComponent<WeaponHook>();
            runtime.weaponHook.weaponData = this;
            runtime.weaponHook.Init();
        }

        public void UnEquip()
        {
            Destroy(runtime.modelInstance);
            runtime = null;
        }

        public class RuntimeWeapon
        {
            public GameObject modelInstance;
            public WeaponHook weaponHook;
        }

        public enum FireMode
        {
            FullAuto,
            SemiAuto
        }
    }
}