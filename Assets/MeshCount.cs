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
		var mesh = meshFilter.mesh;

		Debug.Log($"{gameObject.name} meshcount:{mesh.vertexCount}");
		Debug.Log($"{gameObject.name} trianglecount:{mesh.triangles.Length}");

		var vertices = mesh.vertices;

		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = vertices[i] + (Vector3.forward * i * 0.1f);
			Debug.Log($"{gameObject.name} vertices:{i}: {vertices[i]}");
		}

		mesh.vertices = vertices;

		//Debug.Log(mesh.bindposes.Length);
		//for (int i = 0; i < mesh.boneWeights.Length; i++)
		//{
		//	Debug.Log($"{gameObject.name} mesh.boneWeights:{i}: {mesh.boneWeights[i]}");
		//}
	}
}
