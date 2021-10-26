using UnityEngine;
using System;
using RPG.Control;

namespace RPG.Abilities.Targeting
{
    [CreateAssetMenu(fileName = "Directional Targeting", menuName = "RPG/Abilities/Targeting/Directional", order = 0)]
    public class DirectionalTargeting : TargetingStrategy
    {
        [SerializeField] LayerMask layerMask;
        [SerializeField] float groundOffset = 1f;

        // Note: using a layer mask to identify the terrain. Could not have a terrain selected so the projectile could 
        // move upwards to potentially hit enemies above
        public override void StartTargeting(AbilityData data, Action finished)
        {
            Vector3 targetPoint = Vector3.zero;
            Ray ray = PlayerController.GetMouseRay();
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, layerMask))
            {
                //See note
                data.SetTargetedPoint(hit.point + ray.direction * groundOffset / ray.direction.y);
                finished();
            }

        }
    }

}

/* When two triangles with the same angles are scaled up or down, the ratio of the length of the sides remain the same.
    SideA / SideB of one triangle has the same ratio of SideC / SideD of the second. So A/B = C/D.
    We have a ray being cast down from the camera to the hitpoint but we have to figure out how far backwards along the same ray
    do we have to move the target point so the projectile moves through the cursor and not over where the actual hitpoint is. 
    
    The smaller triangle is the unit vector. It has a length of one but still has info about the actual direction and the scaled triangle 
    represents where on the ray do we put the target. You'll have to look at your notes for drawings.

    Anyway, if A is the direction vector of the ray, B is the Y component of the direction vector of the ray, and D is the ground offset,
    C represents how far back along the ray we want our new target so we're solving for C.
    A/B = C/D    C = D * A/B  or C = Ground Offset * ray.directionVector.length / ray.directionVector.Y

    Since the directionVector.Length is always 1, we can simplify even further. C = groundOffset / ray.direction.Y

*/
