using UnityEngine;
using System.Collections.Generic;

public class Worker : MonoBehaviour
{
    public enum WorkerState
    {
        Idle,
        Working,
        GoingToDistraction,
        Eating,
        WatchingTV,
        Resting,
        DrinkingCoffee,
        ListeningRadio
    }

    [Header("Current Status")]
    [SerializeField] private WorkerState currentState = WorkerState.Idle;

    [Header("Happiness")]
    [SerializeField] private float happiness = 100f;
    [SerializeField] private float maxHappiness = 100f;

    [Header("Work")]
    [SerializeField] private float moneyPerSecond = 1f;
    [SerializeField] private float happinessLoss = 5f;

    [Header("Relaxation")]
    [SerializeField] private float happinessRecovery = 5f;

    [Header("Disobedience")]
    [SerializeField] private float minWorkTime = 3f;
    [SerializeField] private float maxWorkTime = 7f;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource loopAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    [SerializeField] private AudioClip workingSound;
    [SerializeField] private AudioClip walkingSound;

    [SerializeField] private AudioClip eatingSound;
    [SerializeField] private AudioClip tvSound;
    [SerializeField] private AudioClip restingSound;
    [SerializeField] private AudioClip coffeeSound;
    [SerializeField] private AudioClip radioSound;

    [SerializeField] private AudioClip disobeySound;


    private float workTimer;

    private ActivityZone targetZone;

    private Rigidbody rb;
    private Draggable draggable;

    private ActivityZone[] allZones;

    public WorkerState CurrentState => currentState;
    public float Happiness => happiness;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        draggable = GetComponent<Draggable>();

        allZones = FindObjectsByType<ActivityZone>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleCurrentActivity(); 
    }
    
    
    void FixedUpdate()
    {
        HandleAutomaticMovement();
    }


    void HandleCurrentActivity()
    {
        if (currentState == WorkerState.Working)
        {
            //Produce money
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddMoney(moneyPerSecond * Time.deltaTime);
            }

            //Lose happiness
            happiness -= happinessLoss * Time.deltaTime;

            // Countdown until worker gets bored
            workTimer -= Time.deltaTime;

            if (workTimer <= 0f)
            {
                ChooseDistraction();
            }
        }
        else if (currentState != WorkerState.Idle && currentState != WorkerState.GoingToDistraction)
        {
            // Recover happiness while doing a distraction
            happiness += happinessRecovery * Time.deltaTime;
        }
        happiness = Mathf.Clamp(happiness, 0f, maxHappiness);
    }


    void ChooseDistraction()
    {
        List<ActivityZone> distractions = new List<ActivityZone>();

        foreach (ActivityZone zone in allZones)
        {
            if (zone.zoneType != ActivityZone.ZoneType.Desk)
            {
                distractions.Add(zone);
            }
        }

        if (distractions.Count == 0)
        {
            Debug.LogWarning("No distractions available found!");
            return;
        }

        int randomIndex = Random.Range(0, distractions.Count);
        targetZone = distractions[randomIndex];
        currentState = WorkerState.GoingToDistraction;

        PlaySFX(disobeySound);

        PlayLoop(walkingSound);

        Debug.Log(gameObject.name + " stopped working and is going to " + targetZone.zoneType);
    }


    void HandleAutomaticMovement()
    {
        if (currentState != WorkerState.GoingToDistraction || targetZone == null)
        {
            return;
        }

        // Player is currently holder worker
        if (draggable != null && draggable.IsDragging)
        {
            return;
        }

        Vector3 targetPosition = targetZone.transform.position;

        // Keep worker at current height (y position)
        targetPosition.y = rb.position.y;

        Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
        rb.MovePosition(newPosition);

        float distance = Vector3.Distance(rb.position, targetPosition);

        if (distance < 0.15f)
        {
            SetActivity(targetZone.zoneType);
            targetZone = null;
        }
    }


    public void SetActivity(ActivityZone.ZoneType zoneType)
    {
        targetZone = null;

        switch (zoneType)
        {
            case ActivityZone.ZoneType.Desk:
                currentState = WorkerState.Working;
                ResetWorkTimer();
                break;

            case ActivityZone.ZoneType.Kitchen:
                currentState = WorkerState.Eating;
                break;

            case ActivityZone.ZoneType.TV:
                currentState = WorkerState.WatchingTV;
                break;

            case ActivityZone.ZoneType.Sofa:
                currentState = WorkerState.Resting;
                break;

            case ActivityZone.ZoneType.CoffeeMachine:
                currentState = WorkerState.DrinkingCoffee;
                break;

            case ActivityZone.ZoneType.Radio:
                currentState = WorkerState.ListeningRadio;
                break;

            default:
                currentState = WorkerState.Idle;
                break;
        }

        switch (currentState)
        {
            case WorkerState.Working:
                PlayLoop(workingSound);
                break;

            case WorkerState.Eating:
                PlayLoop(eatingSound);
                break;

            case WorkerState.WatchingTV:
                PlayLoop(tvSound);
                break;

            case WorkerState.Resting:
                PlayLoop(restingSound);
                break;

            case WorkerState.DrinkingCoffee:
                PlayLoop(coffeeSound);
                break;

            case WorkerState.ListeningRadio:
                PlayLoop(radioSound);
                break;

            default:
                StopLoop();
                break;
        }

        Debug.Log(gameObject.name + " is now in state: " + currentState);
    }


    void ResetWorkTimer()
    {
        workTimer = Random.Range(minWorkTime, maxWorkTime);
        Debug.Log(gameObject.name + " will work for " + workTimer.ToString("F1") + " seconds.");
    }   



    public void SetIdle()
    {
        targetZone = null;
        currentState = WorkerState.Idle;
        //workTimer = 0;

        StopLoop();

        Debug.Log(gameObject.name + " is now idle");
    }


    void PlayLoop(AudioClip clip)
    {
        if (clip == null || loopAudioSource == null)
        {
            return;
        }

        if (loopAudioSource.clip == clip && loopAudioSource.isPlaying)
        {
            return;
        }

        loopAudioSource.Stop();
        loopAudioSource.clip = clip;
        loopAudioSource.loop = true;

        loopAudioSource.Play();
    }


    void StopLoop()
    {
        if (loopAudioSource == null)
        {
            return;
        }

        loopAudioSource.Stop();
        loopAudioSource.clip = null;
    }


    void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxAudioSource == null)
        {
            return;
        }

        sfxAudioSource.PlayOneShot(clip);
    }

    public void StopAllAudio()
    {
        StopLoop();
        if (sfxAudioSource != null)
        {
            sfxAudioSource.Stop();
        }   
    }
}
