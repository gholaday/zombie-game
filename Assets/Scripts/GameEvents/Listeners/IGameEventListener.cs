using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieGame
{
    public interface IGameEventListener<T>
    {
        void OnEventRaised(T data);
    }
}