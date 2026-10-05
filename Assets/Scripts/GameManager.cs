using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public event Action OnShiftEnded;

    [Header("Money")]
    [SerializeField] private float money = 0f;
    [SerializeField] private float quota = 60f;

    [Header("Shift")]
    [SerializeField] private float shiftDuration = 60f;

    private float timeRemaining;
    private bool shiftRunning = true;

    public float Money => money;
    public float Quota => quota;
    public float TimeReamaining => timeRemaining;
    public bool ShiftRunning => shiftRunning;


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    void Start()
    {
        timeRemaining = shiftDuration;
    }


    void Update()
    {
        if (!shiftRunning)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            shiftRunning = false;

            Debug.Log("SHIFT OVER");

            if (money >= quota)
            {
                Debug.Log("QUOTA REACHED!");
            }
            else
            {
                Debug.Log("QUOTA FAILED!");
            }
            // 
            OnShiftEnded?.Invoke();
        }
    }
    
    public void AddMoney(float amount)
    {
        if (!shiftRunning)
        {
            return;
        }

        money += amount;
    }
}
