using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Define the LevelManager class inheriting from MonoBehaviour
public class LevelManager : MonoBehaviour
{
    // Static instance for Singleton pattern
    private static LevelManager _instance;

    // SerializeField allows this to be set in the Unity Editor
    [SerializeField] private int lives = 10;

    // Public properties to access and set total lives and the current wave
    public int TotalLives { get; set; }
    public int CurrentWave { get; set; }

    // Public action delegate for handling end-reached events
    public Action<Shroom> OnEndReached;

    // Property for accessing the Singleton instance of LevelManager
    public static LevelManager Instance
    {
        get
        {
            // If an instance doesn't exist, find it or create a new one
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>();
                if (_instance == null)
                {
                    GameObject singleton = new GameObject("LevelManagerSingleton");
                    _instance = singleton.AddComponent<LevelManager>();
                }
            }
            return _instance;
        }
    }

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Initialize the Singleton instance
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            // Ensure that there is only one instance in the scene
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Initialize total lives and the current wave
        TotalLives = lives;
        CurrentWave = 1;
    }

    // Method to reduce lives when an enemy reaches the end
    private void ReduceLives(Shroom enemy)
    {
        TotalLives--;
        // Check if the lives have run out
        if (TotalLives <= 0)
        {
            TotalLives = 0;
            GameOver();
        }
    }

    // Method to handle game over scenario
    private void GameOver()
    {
        // Implement game over logic
    }

    // Method to handle completion of a wave
    private void WaveCompleted()
    {
        // Implement wave completed logic
    }

    // OnEnable is called when the object becomes enabled and active
    private void OnEnable()
    {
        // Subscribe to the OnEndReached event
        OnEndReached += ReduceLives;
    }

    // OnDisable is called when the object becomes disabled or inactive
    private void OnDisable()
    {
        // Unsubscribe from the OnEndReached event
        OnEndReached -= ReduceLives;
    }
}
