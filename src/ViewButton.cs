using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ViewButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	private MagicCube magicCube;
	private int direction;
	public void initialize(MagicCube magicCube, int direction) {
		this.magicCube = magicCube;
		this.direction = direction;
	}

	public void OnPointerDown(PointerEventData eventData) {
		magicCube.moveViews[direction] = true;
	}

	public void OnPointerUp(PointerEventData eventData) {
		magicCube.moveViews[direction] = false;
	}
}
