using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define the Projectile class inheriting from MonoBehaviour
public class Projectile : MonoBehaviour
{
    // Public static event to notify when an enemy is hit by a projectile
    public static Action<Enemy, float> OnEnemyHit;
    
    // SerializeField attributes allow these values to be set in the Unity Editor
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] private float minDistanceToDealDamage = 0.1f;

    // Public properties to set the turret owner and damage of the projectile
    public TurretProjectile TurretOwner { get; set; }
    public float Damage { get; set; }
    
    // Protected variable to store the target enemy
    protected Enemy _enemyTarget;

    // Update is called once per frame
    protected virtual void Update()
    {
        // If there is an enemy target, move and rotate the projectile
        if (_enemyTarget != null)
        {
            MoveProjectile();
            RotateProjectile();
        }
    }

    // Method to move the projectile towards the target
    protected virtual void MoveProjectile()
    {
        // Move the projectile towards the enemy target
        transform.position = Vector2.MoveTowards(transform.position, 
            _enemyTarget.transform.position, moveSpeed * Time.deltaTime);

        // Calculate the distance to the target
        float distanceToTarget = (_enemyTarget.transform.position - transform.position).magnitude;

        // If the projectile is close enough to the target, deal damage
        if (distanceToTarget < minDistanceToDealDamage)
        {
            // Invoke the OnEnemyHit event
            OnEnemyHit?.Invoke(_enemyTarget, Damage);
            // Deal damage to the enemy
            _enemyTarget.EnemyHealth.DealDamage(Damage);
            // Notify the turret owner to reset the projectile
            TurretOwner.ResetTurretProjectile();
            // Return the projectile to the object pool
            ObjectPooler.ReturnToPool(gameObject);
        }
    }

    // Method to rotate the projectile towards the target
    private void RotateProjectile()
    {
        // Calculate the direction to the enemy
        Vector3 enemyPos = _enemyTarget.transform.position - transform.position;
        // Calculate the angle required to rotate towards the enemy
        float angle = Vector3.SignedAngle(transform.up, enemyPos, transform.forward);
        // Apply the rotation to the projectile
        transform.Rotate(0f, 0f, angle);
    }
    
    // Method to set the enemy target for the projectile
    public void SetEnemy(Enemy enemy)
    {
        _enemyTarget = enemy;
    }

    // Method to reset the projectile (typically called when reusing projectiles from a pool)
    public void ResetProjectile()
    {
        // Reset the target enemy and rotation of the projectile
        _enemyTarget = null;
        transform.localRotation = Quaternion.identity;
    }
}
