﻿using System.Collections.Generic;
using UnityEngine;

public class HexMetrics : MonoBehaviour
{

	#region Fields

	public const float outerToInner = 0.866025404f;
	public const float innerToOuter = 1f / outerToInner;

	public const float outerRadius = 10f;

	public const float innerRadius = outerRadius * outerToInner;
	public const float innerDiameter = innerRadius * 2f;

	public const float elevationStep = 2f;//elevation level

	static Vector3[] corners = {
		new Vector3(0f, 0f, outerRadius),
		new Vector3(innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(0f, 0f, -outerRadius),
		new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
		new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
		new Vector3(0f, 0f, outerRadius)
	};

	public const float solidFactor = 0.75f; //increasing this makes the hexcell flatter for content

	public const float blendFactor = 1f - solidFactor;

	#region Terrace
	public const int terracesPerSlope = 2;

	public const int terraceSteps = terracesPerSlope * 2 + 1;

	public const float horizontalTerraceStepSize = 1f / terraceSteps;

	public const float verticalTerraceStepSize = 1f / (terracesPerSlope + 1);

	#endregion

	#region HexEdgeType

	public enum HexEdgeType
	{
		Flat, Slope, Cliff
	}

	#endregion

	#region noiseSource
	public static Texture2D noiseSource;

	public const float cellPerturbStrength = 4f; //5f; should be the max

	public const float noiseScale = 0.003f;

	public const float elevationPerturbStrength = 1.5f;
	#endregion

	public const int chunkSizeX = 5;
	public const int chunkSizeZ = 5;

	public const float streamBedElevationOffset = -1.75f;

	public const float waterElevationOffset = -0.5f;

	public const float waterFactor = 0.6f;

	public static Vector3 GetFirstWaterCorner(HexDirection direction)
	{
		return corners[(int)direction] * waterFactor;
	}

	public static Vector3 GetSecondWaterCorner(HexDirection direction)
	{
		return corners[(int)direction + 1] * waterFactor;
	}

	public const float waterBlendFactor = 1f - waterFactor;

	public static Vector3 GetWaterBridge (HexDirection direction)
	{
		return (corners[(int)direction] + corners[(int)direction + 1]) * waterBlendFactor;
	}

	public const int hashGridSize = 256;

	static HexHash[] hashGrid;

	public static void InitializeHashGrid(int seed)
	{
		hashGrid = new HexHash[hashGridSize * hashGridSize];
		Random.State currentState = Random.state;
		Random.InitState(seed);
		for (int i = 0; i < hashGrid.Length; i++)
		{
			hashGrid[i] = HexHash.Create();
		}
		Random.state = currentState;
	}

	public const float hashGridScale = 0.25f;

	public static HexHash SampleHashGrid(Vector3 position)
	{
		int x = (int)(position.x * hashGridScale ) % hashGridSize;
		if (x < 0)
		{
			x += hashGridSize;
		}
		int z = (int)(position.z * hashGridScale) % hashGridSize;
		if (z < 0)
		{
			z += hashGridSize;
		}
		return hashGrid[x + z * hashGridSize];
	}

	static float[][] featureThresholds = {
		new float[] {0.0f, 0.0f, 0.4f},
		new float[] {0.0f, 0.4f, 0.6f},
		new float[] {0.4f, 0.6f, 0.8f}
	};

	public static float[] GetFeatureThresholds(int level)
	{
		return featureThresholds[level];
	}

	public const float wallHeight = 4f;
	public const float wallYOffSet = -1f;
	public const float wallThickness = 0.75f;

	public static Vector3 WallThicknessOffset (Vector3 near, Vector3 far)
	{
		Vector3 offset;
		offset.x = far.x - near.x;
		offset.y = 0f;
		offset.z = far.z - near.z;
		return offset.normalized * (wallThickness * 0.5f);
	}

	public const float wallElevationOffset = verticalTerraceStepSize;

	public const float wallTowerThreshold = 0.5f;

	public const float bridgeDesignLength = 7f;

	public static int wrapSize;

	public static bool Wrapping
	{
		get
		{
			return wrapSize > 0;
		}
	}

	#endregion

	#region Properties

	public static Vector3 Perturb(Vector3 position)
	{
		Vector4 sample = SampleNoise(position);

		position.x += (sample.x * 2f - 1f) * cellPerturbStrength;

		//Keeps the middle flat, so labels are easy to read. Also terraces look good too
		//position.y += (sample.y * 2f - 1f) * HexMetrics.cellPerturbStrength;

		position.z += (sample.z * 2f - 1f) * cellPerturbStrength;

		return position;
	}

	public static Vector3 GetSolidEdgeMiddle(HexDirection direction)
	{
		return
			(corners[(int)direction] + corners[(int)direction + 1]) *
			(0.5f * solidFactor);
	}

	public static Vector4 SampleNoise(Vector3 position)
	{
		Vector4 sample = noiseSource.GetPixelBilinear(
			position.x * noiseScale, 
			position.z * noiseScale
		);

		if (Wrapping && position.x < innerDiameter * 1.5f)
		{
			Vector4 sample2 = noiseSource.GetPixelBilinear(
				(position.x + wrapSize * innerDiameter) * noiseScale,
				position.z * noiseScale
			);
			sample = Vector4.Lerp(
				sample2, sample, position.x * (1f / innerDiameter) - 0.5f
			);
		}

		return sample;
	}

	public static HexEdgeType GetEdgeType(int elevation1, int elevation2)
	{
		if (elevation1 == elevation2)
		{
			return HexEdgeType.Flat;
		}
		int delta = elevation2 - elevation1;
		if (delta == 1 || delta == -1)
		{
			return HexEdgeType.Slope;
		}
		return HexEdgeType.Cliff;
	}

	public static Vector3 TerraceLerp(Vector3 a, Vector3 b, int step)
	{
		float h = step * HexMetrics.horizontalTerraceStepSize;
		a.x += (b.x - a.x) * h;
		a.z += (b.z - a.z) * h;
		float v = ((step + 1) / 2) * HexMetrics.verticalTerraceStepSize;
		a.y += (b.y - a.y) * v;
		return a;
	}

	public static Color TerraceLerp(Color a, Color b, int step)
	{
		float h = step * HexMetrics.horizontalTerraceStepSize;
		return Color.Lerp(a, b, h);
	}

	public static Vector3 GetFirstCorner(HexDirection direction)
	{
		return corners[(int)direction];
	}

	public static Vector3 GetSecondCorner(HexDirection direction)
	{
		return corners[(int)direction + 1];
	}

	public static Vector3 GetFirstSolidCorner(HexDirection direction)
	{
		return corners[(int)direction] * solidFactor;
	}

	public static Vector3 GetSecondSolidCorner(HexDirection direction)
	{
		return corners[(int)direction + 1] * solidFactor;
	}

	public static Vector3 GetBridge(HexDirection direction)
	{
		return (corners[(int)direction] + corners[(int)direction + 1]) *
			blendFactor;
	}

	public static Vector3 WallLerp(Vector3 near, Vector3 far)
	{
		near.x += (far.x - near.x) * 0.5f;
		near.z += (far.z - near.z) * 0.5f;
		float v =
			near.y < far.y ? wallElevationOffset : (1f - wallElevationOffset);
		near.y += (far.y - near.y) * v + wallYOffSet;
		return near;
	}

	#endregion

}
