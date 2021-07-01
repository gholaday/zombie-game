using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieGame
{
    public class SelfDestroy : MonoBehaviour
    {
        public float lifetime = 5f;

        private void Start()
        {
            Invoke("Die", lifetime);
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}