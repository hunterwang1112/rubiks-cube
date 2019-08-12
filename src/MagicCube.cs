using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicCube : MonoBehaviour {
	public GameObject Camera;
	public GameObject CubePrefab;
	public GameObject[][][] cubes;
	private int size;
	const float LENGTH = 5;
	private const float cameraSpeed = 120f;
	private float degreeLeft = 0;
	private float originalSpeed = Cube.speed;
	private Queue<Manipulation> manipulations = new Queue<Manipulation>();
	private Stack<Manipulation> redoStack;

	public GameObject SizeSelection;
	public GameObject Game;
	public Dropdown AxisDropDown;
	public Dropdown LevelDropDown;
	public Button clockwiseButton;
	public Button counterButton;
	public GameObject[] ViewButtons;
	public GameObject youWin;
	public GameObject ScrambleSelection;
	public bool[] moveViews = new bool[4];
	private bool started = false;

	void Start() {
		for (int i = 0; i < ViewButtons.Length; i++) {
			ViewButtons[i].GetComponent<ViewButton>().initialize(this, i);
		}
		Camera.transform.RotateAround(Vector3.zero, Vector3.right, 25);
		Camera.transform.RotateAround(Vector3.zero, Vector3.up, 45);
	}

	public void SizeSelect(int size) {
		SetSize(size);
		SizeSelection.SetActive(false);
		ScrambleSelection.SetActive(true);
	}

	public void scrambleSelect(bool scramble) {
		GenerateCube(scramble);
		ScrambleSelection.SetActive(false);
		Game.SetActive(true);
		gameObject.SetActive(true);
	}

	public void ToSizeSelect() {
		SizeSelection.SetActive(true);
		Game.SetActive(false);
		gameObject.SetActive(false);
		youWin.SetActive(false);
		ScrambleSelection.SetActive(false);
	}

	public void SetSize(int size) {
		this.size = size;
	}

	public void GenerateCube(bool scramble) {
		started = false;
		clockwiseButton.interactable = false;
		counterButton.interactable = false;
		for (int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}
		redoStack = null;
		cubes = new GameObject[size][][];
		for (int i = 0; i < size; i++) {
			cubes[i] = new GameObject[size][];
			for (int j = 0; j < size; j++)  {
				cubes[i][j] = new GameObject[size];
				for (int k = 0; k < size; k++) {
					GameObject cube = Instantiate(CubePrefab);
					cube.transform.SetParent(transform);
					Cube control = cube.GetComponent<Cube>();
					float sideLength = LENGTH / size;
					control.SetSize(sideLength);
					control.SetPosition(-LENGTH / 2 + sideLength / 2 + k * sideLength,
					 -LENGTH / 2 + sideLength / 2 + j * sideLength, -LENGTH / 2 + sideLength / 2 + i * sideLength);
					control.SetRotation(0, 0, 0);
					control.SetIndex(k, j, i);
					control.size = size;
					cubes[i][j][k] = cube;
				}
			}
		}
		
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				cubes[x][y][size - 1].GetComponent<Cube>().SetColor(Cube.FaceIndex.Right);
				cubes[x][y][0].GetComponent<Cube>().SetColor(Cube.FaceIndex.Left);
			}
		}
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				cubes[x][size - 1][y].GetComponent<Cube>().SetColor(Cube.FaceIndex.Top);
				cubes[x][0][y].GetComponent<Cube>().SetColor(Cube.FaceIndex.Bottom);
				
			}
		}
		
		for (int x = 0; x < size; x++) {
			for (int y = 0; y < size; y++) {
				cubes[size - 1][x][y].GetComponent<Cube>().SetColor(Cube.FaceIndex.Back);
				cubes[0][x][y].GetComponent<Cube>().SetColor(Cube.FaceIndex.Front);
			}
		}

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		LevelDropDown.ClearOptions();
		for (int x = 1; x <= size; x++) {
			options.Add(new Dropdown.OptionData("" + x));
		}
		LevelDropDown.AddOptions(options);
		LevelDropDown.value = 0;
		Scramble(scramble);
	}

	public void Scramble(bool whether) {
		if (whether) {
			int axisBefore = -1;
			int indexBefore = -1;
			Cube.speed = Cube.speed * 2;
			for (int i = 0; i < size * 10; i++) {
				int randomAxis = (int)(Random.value * 3);
				while (axisBefore == randomAxis) {
					randomAxis = (int)(Random.value * 3);
				}
				axisBefore = randomAxis;
				int randomIndex = (int)(Random.value * size);
				while (indexBefore == randomIndex) {
					randomIndex = (int)(Random.value * size);
				}
				indexBefore = randomIndex;
				Rotate((Manipulation.Axis)randomAxis, randomIndex, Random.value > 0.5);
			}
		} else {
			clockwiseButton.interactable = true;
			counterButton.interactable = true;
		}
		redoStack = new Stack<Manipulation>();
	}
	
	public void PressButton(bool clockwise) {
		Rotate((Manipulation.Axis)AxisDropDown.value, (int)LevelDropDown.value, clockwise);
	}

	public void Rotate(Manipulation.Axis axis, int index, bool clockwise) {
		manipulations.Enqueue(new Manipulation(axis, index, clockwise));
		if (redoStack != null) {
			redoStack.Push(new Manipulation(axis, index, !clockwise));
			if (!started) {
				started = true;
			}
		}
	}

	public void Redo() {
		if  (redoStack != null && redoStack.Count > 0) {
			manipulations.Enqueue(redoStack.Pop());
		}
	}

	void Update() {
		if (degreeLeft > 0) {
			degreeLeft -= Time.deltaTime * Cube.speed;
		}
		if (degreeLeft <= 0) {
			if (manipulations.Count > 0) {
				for (int i = 0; i < size; i++) {
					for (int j = 0; j < size; j++) {
						for (int k = 0; k < size; k++) {
							Manipulation front = manipulations.Peek();
							if (front.axis == Manipulation.Axis.x) {
								if (cubes[i][j][k].GetComponent<Cube>().x == front.index) {
									cubes[i][j][k].GetComponent<Cube>().xRotate(front.clockwise);
								}
							} else if (front.axis == Manipulation.Axis.y) {
								if (cubes[i][j][k].GetComponent<Cube>().y == front.index) {
									cubes[i][j][k].GetComponent<Cube>().yRotate(front.clockwise);
								}
							} else {
								if (cubes[i][j][k].GetComponent<Cube>().z == front.index) {
									cubes[i][j][k].GetComponent<Cube>().zRotate(front.clockwise);
								}
							}
						}
					}
				}
				manipulations.Dequeue();
				degreeLeft = 90;
			} else {
				if (originalSpeed != Cube.speed) {
					Cube.speed = originalSpeed;
					clockwiseButton.interactable = true;
					counterButton.interactable = true;
				}
				if (started && recover()) {
					youWin.SetActive(true);
					gameObject.SetActive(false);
					Game.SetActive(false);
				}
			}
		}
		if (moveViews[0]) {
			Camera.transform.RotateAround(Vector3.zero, Vector3.forward, cameraSpeed * Time.deltaTime);
		}
		if (moveViews[1]) {
			Camera.transform.RotateAround(Vector3.zero, -Vector3.forward, cameraSpeed * Time.deltaTime);
		}
		if (moveViews[2]) {
			Camera.transform.RotateAround(Vector3.zero, Vector3.up, cameraSpeed * Time.deltaTime);
		}
		if (moveViews[3]) {
			Camera.transform.RotateAround(Vector3.zero, -Vector3.up, cameraSpeed * Time.deltaTime);
		}
	}

	public void Quit() {
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	private bool recover() {
		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				for (int k = 0; k < size; k++) {
					Cube control = cubes[i][j][k].GetComponent<Cube>();
					if (control.x != k || control.y != j || control.z != i) {
						return false;
					}
				}
			}
		}
		return true;
	}
}
