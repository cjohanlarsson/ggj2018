using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteGrid : MonoBehaviour {

	public Camera camera;
	public MeshRenderer renderer;

	MaterialPropertyBlock matProps;

	static readonly int _MainTex_ST = Shader.PropertyToID( "_MainTex_ST" );

	// Use this for initialization
	void Start () {
		camera = Camera.main;
		matProps = new MaterialPropertyBlock();
	}

	void LateUpdate () {
		var camHeight = camera.orthographicSize * 2;
		var camWidth  = camHeight * camera.aspect;
		transform.localScale = new Vector3( camWidth, camHeight );

		var camPosition = new Vector3( camera.transform.position.x, camera.transform.position.y );
		transform.position = camPosition;



		var st = new Vector4( camWidth, camHeight, camPosition.x - camWidth * 0.5f, camPosition.y - camHeight * 0.5f );
		matProps.SetVector( _MainTex_ST, st );
		renderer.SetPropertyBlock( matProps );
	}
}
