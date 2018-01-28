﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDrop : MonoBehaviour 
{
	GameObject dragObject;

	// Update is called once per frame
	void Update () {
		if(dragObject == null)
		{
			if(Input.GetMouseButtonDown(0))
			{
				var ray = Camera.main.ScreenPointToRay( Input.mousePosition );
				RaycastHit hitInfo;
				if( Physics.Raycast( ray, out hitInfo, 1000f) )
				{
					dragObject = hitInfo.collider.gameObject;
				}
			}
		}
		else
		{
			bool isUp = Input.GetMouseButtonUp(0);

			var mousePos = Input.mousePosition;
			mousePos.z = Camera.main.transform.position.magnitude;
			var dragPos = Camera.main.ScreenToWorldPoint(mousePos);
			if(isUp)
			{
				dragPos.x = Mathf.Round(dragPos.x);
				dragPos.y = Mathf.Round(dragPos.y);

				var go = dragObject.GetComponent<GridObject>();
				go.SetGridPos( new Vector2Int( Mathf.RoundToInt(dragPos.x), Mathf.RoundToInt(dragPos.y) ) );
				dragObject.transform.position = new Vector3(go.gridPos.x,go.gridPos.y,0);

			}
			else
			{
				dragPos.z = 0f;
				dragObject.transform.position = dragPos;
			}
			if(isUp)
				dragObject = null;
		}
	}
}