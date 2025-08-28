using UnityEngine;


namespace EnemeyFSM
{
    public class EnemyAttack : EnemyFSMState
    {
        private Player target;
        private float attackElpasedTime = 0f;
        private bool isAttacking = false;

        private float sqrRange;

        private float checkElapsedTime = 0f;
        private bool isCheckAble = false;

        public override void OnEnter()
        {
            anim.SetTrigger(Enemy.EnemyAnimation.Attack.ToString());

            sqrRange = enemy.AttackRange * enemy.AttackRange;
            target = enemy.Target.GetComponentInParent<Player>();
            if(target == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Wrong target Detacted");
#endif
                manager.ChangeState(FSMState.Idle);
                return;
            }

            Debug.Log(enemy.name + " " + isCheckAble);
            attackElpasedTime = 0f;
            isAttacking = true;
            isCheckAble = false;
        }

        public override void OnUpdate()
        {
            CoolTime();

            if(isCheckAble == true && IsTargetFarAway() == true)
            {
                manager.ChangeState(FSMState.Chase);
            }
        }

        public override void OnLateUpdate()
        {
            isAttacking = true;
        }

        private void Attack()
        {
            SoundManager.Instance.PlaySFX(Path.SFX.Zombie.Attack);

            if (IsTargetFarAway() == true)
                return;

            target.GetDamaged(1, enemy.gameObject);

            isAttacking = false;
        }

        private void CoolTime()
        {
            if(isCheckAble == false)
            {
                checkElapsedTime += Time.deltaTime;

                if (checkElapsedTime >= 0.75f)
                {
                    isCheckAble = true;
                    checkElapsedTime = 0f;
                }
            }
        }

        public override void OnTriggerEnter(Collider2D collision)
        {
            if (isAttacking == true)
            {
                Attack();
            }
        }

        private bool IsTargetFarAway() => ((Vector2)enemy.Target.transform.position - enemy.Rigid.position).sqrMagnitude > sqrRange;

        public override void OnExit()
        {
        }
    }
}

