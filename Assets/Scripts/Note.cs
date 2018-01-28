using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {

    [SerializeField]
    LineRenderer line;
    [HideInInspector]
    Grid grid;
    [SerializeField, HideInInspector]
    Vector2Int _gridPos;
    public Vector2Int gridPos {
        get {
            return _gridPos;
        }
        set {
            positionHistory.Add( _gridPos );
            _gridPos = value;
            transform.position = new Vector3( _gridPos.x, _gridPos.y );
        }
    }
    List<Vector2Int> positionHistory = new List<Vector2Int>();

    [Space]
	public float duration = 1; // in beats
	public MoveDirection direction;
	public int pitch;

    public int beatWait;

    [NonSerialized]
    public bool updated;

    Coroutine moveCo;

    public void Init ( Grid grid ) {
        this.grid = grid;
		gridPos = new Vector2Int( (int)transform.position.x, (int)transform.position.y );

        line.positionCount = 2;
        line.SetPosition( 0, transform.position );
        line.SetPosition( 1, transform.position );
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

    public void UpdateAnimations () {
        if( moveCo != null ) StopCoroutine( moveCo );
        moveCo = StartCoroutine( MoveCo() );
    }

    private IEnumerator MoveCo() {
        var projectedNextPos = ( gridPos + Grid.GetDirectionVector( direction ) ).ToVector3();
        var moveSpeed = (float)grid.frequency;
        var time = 0f;
        var endStartPos = positionHistory[ positionHistory.Count - 1 ].ToVector3();
        var historyDuration = duration;
        var lineHistory = new List<Vector3>();
        var positionIndex = positionHistory.Count - 1;
        Vector3 startFrom = Vector3.zero;
        Vector3 endFrom = gridPos.ToVector3();
        Vector3 endTo = projectedNextPos;

        while( historyDuration > 0 && positionIndex > 0 ) {
            if( historyDuration > 1 ) {
                lineHistory.Add( positionHistory[ positionIndex ].ToVector3() );
            } else {
                startFrom = positionHistory[ positionIndex ].ToVector3();
            }
            positionIndex -= 1;
            historyDuration -= 1;
        }
        lineHistory.Add( startFrom );
        lineHistory.Reverse();
        lineHistory.Add( endFrom );
        lineHistory.Add( endTo );

        line.positionCount = lineHistory.Count;
        
        while( time < moveSpeed ) {
            time += Time.deltaTime;
            var t = time / moveSpeed;
            
            lineHistory[ 0 ] = Vector3.Lerp( startFrom, lineHistory[ 1 ], t );
            lineHistory[ lineHistory.Count - 1 ] = Vector3.Lerp( lineHistory[ lineHistory.Count - 2 ], endTo, t );

            foreach( var l in lineHistory ) {
                DebugExtension.DebugPoint( l, Color.red );
            }

            line.SetPositions( lineHistory.ToArray() );

            yield return null;
        }
    }

    public void OnDestroy() {
		Destroy( gameObject );
    }

    void OnDrawGizmos () {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere( transform.position, 0.25f );
    }
}
