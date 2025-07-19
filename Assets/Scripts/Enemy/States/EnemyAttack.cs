using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace EnemeyFSM
{
    public class EnemyAttack : EnemyFSMState
    {
        private Player target;
        private float elpasedTime = 0f;
        private bool isAttacking = false;

        private float sqrRange;

        private bool isCheckAble = false;

        public override void OnEnter()
        {
            enemy.SetAnimationTrigger(Enemy.EnemyAnimation.Attack);

            sqrRange = enemy.AttackRange * enemy.AttackRange;
            target = enemy.Target.GetComponent<Player>();
            if(target == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Rong target Detacted");
#endif
                manager.ChangeState(FSMState.Idle);
                return;
            }

            Debug.Log(enemy.name + " " + isCheckAble);
            elpasedTime = 0f;
            isAttacking = true;
            isCheckAble = false;
        }

        public override void OnUpdate()
        {
            CoolTime();

            if(isCheckAble == true && isTargetFarAway() == true)
            {
                manager.ChangeState(FSMState.Chase);
            }
        }

        private void Attack()
        {
            var res = target.TryAttack();

            if(!res)
            {
                enemy.isParried = true;
                enemy.GetDamaged(2);
            }
            else
            {
                // todo. ����� �ڵ� �����
                Debug.Log("Attack");
                isAttacking = false;
            }
        }

        private void CoolTime()
        {
            elpasedTime += Time.deltaTime;

            if (elpasedTime >= 0.75f)
            {
                isCheckAble = true;
                return;
            }

            if (isAttacking == true)
                return;

            if(elpasedTime >= enemy.AttackDelay)
            {
                elpasedTime = 0;
                isAttacking = true;
            }
        }

        public override void OnTriggerEnter(Collider2D collision)
        {
            if (isAttacking == true)
            {
                Attack();
            }
        }

        private bool isTargetFarAway()
        {
            return ((Vector2)enemy.Target.transform.position - enemy.Rigid.position).sqrMagnitude > sqrRange;
        }

        public override void OnExit()
        {
        }
    }
}

