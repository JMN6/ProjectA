using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyDeath : EnemyFSMState
    {
        public override void OnEnter()
        {
            // todo: �ӽ� �ڵ�
            enemy.DestroySelf();
        }

        public override void OnExit()
        {
        }

        public override void OnUpdate()
        {
        }
    }
}
