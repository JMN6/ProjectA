using System;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyChase : EnemyFSMState
    {
        private Transform targetTrasnform;

        private float sqrAttackRange;

        private int targetDirection;

        public override void OnEnter()
        {
            anim.SetTrigger(Enemy.EnemyAnimation.Run.ToString());
            enemy.StopLookingForPlayer();

            targetTrasnform = enemy.Target.transform;

            sqrAttackRange = enemy.AttackRange * enemy.AttackRange;
            targetDirection = 0;
        }

        public override void OnFixedUpdate()
        {
            //ChaseTarget();
        }

        public override void OnUpdate()
        {
            ChaseTarget();

            if (IsTargetCloseEnough())
            {
                manager.ChangeState(FSMState.Attack);
            }
        }

        private void ChaseTarget()
        {
            float dir = (targetTrasnform.position - move.Rigid.transform.position).x;
            int newDir = Convert.ToInt32(Mathf.Sign(dir));

            // ��������Ʈ ���� �ٲٱ�
            if(targetDirection != newDir && newDir != 0)
            {
                targetDirection = newDir;

                enemy.spriteRenderer.flipX = dir < 0f;
            }

            move.SetVelocity(newDir * enemy.Speed, move.Rigid.linearVelocityY);
        }

        private bool IsTargetCloseEnough() => (move.Rigid.position - (Vector2)targetTrasnform.position).sqrMagnitude < sqrAttackRange;

        public override void OnExit()
        {
            sqrAttackRange = 0f;
            targetDirection = 0;
        }
    }
}

