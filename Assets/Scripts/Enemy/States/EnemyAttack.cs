using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyAttack : EnemyFSMState
    {
        private IDamagalbe target;
        private float elpasedTime = 0f;
        private bool isAttacking = false;

        private float sqrRange;
        

        public override void OnEnter()
        {
            sqrRange = enemy.AttackRange * enemy.AttackRange;
            target = enemy.Target.GetComponent<IDamagalbe>();
            if(target == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Rong target Detacted");
#endif
                manager.ChangeState(FSMState.Idle);
                return;
            }

            elpasedTime = 0f;
            isAttacking = true;
        }

        public override void OnUpdate()
        {
            // todo. 공격 로직 수정
            if(isAttacking == true)
            {
                Attack();
                return;
            }

            CoolTime();

            if(isTargetFarAway() == true)
            {
                manager.ChangeState(FSMState.Chase);
            }
        }

        private void Attack()
        {
            target.GetDamaged(enemy.Damage);
            // todo. 디버그 코드 지우기
            Debug.Log("Attack");
        }

        private void CoolTime()
        {
            elpasedTime += Time.deltaTime;
            if(elpasedTime >= enemy.AttackDelay)
            {
                elpasedTime = 0;
                isAttacking = true;
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

