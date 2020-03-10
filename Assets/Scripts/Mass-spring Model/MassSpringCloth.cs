using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassSpringCloth : MonoBehaviour
{
	#region Parameter
	/// <summary>
	/// 布の質点の定義
	/// </summary>
	public class ClothPoint
	{
		/// <summary> 動かすTransform </summary>
		public Transform transform;

		/// <summary> 現在の位置 </summary>
 		public Vector3 position { get { return transform.position; } set { transform.position = value; } }

		/// <summary> 前フレームの位置 /// </summary>
		public Vector3 prePosition;

		/// <summary> 運動計算の重み(0.0 - 1.0) </summary>
		public float weight;

		public ClothPoint(Transform transform, float weight)
		{
			this.transform = transform;
			this.prePosition = transform.position;
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
	public class ClothConstraint
	{
		/// <summary> 質点1 </summary>
		public ClothPoint p1;

		/// <summary> 質点2 </summary>
		public ClothPoint p2;

		/// <summary> 自然長 </summary>
		public float restLength;   // 自然長

		/// <summary> バネの種類 </summary>
		public SpringType type;

		public ClothConstraint(ClothPoint p1, ClothPoint p2, float restLength, SpringType type)
		{
			this.p1 = p1;
			this.p2 = p2;
			this.restLength = restLength;
			this.type = type;
		}
	}
	#endregion

	#region クラスメンバー
	/// <summary> 質点の集合 </summary>
	public ClothPoint[] clothPoints { get; private set; }

	/// <summary> 制約の集合 </summary>
	public ClothConstraint[] constraints { get; private set; }

	/// <summary> 布の質量 </summary>
	[Header("布の質量")]
	public float mass = 0.5f;

	/// <summary> 質点の質量 </summary>
	float massPoint;

	/// <summary> 重力 </summary>
	[Header("重力")]
	public Vector3 gravity = new Vector3(0, -9.8f, 0);

	/// <summary> 空気抵抗 </summary>
	[Header("空気抵抗"), Range(0f, 1.0f)]
	public float drag = 0.1f;

	/// <summary> 反復処理回数 </summary>
	[Header("反復処理回数")]
	public int relaxationCount = 2;

	/// <summary> 構造伸び抵抗 </summary>
	[Header("構造伸び抵抗"), Range(0, 20.0f)]
	public float structuralShrink = 10.0f;

	/// <summary> 構造縮み抵抗 </summary>
	[Header("構造縮み抵抗"), Range(0, 20.0f)]
	public float structuralStretch = 10.0f;

	/// <summary> せん断伸び抵抗 </summary>
	[Header("せん断伸び抵抗"), Range(0, 20.0f)]
	public float shrinkShrink = 0.1f;

	/// <summary> せん断縮み抵抗 </summary>
	[Header("せん断縮み抵抗"), Range(0, 20.0f)]
	public float shrinkStretch = 0.1f;

	/// <summary> 曲げ伸び抵抗 </summary>
	[Header("曲げ伸び抵抗"), Range(0, 20.0f)]
	public float bendingShrink = 10.0f;

	/// <summary> 曲げ縮み抵抗 </summary>
	[Header("曲げ縮み抵抗"), Range(0, 20.0f)]
	public float bendingStretch = 0.1f;
	#endregion

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="clothPoints"></param>
	/// <param name="constraints"></param>
	public void Initialize(ClothPoint[] clothPoints, ClothConstraint[] constraints)
	{
		this.clothPoints = clothPoints;
		this.constraints = constraints;
		massPoint = mass / clothPoints.Length;
	}
	
	/// <summary>
	/// 積分して質点位置を求める
	/// </summary>
	/// <param name="dt"></param>
	private void Integral(float dt)
	{
		// TODO ここ並列化可能
		for(int i = 0; i < clothPoints.Length; i++)
		{
			var p = clothPoints[i];

			// 初速を求める
			var v0 = (p.position - p.prePosition) / dt;

			// 前回位置更新
			p.prePosition = p.position;

			// 力による変位を計算する
			// 力
			var f = (gravity * massPoint) + (v0 * drag * -1.0f);

			// 運動量の変化より速度を求める
			var v = (gravity * dt) + v0;

			// 新しい位置を計算する
			p.position = p.position + ((v * dt) * p.weight);
		}
	}

	/// <summary>
	/// バネの制約を充足させる
	/// </summary>
	/// <param name="count">反復回数</param>
	private void SatisfyConstraint(int count, float dt)
	{
		var ddt = dt / count;

		for(int ite = 0; ite < count; ite++)
		{
			for(int i=0; i < constraints.Length; i++)
			{ 
				if(constraints[i] == null) { continue; }

				// 固定点の場合は無視
				if (constraints[i].p1.weight <= 0 && constraints[i].p2.weight <= 0)
				{
					continue;
				}

				float shrink = 0;   // 伸び抵抗
				float stretch = 0;	// 縮み抵抗
				
				// 構造バネ
				if (constraints[i].type == SpringType.Structual)
				{
					shrink = structuralShrink;
					stretch = structuralStretch;
				}

				// せん断バネ
				else if (constraints[i].type == SpringType.Shear)
				{
					shrink = shrinkShrink;
					stretch = shrinkStretch;
				}

				// 曲げバネ
				else
				{
					shrink = bendingShrink;
					stretch = bendingStretch;
				}

				// バネの力の計算
				// 伸びを計算
				var diff = (constraints[i].p2.position - constraints[i].p1.position);
				var mag = diff.magnitude;
				var f_scalar = mag - constraints[i].restLength;
				f_scalar = f_scalar >= 0 ? f_scalar * shrink : f_scalar * stretch;
				var f = f_scalar * (diff / Mathf.Abs(mag));

				// 位置の変位を計算
				var v = (f * ddt) / massPoint;  // とりあえず、初速0で計算

				// p1の重み
				var p1w = constraints[i].p1.weight / (constraints[i].p1.weight + constraints[i].p2.weight);

				// p2の重み
				var p2w = constraints[i].p2.weight / (constraints[i].p1.weight + constraints[i].p2.weight);

				// 位置を更新
				constraints[i].p1.position = constraints[i].p1.position + (p1w * v * ddt);
				constraints[i].p2.position = constraints[i].p2.position - (p2w * v * ddt);
			}
		}
	}

	private void FixedUpdate()
	{
		Integral(Time.deltaTime);
		SatisfyConstraint(relaxationCount, Time.deltaTime);
	}
}
