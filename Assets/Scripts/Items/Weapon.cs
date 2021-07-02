using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieGame
{
    [CreateAssetMenu(menuName = "Items/Weapon")]
    public class Weapon : Item
    {
        [Header("Weapon Data")]
        public int magazineSize = 30;

        [Tooltip("Bullets fired per second")]
        public float fireRate = 0.2f;

        public bool fixedFireRate = false;

        [Header("Animation")]
        public string animationIdleName;

        [Header("GameObjects")]
        public GameObject modelPrefab;

        public GameObject bulletHolePrefab;

        public RuntimeWeapon runtime;

        public void Init(Transform parent, Vector3 position, Quaternion rotation)
        {
            runtime = new RuntimeWeapon();
            runtime.modelInstance = Instantiate(modelPrefab, parent) as GameObject;
            runtime.weaponHook = runtime.modelInstance.GetComponent<WeaponHook>();
            runtime.weaponHook.weaponData = this;
            runtime.weaponHook.Init();
        }

        public void DestroyRuntime()
        {
            Destroy(runtime.modelInstance);
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