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
        

        public override void OnEnter()
        {
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


        public override void OnExit()
        {
        }
    }
}

