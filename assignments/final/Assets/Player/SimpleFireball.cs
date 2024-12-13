using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Cinemachine.Samples
{
    /// <summary>
    /// This component manages player shooting.  It is expected to be on the player object, 
    /// or on a child SimplePlayerAimController object of the player.
    /// 
    /// If an AimTargetManager is specified, then the behaviour aims at that target.
    /// Otherwise, the behaviour aims in the forward direction of the player object,
    /// or of the SimplePlayerAimController object if it exists and is not decoupled
    /// from the player rotation.
    /// </summary>
    class SimpleFireball : SimplePlayerShoot, Unity.Cinemachine.IInputAxisOwner
    {
        public override int stamina { get; set; } = 50;
        float m_LastFireTime;
        SimplePlayerAimController AimController;

        // We pool the bullets for improved performance
        readonly List<GameObject> m_BulletPool = new();


        /// Report the available input axes to the input axis controller.
        /// We use the Input Axis Controller because it works with both the Input package
        /// and the Legacy input system.  This is sample code and we
        /// want it to work everywhere.
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref Fire, Name = "Fire" });
        }

        void OnValidate()
        {
            MaxBulletsPerSec = Mathf.Max(1, MaxBulletsPerSec);
        }


        private void OnEnable()
        {
            selected = false;
        }

        void Start()
        {
            TryGetComponent(out AimController);
        }

        bool shouldFire()
        {
            var now = Time.time;
            return BulletPrefab != null
                && now - m_LastFireTime > 1 / MaxBulletsPerSec
                && Fire.Value > 0.1f && selected && cooldown;
        }


        void Update()
        {
            var now = Time.time;
            bool fireNow = shouldFire();

            // Get the firing direction.  Special case: if there is a decoupled AimController,
            // firing direction is character forward, not AimController forward.
            var fwd = transform.forward;
            bool decoupled = AimController != null
                && AimController.PlayerRotation == SimplePlayerAimController.CouplingMode.Decoupled;
            if (decoupled)
                fwd = transform.parent.forward;

            // Face the firing direction if appropriate
            if (AimController != null && !decoupled)
            {
                var rotationTime = AimController.RotationDamping;
                if (fireNow || now - m_LastFireTime <= rotationTime)
                    AimController.RecenterPlayer(rotationTime);
            }

            // Fire the bullet
            if (fireNow && GM.instance.stamina > stamina)
            {
                GM.instance.publicStamina = stamina;
                GM.instance.recoverS = false;

                m_LastFireTime = now;

                var pos = Vector3.zero;
                var rot = Quaternion.LookRotation(pos);
                if (!decoupled)
                {

                    if (AimTargetManager != null)
                        fwd = AimTargetManager.GetAimDirection(transform.GetChild(0).position, fwd).normalized;


                    pos = transform.GetChild(0).position + fwd;
                    rot = Quaternion.LookRotation(fwd, transform.up);
                }
                else
                {
                    if (AimTargetManager != null)
                        fwd = AimTargetManager.GetAimDirection(transform.position, fwd).normalized;


                    pos = transform.position + fwd;
                    rot = Quaternion.LookRotation(fwd, transform.up);
                }

                // Because creating and destroying GameObjects is costly, we pool them and recycle
                // the deactivated ones.  The bullets deactivate themselves after a time.
                GameObject bullet = null;
                for (var i = 0; bullet == null && i < m_BulletPool.Count; ++i) // Look in the pool if one is available
                {
                    if (!m_BulletPool[i].activeInHierarchy)
                    {
                        bullet = m_BulletPool[i];
                        bullet.transform.SetPositionAndRotation(pos, rot);
                        m_BulletPool.Remove(bullet);
                    }
                }
                // Instantiate a new bullet if none are found in the pool
                if (bullet == null)
                    bullet = Instantiate(BulletPrefab, pos, rot);

                // Off it goes!
                m_BulletPool.Add(bullet);
                bullet.SetActive(true);
                FireEvent.Invoke();
            }
        }
    }
}
