// LaneManager.cs
using UnityEngine;
using System.Collections.Generic;

public class LaneManager : MonoBehaviour
{
    public static LaneManager instance;

    public Transform[] laneSpawnPoints;
    public GameObject notePrefab;

    private Queue<MidiLoader.NoteData> noteSpawnQueue;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (laneSpawnPoints.Length == 0)
        {
            Debug.LogError("Lane spawn points are not assigned!");
            return;
        }

        if (MidiLoader.instance != null)
        {
            noteSpawnQueue = new Queue<MidiLoader.NoteData>(MidiLoader.instance.noteDataList);
        }
        else
        {
            Debug.LogError("MidiLoader instance not found!");
        }
    }

    public void UpdateLaneManager(float songPosition)
    {
        while (noteSpawnQueue.Count > 0 && noteSpawnQueue.Peek().time <= songPosition)
        {
            var noteData = noteSpawnQueue.Dequeue();
            SpawnNote(noteData);
        }
    }

    void SpawnNote(MidiLoader.NoteData noteData)
    {
        if (noteData.lane < 0 || noteData.lane >= laneSpawnPoints.Length)
        {
            Debug.LogError($"Invalid lane number: {noteData.lane}");
            return;
        }

        Transform spawnPoint = laneSpawnPoints[noteData.lane];
        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);

        NoteController noteController = note.GetComponent<NoteController>();
        noteController.Initialize(Conductor.instance.noteSpeed);
    }
}
