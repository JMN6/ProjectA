using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyStatic : EnemyFSMState
    {
        public override void OnEnter() {
            enemy.SetAnimationTrigger(Enemy.EnemyAnimation.Idle);
        }

        public override void OnUpdate()
        {
        }

        public override void OnTriggerEnter(Collider2D collision)
        {
            if (collision.gameObject.layer != playerLayer)
            {
                return;
            }

            enemy.Target = collision.gameObject;
            manager.ChangeState(FSMState.Chase);
        }

        public override void OnExit() {}
    }
}

