using UnityEngine;
using System;
using System.Collections;

public class Conductor : MonoBehaviour
{
    public static Conductor instance;

    [Header("Music Settings")]
    public AudioSource songAudioSource;
    [Tooltip("Speed at which notes travel (units per second).")]
    public float noteSpeed;
    [Tooltip("Delay before the song starts in seconds.")]
    public float songDelayInSeconds;

    [Header("Synchronization Settings")]
    [Tooltip("Beats Per Minute of the song.")]
    public float bpm = 120f; // Default BPM, set this according to your music track

    [HideInInspector]
    public double songStartTime { get; private set; }
    private double dspSongTime;

    [Header("Gameplay References")]
    public GameObject playerObject;
    public GameObject spawnPointObject;

    [Tooltip("Additional buffer time to ensure synchronization.")]
    public float bufferTime = 1f;

    // Event to notify listeners (like PulseSync) when a beat occurs
    public event Action OnBeat;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (playerObject == null || spawnPointObject == null)
        {
            Debug.LogError("Player Object or Spawn Point Object not assigned in the Conductor script.");
            return;
        }

        // Calculate the Z distance between the player and the spawn point
        float playerZ = playerObject.transform.position.z;
        float spawnZ = spawnPointObject.transform.position.z;
        float distance = Mathf.Abs(spawnZ - playerZ);

        // Prevent divide by zero for note speed
        if (noteSpeed <= 0)
        {
            noteSpeed = 1f;
        }

        // Calculate delay and schedule song start
        songDelayInSeconds = distance / noteSpeed;
        songStartTime = AudioSettings.dspTime + songDelayInSeconds + bufferTime;
        songAudioSource.PlayScheduled(songStartTime);

        // Set the song's DSP time
        dspSongTime = songStartTime - songDelayInSeconds;

        // Start the beat tracking coroutine
        StartCoroutine(BeatTrackingCoroutine());
    }

    void Update()
    {
        // Calculate the current song position using DSP time
        double songPosition = AudioSettings.dspTime - dspSongTime;

        // Ensure LaneManager exists before updating it
        if (LaneManager.instance != null)
        {
            LaneManager.instance.UpdateLaneManager((float)songPosition);
        }
        else
        {
            Debug.LogError("LaneManager instance not found!");
        }
    }

    // Coroutine to track and broadcast beats based on the song's BPM
    private IEnumerator BeatTrackingCoroutine()
    {
        float beatInterval = 60f / bpm;
        double nextBeatTime = songStartTime + beatInterval;

        while (true)
        {
            double currentDspTime = AudioSettings.dspTime;
            double waitTime = nextBeatTime - currentDspTime;

            if (waitTime > 0)
            {
                yield return new WaitForSecondsRealtime((float)waitTime);
            }

            // Notify listeners of the beat
            OnBeat?.Invoke();
            Debug.Log($"Conductor: Beat occurred at DSP Time = {nextBeatTime}");

            // Schedule the next beat
            nextBeatTime += beatInterval;
        }
    }
}