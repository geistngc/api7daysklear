using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

// Token: 0x02000A4C RID: 2636
[DisallowMultipleComponent]
public class GearBoneMap : MonoBehaviour
{
	// Token: 0x06004FDB RID: 20443 RVA: 0x001E4A6C File Offset: 0x001E2C6C
	public IReadOnlyList<Transform> GetPartBones(string partName)
	{
		if (string.IsNullOrEmpty(partName))
		{
			return Array.Empty<Transform>();
		}
		GearBoneMap.PartBones partBones = this.parts.FirstOrDefault((GearBoneMap.PartBones p) => string.Equals(p.partName, partName, StringComparison.Ordinal));
		if (partBones != null && partBones.bones != null)
		{
			return partBones.bones;
		}
		int num = partName.IndexOf("_");
		if (num < 0)
		{
			num = partName.Length;
		}
		partName = partName.Substring(0, num);
		partBones = this.parts.FirstOrDefault((GearBoneMap.PartBones p) => string.Equals(p.partName, partName, StringComparison.Ordinal));
		if (partBones != null && partBones.bones != null)
		{
			return partBones.bones;
		}
		return Array.Empty<Transform>();
	}

	// Token: 0x06004FDC RID: 20444 RVA: 0x001E4B26 File Offset: 0x001E2D26
	public IReadOnlyList<string> GetPartNames()
	{
		return (from p in this.parts
		select p.partName).ToArray<string>();
	}

	// Token: 0x06004FDD RID: 20445 RVA: 0x001E4B58 File Offset: 0x001E2D58
	public void SetBones(string partName, IEnumerable<Transform> newBones)
	{
		if (string.IsNullOrEmpty(partName))
		{
			return;
		}
		GearBoneMap.PartBones partBones = this.parts.FirstOrDefault((GearBoneMap.PartBones p) => string.Equals(p.partName, partName, StringComparison.Ordinal));
		if (partBones == null)
		{
			partBones = new GearBoneMap.PartBones
			{
				partName = partName
			};
			this.parts.Add(partBones);
		}
		List<Transform> collection = (from t in (from t in newBones
		where t != null
		select t).Distinct<Transform>()
		orderby GearBoneMap.GetHierarchyPath(t), t.name
		select t).ToList<Transform>();
		partBones.bones.Clear();
		partBones.bones.AddRange(collection);
	}

	// Token: 0x06004FDE RID: 20446 RVA: 0x001E4C48 File Offset: 0x001E2E48
	public void ClearAll()
	{
		this.parts.Clear();
	}

	// Token: 0x06004FDF RID: 20447 RVA: 0x001E4C58 File Offset: 0x001E2E58
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetHierarchyPath(Transform t)
	{
		if (t == null)
		{
			return string.Empty;
		}
		Stack<string> stack = new Stack<string>();
		Transform transform = t;
		while (transform != null)
		{
			stack.Push(transform.name);
			transform = transform.parent;
		}
		return string.Join("/", stack);
	}

	// Token: 0x06004FE0 RID: 20448 RVA: 0x001E4CA8 File Offset: 0x001E2EA8
	public void Bake()
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform transform in base.transform.GetComponentsInChildren<Transform>(true))
		{
			int num = transform.name.IndexOf("_");
			if (num < 0)
			{
				num = transform.name.Length;
			}
			if (transform.parent == base.transform && GearBoneMap.DefaultParts.Contains(transform.name.Substring(0, num)))
			{
				list.Add(transform);
			}
		}
		foreach (Transform transform2 in list)
		{
			SkinnedMeshRenderer[] componentsInChildren2 = transform2.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			HashSet<Transform> hashSet = new HashSet<Transform>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren2)
			{
				if (!(skinnedMeshRenderer == null))
				{
					Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
					if (sharedMesh == null)
					{
						Debug.LogWarning("[GearBoneMap] " + skinnedMeshRenderer.name + " has no sharedMesh.");
					}
					else
					{
						try
						{
							NativeArray<byte> bonesPerVertex = sharedMesh.GetBonesPerVertex();
							NativeArray<BoneWeight1> allBoneWeights = sharedMesh.GetAllBoneWeights();
							Transform[] bones = skinnedMeshRenderer.bones;
							int num2 = 0;
							for (int k = 0; k < bonesPerVertex.Length; k++)
							{
								int num3 = (int)bonesPerVertex[k];
								for (int l = 0; l < num3; l++)
								{
									BoneWeight1 boneWeight = allBoneWeights[num2++];
									if (boneWeight.weight > 0f)
									{
										int boneIndex = boneWeight.boneIndex;
										if (boneIndex >= 0 && bones != null && boneIndex < bones.Length)
										{
											Transform transform3 = bones[boneIndex];
											if (transform3 != null)
											{
												hashSet.Add(transform3);
											}
										}
									}
								}
							}
							if (skinnedMeshRenderer.rootBone != null)
							{
								hashSet.Add(skinnedMeshRenderer.rootBone);
							}
						}
						catch (Exception ex)
						{
							Debug.LogError("[GearBoneMap] Failed reading bone weights for " + skinnedMeshRenderer.name + ": " + ex.Message);
						}
					}
				}
			}
			this.SetBones(transform2.name, hashSet);
		}
	}

	// Token: 0x04003D71 RID: 15729
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public List<GearBoneMap.PartBones> parts = new List<GearBoneMap.PartBones>();

	// Token: 0x04003D72 RID: 15730
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static readonly string[] DefaultParts = new string[]
	{
		"head",
		"body",
		"feet",
		"hands"
	};

	// Token: 0x02000A4D RID: 2637
	[Serializable]
	public class PartBones
	{
		// Token: 0x04003D73 RID: 15731
		[Tooltip("e.g. head, body, feet, hands")]
		public string partName;

		// Token: 0x04003D74 RID: 15732
		[Tooltip("The unique bones actually referenced by skin weights for this part")]
		public List<Transform> bones = new List<Transform>();
	}
}
