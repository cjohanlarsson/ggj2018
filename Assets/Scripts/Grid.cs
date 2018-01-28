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

[System.Serializable]
public class OutputGoal
{
	public int pitch;
	public int beatIndex;

	public bool Complete { get; set; }

	public bool Matches(Note n)
	{
		return n.pitch == pitch;
	}

}

[RequireComponent( typeof( AudioSource ))]
public class Grid : MonoBehaviour {
	public static Grid Singleton;

    public double beatTimer;
	public float currentBeat;
	public int bpm;
	public int maxNoteBeatLifetime = 1000;
	[SerializeField] private List<OutputGoal> goals;

	int sampleRate;
	public double frequency;

	public List<Note> notes;
	public List<GridObject> gridObjects;
	public Dictionary<Vector2Int,GridObject> gridObjectsByPos = new Dictionary<Vector2Int, GridObject>();
	public List<Emitter> emitters;
	public List<Output> outputs;

	public Note notePrefab;
	bool requiresUpdate;

	public int BeatsTicked { get; private set; }

	void Awake () {
		Singleton = this;
		sampleRate = AudioSettings.outputSampleRate;

		// beatTimer = AudioSettings.dspTime * sampleRate;
		frequency = 60.0 / bpm;
		beatTimer = Time.time + frequency;

		
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
	}

	void OnDestroy() {
		Singleton = null;
	}

	void Update () {
		if( Time.time > beatTimer ) {
			beatTimer += frequency;
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
					if(obj.color == NoteColor.None || obj.color == note.color)
						obj.OnNoteEnter( note );
				}

				if( note.alive ) note.UpdateAnimations();
			}

			foreach(var e in emitters) {
				if(e.CheckReady()) {
					CreateNote(e);
				}
			}

			for(int i=0;i<this.notes.Count;i++) {
				if(this.notes[i].BeatLifetime > maxNoteBeatLifetime) {
					this.notes[i].DestroyNote();
					this.notes.RemoveAt(i--);
				}
			}

			if(goals.Count > 0 && goals[0].Complete)
				currentBeatTowardsGoal++;
			BeatsTicked++;
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
		note.DestroyNote();
    }

	void OnAudioFilterRead ( float[] data, int channels ) {
		// var sample = AudioSettings.dspTime * sampleRate;
		// if( sample > beatTimer ) {
		// 	beatTimer = sample + ( frequency * sampleRate );
		// 	requiresUpdate = true;
		// }
    }

    public Note CloneNote( Note note ) {
		var clone = Instantiate( note );
		clone.grid = this;
		notes.Add( clone );
		return clone;
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

    internal static MoveDirection RotateDirection( MoveDirection direction, bool clockwise = true ) {
		if( clockwise ) {
			switch( direction ) {
				case MoveDirection.Up:
					return MoveDirection.Right;
				case MoveDirection.Right:
					return MoveDirection.Down;
				case MoveDirection.Down:
					return MoveDirection.Left;
				case MoveDirection.Left:
					return MoveDirection.Up;
			}
		} else {
			switch( direction ) {
				case MoveDirection.Up:
					return MoveDirection.Left;
				case MoveDirection.Right:
					return MoveDirection.Up;
				case MoveDirection.Down:
					return MoveDirection.Right;
				case MoveDirection.Left:
					return MoveDirection.Down;
			}
		}

		return MoveDirection.Up;
    }

    void CreateNote(Emitter e) {
		var note = GameObject.Instantiate(notePrefab, e.transform.position, Quaternion.identity);
		note.duration = e.duration;
		note.color = e.color;
		note.Init(this);

		notes.Add(note);
    }

    #region Goals
    private int currentGoalIndex = 0;
    private int currentBeatTowardsGoal = 0;
    public void MarkGoal(Note note)
    {
		if(currentGoalIndex >= goals.Count)
			return;
		
		if(goals[currentGoalIndex].Matches( note ) && goals[currentGoalIndex].beatIndex == currentBeatTowardsGoal)
		{
			goals[currentGoalIndex++].Complete = true;
		}
		else
		{
			Debug.Log( "Fail: " + note.pitch.ToString() );
			currentGoalIndex = 0;
			foreach(var g in goals)
				g.Complete = false;
		}
    }

    public bool IsGoalComplete
    {
    	get
    	{
    		return goals.TrueForAll( (o) => o.Complete );
    	}
    }
    #endregion

}

static class Vector2IntExtensions {
	public static Vector3 ToVector3 ( this Vector2Int vec ) {
		return new Vector3( vec.x, vec.y );
	}
}