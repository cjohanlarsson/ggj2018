using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public struct LevelCameraInfo {
	public float size;
	public Vector3 position;
}

public struct CameraZone {
	public Rect bounds;

	public void SetBounds ( Vector2 size, Vector3 center ) {
		bounds.size = size;
		bounds.center = center;
	}

	public Vector2 CalculateTargetSize ( Camera camera ) {
		var size = new Vector2();

		size.y = bounds.size.y;
		size.x = size.y * camera.aspect;

		if( size.y == 0 ) size.y = 0.0001f;
		if( size.x == 0 ) size.x = 0.0001f;

		var zoneSize = bounds.size;

		if( size.x < zoneSize.x ) {
			size.y *= zoneSize.x / size.x;
			size.x = zoneSize.x;
		}

		if( size.y < zoneSize.y ) {
			size.x *= zoneSize.y / size.y;
			size.y = zoneSize.y;
		}

		return size;
	}
}

public class GameCamera : MonoBehaviour {

	public Camera camera;

	public CameraZone zone;

	[Space]
	public AnimationCurve zoomInCurve;
	public AnimationCurve zoomInMovementCurve;

	[Space]
	public AnimationCurve zoomOutCurve;
	public AnimationCurve zoomOutRotationCurve;
	public AnimationCurve zoomOutMovementCurve;

	[Space]
	public AnimationCurve zoomTunnelRotationCurve;
	public AnimationCurve zoomTunnelRotationCurve2;

	public delegate void GameCameraEvent ( GameCamera camera );

	public event GameCameraEvent onCompleteZoom;

	public Rect GetCurrentBounds () {
		var bounds = new Rect();

		var height = camera.orthographicSize * 2;
		var width = height * camera.aspect;
		bounds.size = new Vector2( width, height );

		bounds.center = transform.position;

		return bounds;
	}

	public bool dragging;
	Vector3 lastPos;
	private Vector3 zoomVel;

	public void CenterOnZone () {
		if( zone.bounds.size == Vector2.zero ) {
			zone.bounds.size = new Vector2( 5, 5 );
			zone.bounds.center = Vector2.zero;
		}

		var targetSize = zone.CalculateTargetSize( camera );

		camera.orthographicSize = targetSize.y * 0.5f;
		var z = camera.transform.position.z;
		var pos = (Vector3)zone.bounds.center;
		pos.z = z;
		camera.transform.position = pos;
	}

	public void LateUpdate () {
		if( Input.GetMouseButtonDown( 2 ) ) {
			dragging = true;
			lastPos = Input.mousePosition;
		}

		if( Input.GetMouseButtonUp( 2 ) ) {
			dragging = false;
		}

		if( dragging ) {
			var delta = lastPos - Input.mousePosition;

			camera.transform.position += delta * 0.01f;

			lastPos = Input.mousePosition;
		}

		if( Input.mouseScrollDelta.y != 0 ) {
			var screenBounds = new Rect( 0, 0, Screen.width, Screen.height );

			if( screenBounds.Contains( Input.mousePosition ) ) {
				camera.orthographicSize -= Input.mouseScrollDelta.y * 0.2f;
			}
		}

		if( Input.GetKeyDown( KeyCode.Space ) ) {
			CenterOnZone();
		}
	}
}
