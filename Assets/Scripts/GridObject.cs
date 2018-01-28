using UnityEngine;
public abstract class GridObject : MonoBehaviour {
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
	}

	public abstract void OnNoteEnter ( Note note );
}
