using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyStatic : EnemyFSMState
    {
        public override void OnEnter() { }

        public override void OnUpdate()
        {
            // todo. 局聪皋捞记 贸府
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

