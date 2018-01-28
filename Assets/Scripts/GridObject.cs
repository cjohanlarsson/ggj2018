using UnityEngine;
public abstract class GridObject : MonoBehaviour {
	[HideInInspector]
	public Grid grid;
	[HideInInspector]
	public Vector2Int gridPos;

	public virtual void Init ( Grid grid ) {
		this.grid = grid;
		gridPos = new Vector2Int( (int)transform.position.x, (int)transform.position.y );
	}

	public abstract void OnNoteEnter ( Note note );
}
