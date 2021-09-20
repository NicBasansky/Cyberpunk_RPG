using System;
using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Attributes;
using RPG.Combat;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using GameDevTV.Inventories;


// TODO add 
namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {

        [System.Serializable]
        struct CursorDetails
        {
            public CursorType cursorType;
            public Texture2D cursorImage;
            public Vector2 hotspot;
        }
        [SerializeField] float raycastRadius = 0.75f;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] CursorDetails[] cursorDetails = null;

        bool isDraggingUI = false;

        Mover mover;
        Fighter fighter;
        Health health;


        private void Awake()
        {
            mover = GetComponent<Mover>();
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
        }

        private void Update()
        {
            
            if (InteractWithUI()) return;

            if (health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }
            CheckSpecialAbilityKeys();

            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;


            SetCursor(CursorType.None);
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted(); //Physics.RaycastAll(GetMouseRay());

            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    // see if the raycastable can handle the request
                    if (raycastable.HandleRaycast(this))
                    {
                        // for now, just use the combat cursor and we'll change it later for interactables
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }



        private bool InteractWithMovement()
        {
            //RaycastHit hit;
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (hasHit)
            {
                if (!mover.CanMoveTo(target)) return false;

                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(target, 1);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }

            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }

                SetCursor(CursorType.UI);
                return true;
            }

            if (isDraggingUI)
            {
                return true;
            }

            return false;
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorDetails details = GetCursorDetails(cursorType);
            Cursor.SetCursor(details.cursorImage, details.hotspot, CursorMode.Auto);
            //SetCursor(CursorType.None);
        }

        private CursorDetails GetCursorDetails(CursorType type)
        {
            foreach (var cursor in cursorDetails)
            {
                if (cursor.cursorType == type)
                    return cursor;
            }
            return cursorDetails[0];
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasNavMeshNearby = NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasNavMeshNearby) return false;

            target = navMeshHit.position;


            return true;
        }



        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
            float[] distances = new float[hits.Length];

            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);

            return hits;
        }

        private void CheckSpecialAbilityKeys()
        {
            for (int i = 0; i < 6; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    GetComponent<ActionStore>().Use(i, gameObject);
                }

            }
        }
    }

}
