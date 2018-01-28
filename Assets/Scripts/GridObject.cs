using System;
using System.Collections;
using UnityEngine;
public abstract class GridObject : MonoBehaviour {
	public GameObject graphics;

	public NoteColor color;
	public Renderer[] renderersToSwapForColor;

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
				r.sharedMaterial = Visuals.Singleton.ConvertNoteColorToMaterial( color );
			}
		}
	}

    public virtual void Rotate ( bool clockwise = true ) {
		
	}

	public abstract void OnNoteEnter ( Note note );

	public virtual void OnStop () {
		
	}

	public virtual void Tick () {

	}

	public void Waver ( float duration ) {
		if( pulseCo != null ) StopCoroutine( pulseCo );
		pulseCo = StartCoroutine( WaverAnim( duration ) );
	}

	public void Pulse ( float magnitude = 1 ) {
		if( pulseCo != null ) StopCoroutine( pulseCo );
		pulseCo = StartCoroutine( PulseAnim( magnitude ) );
	}

	private IEnumerator PulseAnim ( float magnitude ) {
		var duration = (float)grid.frequency * 0.75f;

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

			scale = scale * 0.15f * waver;

			graphics.transform.localScale = Vector3.one + Vector3.one * scale;

			yield return null;
		}

		graphics.transform.localScale = Vector3.one;
	}
}
