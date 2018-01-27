using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveDirection {
	Up,
	Right,
	Down,
	Left
}

[RequireComponent( typeof( AudioSource ))]
public class Grid : MonoBehaviour {
	public double beatTimer;
	public float currentBeat;
	public int bpm;
	
	int sampleRate;
	double frequency;

	public List<Note> notes;
	public List<GridObject> gridObjects;
	public List<Emitter> emitters;

	bool requiresUpdate;

	void Awake () {
		sampleRate = AudioSettings.outputSampleRate;
		var allGridObjects = FindObjectsOfType<GridObject>();
		var allNotes = FindObjectsOfType<Note>();
		gridObjects = new List<GridObject>( allGridObjects );
		notes = new List<Note>( allNotes );

		foreach( var obj in gridObjects ) {
			obj.Init( this );
			if( obj is Emitter ) {
				emitters.Add( (Emitter)obj );
			}
		}

		foreach( var note in notes ) {
			note.Init( this );
		}

		frequency = bpm / 60.0;
		Debug.Log( frequency );
	}

	void Update () {
		if( requiresUpdate ) {
			requiresUpdate = false;
			
			foreach( var note in notes ) {
				var dir = GetDirectionVector( note.direction );
				note.gridPos += dir;
			}
		}
	}

    public void DestroyNote ( Note note ) {
		notes.Remove( note );
		note.OnDestroy();
    }
	
	void OnAudioFilterRead ( float[] data, int channels ) {
		var sample = AudioSettings.dspTime * sampleRate;
		if( sample > beatTimer ) {
			beatTimer = sample + ( frequency * sampleRate );
			requiresUpdate = true;
		}
    }

	Vector2Int GetDirectionVector ( MoveDirection direction ) {
		switch( direction ) {
			case MoveDirection.Up:
				return new Vector2Int( 0, 1 );
			case MoveDirection.Right:
				return new Vector2Int( 1, 0 );
			case MoveDirection.Down:
				return new Vector2Int( 0, -1 );
			case MoveDirection.Left:
				return new Vector2Int( -1, 0 );
		}
		return Vector2Int.zero;
	}
}