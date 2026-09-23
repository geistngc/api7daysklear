using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001337 RID: 4919
public class FastWireNode : MonoBehaviour, IWireNode
{
	// Token: 0x1700126E RID: 4718
	// (get) Token: 0x06009B3A RID: 39738 RVA: 0x003AA5D9 File Offset: 0x003A87D9
	// (set) Token: 0x06009B3B RID: 39739 RVA: 0x003AA5E1 File Offset: 0x003A87E1
	public Vector3 StartPosition
	{
		get
		{
			return this.startPosition;
		}
		set
		{
			this.startPosition = value;
		}
	}

	// Token: 0x1700126F RID: 4719
	// (get) Token: 0x06009B3C RID: 39740 RVA: 0x003AA5EA File Offset: 0x003A87EA
	// (set) Token: 0x06009B3D RID: 39741 RVA: 0x003AA5F2 File Offset: 0x003A87F2
	public Vector3 EndPosition
	{
		get
		{
			return this.endPosition;
		}
		set
		{
			this.endPosition = value;
		}
	}

	// Token: 0x06009B3E RID: 39742 RVA: 0x003AA5FC File Offset: 0x003A87FC
	public void Awake()
	{
		if (FastWireNode.BaseMaterial == null)
		{
			FastWireNode.BaseMaterial = Resources.Load<Material>("Materials/WireMaterial");
		}
		if (this.meshFilter == null)
		{
			this.meshFilter = base.transform.gameObject.AddMissingComponent<MeshFilter>();
		}
		if (this.meshCollider == null)
		{
			this.meshCollider = base.transform.gameObject.AddMissingComponent<MeshCollider>();
			this.meshCollider.convex = true;
			this.meshCollider.isTrigger = true;
		}
		if (this.meshRenderer == null)
		{
			this.meshRenderer = base.transform.gameObject.AddMissingComponent<MeshRenderer>();
			this.meshRenderer.material = FastWireNode.BaseMaterial;
		}
		Utils.SetColliderLayerRecursively(base.gameObject, 29);
	}

	// Token: 0x06009B3F RID: 39743 RVA: 0x003AA6C8 File Offset: 0x003A88C8
	public void BuildMesh()
	{
		Vector3 vector = this.EndPosition + this.EndOffset;
		Vector3 vector2 = this.StartPosition + this.StartOffset;
		float num = Vector3.Distance(vector, vector2);
		if (num > 256f)
		{
			return;
		}
		if (num < 0.01f)
		{
			return;
		}
		Vector3 a = vector2 - vector;
		if (a.magnitude == 0f)
		{
			return;
		}
		Vector3 b = a / 16f;
		if (b.magnitude == 0f)
		{
			return;
		}
		Vector3 vector3 = vector + b;
		List<Vector3> list = new List<Vector3>();
		list.Add(vector);
		for (int i = 0; i < 15; i++)
		{
			if (a.normalized != Vector3.up && a.normalized != Vector3.down)
			{
				float num2 = Mathf.Abs((8f - (float)(i + 1)) / 8f);
				num2 *= num2;
				list.Add(vector3 - new Vector3(0f, Mathf.Lerp(0f, this.maxWireDip, 1f - num2), 0f));
			}
			else
			{
				list.Add(vector3);
			}
			vector3 += b;
		}
		if (list.Count > 1)
		{
			list[0] = vector;
			list[list.Count - 1] = vector2;
		}
		Vector3 vector4 = Vector3.one * float.PositiveInfinity;
		Vector3 vector5 = -Vector3.one * float.PositiveInfinity;
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		List<Vector2> list4 = new List<Vector2>();
		List<Vector2> list5 = new List<Vector2>();
		int[] array = new int[396];
		for (int j = 0; j < list.Count; j++)
		{
			float d = (float)j / (float)list.Count * (num * 0.25f);
			list4.Add(Vector2.right * d);
			list4.Add(Vector2.right * d + Vector2.up);
			list4.Add(Vector2.right * d);
			list4.Add(Vector2.right * d + Vector2.up);
			list5.Add(Vector2.zero);
			list5.Add(Vector2.zero);
			list5.Add(Vector2.zero);
			list5.Add(Vector2.zero);
			if (j > 0)
			{
				a = list[j] - list[j - 1];
			}
			Vector3 normalized = Vector3.Cross(Vector3.up, a.normalized).normalized;
			Vector3 normalized2 = Vector3.Cross(a.normalized, normalized).normalized;
			if (a.normalized == Vector3.up || a.normalized == Vector3.down)
			{
				normalized = Vector3.Cross(Vector3.forward, a.normalized).normalized;
				normalized2 = Vector3.Cross(a.normalized, normalized).normalized;
			}
			list2.Add(normalized2 * this.wireRadius + list[j]);
			list2.Add(-normalized2 * this.wireRadius + list[j]);
			list2.Add(normalized * this.wireRadius + list[j]);
			list2.Add(-normalized * this.wireRadius + list[j]);
			if (j == 0)
			{
				normalized2 = Vector3.Lerp(normalized2, (vector - vector2).normalized, 0.5f).normalized;
				normalized = Vector3.Lerp(normalized, (vector - vector2).normalized, 0.5f).normalized;
			}
			else if (j == list.Count - 1)
			{
				normalized2 = Vector3.Lerp(normalized2, -(vector - vector2).normalized, 0.5f).normalized;
				normalized = Vector3.Lerp(normalized, -(vector - vector2).normalized, 0.5f).normalized;
			}
			list3.Add(normalized2);
			list3.Add(-normalized2);
			list3.Add(normalized);
			list3.Add(-normalized);
			if (list[j].x < vector4.x)
			{
				vector4.x = list[j].x;
			}
			if (list[j].x > vector5.x)
			{
				vector5.x = list[j].x;
			}
			if (list[j].y < vector4.y)
			{
				vector4.y = list[j].y;
			}
			if (list[j].y > vector5.y)
			{
				vector5.y = list[j].y;
			}
			if (list[j].z < vector4.z)
			{
				vector4.z = list[j].z;
			}
			if (list[j].z > vector5.z)
			{
				vector5.z = list[j].z;
			}
		}
		int num3 = 0;
		for (int k = 0; k < 15; k++)
		{
			array[num3++] = 4 * k;
			array[num3++] = 4 + 4 * k;
			array[num3++] = 7 + 4 * k;
			array[num3++] = 7 + 4 * k;
			array[num3++] = 3 + 4 * k;
			array[num3++] = 4 * k;
			array[num3++] = 4 + 4 * k;
			array[num3++] = 4 * k;
			array[num3++] = 2 + 4 * k;
			array[num3++] = 2 + 4 * k;
			array[num3++] = 6 + 4 * k;
			array[num3++] = 4 + 4 * k;
			array[num3++] = 3 + 4 * k;
			array[num3++] = 7 + 4 * k;
			array[num3++] = 5 + 4 * k;
			array[num3++] = 5 + 4 * k;
			array[num3++] = 1 + 4 * k;
			array[num3++] = 3 + 4 * k;
			array[num3++] = 6 + 4 * k;
			array[num3++] = 2 + 4 * k;
			array[num3++] = 1 + 4 * k;
			array[num3++] = 1 + 4 * k;
			array[num3++] = 5 + 4 * k;
			array[num3++] = 6 + 4 * k;
		}
		array[num3++] = 0;
		array[num3++] = 3;
		array[num3++] = 1;
		array[num3++] = 1;
		array[num3++] = 2;
		array[num3++] = 0;
		array[num3++] = 60;
		array[num3++] = 62;
		array[num3++] = 61;
		array[num3++] = 61;
		array[num3++] = 63;
		array[num3++] = 60;
		if (list2.Count < 3)
		{
			return;
		}
		if (array.Length < 3)
		{
			return;
		}
		if (this.mesh == null)
		{
			this.mesh = new Mesh();
		}
		this.mesh.SetVertices(list2);
		this.mesh.uv = list4.ToArray();
		this.mesh.uv2 = list5.ToArray();
		this.mesh.SetNormals(list3);
		this.mesh.SetIndices(array, MeshTopology.Triangles, 0);
		this.mesh.RecalculateBounds();
		this.meshFilter.mesh = this.mesh;
		this.meshCollider.sharedMesh = this.mesh;
		if (this.prevWireColor != this.wireColor)
		{
			this.prevWireColor = this.wireColor;
			this.SetWireColor(this.wireColor);
		}
	}

	// Token: 0x06009B40 RID: 39744 RVA: 0x003AAF5A File Offset: 0x003A915A
	public void SetWireColor(Color color)
	{
		if (this.meshRenderer.material == null)
		{
			return;
		}
		this.meshRenderer.material.SetColor("_Color", color);
		this.wireColor = color;
	}

	// Token: 0x06009B41 RID: 39745 RVA: 0x003AAF8D File Offset: 0x003A918D
	public void SetPulseSpeed(float speed)
	{
		if (this.meshRenderer.material == null)
		{
			return;
		}
		this.meshRenderer.material.SetFloat("_PulseSpeed", speed);
	}

	// Token: 0x06009B42 RID: 39746 RVA: 0x003AAFB9 File Offset: 0x003A91B9
	public void SetPulseColor(Color color)
	{
		this.pulseColor = color;
	}

	// Token: 0x06009B43 RID: 39747 RVA: 0x003AAFC2 File Offset: 0x003A91C2
	public void TogglePulse(bool isOn)
	{
		if (this.meshRenderer.material == null)
		{
			return;
		}
		this.meshRenderer.material.SetColor("_PulseColor", isOn ? this.pulseColor : this.wireColor);
	}

	// Token: 0x06009B44 RID: 39748 RVA: 0x003AAFFE File Offset: 0x003A91FE
	public void SetStartPosition(Vector3 pos)
	{
		this.StartPosition = pos;
	}

	// Token: 0x06009B45 RID: 39749 RVA: 0x003AB007 File Offset: 0x003A9207
	public void SetStartPositionOffset(Vector3 pos)
	{
		this.StartOffset = pos;
	}

	// Token: 0x06009B46 RID: 39750 RVA: 0x003AB010 File Offset: 0x003A9210
	public void SetEndPosition(Vector3 pos)
	{
		this.EndPosition = pos;
	}

	// Token: 0x06009B47 RID: 39751 RVA: 0x003AB019 File Offset: 0x003A9219
	public void SetEndPositionOffset(Vector3 pos)
	{
		this.EndOffset = pos;
	}

	// Token: 0x06009B48 RID: 39752 RVA: 0x003AB022 File Offset: 0x003A9222
	public void SetWireDip(float _dist)
	{
		this.maxWireDip = _dist;
	}

	// Token: 0x06009B49 RID: 39753 RVA: 0x003AB02B File Offset: 0x003A922B
	public float GetWireDip()
	{
		return this.maxWireDip;
	}

	// Token: 0x06009B4A RID: 39754 RVA: 0x003AB033 File Offset: 0x003A9233
	public void SetWireRadius(float _radius)
	{
		this.wireRadius = _radius;
	}

	// Token: 0x06009B4B RID: 39755 RVA: 0x003AB03C File Offset: 0x003A923C
	public void SetWireCanHide(bool _canHide)
	{
		this.canHide = _canHide;
	}

	// Token: 0x06009B4C RID: 39756 RVA: 0x003AB045 File Offset: 0x003A9245
	public Vector3 GetStartPosition()
	{
		return this.StartPosition;
	}

	// Token: 0x06009B4D RID: 39757 RVA: 0x003AB04D File Offset: 0x003A924D
	public Vector3 GetStartPositionOffset()
	{
		return this.StartOffset;
	}

	// Token: 0x06009B4E RID: 39758 RVA: 0x003AB055 File Offset: 0x003A9255
	public Vector3 GetEndPosition()
	{
		return this.EndPosition;
	}

	// Token: 0x06009B4F RID: 39759 RVA: 0x003AB05D File Offset: 0x003A925D
	public Vector3 GetEndPositionOffset()
	{
		return this.EndOffset;
	}

	// Token: 0x06009B50 RID: 39760 RVA: 0x003AB065 File Offset: 0x003A9265
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06009B51 RID: 39761 RVA: 0x003AB06D File Offset: 0x003A926D
	public void SetVisible(bool _visible)
	{
		if (this.canHide)
		{
			base.gameObject.SetActive(_visible);
			return;
		}
		base.gameObject.SetActive(true);
	}

	// Token: 0x06009B52 RID: 39762 RVA: 0x003AB090 File Offset: 0x003A9290
	public Bounds GetBounds()
	{
		return this.mesh.bounds;
	}

	// Token: 0x06009B53 RID: 39763 RVA: 0x003AB09D File Offset: 0x003A929D
	public void Reset()
	{
		this.maxWireDip = 0.25f;
		this.wireRadius = 0.01f;
		this.pulseColor = Color.yellow;
	}

	// Token: 0x04007514 RID: 29972
	public const int cLayerMaskRayCast = 65537;

	// Token: 0x04007515 RID: 29973
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const int NODE_COUNT = 15;

	// Token: 0x04007516 RID: 29974
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float BASE_WIRE_RADIUS = 0.01f;

	// Token: 0x04007517 RID: 29975
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float BASE_MIN_WIRE_DIP = 0f;

	// Token: 0x04007518 RID: 29976
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public const float BASE_MAX_WIRE_DIP = 0.25f;

	// Token: 0x04007519 RID: 29977
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public static Material BaseMaterial;

	// Token: 0x0400751A RID: 29978
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float maxWireDip = 0.25f;

	// Token: 0x0400751B RID: 29979
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float wireRadius = 0.01f;

	// Token: 0x0400751C RID: 29980
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 startPosition;

	// Token: 0x0400751D RID: 29981
	public Vector3 StartOffset;

	// Token: 0x0400751E RID: 29982
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 endPosition;

	// Token: 0x0400751F RID: 29983
	public Vector3 EndOffset;

	// Token: 0x04007520 RID: 29984
	public Color pulseColor = Color.yellow;

	// Token: 0x04007521 RID: 29985
	public Color wireColor = Color.black;

	// Token: 0x04007522 RID: 29986
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool canHide = true;

	// Token: 0x04007523 RID: 29987
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Mesh mesh;

	// Token: 0x04007524 RID: 29988
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshFilter meshFilter;

	// Token: 0x04007525 RID: 29989
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshCollider meshCollider;

	// Token: 0x04007526 RID: 29990
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public MeshRenderer meshRenderer;

	// Token: 0x04007527 RID: 29991
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Color prevWireColor = Color.white;
}
