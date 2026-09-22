using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001334 RID: 4916
[ExecuteInEditMode]
public class DebugDrawNormals : MonoBehaviour
{
	// Token: 0x06009B2F RID: 39727 RVA: 0x003AA23C File Offset: 0x003A843C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.die)
		{
			UnityEngine.Object.DestroyImmediate(this);
			return;
		}
		this.TriangleCount = 0;
		this.VertCount = 0;
		MeshFilter component = base.GetComponent<MeshFilter>();
		if (component)
		{
			this.MeshCount = 1;
			this.Draw(component);
			return;
		}
		MeshFilter[] componentsInChildren = base.GetComponentsInChildren<MeshFilter>();
		this.MeshCount = componentsInChildren.Length;
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			this.Draw(componentsInChildren[i]);
		}
		if (componentsInChildren.Length == 0)
		{
			this.SetDie();
		}
	}

	// Token: 0x06009B30 RID: 39728 RVA: 0x003AA2B8 File Offset: 0x003A84B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Draw(MeshFilter mf)
	{
		Mesh sharedMesh = mf.sharedMesh;
		if (!sharedMesh)
		{
			return;
		}
		DebugDrawNormals.Data data;
		if (this.list.Count > 0 && !this.Record)
		{
			data = this.list[0];
		}
		else
		{
			data = new DebugDrawNormals.Data();
			this.list.Add(data);
			this.Record = false;
		}
		sharedMesh.GetVertices(data.verts);
		sharedMesh.GetNormals(data.normals);
		sharedMesh.GetTriangles(data.indices, 0);
		this.VertCount += data.verts.Count;
		this.TriangleCount += data.indices.Count / 3;
		Matrix4x4 localToWorldMatrix = mf.transform.localToWorldMatrix;
		for (int i = 0; i < this.list.Count; i++)
		{
			DebugDrawNormals.Data data2 = this.list[i];
			for (int j = 0; j < data2.normals.Count; j++)
			{
				Utils.DrawRay(localToWorldMatrix.MultiplyPoint(data2.verts[j]), localToWorldMatrix.MultiplyVector(data2.normals[j]) * this.VertexNormalScale, Color.white, Color.blue, 3, 0f);
			}
			for (int k = 0; k < data2.indices.Count - 2; k += 3)
			{
				Vector3 vector = data2.verts[data2.indices[k]];
				Vector3 vector2 = data2.verts[data2.indices[k + 1]];
				Vector3 vector3 = data2.verts[data2.indices[k + 2]];
				Vector3 point = (vector + vector2 + vector3) * 0.33333334f;
				Vector3 normalized = Vector3.Cross(vector2 - vector, vector3 - vector).normalized;
				Utils.DrawRay(localToWorldMatrix.MultiplyPoint(point), localToWorldMatrix.MultiplyVector(normalized) * this.TriangleNormalScale, Color.yellow, Color.red, 3, 0f);
			}
		}
	}

	// Token: 0x06009B31 RID: 39729 RVA: 0x003AA4F0 File Offset: 0x003A86F0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDisable()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			this.SetDie();
		}
	}

	// Token: 0x06009B32 RID: 39730 RVA: 0x003AA505 File Offset: 0x003A8705
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetDie()
	{
		this.list = null;
		this.die = true;
	}

	// Token: 0x04007508 RID: 29960
	public float VertexNormalScale = 0.05f;

	// Token: 0x04007509 RID: 29961
	public float TriangleNormalScale = 0.05f;

	// Token: 0x0400750A RID: 29962
	public int MeshCount;

	// Token: 0x0400750B RID: 29963
	public int TriangleCount;

	// Token: 0x0400750C RID: 29964
	public int VertCount;

	// Token: 0x0400750D RID: 29965
	public bool Record;

	// Token: 0x0400750E RID: 29966
	public List<DebugDrawNormals.Data> list = new List<DebugDrawNormals.Data>();

	// Token: 0x0400750F RID: 29967
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool die;

	// Token: 0x02001335 RID: 4917
	[Serializable]
	public class Data
	{
		// Token: 0x04007510 RID: 29968
		public List<Vector3> verts = new List<Vector3>();

		// Token: 0x04007511 RID: 29969
		public List<Vector3> normals = new List<Vector3>();

		// Token: 0x04007512 RID: 29970
		public List<int> indices = new List<int>();
	}
}
