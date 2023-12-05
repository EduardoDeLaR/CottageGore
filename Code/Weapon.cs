using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define the Turret class inheriting from MonoBehaviour
public class Turret : MonoBehaviour
{
    // SerializeField attribute allows the attackRange to be set in the Unity Editor
    [SerializeField] private float attackRange = 3f;

    // Public properties for accessing and setting the current enemy target and turret upgrades
    public Enemy CurrentEnemyTarget { get; set; }
    public TurretUpgrade TurretUpgrade { get; set; }
    public float AttackRange => attackRange; // Read-only property for attackRange
    
    // Private fields for game state and list of enemies
    private bool _gameStarted;
    private List<Enemy> _enemies;

    // Start is called before the first frame update
    private void Start()
    {
        _gameStarted = true; // Mark the game as started
        _enemies = new List<Enemy>(); // Initialize the list of enemies

        // Get the TurretUpgrade component attached to the same GameObject
        TurretUpgrade = GetComponent<TurretUpgrade>();
    }

    // Update is called once per frame
    private void Update()
    {
        GetCurrentEnemyTarget(); // Determine the current enemy target
        RotateTowardsTarget(); // Rotate the turret towards the current target
    }

    // Method to determine the current enemy target
    private void GetCurrentEnemyTarget()
    {
        if (_enemies.Count <= 0) // Check if there are no enemies
        {
            CurrentEnemyTarget = null; // Set the current target to null
            return;
        }

        // Set the current target to the first enemy in the list
        CurrentEnemyTarget = _enemies[0];
    }

    // Method to rotate the turret towards its current target
    private void RotateTowardsTarget()
    {
        if (CurrentEnemyTarget == null) // Check if there is no current target
        {
            return; // Exit the method early
        }

        // Calculate the direction to the target
        Vector3 targetPos = CurrentEnemyTarget.transform.position - transform.position;
        // Calculate the angle required to rotate towards the target
        float angle = Vector3.SignedAngle(transform.up, targetPos, transform.forward);
        // Apply the rotation to the turret
        transform.Rotate(0f, 0f, angle);
    }
    
    // Trigger event for entering 2D colliders
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Check if the collider belongs to an enemy
        {
            // Get the Enemy component from the collided object
            Enemy newEnemy = other.GetComponent<Enemy>();
            // Add the enemy to the list
            _enemies.Add(newEnemy);
        }
    }

    // Trigger event for exiting 2D colliders
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // Check if the collider belongs to an enemy
        {
            // Get the Enemy component from the collider
            Enemy enemy = other.GetComponent<Enemy>();
            // Remove the enemy from the list if it exists
            if (_enemies.Contains(enemy))
            {
                _enemies.Remove(enemy);
            }
        }
    }

    // Method for drawing Gizmos in the Unity Editor
    private void OnDrawGizmos()
    {
        // Set the radius of the collider before the game starts
        if (!_gameStarted)
        {
            GetComponent<CircleCollider2D>().radius = attackRange;
        }
        
        // Draw a wireframe sphere representing the attack range in the Unity Editor
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
