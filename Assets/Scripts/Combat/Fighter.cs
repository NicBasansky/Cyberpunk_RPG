using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using GameDevTV.Saving;
using RPG.Stats;
using GameDevTV.Utils;
using GameDevTV.Inventories;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] float timeBetweenAttacks = 2f; // todo change to .5

        float timeSinceLastAttack = Mathf.Infinity;

        WeaponConfig currentWeaponConfig;
        Equipment equipment;
        LazyValue<Weapon> currentWeapon;
        Health target = null;
        ActionScheduler actionScheduler;
        Mover mover;
        Animator anim;
        BaseStats baseStats;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            anim = GetComponent<Animator>();
            baseStats = GetComponent<BaseStats>();
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
            equipment = GetComponent<Equipment>();
            if (equipment)
            {
                equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private void Start()
        {
            currentWeapon.ForceInit();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead())
            {
                anim.ResetTrigger("Attack");
                anim.SetTrigger("StopAttacking");
                return;
            }

            if (!IsInWeaponRange(target.transform))
            {
                mover.MoveTo(target.transform.position, 1);
            }
            else
            {
                AttackBehaviour();
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeapon);
        }


        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void UpdateWeapon()
        {
            var weapon = equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;
            if (weapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private void AttackBehaviour()
        {
            mover.Cancel();
            transform.LookAt(target.transform.position, Vector3.up);

            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                anim.ResetTrigger("StopAttacking");
                anim.SetTrigger("Attack"); // todo see if a bool is better for more fluid looping attack animations
                timeSinceLastAttack = 0;
            }

        }

        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (!mover.CanMoveTo(combatTarget.transform.position) &&
                !IsInWeaponRange(combatTarget.transform))
            {
                return false;
            }
            Health health = combatTarget.GetComponent<Health>();
            return health != null && !health.IsDead();
        }

        // animation event
        public void Hit()
        {
            if (target == null) return;
            float damage = baseStats.GetStat(Stat.Damage);



            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, this.gameObject, damage);
            }
            else if (IsInWeaponRange(target.transform))
            {
                if (currentWeapon.value != null) // todo see if this needs to be put back on line 118
                {
                    currentWeapon.value.OnHit();
                }

                target.TakeDamage(damage, this.gameObject);
            }
        }

        private bool IsInWeaponRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= currentWeaponConfig.GetWeaponRange();
        }

        public Health GetTarget()
        {
            if (target != null)
            {
                return target;
            }
            else
            {
                return null;
            }
        }

        public void Cancel()
        {
            anim.ResetTrigger("Attack");
            anim.SetTrigger("StopAttacking");
            target = null;
            mover.Cancel();
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }

}
