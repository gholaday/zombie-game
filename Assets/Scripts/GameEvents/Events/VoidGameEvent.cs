using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieGame
{
    [CreateAssetMenu(menuName = "Game Events/Void")]
    public class VoidGameEvent : GameEvent<Void>
    {
        public override void Raise(Void item)
        {
            base.Raise(new Void());
        }
    }
}