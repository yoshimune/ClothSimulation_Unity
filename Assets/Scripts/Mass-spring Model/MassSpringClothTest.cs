using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MassSpringCloth))]
public class MassSpringClothTest : MonoBehaviour
{
	MassSpringCloth massSpringCloth = default;

	[SerializeField]
	float scale = 1.0f;

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
		clothPoints = new MassSpringCloth.ClothPoint[(div + 1) * (div + 1)];

		for(int y = 0; y < div + 1; y++)
		{
			for(int x = 0; x < div + 1; x++)
			{
				var pos = new Vector3(
					x / (float)div * 2.0f - 1.0f,
					1.0f,
					y / (float)div * 2.0f) * scale;

				var weight = y == 0 ? 0.0f : 1.0f;
				clothPoints[(div * y) + x] = new MassSpringCloth.ClothPoint(pos, weight);
			}
		}

		// Constraint
		constraints = new MassSpringCloth.ClothConstraint[((div + 1) * (div + 1)) * 6];
		for(int y = 0; y < div + 1; y++)
		{
			for(int x = 0; x < div; x++)
			{
				// 構成バネ
				constraints[(div * y) + x] = GetConstraint(x, y, -1, 0, MassSpringCloth.SpringType.Structual);	//左
				constraints[(div * y) + x + 1] = GetConstraint(x, y, 0, -1, MassSpringCloth.SpringType.Structual);  //上

				// せん断バネ
				constraints[(div * y) + x + 2] = GetConstraint(x, y, -1, -1, MassSpringCloth.SpringType.Shear);  //左上
				constraints[(div * y) + x + 3] = GetConstraint(x, y, 1, -1, MassSpringCloth.SpringType.Shear);   //右上

				// 曲げバネ
				constraints[(div * y) + x + 4] = GetConstraint(x, y, -2, 0, MassSpringCloth.SpringType.Bending);  //左上
				constraints[(div * y) + x + 5] = GetConstraint(x, y, 0, -2, MassSpringCloth.SpringType.Bending);  //右上
			}
		}

		massSpringCloth.Initialize(clothPoints, constraints, GetTriangles());
    }

	private MassSpringCloth.ClothConstraint GetConstraint(int x, int y, int offsetX, int offsetY, MassSpringCloth.SpringType type)	
	{
		int targetX = x + offsetX;
		int targetY = y + offsetY;

		if(targetX >= 0 && targetX < div + 1 && targetY >= 0 && targetY < div + 1)
		{
			return new MassSpringCloth.ClothConstraint(
				clothPoints[y * (div + 1) + x],
				clothPoints[targetY * (div + 1) + targetX],
				scale * 2.0f / div * Mathf.Sqrt(offsetX * offsetX + offsetY * offsetY),
				1.0f,
				type);
		}

		return default;
	}

	private int[] GetTriangles()
	{
		var triangles = new int[(div + 1) * (div + 1) * 3];
		for(int y = 0; y < div; y++)
		{
			for (int x = 0; x < div; x++)
			{
				triangles[y * (div + 1) + x] = y * (div + 1) + x;
				triangles[y * (div + 1) + x + 1] = y * (div + 1) + x + 1;
				triangles[y * (div + 1) + x + 2] = (y+1) * (div+1) + x;
			}
		}

		return triangles;
	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
