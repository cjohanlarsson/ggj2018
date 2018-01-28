using System;
using UnityEngine;

public class Note : MonoBehaviour {
    [HideInInspector]
    Grid grid;
    [SerializeField, HideInInspector]
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

    [NonSerialized]
    public bool updated;

    public void Init ( Grid grid ) {
        this.grid = grid;
		gridPos = new Vector2Int( (int)transform.position.x, (int)transform.position.y );
    }

    public bool Move () {
        if( beatWait > 0 ) {
            beatWait -= 1;
            return false;
        } else {
            gridPos += Grid.GetDirectionVector( direction );
            return true;
        }
    }

    public void OnDestroy() {
		Destroy( gameObject );
    }
}
