using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpSynth.Effects;
using CSharpSynth.Sequencer;
using CSharpSynth.Synthesis;
using CSharpSynth.Midi;

public class MusicPlayer : MonoBehaviour 
{
	public static MusicPlayer Singleton;


	//Try also: "FM Bank/fm" or "Analog Bank/analog" for some different sounds
    public string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    public int midiNote = 60;
    public int midiNoteVolume = 100;
    [Range(0, 127)] //From Piano to Gunshot
    public int midiInstrument = 0;
    //Private 
    private float[] sampleBuffer;
    private float gain = 1f;
    private StreamSynthesizer midiStreamSynthesizer;

    // Awake is called when the script instance
    // is being loaded.
    void Awake()
    {
		Singleton = this;
        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];
        
        midiStreamSynthesizer.LoadBank(bankFilePath);
    }


    // Start is called just before any of the
    // Update methods is called the first time.
    void Start()
    {
    }

	void OnDestroy() {
		Singleton = null;
	}

    // Update is called every frame, if the
    // MonoBehaviour is enabled.
	KeyCode[] keys = new KeyCode[] { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y };
    void Update()
    {
    	for(int i=0;i<keys.Length;i++)
    	{
	        if (Input.GetKeyDown(keys[i]))
	        	midiStreamSynthesizer.NoteOn(0, midiNote + i, midiNoteVolume, midiInstrument);
	        

			if (Input.GetKeyUp(keys[i]))
	        	midiStreamSynthesizer.NoteOff(0, midiNote + i);
        }


    }

    public void PlayNote(int pitch, float duration)
    {
    	StartCoroutine( PlayNoteCoro(pitch, duration) );
    }

	IEnumerator PlayNoteCoro(int i, float secs)
	{
		midiStreamSynthesizer.NoteOn(0, midiNote + i, midiNoteVolume, midiInstrument);
		yield return new WaitForSeconds(secs);
		midiStreamSynthesizer.NoteOff(0, midiNote + i);
	}

    // See http://unity3d.com/support/documentation/ScriptReference/MonoBehaviour.OnAudioFilterRead.html for reference code
    //	If OnAudioFilterRead is implemented, Unity will insert a custom filter into the audio DSP chain.
    //
    //	The filter is inserted in the same order as the MonoBehaviour script is shown in the inspector. 	
    //	OnAudioFilterRead is called everytime a chunk of audio is routed thru the filter (this happens frequently, every ~20ms depending on the samplerate and platform). 
    //	The audio data is an array of floats ranging from [-1.0f;1.0f] and contains audio from the previous filter in the chain or the AudioClip on the AudioSource. 
    //	If this is the first filter in the chain and a clip isn't attached to the audio source this filter will be 'played'. 
    //	That way you can use the filter as the audio clip, procedurally generating audio.
    //
    //	If OnAudioFilterRead is implemented a VU meter will show up in the inspector showing the outgoing samples level. 
    //	The process time of the filter is also measured and the spent milliseconds will show up next to the VU Meter 
    //	(it turns red if the filter is taking up too much time, so the mixer will starv audio data). 
    //	Also note, that OnAudioFilterRead is called on a different thread from the main thread (namely the audio thread) 
    //	so calling into many Unity functions from this function is not allowed ( a warning will show up ). 	
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
