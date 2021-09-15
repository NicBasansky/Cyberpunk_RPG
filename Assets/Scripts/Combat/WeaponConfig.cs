using UnityEngine;
using RPG.Attributes;
using GameDevTV.Inventories;
using RPG.Stats;
using System;
using System.Collections.Generic;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make New Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifier
    {
        [SerializeField] Weapon equippedWeaponPrefab = null;
        [SerializeField] AnimatorOverrideController animatorOverride = null;
        [SerializeField] Projectile projectile = null;
        [SerializeField] float damage = 5f;
        [SerializeField] float percentageBonus = 0;
        [SerializeField] float weaponRange = 1.5f;
        [SerializeField] bool isRightHanded = true;
        [SerializeField] bool weaponIsABow = false;

        const string weaponName = "Weapon";


        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if (equippedWeaponPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);

                weapon = Instantiate(equippedWeaponPrefab, handTransform);
                weapon.gameObject.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;

        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Projectile projectileInstance = Instantiate(projectile, GetLaunchTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, damage, instigator, calculatedDamage);
        }


        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return; // nothing to destroy

            oldWeapon.name = "Destroying";
            Destroy(oldWeapon.gameObject);
        }

        public float GetPercentageBonus()
        {
            return percentageBonus;
        }

        private Transform GetTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHandTransform;
            else handTransform = leftHandTransform;

            return handTransform;
        }

        private Transform GetLaunchTransform(Transform rightHandTransform, Transform leftHandTransform)
        {
            Transform launchTransform;
            if (weaponIsABow) launchTransform = rightHandTransform;
            else launchTransform = GetTransform(rightHandTransform, leftHandTransform);

            return launchTransform;
        }

        public bool HasProjectile()
        {
            return projectile != null;
        }

        public float GetDamage()
        {
            return damage;
        }

        public float GetWeaponRange()
        {
            return weaponRange;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return percentageBonus;
            }
        }
    }
}