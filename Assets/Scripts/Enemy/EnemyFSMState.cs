using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnemeyFSM
{
    public abstract class EnemyFSMState
    {
        // Util
        protected static int playerLayer = -1;

        protected Enemy enemy;
        protected EnemyFSMManager manager;

        public void init(Enemy _enemy, EnemyFSMManager _manager)
        {
            if (playerLayer == -1)
            {
                playerLayer = LayerMask.NameToLayer("Player");
            }
            enemy = _enemy;
            manager = _manager; 
        }

        public abstract void OnEnter();

        public abstract void OnUpdate();
        public virtual void OnFixedUpdate() { }

        public virtual void OnTriggerEnter(Collider2D collision) { }
        public virtual void OnTriggerStay(Collider2D collision) { }
        public virtual void OnTriggerExit(Collider2D collision) { }

        public virtual void OnCollisionEnter(Collision2D collision) { }
        public virtual void OnCollisionStay(Collision2D collision) { }
        public virtual void OnCollisionExit(Collision2D collision) { }


        public abstract void OnExit();
    }
}
