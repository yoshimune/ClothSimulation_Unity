using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshCount : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		var meshFilter = GetComponent<MeshFilter>();
		var mesh = meshFilter.sharedMesh;

		Debug.Log($"{gameObject.name} meshcount:{mesh.vertexCount}");
		Debug.Log($"{gameObject.name} trianglecount:{mesh.triangles.Length}");

		//for(int i = 0; i < 30; i++)
		//{
		//	Debug.Log($"{gameObject.name} triangle:{i}: {mesh.triangles[i]}");
		//}
	}
}
