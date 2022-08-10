using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGridChunk : MonoBehaviour
{

	#region Fields

	HexCell[] cells;

	HexMesh hexMesh;
	Canvas gridCanvas;

	#endregion

	void Awake()
	{
		gridCanvas = GetComponentInChildren<Canvas>();
		hexMesh = GetComponentInChildren<HexMesh>();

		cells = new HexCell[HexMetrics.chunkSizeX * HexMetrics.chunkSizeZ];
		ShowUI(false);
	}

	public void AddCell(int index, HexCell cell)
	{
		cells[index] = cell;
		cell.chunk = this;
		cell.transform.SetParent(transform, false);
		cell.uiRect.SetParent(gridCanvas.transform, false);
	}

	public void Refresh()
	{
		enabled = true;
	}

	void LateUpdate()
	{
		hexMesh.Triangulate(cells);
		enabled = false;
	}

	public void ShowUI (bool visible)
	{
		gridCanvas.gameObject.SetActive(visible);
	}

}
