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

	public AnimationCurve pulseCurve;
	public AnimationCurve waverCurve;

    public double beatTimer;

    public float currentBeat;

    public int bpm;
	public int maxNoteBeatLifetime = 1000;
	public List<OutputGoal> goals;

    int sampleRate;
	public double frequency;

	public List<Note> notes;
	public List<GridObject> gridObjects;
	public Dictionary<Vector2Int,GridObject> gridObjectsByPos = new Dictionary<Vector2Int, GridObject>();
	public List<Emitter> emitters;
	public List<Output> outputs;

	public Note notePrefab;
	bool requiresUpdate;

	public bool isPlaying { get; private set; }

	public int BeatsTicked { get; private set; }

	void Awake () {
		isPlaying = true;
		Singleton = this;
		sampleRate = AudioSettings.outputSampleRate;

		// beatTimer = AudioSettings.dspTime * sampleRate;
		frequency = 60.0 / bpm;
		beatTimer = Time.time;

		
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
		// Stop();
	}

	void OnDestroy() {
		Singleton = null;
	}

	void Update () {
		if( !isPlaying ) return;

		if( Time.time > beatTimer ) {
			beatTimer += frequency;
			requiresUpdate = false;

			var dirtyNotes = new List<Note>();
			foreach( var note in notes ) {
				if( !note.alive ) continue;
				var moved = note.Move();
				if( moved ) {
					dirtyNotes.Add( note );
				}
			}
			
			foreach( var note in dirtyNotes ) {
				var pos = note.gridPos;
				GridObject obj;
				var inBounds = pos.x < 30 && pos.x > -30 && pos.y > -30 && pos.y < 30;
				if( note.BeatLifetime > maxNoteBeatLifetime || !inBounds ) {
					DestroyNote( note );
				} else {
					if( gridObjectsByPos.TryGetValue( pos, out obj ) ) {
						if(obj.color == NoteColor.None || obj.color == note.color)
							obj.OnNoteEnter( note );
					}
				}

				if( note.alive ) note.UpdateAnimations();
			}

			foreach( var obj in gridObjects ) {
				obj.Tick();
			}

			foreach(var e in emitters) {
				if(e.CheckReady()) {
					CreateNote(e);
				}
			}

			// for(int i=0;i<this.notes.Count;i++) {
			// 	var note = notes[ i ];
			// 	if(note.BeatLifetime > maxNoteBeatLifetime) {
			// 		note.DestroyNote();
			// 		this.notes.RemoveAt(i--);
			// 	}
			// }

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

    public void RemoveObject ( GridObject obj ) {
		gridObjectsByPos.Remove( obj.gridPos );
		gridObjects.Remove( obj );
    }

    public void AddObject ( GridObject obj ) {
		gridObjectsByPos.Add( obj.gridPos, obj );
		gridObjects.Add( obj );
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
		clone.name = clone.name.Replace( "(Clone)", "" );
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

    public static MoveDirection RotateDirection( MoveDirection direction, bool clockwise = true ) {
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

    public static MoveDirection GetOppositeDirection( MoveDirection direction ) {
		switch( direction ) {
			case MoveDirection.Up:
				return MoveDirection.Down;
			case MoveDirection.Right:
				return MoveDirection.Left;
			case MoveDirection.Down:
				return MoveDirection.Up;
			case MoveDirection.Left:
				return MoveDirection.Right;
		}

		return MoveDirection.Up;
    }

	public static Vector3 GetParticleOffset ( MoveDirection direction, float scale ) {
		return Grid.GetDirectionVector( Grid.GetOppositeDirection( direction ) ).ToVector3() * scale;
	}

	public static Quaternion GetDirectionRotation ( MoveDirection direction ) {
        Vector3 d1 = Grid.GetDirectionVector(direction).ToVector3();
        var angle = Vector3.Angle(Vector3.up, d1);

        if (direction == MoveDirection.Left)
        {
            angle = -angle;
        }

        return Quaternion.AngleAxis(angle, Vector3.back);
	}

    void CreateNote(Emitter e) {
		var note = GameObject.Instantiate(notePrefab, e.transform.position, Quaternion.identity);
		note.direction = e.emitDirection;
		note.duration = e.duration;
		note.color = e.color;
		note.Init(this);

		notes.Add(note);
    }

    public void SetPlaying( bool playing ) {
		isPlaying = playing;

		if( !isPlaying ) {
			Stop();
		} else {
			beatTimer = Time.time;
		}
    }

	void Stop () {
		beatTimer = 0;
		BeatsTicked = 0;
		currentGoalIndex = 0;
		currentBeatTowardsGoal = 0;

		foreach( var note in notes ) {
			note.DestroyForStop();
		}

		foreach( var obj in gridObjects ) {
			obj.OnStop();
		}

		notes.Clear();
	}

    public static ParticleDestruct MakeNoteCollideParticles ( Note note ) {
		var particles = Instantiate( Resources.Load<ParticleDestruct>( "gfx/line_collide_particles" ) );
		particles.SetColor( note.color );
        float freq = (float)Grid.Singleton.frequency;
        particles.countdown = note.duration * freq - 0.04f;
        return particles;
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