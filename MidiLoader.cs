using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.Collections.Generic;
using System.IO;

public class MidiLoader : MonoBehaviour
{
    public static MidiLoader instance;

    public string midiFileName; // Name of the MIDI file (e.g., "Some Great Song.mid")

    private Dictionary<int, int> noteToLane = new Dictionary<int, int>
    {
        { 36, 0 },
        { 37, 1 },
        { 38, 2 },
        { 39, 3 },
        { 40, 4 }
    };

    public List<NoteData> noteDataList = new List<NoteData>();

    void Awake()
    {
        instance = this;
        LoadMidiFile();
    }

    void LoadMidiFile()
    {
        try
        {
            // Construct the full file path
            string filePath = Path.Combine(Application.streamingAssetsPath, midiFileName);

            // Check if the file exists
            if (!File.Exists(filePath))
            {
                Debug.LogError("MIDI file not found at path: " + filePath);
                return;
            }

            // Read the MIDI file from the file path
            var midiFile = MidiFile.Read(filePath);

            var tempoMap = midiFile.GetTempoMap();
            var notes = midiFile.GetNotes();

            foreach (var note in notes)
            {
                if (noteToLane.TryGetValue(note.NoteNumber, out int lane))
                {
                    var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, tempoMap);
                    double time = metricTimeSpan.TotalSeconds;

                    noteDataList.Add(new NoteData { time = time, lane = lane });
                }
            }

            // Sort notes by time
            noteDataList.Sort((a, b) => a.time.CompareTo(b.time));
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading MIDI file: " + ex.Message);
        }
    }

    public class NoteData
    {
        public double time;
        public int lane;
    }
}