using UnityEngine;
using System.Collections.Generic;
using static HexMetrics;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexMesh : MonoBehaviour
{

	#region Fields

	Mesh hexMesh;
	static List<Vector3> vertices = new List<Vector3>();
	static List<Color> colors = new List<Color>();
	static List<int> triangles = new List<int>();
	MeshCollider meshCollider;

	public int elevation;
	public const float elevationStep = 5f;
	#endregion

	void Awake()
	{
		GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
		meshCollider = gameObject.AddComponent<MeshCollider>();
		hexMesh.name = "Hex Mesh";
	}

	public void AddTriangleColor(Color color)
	{
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	public void AddTriangleColor(Color c1, Color c2, Color c3)
	{
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
	}

	public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(v1);
		vertices.Add(v2);
		vertices.Add(v3);
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
	}

	public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		int vertexIndex = vertices.Count;
		vertices.Add(Perturb(v1));
		vertices.Add(Perturb(v2));
		vertices.Add(Perturb(v3));
		vertices.Add(Perturb(v4));
		triangles.Add(vertexIndex);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 1);
		triangles.Add(vertexIndex + 2);
		triangles.Add(vertexIndex + 3);
	}

	public void AddQuadColor(Color c1, Color c2, Color c3, Color c4)
	{
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c3);
		colors.Add(c4);
	}

	public void AddQuadColor(Color c1, Color c2)
	{
		colors.Add(c1);
		colors.Add(c1);
		colors.Add(c2);
		colors.Add(c2);
	}

	public void AddQuadColor(Color color)
	{
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
		colors.Add(color);
	}

	public void Clear()
	{
		hexMesh.Clear();
		vertices.Clear();
		colors.Clear();
		triangles.Clear();
	}

	public void Apply()
	{
		hexMesh.SetVertices(vertices);
		hexMesh.SetColors(colors);
		hexMesh.SetTriangles(triangles, 0);
		hexMesh.RecalculateNormals();
		meshCollider.sharedMesh = hexMesh;
	}

}
