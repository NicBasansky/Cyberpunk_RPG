using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Control;
using RPG.Attributes;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon;
        [SerializeField] float respawnSeconds = 5f;
        [SerializeField] float healthRestore = 0;
        MeshRenderer[] meshRenderers;
        SphereCollider sphereCollider;
        ParticleSystem ps;

        private void Awake()
        {
            meshRenderers = GetComponentsInChildren<MeshRenderer>();
            sphereCollider = GetComponent<SphereCollider>();
            ps = GetComponentInChildren<ParticleSystem>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        private void Pickup(GameObject other)
        {
            if (weapon != null)
            {
                other.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if (healthRestore > 0)
            {
                other.GetComponent<Health>().Heal(healthRestore);
            }

            StartCoroutine(MakePickupInvisible());
        }

        IEnumerator MakePickupInvisible()
        {
            EnableMeshRenderers(false);
            sphereCollider.enabled = false;

            EnableParticleEmission(false);

            yield return new WaitForSeconds(respawnSeconds);

            EnableMeshRenderers(true);
            sphereCollider.enabled = true;

            EnableParticleEmission(true);
        }

        private void EnableParticleEmission(bool isEnabled)
        {
            if (ps == null) return;
            ParticleSystem.EmissionModule em = ps.emission;
            em.enabled = isEnabled;
        }

        private void EnableMeshRenderers(bool isEnabled)
        {
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.enabled = isEnabled;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            // todo remove the if stmt to make the player walk over to the item in order to pick it up
            if (Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);
            }
            return true;
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }
    }

}