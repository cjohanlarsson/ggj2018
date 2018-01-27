using UnityEngine;
public class Note : MonoBehaviour {
    Grid grid;
    [SerializeField]
    Vector2Int _gridPos;
    public Vector2Int gridPos {
        get {
            return _gridPos;
        }
        set {
            _gridPos = value;
            transform.position = new Vector3( _gridPos.x, _gridPos.y );
        }
    }

	public float duration = 1; // in beats
	public MoveDirection direction;
	public int pitch;

    public int beatWait;

    public void Init ( Grid grid ) {
        this.grid = grid;
		gridPos = new Vector2Int( (int)transform.position.x, (int)transform.position.y );
    }

    public void Move () {
        if( beatWait > 0 ) {
            beatWait -= 1;
        } else {
            gridPos += Grid.GetDirectionVector( direction );
        }
    }

    public void OnDestroy() {
		Destroy( gameObject );
    }
}
