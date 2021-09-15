using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        Health target = null;
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
        //AudioSource audioSource;
        float damage = 0;
        GameObject instigator = null;

        // private void Awake()
        // {
        //     audioSource = GetComponent<AudioSource>();
        // }

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
            if (target != null && isHomingProjectile)
            {
                transform.LookAt(GetTargetLocation());
            }
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetTarget(Health aimTarget, float weaponDamage, GameObject instigator, float calculatedDamage)
        {
            target = aimTarget;
            damage = calculatedDamage; //weaponDamage;
            transform.LookAt(GetTargetLocation());
            this.instigator = instigator;
        }

        private Vector3 GetTargetLocation()
        {
            CapsuleCollider capsule = target.GetComponent<CapsuleCollider>();
            if (capsule == null)
            {
                return target.transform.position;
            }
            return target.transform.position + Vector3.up * capsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return; // todo have other things like walls be able to block arrows

            if (impactSound != null)
            {
                AudioSource.PlayClipAtPoint(impactSound, transform.position);
            }

            if (target.IsDead()) return;

            speed = 0;

            target.TakeDamage(damage, instigator);
            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetTargetLocation(), transform.rotation);
            }

            if (ParticlesToStopEmittingOnImpact != null)
            {
                ParticleSystem.EmissionModule ps = ParticlesToStopEmittingOnImpact.emission;
                ps.enabled = false;
            }

            foreach (GameObject obj in destroyOnImpact)
            {
                Destroy(obj);
            }
            Destroy(gameObject, lifeAfterImpact);
        }
    }

}