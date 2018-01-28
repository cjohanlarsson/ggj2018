using System;
using System.Collections;
using UnityEngine;
public abstract class GridObject : MonoBehaviour {
	public bool pinned;
	public GameObject graphics;

	public NoteColor color;
	public SpriteRenderer[] renderersToSwapForColor;

	[HideInInspector]
	public Grid grid;
	private Vector2Int _gridPos;
	public Vector2Int gridPos
	{
		get
		{
			return _gridPos;
		}
	}

	Coroutine pulseCo;

	SpriteRenderer spr;

	public bool SetGridPos(Vector2Int newPos) {
		if(this.grid.UpdateGridObject(this, newPos)) {
			_gridPos = newPos;
			return true;
		} else {
			return false;
		}
	}

	public virtual void Init ( Grid grid ) {
		this.grid = grid;
		SetGridPos( new Vector2Int( (int)transform.position.x, (int)transform.position.y ) );

		if(renderersToSwapForColor != null && color != NoteColor.None) {
			foreach(var r in renderersToSwapForColor) {
				r.color = Visuals.Singleton.ConvertNoteColorToColor( color );
			}
		}

		if( !pinned ) {
			spr = Instantiate( Resources.Load<SpriteRenderer>( "pinned_sprite" ) );
			spr.transform.position = transform.position;
			spr.transform.SetParent( transform );
		}
	}

    public virtual void Rotate ( bool clockwise = true ) {
		
	}

	public abstract void OnNoteEnter ( Note note );

	public virtual void OnStart () {
		if( spr != null ) {
			spr.color = Grid.Singleton.pinnedOffColor;
		}
	}

	public virtual void OnStop () {
		if( spr != null ) {
			spr.color = Grid.Singleton.pinnedOnColor;
		}
	}

	public virtual void Tick () {

	}

	public void Waver ( float duration ) {
		if( pulseCo != null ) StopCoroutine( pulseCo );
		pulseCo = StartCoroutine( WaverAnim( duration ) );
	}

	public void Pulse ( float magnitude = 1, Note note = null ) {
		if( pulseCo != null ) StopCoroutine( pulseCo );
		pulseCo = StartCoroutine( PulseAnim( magnitude, note ) );
	}

	private IEnumerator PulseAnim ( float magnitude, Note note ) {
		var duration = (float)grid.frequency * 0.75f;

		if( note != null ) {
			var hitParticles = Grid.MakeNoteCollideParticles2( color );
			hitParticles.transform.position = transform.position;
		}

		var time = 0f;

		while( time < duration ) {
			time += Time.deltaTime;

			var p = time / duration;

			var scale = grid.pulseCurve.Evaluate( p );

			graphics.transform.localScale = Vector3.one + Vector3.one * scale * 0.5f * magnitude;

			yield return null;
		}

		graphics.transform.localScale = Vector3.one;
	}

	private IEnumerator WaverAnim ( float duration ) {
		var time = 0f;

		while( time < duration ) {
			time += Time.deltaTime;

			var scale = Mathf.Sin( time * 40 );

			var p = time / duration;

			var waver = grid.waverCurve.Evaluate( p );

			scale = scale * 0.2f * waver;

			graphics.transform.localScale = Vector3.one + Vector3.one * scale;

			yield return null;
		}

		graphics.transform.localScale = Vector3.one;
	}

	private void OnDrawGizmos () {
		if( pinned ) {
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube( transform.position, Vector3.one * 0.6f );
		}
	}
}
