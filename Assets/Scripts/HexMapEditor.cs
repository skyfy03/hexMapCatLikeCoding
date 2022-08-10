using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{

	#region Fields

	public Color[] colors;

	public HexGrid hexGrid;

	private Color activeColor;

	int activeElevation;

	public const int chunkSizeX = 5;
	public const int chunkSizeZ = 5;

	#endregion

	void Awake()
	{
		SelectColor(0);
	}

	void Update()
	{
		if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			HandleInput();
		}
	}

	void HandleInput()
	{
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit))
		{
			EditCell(hexGrid.GetCell(hit.point));
		}
	}

	void EditCell(HexCell cell)
	{
		cell.Color = activeColor;
		cell.Elevation = activeElevation;
	}

	public void SelectColor(int index)
	{
		activeColor = colors[index];
	}

	public void SetElevation(float elevation)
	{
		activeElevation = (int)elevation;
	}
}
