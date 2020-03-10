using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneMassSpringClothTest : MonoBehaviour
{
	MassSpringCloth massSpringCloth = default;

	[SerializeField]
	Transform rootBone = default;

	[SerializeField]
	int div = 16;

	MassSpringCloth.ClothPoint[] clothPoints = default;
	MassSpringCloth.ClothConstraint[] constraints;


	// Start is called before the first frame update
	void Start()
    {
		massSpringCloth = GetComponent<MassSpringCloth>();

		// Initialize
		// 頂点
		clothPoints = new MassSpringCloth.ClothPoint[div * div];

		int px = 0;
		int py = 0;
		foreach (Transform bone in rootBone.transform)
		{
			//Debug.Log(bone.name);
			var weight = py == 0 ? 0.0f : 1.0f;
			clothPoints[(py * div) + px] = new MassSpringCloth.ClothPoint(bone, weight);

			px++;
			if (px >= div)
			{
				px = 0;
				py++;
			}
		}

		// Constraint
		constraints = new MassSpringCloth.ClothConstraint[(div * div) * 6];
		for (int y = 0; y < div; y++)
		{
			for (int x = 0; x < div; x++)
			{
				// 構成バネ
				// 左
				if (x > 0)
				{
					constraints[(div * y * 6) + (x * 6)] = GetConstraint(ref clothPoints[(div * y) + x], ref clothPoints[(div * y) + x - 1], MassSpringCloth.SpringType.Structual);
				}
				else
				{
					constraints[(div * y * 6) + (x * 6)] = null;
				}

				// 上
				if (y + 1 < div)
				{
					constraints[(div * y * 6) + ((x * 6) + 1)] = GetConstraint(ref clothPoints[(div * y) + x], ref clothPoints[(div * (y + 1)) + x], MassSpringCloth.SpringType.Structual);
				}
				else
				{
					constraints[(div * y * 6) + ((x * 6) + 1)] = null;
				}


				// せん断バネ
				// 左上
				if (x > 0 && y + 1 < div)
				{
					constraints[(div * y * 6) + ((x * 6) + 2)] = GetConstraint(ref clothPoints[(div * y) + x], ref clothPoints[(div * (y + 1)) + x - 1], MassSpringCloth.SpringType.Shear);
				}
				else
				{
					constraints[(div * y * 6) + ((x * 6) +2)] = null;
				}

				// 右上
				if(x + 1 < div && y + 1 < div)
				{
					constraints[(div * y * 6) + ((x * 6) + 3)] = GetConstraint(ref clothPoints[(div * y) + x], ref clothPoints[(div * (y + 1)) + x + 1], MassSpringCloth.SpringType.Shear);
				}
				else
				{
					constraints[(div * y * 6) + ((x * 6) + 3)] = null;
				}


				// 曲げバネ
				// 左
				if (x > 1)
				{
					constraints[(div * y * 6) + ((x * 6) + 4)] = GetConstraint(ref clothPoints[(div * y) + x], ref clothPoints[(div * y) + x - 2], MassSpringCloth.SpringType.Bending);
				}
				else
				{
					constraints[(div * y * 6) + ((x * 6) + 4)] = null;
				}

				// 上
				if (y + 1 < div - 1)
				{
					constraints[(div * y * 6) + ((x * 6) + 5)] = GetConstraint(ref clothPoints[(div * y) + x], ref clothPoints[(div * (y + 2)) + x], MassSpringCloth.SpringType.Bending);
				}
				else
				{
					constraints[(div * y * 6) + ((x * 6) + 5)] = null;
				}
			}
		}

		massSpringCloth.Initialize(clothPoints, constraints);
	}

	private MassSpringCloth.ClothConstraint GetConstraint(
		ref MassSpringCloth.ClothPoint p1,
		ref MassSpringCloth.ClothPoint p2, 
		MassSpringCloth.SpringType type)
	{
		if (p1 == null || p2 == null) { return null; }

		return new MassSpringCloth.ClothConstraint(
			p1,
			p2,
			(p2.position - p1.position).magnitude,
			type);
	}
}
