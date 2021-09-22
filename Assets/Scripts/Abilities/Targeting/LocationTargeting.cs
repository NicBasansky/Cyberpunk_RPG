using UnityEngine;
using RPG.Control;
using System.Collections;
using System;
using System.Collections.Generic;


// NOTE! This currently is only effective on objects in the layerMask, ie the Terrain layer only
namespace RPG.Abilities
{
    [CreateAssetMenu(fileName = "Location Targeting", menuName = "RPG/Abilities/Targeting/Location", order = 0)]
    public class LocationTargeting : TargetingStrategy
    {
        [SerializeField] Texture2D cursorTexture;
        [SerializeField] Vector2 cursorHotSpot;
        [SerializeField] float effectRadius = 5f;
        [SerializeField] LayerMask layerMask;
        [SerializeField] GameObject targetingPrefab;
        GameObject targetingInstancePrefab = null;

        public override void StartTargeting(AbilityData data, Action finished)
        {
            PlayerController controller = data.GetUser().GetComponent<PlayerController>();
            
            // notice how the controller is calling THIS coroutine
            controller.StartCoroutine(Targeting(data, controller, finished));
        }

        private IEnumerator Targeting(AbilityData data, PlayerController controller, Action finished)
        {
            controller.enabled = false;
            if (!targetingInstancePrefab)
            {
                targetingInstancePrefab = Instantiate(targetingPrefab);
            }
            else
            {
                targetingInstancePrefab.SetActive(true);
            }
            targetingInstancePrefab.transform.localScale = new Vector3(effectRadius * 2, 1, effectRadius * 2);

            while (true)
            {
                Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
                Ray ray = PlayerController.GetMouseRay();
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, layerMask))
                {
                    targetingInstancePrefab.transform.position = hit.point;
                    if (Input.GetMouseButtonDown(0))
                    {
                        // while (Input.GetMouseButton(0))
                        // {                                                      
                        //     yield return null;
                        // }
                        // this is a more compact way of writing the above code
                        // absorb the whole mouse click to not interfere with player controller movement
                        yield return new WaitWhile(() => Input.GetMouseButton(0));

                        targetingInstancePrefab.SetActive(false);
                        controller.enabled = true;

                        // need to pass in the list of targets into finished()
                        data.SetTargets(GetGameObjectsInRadius(hit.point));
                        finished();
                        break;
                    }
                }
                yield return null;
            }
        }

        private IEnumerable<GameObject> GetGameObjectsInRadius(Vector3 origin)
        {
            RaycastHit[] hits = Physics.SphereCastAll(origin, effectRadius, new Vector3(0f, 0.01f, 0f), 0.01f);
            foreach (var go in hits)
            {
                yield return go.collider.gameObject;
            }

        }
    }
}

