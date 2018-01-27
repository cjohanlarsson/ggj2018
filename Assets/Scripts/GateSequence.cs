using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

[RequireComponent(typeof(AudioSource))]
public class GateSequence : MonoBehaviour 
{
	[Header("Sequence slots")]
	public int startingMidiIndex;
	public Gate[] gates;
	public int bpm = 120;

	[Header("MIDI Info")]
	public string bankFilePath = "GM Bank/gm";
	public int bufferSize = 1024;
	public int midiNote = 60;
	public int midiNoteVolume = 100;
	[Range(0, 127)] //From Piano to Gunshot
	public int midiInstrument = 0;

	private float[] sampleBuffer;
	private float gain = 1f;
	private StreamSynthesizer midiStreamSynthesizer;

	private int gateIndex;
	private float timeTillNextBeat;
	private int currentMidiIndex;

	void Awake()
	{
		midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
		sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

		midiStreamSynthesizer.LoadBank(bankFilePath);

		Reset();
	}

	void Reset()
	{
		gateIndex = 0;
		currentMidiIndex = startingMidiIndex;
		timeTillNextBeat = 2f;
	}

	void Update()
	{
		if(gateIndex >= gates.Length)
			Reset();

		timeTillNextBeat -= Time.deltaTime;
		if(timeTillNextBeat <= 0)
		{
			Debug.Log( "PLAY GATE: " + gateIndex.ToString() );
			timeTillNextBeat += 1 / (bpm / 60f);

			bool isSet = gates[gateIndex].IsSet;
			currentMidiIndex += isSet ? gates[gateIndex].GetDiff() : 0;
			StartCoroutine( PlayGate(gates[gateIndex], isSet, currentMidiIndex, 0.25f) );
			gateIndex++;
		}
	}

	IEnumerator PlayGate(Gate g, bool isSet, int i, float secs)
	{
		g.transform.localScale = Vector3.one * 1.25f;
		if(isSet)
			midiStreamSynthesizer.NoteOn(0, midiNote + i, midiNoteVolume, midiInstrument);
		yield return new WaitForSeconds(secs);
		g.transform.localScale = Vector3.one;
		if(isSet)
		midiStreamSynthesizer.NoteOff(0, midiNote + i);
	}

	private void OnAudioFilterRead(float[] data, int channels)
	{
		//This uses the Unity specific float method we added to get the buffer
		midiStreamSynthesizer.GetNext(sampleBuffer);

		for (int i = 0; i < data.Length; i++)
		{
		    data[i] = sampleBuffer[i] * gain;
		}
	}
}
