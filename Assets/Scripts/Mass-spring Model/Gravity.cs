using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
	[SerializeField]
	Vector3 g = new Vector3(0, -9.8f, 0);

	Vector3 prePosition;

	private void Start()
	{
		prePosition = transform.position;
	}

	private void FixedUpdate()
	{
		// 初速
		var v0 = (transform.position - prePosition) / Time.fixedDeltaTime;

		prePosition = transform.position;

		// 速度
		var v = v0 + (g * Time.fixedDeltaTime);

		transform.position = transform.position + (v * Time.fixedDeltaTime);

	}
}
