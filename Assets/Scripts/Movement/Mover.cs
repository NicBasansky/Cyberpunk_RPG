using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Attributes;
using Random = UnityEngine.Random;
using GameDevTV.Saving;
using System;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        NavMeshAgent agent = null;
        Animator anim = null;
        float speed = 0;
        Health health;
        //bool animCycleSet = false; // TODO need? otherwise remove

        [SerializeField] float maxSpeed = 3.863f;
        [SerializeField] float maxPathLength = 40f;
        [SerializeField] float speakerFacingDist = 2f;

        bool freezeMovement = false;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            health = GetComponent<Health>();
        }

        private void Start()
        {
            agent.speed = maxSpeed;
            RandomizeIdleAnimationStartCycle();
        }

        void Update()
        {
            agent.enabled = !health.IsDead();

            UpdateAnimator();

        }

        private void RandomizeIdleAnimationStartCycle()
        {
            float number = Random.Range(0f, 1f);
            anim.SetFloat("CycleOffset", number);
        }

        public bool CanMoveTo(Vector3 destination)
        {
            if (freezeMovement) return false;

            // if there is no way to access this piece of navMesh then return false
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(
                                        transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath || path.status != NavMeshPathStatus.PathComplete)
            {
                return false;
            }

            if (GetPathLength(path) > maxPathLength)
            {
                return false;
            }

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            agent.isStopped = false;
            agent.destination = destination;
            agent.speed = maxSpeed * speedFraction;
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public void MoveToFrontOfSpeaker(Transform speakerTransform, float speedFraction)
        {
            Vector3 targetPos = GetPlayerPosForSpeaking(speakerTransform);
            StartMoveAction(targetPos, 1);
        }

        public Vector3 GetPlayerPosForSpeaking(Transform speakerTransform)
        {
            return speakerTransform.position + (speakerTransform.forward * speakerFacingDist);
        }

        public void Cancel()
        {
            agent.isStopped = true;
        }

        void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = agent.transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            anim.SetFloat("ForwardSpeed", speed);
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        public object CaptureState() // todo adjust if enemies stand just outside portal if they are chasing player
        {                            // this currently saves their location as we enter a portal
            return new SerializableVector3(transform.position);
        }


        public void RestoreState(object state) // note: if a char. does NOT have a nav Mesh agent active, it's 
                        //position will not be saved if it differs from their initial scene placement
        {
            if (GetComponent<NavMeshAgent>().enabled)
            {
                SerializableVector3 location = (SerializableVector3)state;
                GetComponent<NavMeshAgent>().enabled = false;
                transform.position = location.ToVector();
                GetComponent<NavMeshAgent>().enabled = true;
            }
            
        }
    }
}
