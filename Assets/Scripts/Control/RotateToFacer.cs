using RPG.Core;
using UnityEngine;

namespace RPG.Control
{

    public class RotateToFacer : MonoBehaviour, IAction
    {
        [SerializeField] float rotationSpeed = 6.0f;

        bool rotate = false;
        float timeElapsed = 0;

        Vector3 targetDirection;
        ActionScheduler scheduler;


        void LateUpdate()
        {
            if (rotate)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetDirection, Vector3.up),
                                                   rotationSpeed * timeElapsed);
                timeElapsed += Time.deltaTime;

                if (Vector3.Angle(transform.forward, new Vector3(targetDirection.x, transform.position.y, targetDirection.z)) < 5.0f)
                {
                    rotate = false;
                    //Cancel();
                }
            }
        }

        public void RotateToFaceTargetLocation(Vector3 target)
        {
            targetDirection = target - transform.position;
            scheduler = GetComponent<ActionScheduler>();
            if (scheduler)
            {
                scheduler.StartAction(this);
            }
            timeElapsed = 0;
            rotate = true;
        }

        //needed for IAction
        public void Cancel()
        {
            rotate = false;
        }
    }

}