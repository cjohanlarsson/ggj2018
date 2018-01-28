using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour {

    [SerializeField]
    LineRenderer line;
    [HideInInspector]
    public Grid grid;
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

    List<Vector3> lineHistory = new List<Vector3>();

    [Space]
	public float duration = 1; // in beats
	public MoveDirection direction;
	public int pitch;

    public int beatWait;
    public int BeatLifetime { get; private set; }

    [NonSerialized]
    public bool updated;

    Coroutine moveCo;

    public void Init ( Grid grid, bool updateGridPos = true ) {
        this.grid = grid;
		if( updateGridPos ) _gridPos = new Vector2Int( (int)transform.position.x, (int)transform.position.y );

        UpdateAnimations();
    }

    public bool Move () {
		BeatLifetime++;
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
        var historyDuration = duration;
        lineHistory.Clear();
        var positionIndex = positionHistory.Count - 1;
        
        Vector3 endFrom = gridPos.ToVector3();
        Vector3 endTo = projectedNextPos;

        while( historyDuration > 0 && positionIndex >= 0 ) {
            lineHistory.Add( positionHistory[ positionIndex ].ToVector3() );
            positionIndex -= 1;
            historyDuration -= 1;
        }

        lineHistory.Reverse();
        Vector3 startFrom;
        Vector3 startTo;
        if( lineHistory.Count > 0 ) {
            if( lineHistory.Count > 1 ) {
                startFrom = lineHistory[ 0 ];
                startTo   = lineHistory[ 1 ];
            } else {
                startFrom = lineHistory[ 0 ];
                startTo   = endFrom;
            }
        } else {
            startFrom = endFrom;
            startTo   = endFrom;
        }

        lineHistory.Add( endFrom );
        lineHistory.Add( endTo );

        line.positionCount = lineHistory.Count;

        while( time < moveSpeed ) {
            time += Time.deltaTime;
            var t = time / moveSpeed;

            lineHistory[ 0 ] = Vector3.Lerp( startFrom, startTo, t );
            lineHistory[ lineHistory.Count - 1 ] = Vector3.Lerp( endFrom, endTo, t );

            line.SetPositions( lineHistory.ToArray() );

            yield return null;
        }
    }

    public void DestroyNote () {
		// Destroy( gameObject );
        Debug.Log( moveCo );
        if( moveCo != null ) StopCoroutine( moveCo );
        StartCoroutine( GracefulDestroy() );
    }

    IEnumerator GracefulDestroy () {
		GameObject.Destroy (this.gameObject);

        while( lineHistory.Count > 0 ) {
            foreach( var pos in lineHistory ) {
                DebugExtension.DebugPoint( pos );
            }

            yield return null;
        }
    }

    void OnDrawGizmos () {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere( transform.position, 0.25f );
    }
}
