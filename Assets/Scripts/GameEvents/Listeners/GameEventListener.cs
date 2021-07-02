using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ZombieGame
{
    public abstract class GameEventListener<T, E, UER> : MonoBehaviour, IGameEventListener<T> where E : GameEvent<T> where UER : UnityEvent<T>
    {
        [SerializeField] public E gameEvent;
        [SerializeField] public UER unityEventResponse;

        private void OnEnable()
        {
            if (gameEvent != null)
            {
                gameEvent.RegisterListener(this);
            }
        }

        private void OnDisable()
        {
            if (gameEvent != null)
            {
                gameEvent.UnregisterListener(this);
            }
        }

        public void OnEventRaised(T data)
        {
            if (unityEventResponse != null)
            {
                unityEventResponse.Invoke(data);
            }
        }
    }
}