using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Movement;
using System;
using RPG.Core;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionDistance = 8f;
        [SerializeField] float suspicionTime = 5f;
        [SerializeField] float aggroCooldownSeconds = 5f;
        [SerializeField] float shoutRadius = 4f;
        [SerializeField] PatrolPath patrolPath;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.6f;
        [Range(0, 1)]
        [SerializeField] float suspicionSpeedFraction = 0.8f;
        [SerializeField] float dwellTime = 1.7f; // todo introduce some randomness
        int currentWaypointIndex = 0;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceLastAggravated = Mathf.Infinity;

        LazyValue<Vector3> initialGuardPosition;
        Vector3 lastKnownPlayerPos;

        GameObject player;
        Fighter fighter;
        Health health;
        Health playerHealth;
        Mover mover;
        ActionScheduler actionScheduler;

        private void Awake()
        {
            player = GameObject.FindWithTag("Player");
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            playerHealth = player.GetComponent<Health>();
            initialGuardPosition = new LazyValue<Vector3>(GetInitialGuardPosition);
        }

        void Start()
        {
            initialGuardPosition.ForceInit();
        }

        void Update()
        {
            if (health.IsDead()) return;

            if (IsAggravated() && fighter.CanAttack(player))
            {
                AttackBehaviour();

                lastKnownPlayerPos = player.transform.position;
                timeSinceLastSawPlayer = 0;
            }
            else if (timeSinceLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();
            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceArrivedAtWaypoint += Time.deltaTime;
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceLastAggravated += Time.deltaTime;
        }

        private Vector3 GetInitialGuardPosition()
        {
            return transform.position;
        }

        private void AttackBehaviour()
        {
            fighter.Attack(player);
            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, shoutRadius, Vector3.forward, 0.01f);
            foreach (var hit in hits)
            {
                AIController enemy = hit.transform.GetComponent<AIController>();
                if (enemy == null) continue;

                enemy.Aggravate();
            }
        }

        private void SuspicionBehaviour()
        {
            mover.StartMoveAction(lastKnownPlayerPos, suspicionSpeedFraction);
            if (Vector3.Distance(transform.position, player.transform.position) <= 2f)
            {
                actionScheduler.CancelCurrentAction();
            }
        }

        private void PatrolBehaviour()
        {

            Vector3 nextPosition = initialGuardPosition.value;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoints();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if (timeSinceArrivedAtWaypoint > dwellTime)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }

        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < 0.1f;
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoints()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool IsAggravated()
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
            return distanceFromPlayer <= chaseDistance || timeSinceLastAggravated < aggroCooldownSeconds;
        }

        public void Aggravate()
        {
            timeSinceLastAggravated = 0;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

    }


}