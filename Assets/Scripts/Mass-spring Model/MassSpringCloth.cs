using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSpringCloth : MonoBehaviour
{
	#region Parameter
	/// <summary>
	/// 布の質点の定義
	/// </summary>
	public struct ClothPoint
	{
		/// <summary> 現在の位置 </summary>
 		public Vector3 position;

		/// <summary> 前フレームの位置 /// </summary>
		public Vector3 prePosition;

		/// <summary> 運動計算の重み(0.0 - 1.0) </summary>
		public float weight;

		public ClothPoint(Vector3 position, float weight)
		{
			this.position = position;
			this.prePosition = position;
			this.weight = weight;
		}
	}

	/// <summary>
	/// バネの種類
	/// </summary>
	public enum SpringType
	{
		Structual,  // 構成バネ
		Shear,      // せん断バネ
		Bending,    // 曲げバネ
	}

	/// <summary>
	/// コンストレイント
	/// </summary>
	public struct ClothConstraint
	{
		/// <summary> 質点1 </summary>
		public ClothPoint p1;

		/// <summary> 質点2 </summary>
		public ClothPoint p2;

		/// <summary> 自然長 </summary>
		public float restLength;   // 自然長

		/// <summary> ばね定数 </summary>
		public float k;            // ばね定数

		/// <summary> バネの種類 </summary>
		public SpringType type;

		public ClothConstraint(ClothPoint p1, ClothPoint p2, float restLength, float k, SpringType type)
		{
			this.p1 = p1;
			this.p2 = p2;
			this.restLength = restLength;
			this.k = k;
			this.type = type;
		}
	}
	#endregion

	#region クラスメンバー
	/// <summary> 質点の集合 </summary>
	public ClothPoint[] clothPoints { get; private set; }

	/// <summary> トライアングル </summary>
	public int[] clothTriangles { get; private set; }

	/// <summary> 制約の集合 </summary>
	public ClothConstraint[] constraints { get; private set; }
	#endregion

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="clothPoints"></param>
	/// <param name="constraints"></param>
	public void Initialize(ClothPoint[] clothPoints, ClothConstraint[] constraints, int[] triangles)
	{
		this.clothPoints = clothPoints;
		this.clothTriangles = triangles;
		this.constraints = constraints;
	}

	private void Update()
	{
		
	}
}
