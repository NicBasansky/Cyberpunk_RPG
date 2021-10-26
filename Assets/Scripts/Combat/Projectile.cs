using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 20f;
        [SerializeField] bool isHomingProjectile = false;
        [SerializeField] GameObject hitEffect = null;
        [SerializeField] float maxLifeSeconds = 10f;
        [SerializeField] GameObject[] destroyOnImpact;
        [SerializeField] float lifeAfterImpact = 2.0f;
        [SerializeField] ParticleSystem ParticlesToStopEmittingOnImpact = null;

        [Header("Sound FX")]
        [SerializeField] AudioClip launchSound;
        [SerializeField] AudioClip impactSound;

        Health target = null;
        float damage = 0;
        GameObject instigator = null;
        Vector3 targetPoint;

        private void Start()
        {
            if (launchSound != null)
            {
                AudioSource.PlayClipAtPoint(launchSound, transform.position);
            }

            Destroy(gameObject, maxLifeSeconds); // todo make an object pool in the future
        }

        void Update()
        {
            if (target != null && !target.IsDead() && isHomingProjectile)
            {
                transform.LookAt(GetTargetLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            SetTarget(instigator, damage, target);
        }

        public void SetTarget(GameObject instigator, float damage, Vector3 targetPoint)
        {
            SetTarget(instigator, damage, null, targetPoint);
        }

        public void SetTarget(GameObject instigator, float damage, Health target=null, Vector3 targetPoint=default)
        {
            this.target = target;
            this.damage = damage; 
            this.instigator = instigator;
            this.targetPoint = targetPoint;
            transform.LookAt(GetTargetLocation());

        }

        private Vector3 GetTargetLocation()
        {
            if (target == null)
            {
                return targetPoint;
            }
            CapsuleCollider capsule = target.GetComponent<CapsuleCollider>();
            if (capsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * capsule.height / 2;
        }

        // todo have other things like walls be able to block arrows
        private void OnTriggerEnter(Collider other)
        {
            Health health = other.GetComponent<Health>();

            // if we have a target health and it's not the correct matching health target then return
            if (target != null && target != health) return; 
            // if we hit a something without a health component or we did and it's already dead then return 
            if (health == null || health.IsDead()) return;
            // if we hit ourselves while launching then return
            if (other.gameObject == instigator) return;

            if (impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }

            //if (target.IsDead()) return;

            speed = 0;

            health.TakeDamage(damage, instigator);

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetTargetLocation(), transform.rotation);
            }

            if (ParticlesToStopEmittingOnImpact != null)
            {
                ParticleSystem.EmissionModule ps = ParticlesToStopEmittingOnImpact.emission;
                ps.enabled = false;
            }

            // disable the collider upon impact
            Collider collider = GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }

            foreach (GameObject obj in destroyOnImpact)
            {
                Destroy(obj);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }

}