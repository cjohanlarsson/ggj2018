﻿using System;
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
	public Dictionary<Vector2Int,GridObject> gridObjectsByPos = new Dictionary<Vector2Int, GridObject>();
	public List<Emitter> emitters;
	public List<Output> outputs;

	public Note notePrefab;
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
			if( obj is Output ) {
				outputs.Add( (Output)obj );
			}
			
		}

		foreach( var note in notes ) {
			note.Init( this );
		}

		beatTimer = AudioSettings.dspTime * sampleRate;
		frequency = 60.0 / bpm;
		Debug.Log( frequency );
	}

	void Update () {
		if( requiresUpdate ) {
			requiresUpdate = false;

			var dirtyNotes = new List<Note>();
			foreach( var note in notes ) {
				var moved = note.Move();
				if( moved ) {
					dirtyNotes.Add( note );
				}
			}
			
			foreach( var note in dirtyNotes ) {
				var pos = note.gridPos;
				GridObject obj;
				if( gridObjectsByPos.TryGetValue( pos, out obj ) ) {
					obj.OnNoteEnter( note );
				}
			}

			foreach(var e in emitters) {
				if(e.CheckReady()) {
					CreateNote(e.transform.position);
				}
			}
		}
	}

	public bool UpdateGridObject( GridObject go, Vector2Int pos ) {
		
		if(!gridObjectsByPos.ContainsKey(pos))
		{
			gridObjectsByPos.Remove(go.gridPos);
			gridObjectsByPos.Add(pos, go);
			return true;
		}
		else
		{
			return false;
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

	public static Vector2Int GetDirectionVector ( MoveDirection direction ) {
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

    public Note CloneNote( Note note ) {
		var clone = Instantiate( note );
		notes.Add( clone );
		return clone;
    }

    void CreateNote(Vector3 pos) {
		var note = GameObject.Instantiate(notePrefab, pos, Quaternion.identity);
		note.Init(this);
		notes.Add(note);
    }
}