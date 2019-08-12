using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulation {
	public enum Axis {
		x, y, z
	}
	public Axis axis;
	public int index;
	public bool clockwise;
	public Manipulation(Axis axis, int index, bool clockwise) {
		this.axis = axis;
		this.index = index;
		this.clockwise = clockwise;
	}
}
