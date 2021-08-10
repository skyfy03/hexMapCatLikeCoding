using UnityEngine;

public class HexCell : MonoBehaviour
{
	#region Fields
	
	public HexCoordinates coordinates;
	public Color color;

	[SerializeField]
	HexCell[] neighbors;

	#endregion

	#region Properties

	#region GetNeighbor
	public HexCell GetNeighbor(HexDirection direction)
	{
		return neighbors[(int)direction];
	}
	#endregion

	#region SetNeighbor
	public void SetNeighbor(HexDirection direction, HexCell cell)
	{
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}
	#endregion

	#endregion

}
