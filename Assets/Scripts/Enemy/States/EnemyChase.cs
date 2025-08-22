using System;
using UnityEngine;

namespace EnemeyFSM
{
    public class EnemyChase : EnemyFSMState
    {
        private Transform targetTrasnform;

        private float sqrAttackRange;

        private int targetDirection;

        private WayFindingManager wayFindingManager;

        private Vector3 nextTilePosition;

        public override void OnEnter()
        {
            anim.SetTrigger(Enemy.EnemyAnimation.Run.ToString());
            enemy.StopLookingForPlayer();

            targetTrasnform = enemy.Target.transform;
            wayFindingManager = enemy.wayFindingManager;
            wayFindingManager.IsWayFindingCoolTimeOver = true;
            if (wayFindingManager.SearchWay(enemy.transform.position, targetTrasnform.position, true))
            {
                SetNextTilePosition();
            }

            sqrAttackRange = enemy.AttackRange * enemy.AttackRange;
            targetDirection = 0;
        }

        private void SetNextTilePosition()
        {
            Vector2Int index = wayFindingManager.Route.Pop();
            nextTilePosition = TileMapDataConverter.Instance.GetTileInfo(index).Position;

            float dir = (targetTrasnform.position - move.Rigid.transform.position).x;
            newDir = Convert.ToInt32(Mathf.Sign(dir));

            if (targetDirection != newDir && newDir != 0)
            {
                targetDirection = newDir;

                enemy.spriteRenderer.flipX = dir < 0f;
            }
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

        private int newDir = 0;
        private void ChaseTarget()
        {
            // 타겟이 공중에 있을 경우 그 떨어지는 곳으로 이동
            Vector2Int i = TileMapDataConverter.Instance.GetNodeIndex(targetTrasnform.position);
            int y = TileMapDataConverter.Instance.GetFloorTileYIndexValue(i);
            i.y = y;
            Vector3 newTargetPos = TileMapDataConverter.Instance.GetTilePosition(i);

            if (wayFindingManager.SearchWay(enemy.transform.position, newTargetPos))
            {
                SetNextTilePosition();
            }

            move.SetVelocity(newDir * enemy.Speed, move.Rigid.linearVelocityY);
        }

        private bool IsTargetCloseEnough() => 
            (move.Rigid.position - (Vector2)targetTrasnform.position).sqrMagnitude < sqrAttackRange;
        private bool IsNextTileReached()
        {
            var d = (Vector2)nextTilePosition - move.Rigid.position;
            return d.sqrMagnitude <= 0.1f;
        }


        public override void OnExit()
        {
            sqrAttackRange = 0f;
            targetDirection = 0;
        }
    }
}

