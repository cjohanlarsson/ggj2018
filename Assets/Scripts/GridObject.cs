using UnityEngine;
public abstract class GridObject : MonoBehaviour {

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
}
