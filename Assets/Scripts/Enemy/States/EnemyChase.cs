using System;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyChase : EnemyFSMState
    {
        private Rigidbody2D rigid;
        private Transform targetTrasnform;

        private float sqrAttackRange;

        private int targetDirection;

        public override void OnEnter()
        {
            enemy.SetAnimationTrigger(Enemy.EnemyAnimation.Run);
            enemy.StopLookingForPlayer();

            rigid = enemy.Rigid;
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
            float dir = (targetTrasnform.position - rigid.transform.position).x;
            int newDir = Convert.ToInt32(Mathf.Sign(dir));

            // 스프라이트 방향 바꾸기
            if(targetDirection != newDir && newDir != 0)
            {
                targetDirection = newDir;

                enemy.spriteRenderer.flipX = dir < 0f;
            }

            rigid.velocity = new Vector2 (newDir * enemy.Speed, rigid.velocity.y);
        }

        private bool IsTargetCloseEnough() => (rigid.position - (Vector2)targetTrasnform.position).sqrMagnitude < sqrAttackRange;

        public override void OnExit()
        {
            sqrAttackRange = 0f;
            targetDirection = 0;
        }
    }
}

