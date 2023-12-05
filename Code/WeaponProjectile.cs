using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define the WeaponProjectile class inheriting from MonoBehaviour
public class WeaponProjectile : MonoBehaviour
{
    // SerializeField attributes allow these properties to be set in the Unity Editor
    [SerializeField] protected Transform projectileSpawnPosition;
    [SerializeField] protected float delayBtwAttacks = 2f;
    [SerializeField] protected float damage = 2f;

    // Public properties for accessing and setting damage and delay per shot
    public float Damage { get; set; }
    public float DelayPerShot { get; set; }
    
    // Protected fields for internal mechanics of the weapon
    protected float _nextAttackTime;
    protected ObjectPooler _pooler;
    protected Turret _turret;
    protected Projectile _currentProjectileLoaded;

    // Start is called before the first frame update
    private void Start()
    {
        // Get the Turret and ObjectPooler components attached to the same GameObject
        _turret = GetComponent<Turret>();
        _pooler = GetComponent<ObjectPooler>();

        // Initialize Damage and DelayPerShot from serialized fields
        Damage = damage;
        DelayPerShot = delayBtwAttacks;

        // Load the first projectile
        LoadProjectile();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Check if the turret is empty, load projectile if it is
        if (IsTurretEmpty())
        {
            LoadProjectile();
        }

        // Check if it's time for the next attack
        if (Time.time > _nextAttackTime)
        {
            // Check if there is a target and a loaded projectile and target has health
            if (_turret.CurrentEnemyTarget != null && _currentProjectileLoaded != null &&
                _turret.CurrentEnemyTarget.EnemyHealth.CurrentHealth > 0f)
            {
                // Launch the projectile
                _currentProjectileLoaded.transform.parent = null;
                _currentProjectileLoaded.SetEnemy(_turret.CurrentEnemyTarget);
                AudioManager.Instance.PlayerSound(AudioManager.Sound.rocket);
            }

            // Update the time for the next attack
            _nextAttackTime = Time.time + DelayPerShot;
        }
    }

    // Load a new projectile from the object pool
    protected virtual void LoadProjectile()
    {
        // Get a new instance from the object pool
        GameObject newInstance = _pooler.GetInstanceFromPool();
        // Set its position and parent
        newInstance.transform.localPosition = projectileSpawnPosition.position;
        newInstance.transform.SetParent(projectileSpawnPosition);

        // Get the Projectile component and configure it
        _currentProjectileLoaded = newInstance.GetComponent<Projectile>();
        _currentProjectileLoaded.TurretOwner = this;
        _currentProjectileLoaded.ResetProjectile();
        _currentProjectileLoaded.Damage = Damage;
        // Activate the new instance
        newInstance.SetActive(true);
    }

    // Check if the turret currently has a loaded projectile
    private bool IsTurretEmpty()
    {
        return _currentProjectileLoaded == null;
    }
    
    // Reset the current projectile, typically after firing
    public void ResetTurretProjectile()
    {
        _currentProjectileLoaded = null;
    }
}
