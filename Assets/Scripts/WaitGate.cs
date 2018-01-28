using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class WaitGate : GridObject {
	public SpriteRenderer center;
    public int waitTime; // in beats

	bool active;

	public Color emptyColor;

	public AnimationCurve fillCurve;
	public AnimationCurve colorCurve;

	struct WaitTimer {
		public int time;
		public Note note;
	}

	List<WaitTimer> timers = new List<WaitTimer>();

	Coroutine fillCo;

	public override void Init ( Grid grid ) {
		base.Init( grid );
		center.color = emptyColor;
	}

	public override void OnNoteEnter( Note note ) {
		if( timers.Count == 0 ) {
			var waitTimer = new WaitTimer();
			waitTimer.time = this.grid.BeatsTicked + Mathf.Max( Mathf.CeilToInt( note.duration ), waitTime );
			var clone = grid.CloneNote( note );
			clone.alive = false;
			clone.Init( grid, false );
			waitTimer.note = clone;
			grid.DestroyNote( note );
			timers.Add( waitTimer );

			Pulse();
			if( fillCo != null ) StopCoroutine( fillCo );
			fillCo = StartCoroutine( FillAnim( note.duration * (float)grid.frequency - 0.04f ) );
		}
	}

	public override void Tick () {
		base.Tick();
		for( int i = 0; i < timers.Count; i++ ) {
			var timer = timers[ i ];
			if( grid.BeatsTicked >= timer.time ) {
				timer.note.alive = true;
				timer.note.UpdateAnimations();
				timers.RemoveAt( i-- );

				Pulse( 0.75f );
				if( fillCo != null ) StopCoroutine( fillCo );
				fillCo = StartCoroutine( DrainAnim( timer.note.duration * (float)grid.frequency, Visuals.Singleton.ConvertNoteColorToColor( timer.note.color ) ) );
			}
		}
	}

	private IEnumerator FillAnim ( float duration ) {
		var time = 0f;

		center.color = Visuals.Singleton.ConvertNoteColorToColor( timers[ 0 ].note.color );

		while( time < duration ) {
			time += Time.deltaTime;

			var p = time / duration;

			var scale = fillCurve.Evaluate( p );

			center.transform.localScale = Vector3.one * scale;

			yield return null;
		}

		center.transform.localScale = Vector3.one * fillCurve.Evaluate( 1 );
	}

	private IEnumerator DrainAnim ( float duration, Color fillColor ) {
		var time = 0f;

		while( time < duration ) {
			time += Time.deltaTime;

			var p = time / duration;

			var waver = fillCurve.Evaluate( 1 - p );

			var scale = waver;

			center.transform.localScale = Vector3.one * scale;

			yield return null;
		}

		center.transform.localScale = Vector3.one * fillCurve.Evaluate( 0 );
	}
}