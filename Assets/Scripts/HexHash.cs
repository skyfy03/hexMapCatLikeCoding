using UnityEngine;

public struct HexHash {

	public float a;
	public float b;

	public static HexHash Create()
	{
		HexHash hash;
		hash.a = Random.value;
		hash.b = Random.value;
		return hash;
	}
}
