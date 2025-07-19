using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace EnemeyFSM
{
    public enum FSMState
    {
        Idle,
        Chase,
        Attack,
        Death,
        MAX
    }

    public enum FSMScripts
    {
        EnemyStatic,
        EnemyPatrol,
        EnemyChase,
        EnemyAttack,
        EnemyDeath,
        MAXs
    }

    public class EnemyFSMManager : MonoBehaviour
    {
        [Serializable]
        private class FSMStates
        {
            [field: SerializeField] public FSMState StateName { get; private set; }
            [field: SerializeField] public FSMScripts ScriptsName { get; private set; }
        }

        [SerializeField] private Enemy enemy;
        [SerializeField] private List<FSMStates> fsmStates;
        [SerializeField] private FSMState startState;
        private Dictionary<FSMState, EnemyFSMState> fsmStateDict
            = new Dictionary<FSMState, EnemyFSMState>();

        private EnemyFSMState currentStateScript = null;

        [SerializeField] private FSMState currentState = FSMState.MAX;
        private FSMState prevState;

        private void Awake()
        {
            foreach (var _ in fsmStates)
            {
                var t = Type.GetType($"EnemeyFSM.{_.ScriptsName.ToString()}");
                EnemyFSMState script = Activator.CreateInstance(t) as EnemyFSMState;
                script.init(enemy, this);
                fsmStateDict.Add(_.StateName, script);
            }

            currentState = FSMState.MAX;
        }

        private void OnEnable()
        {
            ChangeState(startState);
        }

        public void ChangeState(FSMState _newState)
        {
            enemy.Rigid.velocity = Vector2.zero;

            if (currentState != FSMState.MAX)
            {
                currentStateScript.OnExit();
                prevState = currentState;
            }

            currentState = _newState;
            currentStateScript = fsmStateDict[currentState];

            currentStateScript.OnEnter();
        }

        private void FixedUpdate()
        {
            currentStateScript?.OnFixedUpdate();
        }

        private void Update()
        {
            currentStateScript?.OnUpdate();
        }



        private void OnTriggerEnter2D(Collider2D collision)
        {
            currentStateScript?.OnTriggerEnter(collision);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            currentStateScript?.OnTriggerStay(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            currentStateScript?.OnTriggerExit(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            currentStateScript?.OnCollisionEnter(collision);
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            currentStateScript?.OnCollisionStay(collision);
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            currentStateScript?.OnCollisionExit(collision);
        }
    }
}


