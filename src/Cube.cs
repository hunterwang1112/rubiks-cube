using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour {
	public int size;
	public static float speed = 360; // degree per second
	private float degreeLeft = 0;
	public int x, y, z;
	private int direction;
	private bool isLeft;
	public enum FaceIndex {
		Top, Bottom, Left, Right, Front, Back
	}

	public enum ColorIndex {
		Blue, Green, White, Yellow, Red, Orange
	}

	public static Material[] colors;
	public GameObject[] faces;

	public void SetColor(FaceIndex face) {
		if (colors == null) {
			colors = Resources.LoadAll<Material>("Colors");
		}
		faces[(int)face].GetComponent<Renderer>().material = colors[(int)face];
	}

	public void SetIndex(int x, int y, int z) {
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public void SetSize(float size) {
		transform.localScale = new Vector3(size, size, size);
	}

	public void SetPosition(float x, float y, float z) {
		transform.localPosition = new Vector3(x, y, z);
	}

	public void SetRotation(float x, float y, float z) {
		transform.localRotation = Quaternion.Euler(x, y, z);
	}

	public void yRotate(bool isLeft) {
		direction = 0;
		this.isLeft = isLeft;
		degreeLeft += 90;
		int x2 = isLeft ? z : (size - 1) - z;
		int z2 = isLeft ? (size - 1) - x : x;
		x = x2;
		z = z2;
	}

	public void xRotate(bool isLeft) {
		direction = 1;
		this.isLeft = isLeft;
		degreeLeft += 90;
		int y2 = isLeft ? (size - 1) - z : z;
		int z2 = isLeft ? y : (size - 1) - y;
		y = y2;
		z = z2;
	}

	public void zRotate(bool isLeft) {
		direction = 2;
		this.isLeft = isLeft;
		degreeLeft += 90;
		int x2 = isLeft ? (size - 1) - y : y;
		int y2 = isLeft ? x : (size - 1) - x;
		x = x2;
		y = y2;
	}

	void Update() {
		if  (degreeLeft > 0) {
			Vector3 axis;
			float rotateDegree = degreeLeft > speed * Time.deltaTime ? speed * Time.deltaTime : degreeLeft;
			if (direction == 0) {
				axis = isLeft ? transform.parent.up : -transform.parent.up;
			} else if (direction == 1) {
				axis = isLeft ? transform.parent.right : -transform.parent.right;
			} else {
				axis = isLeft ? transform.parent.forward : - transform.parent.forward;
			}
			transform.RotateAround(Vector3.zero, axis, rotateDegree);
			degreeLeft -= rotateDegree;
		}
	}
}
