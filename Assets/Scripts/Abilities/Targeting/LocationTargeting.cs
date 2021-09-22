using UnityEngine;
using RPG.Control;
using System.Collections;
using System;
using System.Collections.Generic;


// NOTE! This currently is only effective on objects in the layerMask, ie the Terrain layer only

[CreateAssetMenu(fileName = "Location Targeting", menuName = "RPG/Abilities/Targeting/Location", order = 0)]
public class LocationTargeting : TargetingStrategy
{
    [SerializeField] Texture2D cursorTexture;
    [SerializeField] Vector2 cursorHotSpot;
    [SerializeField] float effectRadius = 5f;
    [SerializeField] LayerMask layerMask;

    // we want to click
    // have a cursor show up
    // wait for the next click
    // execute the ability
    public override void StartTargeting(GameObject user, Action<IEnumerable<GameObject>> finished)
    {
        PlayerController controller = user.GetComponent<PlayerController>();
        controller.StartCoroutine(Targeting(user, controller, finished));
    }

    // notice how the controller is calling THIS coroutine
    private IEnumerator Targeting(GameObject user, PlayerController controller, Action<IEnumerable<GameObject>> finished)
    {
        controller.enabled = false;
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // while (Input.GetMouseButton(0))
                // {
                //     yield return null;
                // }
                // this is a more compact way of writing the above code
                // absorb the whole mouse click to not interfere with player controller movement
                yield return new WaitWhile(() => Input.GetMouseButton(0));
                controller.enabled = true;

                // need to pass in the list of targets into finished()
                finished(GetGameObjectsInRadius());
                break;
            }
            Cursor.SetCursor(cursorTexture, cursorHotSpot, CursorMode.Auto);
            yield return null;
        }
    }

    private IEnumerable<GameObject> GetGameObjectsInRadius()
    {
        Ray ray = PlayerController.GetMouseRay();
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            RaycastHit[] hits = Physics.SphereCastAll(hit.point, effectRadius, new Vector3(0f, 0.01f, 0f), 0.01f);
            foreach(var go in hits)
            {
                yield return go.collider.gameObject;
            }                      
        }
    }

    
}
