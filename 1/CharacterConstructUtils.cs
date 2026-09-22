using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GearVariants;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

// Token: 0x0200001E RID: 30
public static class CharacterConstructUtils
{
	// Token: 0x060000D0 RID: 208 RVA: 0x00009540 File Offset: 0x00007740
	public static GameObject Stitch(GameObject sourceObj, GameObject parentObj, SDCSUtils.TransformCatalog boneCatalog, Material eyeMat = null)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(sourceObj, parentObj.transform);
		gameObject.name = sourceObj.name;
		gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(CharacterConstructUtils.tempSMRs);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in CharacterConstructUtils.tempSMRs)
		{
			string name = skinnedMeshRenderer.gameObject.name;
			skinnedMeshRenderer.bones = CharacterConstructUtils.TranslateTransforms(skinnedMeshRenderer.bones, boneCatalog);
			skinnedMeshRenderer.rootBone = CharacterConstructUtils.Find<string, Transform>(boneCatalog, skinnedMeshRenderer.rootBone.name);
			skinnedMeshRenderer.updateWhenOffscreen = true;
			Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				if (sharedMaterials[i] && sharedMaterials[i].HasColor("_Tint"))
				{
					string name2 = sharedMaterials[i].name;
					Material material = null;
					if (name2.Contains("_Body"))
					{
						material = DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseBodyMatLoc, false);
					}
					else if (name2.Contains("_Head"))
					{
						material = DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseHeadMatLoc, false);
					}
					else if (name2.Contains("_Hand"))
					{
						material = DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseHandsMatLoc, false);
					}
					if (material != null && material.HasColor("_Tint"))
					{
						sharedMaterials[i] = new Material(sharedMaterials[i]);
						sharedMaterials[i].SetColor("_Tint", material.GetColor("_Tint"));
					}
				}
			}
			if (name == "eyes" && eyeMat)
			{
				sharedMaterials[0] = eyeMat;
			}
			skinnedMeshRenderer.sharedMaterials = sharedMaterials;
		}
		CharacterConstructUtils.tempSMRs.Clear();
		Transform transform = boneCatalog["Hips"];
		gameObject.GetComponentsInChildren<Cloth>(CharacterConstructUtils.tempCloths);
		foreach (Cloth cloth in CharacterConstructUtils.tempCloths)
		{
			cloth.capsuleColliders = transform.GetComponentsInChildren<CapsuleCollider>();
		}
		CharacterConstructUtils.tempCloths.Clear();
		return gameObject;
	}

	// Token: 0x060000D1 RID: 209 RVA: 0x00009788 File Offset: 0x00007988
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<string> CollectRequiredNamesForSlot(Transform root, Transform slotSubRoot)
	{
		HashSet<string> hashSet = new HashSet<string>(StringComparer.Ordinal);
		GearBoneMap component = root.GetComponent<GearBoneMap>();
		if (component != null)
		{
			IReadOnlyList<Transform> partBones = component.GetPartBones(slotSubRoot.name);
			if (partBones == null || partBones.Count <= 0)
			{
				goto IL_13B;
			}
			using (IEnumerator<Transform> enumerator = partBones.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Transform transform = enumerator.Current;
					if (transform)
					{
						hashSet.Add(transform.name);
					}
				}
				goto IL_13B;
			}
		}
		Debug.LogWarning(string.Concat(new string[]
		{
			"[SDCSUtils] No GearBoneMap found on root ",
			root.name,
			", falling back to collecting all bones from SMRs under ",
			slotSubRoot.name,
			"."
		}));
		CharacterConstructUtils._smrBuf.Clear();
		slotSubRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true, CharacterConstructUtils._smrBuf);
		for (int i = 0; i < CharacterConstructUtils._smrBuf.Count; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = CharacterConstructUtils._smrBuf[i];
			Transform[] array = (skinnedMeshRenderer != null) ? skinnedMeshRenderer.bones : null;
			if (array != null)
			{
				foreach (Transform transform2 in array)
				{
					if (transform2)
					{
						hashSet.Add(transform2.name);
					}
				}
			}
		}
		IL_13B:
		hashSet = CharacterConstructUtils.BuildAllowedWithAncestors(root, hashSet);
		CharacterConstructUtils._hingeBuf.Clear();
		slotSubRoot.GetComponentsInChildren<HingeJoint>(true, CharacterConstructUtils._hingeBuf);
		for (int k = 0; k < CharacterConstructUtils._hingeBuf.Count; k++)
		{
			HingeJoint hingeJoint = CharacterConstructUtils._hingeBuf[k];
			if (hingeJoint)
			{
				hashSet.Add(hingeJoint.transform.name);
				if (hingeJoint.connectedBody)
				{
					hashSet.Add(hingeJoint.connectedBody.transform.name);
				}
			}
		}
		CharacterConstructUtils._bcBuf.Clear();
		root.GetComponentsInChildren<BlendConstraint>(true, CharacterConstructUtils._bcBuf);
		for (int l = 0; l < CharacterConstructUtils._bcBuf.Count; l++)
		{
			BlendConstraint blendConstraint = CharacterConstructUtils._bcBuf[l];
			if (blendConstraint)
			{
				Transform constrainedObject = blendConstraint.data.constrainedObject;
				Transform sourceObjectA = blendConstraint.data.sourceObjectA;
				Transform sourceObjectB = blendConstraint.data.sourceObjectB;
				if (constrainedObject && hashSet.Contains(constrainedObject.name))
				{
					if (sourceObjectA)
					{
						hashSet.Add(sourceObjectA.name);
					}
					if (sourceObjectB)
					{
						hashSet.Add(sourceObjectB.name);
					}
				}
			}
		}
		return CharacterConstructUtils.BuildAllowedWithAncestors(root, hashSet);
	}

	// Token: 0x060000D2 RID: 210 RVA: 0x00009A2C File Offset: 0x00007C2C
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, Transform> MapSourceByName(Transform sourceOrigin)
	{
		CharacterConstructUtils.<>c__DisplayClass9_0 CS$<>8__locals1;
		CS$<>8__locals1.map = new Dictionary<string, Transform>(StringComparer.Ordinal);
		CharacterConstructUtils.<MapSourceByName>g__Recurse|9_0(sourceOrigin, ref CS$<>8__locals1);
		return CS$<>8__locals1.map;
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00009A58 File Offset: 0x00007C58
	[PublicizedFrom(EAccessModifier.Private)]
	public static HashSet<string> BuildAllowedWithAncestors(Transform sourceOrigin, HashSet<string> allowedBones)
	{
		HashSet<string> hashSet = new HashSet<string>(allowedBones, StringComparer.Ordinal);
		Dictionary<string, Transform> dictionary = CharacterConstructUtils.MapSourceByName(sourceOrigin);
		foreach (string key in allowedBones)
		{
			Transform transform;
			if (dictionary.TryGetValue(key, out transform))
			{
				Transform parent = transform.parent;
				while (parent != null)
				{
					hashSet.Add(parent.name);
					if (parent == sourceOrigin)
					{
						break;
					}
					parent = parent.parent;
				}
			}
		}
		return hashSet;
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x00009AF4 File Offset: 0x00007CF4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void AddRequiredChildren(Transform sourceT, Transform targetT, SDCSUtils.TransformCatalog catalog, HashSet<string> allowedBones, AuxBoneTracker auxBoneTracker = null)
	{
		if (auxBoneTracker == null && catalog != null && catalog["Origin"] != null)
		{
			auxBoneTracker = catalog["Origin"].GetComponent<AuxBoneTracker>();
		}
		for (int i = 0; i < sourceT.childCount; i++)
		{
			Transform child = sourceT.GetChild(i);
			if (allowedBones.Contains(child.name))
			{
				Transform transform = null;
				for (int j = 0; j < targetT.childCount; j++)
				{
					Transform child2 = targetT.GetChild(j);
					if (child2.name == child.name)
					{
						transform = child2;
						break;
					}
				}
				if (!transform)
				{
					transform = UnityEngine.Object.Instantiate<GameObject>(child.gameObject, targetT, true).transform;
					transform.name = child.name;
					transform.SetLocalPositionAndRotation(child.localPosition, child.localRotation);
					transform.localScale = child.localScale;
					if (auxBoneTracker && !auxBoneTracker.AuxBoneLookup.ContainsKey(transform.name))
					{
						auxBoneTracker.AuxBoneLookup.Add(transform.name, transform);
					}
				}
				if (!catalog.ContainsKey(transform.name))
				{
					catalog.Add(transform.name, transform);
				}
				CharacterConstructUtils.TransferCharacterJoint(child, transform.gameObject, catalog);
				CharacterConstructUtils.AddRequiredChildren(child, transform, catalog, allowedBones, auxBoneTracker);
			}
		}
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x00009C40 File Offset: 0x00007E40
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetupRigConstraints(RigBuilder rigBuilder, Transform sourceRootT, Transform targetRootT, SDCSUtils.TransformCatalog catalog, HashSet<string> allowedBones)
	{
		Transform transform = sourceRootT.parent.Find("RigConstraints");
		if (!transform)
		{
			return;
		}
		string text = "RigConstraints_" + sourceRootT.name;
		Transform transform2 = targetRootT.Find(text);
		if (!transform2)
		{
			transform2 = new GameObject(text).transform;
			transform2.SetParent(targetRootT, false);
		}
		Rig rig = transform2.gameObject.GetOrAddComponent<Rig>();
		if (!rigBuilder.layers.Any((RigLayer l) => l.rig == rig))
		{
			rigBuilder.layers.Add(new RigLayer(rig, true));
		}
		for (int i = transform2.childCount - 1; i >= 0; i--)
		{
			UnityEngine.Object.DestroyImmediate(transform2.GetChild(i).gameObject);
		}
		foreach (BlendConstraint blendConstraint in transform.GetComponentsInChildren<BlendConstraint>(true))
		{
			string text2 = (blendConstraint.data.constrainedObject != null) ? blendConstraint.data.constrainedObject.name : null;
			if (text2 != null && allowedBones.Contains(text2))
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(blendConstraint.gameObject, transform2, false);
				gameObject.name = blendConstraint.name;
				BlendConstraint component = gameObject.GetComponent<BlendConstraint>();
				component.data.constrainedObject = ((text2 != null) ? CharacterConstructUtils.Find<string, Transform>(catalog, text2) : null);
				string text3 = (blendConstraint.data.sourceObjectA != null) ? blendConstraint.data.sourceObjectA.name : null;
				string text4 = (blendConstraint.data.sourceObjectB != null) ? blendConstraint.data.sourceObjectB.name : null;
				component.data.sourceObjectA = ((text3 != null) ? CharacterConstructUtils.Find<string, Transform>(catalog, text3) : null);
				component.data.sourceObjectB = ((text4 != null) ? CharacterConstructUtils.Find<string, Transform>(catalog, text4) : null);
			}
		}
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00009E38 File Offset: 0x00008038
	public static void MatchRigs(Transform source, Transform target, SDCSUtils.TransformCatalog catalog, HashSet<string> allowedBones)
	{
		Transform transform = source.Find("Origin");
		Transform transform2 = target.Find("Origin");
		if (!transform || !transform2)
		{
			return;
		}
		AuxBoneTracker auxBoneTracker = null;
		if (catalog != null && catalog["Origin"] != null)
		{
			auxBoneTracker = catalog["Origin"].GetComponent<AuxBoneTracker>();
		}
		CharacterConstructUtils.AddRequiredChildren(transform, transform2, catalog, allowedBones, auxBoneTracker);
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x00009EA4 File Offset: 0x000080A4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void TransferCharacterJoint(Transform source, GameObject newBone, SDCSUtils.TransformCatalog transformCatalog)
	{
		CharacterJoint component;
		CharacterJoint characterJoint;
		if ((component = source.GetComponent<CharacterJoint>()) != null && (characterJoint = newBone.AddMissingComponent<CharacterJoint>()) != null)
		{
			Transform transform = CharacterConstructUtils.Find<string, Transform>(transformCatalog, component.connectedBody.name);
			characterJoint.connectedBody = ((transform != null) ? transform.GetComponent<Rigidbody>() : null);
		}
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00009EFC File Offset: 0x000080FC
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform[] TranslateTransforms(Transform[] transforms, SDCSUtils.TransformCatalog transformCatalog)
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			Transform transform = transforms[i];
			if (transform)
			{
				transforms[i] = CharacterConstructUtils.Find<string, Transform>(transformCatalog, transform.name);
			}
			else
			{
				Log.Error("Null transform in bone list");
			}
		}
		return transforms;
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x00009F40 File Offset: 0x00008140
	public static TValue Find<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key)
	{
		TValue result;
		source.TryGetValue(key, out result);
		return result;
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x060000DA RID: 218 RVA: 0x00009F58 File Offset: 0x00008158
	public static string baseBodyLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Common/Meshes/player",
				CharacterConstructUtils.archetype.Sex,
				".fbx"
			});
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x060000DB RID: 219 RVA: 0x00009F98 File Offset: 0x00008198
	public static string baseHeadLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Heads/",
				CharacterConstructUtils.archetype.Race,
				"/",
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"/Meshes/player",
				CharacterConstructUtils.archetype.Sex,
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				".fbx"
			});
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x060000DC RID: 220 RVA: 0x0000A040 File Offset: 0x00008240
	public static string baseHairLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Hair/",
				CharacterConstructUtils.archetype.Hair,
				"/HairMorphMatrix/",
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x060000DD RID: 221 RVA: 0x0000A0B0 File Offset: 0x000082B0
	public static string baseMustacheLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/FacialHair/Mustache/",
				CharacterConstructUtils.archetype.MustacheName,
				"/HairMorphMatrix/",
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x060000DE RID: 222 RVA: 0x0000A120 File Offset: 0x00008320
	public static string baseChopsLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/FacialHair/Chops/",
				CharacterConstructUtils.archetype.ChopsName,
				"/HairMorphMatrix/",
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x060000DF RID: 223 RVA: 0x0000A190 File Offset: 0x00008390
	public static string baseBeardLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/FacialHair/Beard/",
				CharacterConstructUtils.archetype.BeardName,
				"/HairMorphMatrix/",
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060000E0 RID: 224 RVA: 0x0000A1FE File Offset: 0x000083FE
	public static string baseHairColorLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/HairColorSwatches";
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060000E1 RID: 225 RVA: 0x0000A205 File Offset: 0x00008405
	public static string baseEyeColorMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/Eyes/Materials/" + CharacterConstructUtils.archetype.EyeColorName + ".mat";
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060000E2 RID: 226 RVA: 0x0000A220 File Offset: 0x00008420
	public static string baseBodyMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Heads/",
				CharacterConstructUtils.archetype.Race,
				"/",
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"/Materials/player",
				CharacterConstructUtils.archetype.Sex,
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"_Body.mat"
			});
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x060000E3 RID: 227 RVA: 0x0000A2C8 File Offset: 0x000084C8
	public static string baseHeadMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Heads/",
				CharacterConstructUtils.archetype.Race,
				"/",
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"/Materials/player",
				CharacterConstructUtils.archetype.Sex,
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"_Head.mat"
			});
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x060000E4 RID: 228 RVA: 0x0000A370 File Offset: 0x00008570
	public static string baseHandsMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Heads/",
				CharacterConstructUtils.archetype.Race,
				"/",
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"/Materials/player",
				CharacterConstructUtils.archetype.Sex,
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"_Hand.mat"
			});
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x060000E5 RID: 229 RVA: 0x0000A415 File Offset: 0x00008615
	public static string baseRigPrefab
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/BaseRigs/baseRigPrefab.prefab";
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x060000E6 RID: 230 RVA: 0x0000A41C File Offset: 0x0000861C
	public static RuntimeAnimatorController UIAnimController
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return DataLoader.LoadAsset<RuntimeAnimatorController>("@:Entities/Player/Common/AnimControllers/MenuSDCS" + CharacterConstructUtils.archetype.Sex + "Controller" + (CharacterConstructUtils.archetype.IsMale ? ".controller" : ".overrideController"), false);
		}
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x0000A458 File Offset: 0x00008658
	public static void CreateViz(Archetype _archetype, ref GameObject baseRigUI, ref SDCSUtils.TransformCatalog boneCatalogUI)
	{
		CharacterConstructUtils.DestroyViz(baseRigUI, false);
		CharacterConstructUtils.archetype = _archetype;
		CharacterConstructUtils.setupRig(ref baseRigUI, ref boneCatalogUI, CharacterConstructUtils.baseRigPrefab, null, CharacterConstructUtils.UIAnimController);
		CharacterConstructUtils.setupBase(baseRigUI, boneCatalogUI, CharacterConstructUtils.baseParts);
		CharacterConstructUtils.setupHairObjects(baseRigUI, boneCatalogUI, _archetype.Hair, _archetype.MustacheName, _archetype.ChopsName, _archetype.BeardName);
		CharacterConstructUtils.setupEquipment(baseRigUI, boneCatalogUI, _archetype.Equipment, true);
		Transform transform = baseRigUI.transform.Find("IKRig");
		if (transform != null)
		{
			transform.GetComponent<Rig>().weight = 0f;
		}
		foreach (HingeJoint hingeJoint in baseRigUI.GetComponentsInChildren<HingeJoint>())
		{
			if (hingeJoint.connectedBody == null)
			{
				Log.Warning("SDCSUtils::CreateVizUI: No connected body for " + hingeJoint.transform.name + "'s HingeJoint! Disabling for UI until this is solved.");
				hingeJoint.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x0000A544 File Offset: 0x00008744
	public static void DestroyViz(GameObject _baseRigUI, bool _keepRig = false)
	{
		if (_baseRigUI)
		{
			Transform transform = _baseRigUI.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name != "Origin")
				{
					child.GetComponentsInChildren<SkinnedMeshRenderer>(true, CharacterConstructUtils.tempSMRs);
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in CharacterConstructUtils.tempSMRs)
					{
						Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
						if (MeshMorph.IsInstance(sharedMesh))
						{
							UnityEngine.Object.Destroy(sharedMesh);
						}
						skinnedMeshRenderer.GetSharedMaterials(CharacterConstructUtils.tempMats);
						Utils.CleanupMaterials<List<Material>>(CharacterConstructUtils.tempMats);
						CharacterConstructUtils.tempMats.Clear();
					}
				}
			}
			if (!_keepRig)
			{
				UnityEngine.Object.DestroyImmediate(_baseRigUI);
			}
		}
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x0000A61C File Offset: 0x0000881C
	public static void SetVisible(GameObject _baseRigUI, bool _visible)
	{
		if (_baseRigUI)
		{
			Transform transform = _baseRigUI.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name != "Origin")
				{
					child.GetComponentsInChildren<SkinnedMeshRenderer>(true, CharacterConstructUtils.tempSMRs);
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in CharacterConstructUtils.tempSMRs)
					{
						skinnedMeshRenderer.gameObject.SetActive(_visible);
					}
				}
			}
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x0000A6B8 File Offset: 0x000088B8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupRig(ref GameObject _rigObj, ref SDCSUtils.TransformCatalog _boneCatalog, string prefabLocation, Transform parent, RuntimeAnimatorController animController)
	{
		if (!_rigObj)
		{
			_rigObj = UnityEngine.Object.Instantiate<GameObject>(DataLoader.LoadAsset<GameObject>(prefabLocation, false), parent);
			_boneCatalog = new SDCSUtils.TransformCatalog(_rigObj.transform);
			BoneRenderer[] componentsInChildren = _rigObj.GetComponentsInChildren<BoneRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		else
		{
			CharacterConstructUtils.cleanupEquipment(_rigObj, _boneCatalog);
		}
		Animator component = _rigObj.GetComponent<Animator>();
		if (component && component.runtimeAnimatorController != animController)
		{
			component.runtimeAnimatorController = animController;
		}
		if (!CharacterConstructUtils.archetype.IsMale)
		{
			CapsuleCollider orAddComponent = _boneCatalog["Hips"].gameObject.GetOrAddComponent<CapsuleCollider>();
			orAddComponent.center = new Vector3(0f, 0f, -0.03f);
			orAddComponent.radius = 0.15f;
			orAddComponent.height = 0.375f;
		}
	}

	// Token: 0x060000EB RID: 235 RVA: 0x0000A790 File Offset: 0x00008990
	[PublicizedFrom(EAccessModifier.Private)]
	public static void cleanupEquipment(GameObject _rigObj, SDCSUtils.TransformCatalog _boneCatalog)
	{
		SDCSUtils.SlotAllowedBonesCache.Clear();
		RigBuilder component = _rigObj.GetComponent<RigBuilder>();
		if (component)
		{
			List<RigLayer> layers = component.layers;
			for (int i = layers.Count - 1; i >= 0; i--)
			{
				if (layers[i].name != "IKRig")
				{
					layers.RemoveAt(i);
				}
			}
			component.Clear();
		}
		Animator component2 = _rigObj.GetComponent<Animator>();
		if (component2)
		{
			component2.UnbindAllStreamHandles();
		}
		GameUtils.DestroyAllChildrenImmediatelyBut(_rigObj.transform, new List<string>
		{
			"Origin",
			"IKRig"
		});
		CharacterConstructUtils.SanitizeRig(_rigObj, _boneCatalog);
	}

	// Token: 0x060000EC RID: 236 RVA: 0x0000A834 File Offset: 0x00008A34
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SanitizeRig(GameObject _rigObj, SDCSUtils.TransformCatalog _boneCatalog)
	{
		AuxBoneTracker auxBoneTracker = null;
		if (_boneCatalog != null && _boneCatalog["Origin"] != null)
		{
			auxBoneTracker = _boneCatalog["Origin"].GetComponent<AuxBoneTracker>();
		}
		if (auxBoneTracker)
		{
			foreach (string key in auxBoneTracker.AuxBoneLookup.Keys)
			{
				if (_boneCatalog.ContainsKey(key) && _boneCatalog[key] != null)
				{
					UnityEngine.Object.DestroyImmediate(_boneCatalog[key].gameObject);
					_boneCatalog.Remove(key);
				}
			}
			auxBoneTracker.AuxBoneLookup.Clear();
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, Transform> keyValuePair in _boneCatalog)
		{
			if (keyValuePair.Value == null)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (string key2 in list)
		{
			_boneCatalog.Remove(key2);
		}
	}

	// Token: 0x060000ED RID: 237 RVA: 0x0000A990 File Offset: 0x00008B90
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupBase(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string[] baseParts)
	{
		foreach (string text in baseParts)
		{
			GameObject gameObject;
			if (text == "head")
			{
				gameObject = DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseHeadLoc, false);
			}
			else if (text == "hands")
			{
				gameObject = DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseBodyLoc, false);
			}
			else
			{
				gameObject = DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseBodyLoc, false);
			}
			if (gameObject == null)
			{
				return;
			}
			GameObject bodyPartContainingName;
			if (!((bodyPartContainingName = CharacterConstructUtils.getBodyPartContainingName(gameObject.transform, text)) == null))
			{
				bodyPartContainingName.name = text;
				CharacterConstructUtils.Stitch(bodyPartContainingName, _rig, _boneCatalog, DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseEyeColorMatLoc, false));
				if (text == "head")
				{
					Transform transform = _rig.transform;
					Transform transform2 = gameObject.transform;
					CharacterGazeController orAddComponent = transform.FindRecursive("Head").gameObject.GetOrAddComponent<CharacterGazeController>();
					orAddComponent.rootTransform = transform.FindRecursive("Origin");
					orAddComponent.neckTransform = _boneCatalog["Neck"];
					orAddComponent.headTransform = _boneCatalog["Head"];
					orAddComponent.leftEyeTransform = _boneCatalog["LeftEye"];
					orAddComponent.rightEyeTransform = _boneCatalog["RightEye"];
					orAddComponent.eyeSkinnedMeshRenderer = transform.FindRecursive("eyes").GetComponent<SkinnedMeshRenderer>();
					orAddComponent.leftEyeLocalPosition = transform2.FindInChildren("LeftEye").localPosition;
					orAddComponent.rightEyeLocalPosition = transform2.FindInChildren("RightEye").localPosition;
					orAddComponent.eyeLookAtTargetAngle = 35f;
					orAddComponent.eyeRotationSpeed = 30f;
					orAddComponent.twitchSpeed = 25f;
					orAddComponent.headLookAtTargetAngle = 75f;
					orAddComponent.headRotationSpeed = 7f;
					orAddComponent.maxLookAtDistance = 5f;
					EyeLidController orAddComponent2 = transform.FindRecursive("Head").gameObject.GetOrAddComponent<EyeLidController>();
					orAddComponent2.leftTopTransform = _boneCatalog["LeftEyelidTop"];
					orAddComponent2.leftBottomTransform = _boneCatalog["LeftEyelidBot"];
					orAddComponent2.rightTopTransform = _boneCatalog["RightEyelidTop"];
					orAddComponent2.rightBottomTransform = _boneCatalog["RightEyelidBot"];
					orAddComponent2.leftTopLocalPosition = transform2.FindInChildren("LeftEyelidTop").localPosition;
					orAddComponent2.leftBottomLocalPosition = transform2.FindInChildren("LeftEyelidBot").localPosition;
					orAddComponent2.leftTopRotation = transform2.FindInChildren("LeftEyelidTop").localRotation;
					orAddComponent2.leftBottomRotation = transform2.FindInChildren("LeftEyelidBot").localRotation;
					orAddComponent2.rightTopLocalPosition = transform2.FindInChildren("RightEyelidTop").localPosition;
					orAddComponent2.rightBottomLocalPosition = transform2.FindInChildren("RightEyelidBot").localPosition;
					orAddComponent2.rightTopRotation = transform2.FindInChildren("RightEyelidTop").localRotation;
					orAddComponent2.rightBottomRotation = transform2.FindInChildren("RightEyelidBot").localRotation;
				}
			}
		}
	}

	// Token: 0x060000EE RID: 238 RVA: 0x0000AC60 File Offset: 0x00008E60
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupEquipment(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, List<SDCSUtils.SlotData> slotData, bool _ignoreDlcEntitlements)
	{
		if (slotData == null)
		{
			return;
		}
		List<Transform> allGears = new List<Transform>();
		Transform transform = _rig.transform.Find("Origin");
		if (transform)
		{
			List<Transform> list = CharacterConstructUtils.findStartsWith(transform, "RigConstraints");
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				UnityEngine.Object.DestroyImmediate(list[i].gameObject);
			}
		}
		if (CharacterConstructUtils.archetype.Equipment == null)
		{
			CharacterConstructUtils.archetype.Equipment = new List<SDCSUtils.SlotData>();
			foreach (SDCSUtils.SlotData item in slotData)
			{
				CharacterConstructUtils.archetype.Equipment.Add(item);
			}
		}
		foreach (SDCSUtils.SlotData slotData2 in slotData)
		{
			GameObject gameObject = null;
			if ("head".Equals(slotData2.PartName, StringComparison.OrdinalIgnoreCase) && slotData2.PrefabName != null && slotData2.PrefabName.Contains("HeadGearMorphMatrix", StringComparison.OrdinalIgnoreCase))
			{
				gameObject = CharacterConstructUtils.setupHeadgearMorph(_rig, _boneCatalog, slotData2, _ignoreDlcEntitlements);
			}
			else
			{
				Transform transform2 = CharacterConstructUtils.setupEquipmentSlot(_rig, _boneCatalog, slotData2, allGears, _ignoreDlcEntitlements);
				if (transform2)
				{
					gameObject = CharacterConstructUtils.Stitch(transform2.gameObject, _rig, _boneCatalog, null);
					if (gameObject != null)
					{
						Morphable componentInChildren = gameObject.GetComponentInChildren<Morphable>();
						if (componentInChildren)
						{
							componentInChildren.MorphHeadgear(CharacterConstructUtils.archetype, _ignoreDlcEntitlements);
						}
					}
				}
			}
			if (gameObject != null)
			{
				ColorSwatchApplicator componentInChildren2 = gameObject.GetComponentInChildren<ColorSwatchApplicator>();
				if (componentInChildren2 != null)
				{
					componentInChildren2.ApplyColorSwatch(CharacterConstructUtils.archetype.HairColor);
				}
			}
		}
		CharacterConstructUtils.setupEquipmentCommon(_rig, _boneCatalog, allGears);
	}

	// Token: 0x060000EF RID: 239 RVA: 0x0000AE34 File Offset: 0x00009034
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform setupEquipmentSlot(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, SDCSUtils.SlotData wornItem, List<Transform> allGears, bool _ignoreDlcEntitlements)
	{
		string pathForSlotData = CharacterConstructUtils.GetPathForSlotData(wornItem, true);
		if (string.IsNullOrEmpty(pathForSlotData))
		{
			return null;
		}
		GameObject gameObject = DataLoader.LoadAsset<GameObject>(pathForSlotData, _ignoreDlcEntitlements);
		if (gameObject == null && wornItem.PartName == "head")
		{
			pathForSlotData = CharacterConstructUtils.GetPathForSlotData(wornItem, false);
			gameObject = DataLoader.LoadAsset<GameObject>(pathForSlotData, _ignoreDlcEntitlements);
		}
		if (!gameObject)
		{
			Log.Warning(string.Concat(new string[]
			{
				"SDCSUtils::",
				pathForSlotData,
				" not found for item ",
				wornItem.PrefabName,
				"!"
			}));
			return null;
		}
		string targetBodyPath = "";
		for (int i = 0; i < CharacterConstructUtils.archetype.Equipment.Count; i++)
		{
			SDCSUtils.SlotData slotData = CharacterConstructUtils.archetype.Equipment[i];
			if (slotData.PartName.Equals("body", StringComparison.OrdinalIgnoreCase))
			{
				targetBodyPath = CharacterConstructUtils.GetPathForSlotData(slotData, false);
				break;
			}
		}
		Transform clothingPartWithName = CharacterConstructUtils.getClothingPartWithName(gameObject, CharacterConstructUtils.getPartNameWithVariant(wornItem.PartName, pathForSlotData, targetBodyPath));
		if (clothingPartWithName)
		{
			HashSet<string> allowedBones = CharacterConstructUtils.CollectRequiredNamesForSlot(gameObject.transform, clothingPartWithName);
			SDCSUtils.SlotAllowedBonesCache.Set(clothingPartWithName, allowedBones);
			if (!allGears.Contains(clothingPartWithName))
			{
				allGears.Add(clothingPartWithName);
			}
			string baseToTurnOff = wornItem.BaseToTurnOff;
			if (baseToTurnOff != null && baseToTurnOff.Length > 0)
			{
				foreach (string name in wornItem.BaseToTurnOff.Split(',', StringSplitOptions.None))
				{
					Transform transform = _rig.transform.FindInChildren(name);
					if (transform)
					{
						UnityEngine.Object.Destroy(transform.gameObject);
					}
				}
			}
			if (!clothingPartWithName.gameObject.activeSelf)
			{
				clothingPartWithName.gameObject.SetActive(true);
			}
			CharacterConstructUtils.MatchRigs(gameObject.transform, _rig.transform, _boneCatalog, allowedBones);
		}
		return clothingPartWithName;
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x0000AFF8 File Offset: 0x000091F8
	[PublicizedFrom(EAccessModifier.Private)]
	public static string getPartNameWithVariant(string partName, string sourceGearPath, string targetBodyPath)
	{
		if (CharacterConstructUtils.archetype == null || CharacterConstructUtils.archetype.Equipment == null)
		{
			return partName;
		}
		string text = string.Empty;
		if (GearVariantMatrixSO.Instance != null)
		{
			text = GearVariantMatrixSO.Instance.GetVariantOrEmpty(CharacterConstructUtils.archetype.Sex, partName, sourceGearPath, targetBodyPath);
		}
		else
		{
			Log.Warning("SDCSUtils::getPartNameWithVariant: No GearVariantMatrixSO instance found!");
		}
		if (string.IsNullOrEmpty(text))
		{
			return partName;
		}
		return partName + "_" + text;
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x0000B068 File Offset: 0x00009268
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupEquipmentCommon(GameObject _rigObj, SDCSUtils.TransformCatalog _boneCatalog, List<Transform> allGears)
	{
		RigBuilder orAddComponent = _rigObj.GetOrAddComponent<RigBuilder>();
		orAddComponent.enabled = false;
		foreach (Transform transform in allGears)
		{
			HashSet<string> hashSet;
			if (!SDCSUtils.SlotAllowedBonesCache.TryGet(transform, out hashSet) || hashSet == null || hashSet.Count == 0)
			{
				Debug.LogWarning("[SDCS] No required leaves cached for slot '" + transform.name + "'. Skipping constraints.");
			}
			else
			{
				CharacterConstructUtils.SetupRigConstraints(orAddComponent, transform, _rigObj.transform, _boneCatalog, hashSet);
			}
		}
		foreach (HingeJoint hingeJoint in _rigObj.GetComponentsInChildren<HingeJoint>())
		{
			if (hingeJoint.connectedBody != null && _boneCatalog.ContainsKey(hingeJoint.connectedBody.transform.name))
			{
				hingeJoint.connectedBody = _boneCatalog[hingeJoint.connectedBody.transform.name].GetComponent<Rigidbody>();
			}
			hingeJoint.autoConfigureConnectedAnchor = true;
		}
		orAddComponent.enabled = true;
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x0000B17C File Offset: 0x0000937C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHairObjects(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string hairName, string mustacheName, string chopsName, string beardName)
	{
		HairColorSwatch hairColorSwatch = null;
		if (!string.IsNullOrEmpty(CharacterConstructUtils.archetype.HairColor))
		{
			ScriptableObject scriptableObject = DataLoader.LoadAsset<ScriptableObject>(CharacterConstructUtils.baseHairColorLoc + "/" + CharacterConstructUtils.archetype.HairColor + ".asset", false);
			if (!(scriptableObject == null))
			{
				hairColorSwatch = (scriptableObject as HairColorSwatch);
			}
		}
		bool flag = false;
		bool flag2 = true;
		if (CharacterConstructUtils.archetype.Equipment != null && CharacterConstructUtils.archetype.Equipment.Count > 0)
		{
			foreach (SDCSUtils.SlotData slotData in CharacterConstructUtils.archetype.Equipment)
			{
				if (slotData.PartName == "head")
				{
					flag = (slotData.HairMaskType == SDCSUtils.SlotData.HairMaskTypes.Hat);
					flag2 = (slotData.FacialHairMaskType != SDCSUtils.SlotData.HairMaskTypes.None);
				}
			}
		}
		if (!string.IsNullOrEmpty(hairName))
		{
			if (flag)
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseHairLoc + "/hair_" + hairName + "_hat.asset", hairName);
			}
			else
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseHairLoc + "/hair_" + hairName + ".asset", hairName);
			}
		}
		if (flag2)
		{
			if (!string.IsNullOrEmpty(mustacheName))
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseMustacheLoc + "/hair_facial_mustache" + mustacheName + ".asset", mustacheName);
			}
			if (!string.IsNullOrEmpty(chopsName))
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseChopsLoc + "/hair_facial_sideburns" + chopsName + ".asset", chopsName);
			}
			if (!string.IsNullOrEmpty(beardName))
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseBeardLoc + "/hair_facial_beard" + beardName + ".asset", beardName);
			}
		}
		if (hairColorSwatch != null)
		{
			CharacterConstructUtils.ApplySwatchToGameObject(_rig, hairColorSwatch);
		}
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x0000B334 File Offset: 0x00009534
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplySwatchToGameObject(GameObject targetGameObject, HairColorSwatch hairSwatch)
	{
		if (targetGameObject != null)
		{
			foreach (Renderer renderer in targetGameObject.GetComponentsInChildren<Renderer>(true))
			{
				Material[] array;
				if (Application.isPlaying)
				{
					array = renderer.materials;
				}
				else
				{
					array = renderer.sharedMaterials;
				}
				foreach (Material material in array)
				{
					if (material.shader.name == "Game/SDCS/Hair" && !material.name.Contains("lashes"))
					{
						hairSwatch.ApplyToMaterial(material);
					}
				}
			}
			return;
		}
		Debug.LogWarning("No target GameObject selected.");
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x0000B3DC File Offset: 0x000095DC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHair(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string path, string hairName)
	{
		if (string.IsNullOrEmpty(hairName) || hairName == "-1" || hairName == "none")
		{
			return;
		}
		MeshMorph meshMorph = DataLoader.LoadAsset<MeshMorph>(path, false);
		GameObject gameObject = (meshMorph != null) ? meshMorph.GetMorphedSkinnedMesh() : null;
		if (gameObject == null)
		{
			Log.Warning(string.Concat(new string[]
			{
				"SDCSUtils::",
				path,
				" not found for hair ",
				hairName,
				"!"
			}));
			return;
		}
		if (!gameObject.gameObject.activeSelf)
		{
			gameObject.gameObject.SetActive(true);
		}
		CharacterConstructUtils.Stitch(gameObject.gameObject, _rig, _boneCatalog, null);
		UnityEngine.Object.Destroy(gameObject);
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x0000B48C File Offset: 0x0000968C
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetPathForSlotData(SDCSUtils.SlotData _slotData, bool _headgearShortHairMask = false)
	{
		if (_slotData == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(_slotData.PartName))
		{
			return null;
		}
		if (_slotData.PartName.Equals("head", StringComparison.OrdinalIgnoreCase))
		{
			string text = _slotData.PrefabName;
			if (text.Contains("{sex}"))
			{
				text = text.Replace("{sex}", CharacterConstructUtils.archetype.Sex);
			}
			if (text.Contains("{race}"))
			{
				text = text.Replace("{race}", CharacterConstructUtils.archetype.Race);
			}
			if (text.Contains("{variant}"))
			{
				text = text.Replace("{variant}", CharacterConstructUtils.archetype.Variant.ToString("00"));
			}
			if (text.Contains("{hair}"))
			{
				if (_headgearShortHairMask && (string.IsNullOrEmpty(CharacterConstructUtils.archetype.Hair) || CharacterConstructUtils.shortHairNames.ContainsCaseInsensitive(CharacterConstructUtils.archetype.Hair)))
				{
					text = text.Replace("{hair}", "Bald");
				}
				else
				{
					text = text.Replace("{hair}", "");
				}
			}
			return text;
		}
		if (string.IsNullOrEmpty(_slotData.PrefabName))
		{
			return null;
		}
		return CharacterConstructUtils.parseSexedLocation(_slotData.PrefabName, CharacterConstructUtils.archetype.Sex);
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x0000B5C0 File Offset: 0x000097C0
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject setupHeadgearMorph(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, SDCSUtils.SlotData _slotData, bool _ignoreDlcEntitlements)
	{
		string pathForSlotData = CharacterConstructUtils.GetPathForSlotData(_slotData, true);
		if (string.IsNullOrEmpty(pathForSlotData))
		{
			return null;
		}
		MeshMorph meshMorph = DataLoader.LoadAsset<MeshMorph>(pathForSlotData, _ignoreDlcEntitlements);
		if (meshMorph == null)
		{
			pathForSlotData = CharacterConstructUtils.GetPathForSlotData(_slotData, false);
			meshMorph = DataLoader.LoadAsset<MeshMorph>(pathForSlotData, _ignoreDlcEntitlements);
		}
		GameObject gameObject = (meshMorph != null) ? meshMorph.GetMorphedSkinnedMesh() : null;
		if (gameObject == null)
		{
			Log.Warning(string.Concat(new string[]
			{
				"SDCSUtils::",
				pathForSlotData,
				" not found for headgear ",
				_slotData.PrefabName,
				"!"
			}));
			return null;
		}
		if (!gameObject.gameObject.activeSelf)
		{
			gameObject.gameObject.SetActive(true);
		}
		DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseBodyMatLoc, false);
		GameObject result = CharacterConstructUtils.Stitch(gameObject.gameObject, _rig, _boneCatalog, null);
		UnityEngine.Object.Destroy(gameObject);
		return result;
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x0000B688 File Offset: 0x00009888
	public static bool BasePartsExist(Archetype _archetype)
	{
		CharacterConstructUtils.archetype = _archetype;
		if (!DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseBodyLoc, false))
		{
			Log.Error("base body not found at " + CharacterConstructUtils.baseBodyLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseHeadLoc, false))
		{
			Log.Error("base head not found at " + CharacterConstructUtils.baseHeadLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseBodyMatLoc, false))
		{
			Log.Error("body material not found at " + CharacterConstructUtils.baseBodyMatLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseHeadMatLoc, false))
		{
			Log.Error("head material not found at " + CharacterConstructUtils.baseHeadMatLoc);
			return false;
		}
		return true;
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x0000B73C File Offset: 0x0000993C
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Transform> findStartsWith(Transform parent, string key)
	{
		List<Transform> list = new List<Transform>();
		foreach (object obj in parent)
		{
			Transform transform = (Transform)obj;
			if (transform.name.StartsWith(key))
			{
				list.Add(transform);
			}
		}
		return list;
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x0000B7A8 File Offset: 0x000099A8
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject getBodyPartContainingName(Transform parent, string name)
	{
		foreach (object obj in parent.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.name.ToLower().Contains(name))
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	// Token: 0x060000FA RID: 250 RVA: 0x0000B81C File Offset: 0x00009A1C
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform getClothingPartWithName(GameObject clothingPrefab, string partName)
	{
		foreach (object obj in clothingPrefab.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.name.ToLower() == partName.ToLower())
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x060000FB RID: 251 RVA: 0x0000B890 File Offset: 0x00009A90
	[PublicizedFrom(EAccessModifier.Private)]
	public static string parseSexedLocation(string sexedLocation, string sex)
	{
		return sexedLocation.Replace("{sex}", sex);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x0000B950 File Offset: 0x00009B50
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static void <MapSourceByName>g__Recurse|9_0(Transform t, ref CharacterConstructUtils.<>c__DisplayClass9_0 A_1)
	{
		A_1.map[t.name] = t;
		for (int i = 0; i < t.childCount; i++)
		{
			CharacterConstructUtils.<MapSourceByName>g__Recurse|9_0(t.GetChild(i), ref A_1);
		}
	}

	// Token: 0x040000F6 RID: 246
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Cloth> tempCloths = new List<Cloth>();

	// Token: 0x040000F7 RID: 247
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Material> tempMats = new List<Material>();

	// Token: 0x040000F8 RID: 248
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<SkinnedMeshRenderer> tempSMRs = new List<SkinnedMeshRenderer>();

	// Token: 0x040000F9 RID: 249
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] shortHairNames = new string[]
	{
		"buzzcut",
		"cornrows",
		"flattop_fro",
		"mohawk",
		"small_fro"
	};

	// Token: 0x040000FA RID: 250
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<SkinnedMeshRenderer> _smrBuf = new List<SkinnedMeshRenderer>(64);

	// Token: 0x040000FB RID: 251
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<HingeJoint> _hingeBuf = new List<HingeJoint>(32);

	// Token: 0x040000FC RID: 252
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<BlendConstraint> _bcBuf = new List<BlendConstraint>(32);

	// Token: 0x040000FD RID: 253
	[PublicizedFrom(EAccessModifier.Private)]
	public const string ORIGIN = "Origin";

	// Token: 0x040000FE RID: 254
	[PublicizedFrom(EAccessModifier.Private)]
	public const string RIGCON = "RigConstraints";

	// Token: 0x040000FF RID: 255
	[PublicizedFrom(EAccessModifier.Private)]
	public const string IKRIG = "IKRig";

	// Token: 0x04000100 RID: 256
	public const string HEAD = "head";

	// Token: 0x04000101 RID: 257
	public const string EYES = "eyes";

	// Token: 0x04000102 RID: 258
	public const string BEARD = "beard";

	// Token: 0x04000103 RID: 259
	public const string HAIR = "hair";

	// Token: 0x04000104 RID: 260
	public const string BODY = "body";

	// Token: 0x04000105 RID: 261
	public const string HANDS = "hands";

	// Token: 0x04000106 RID: 262
	public const string FEET = "feet";

	// Token: 0x04000107 RID: 263
	public const string HELMET = "helmet";

	// Token: 0x04000108 RID: 264
	public const string TORSO = "torso";

	// Token: 0x04000109 RID: 265
	public const string GLOVES = "gloves";

	// Token: 0x0400010A RID: 266
	public const string BOOTS = "boots";

	// Token: 0x0400010B RID: 267
	public const string SEX_MARKER = "{sex}";

	// Token: 0x0400010C RID: 268
	public const string RACE_MARKER = "{race}";

	// Token: 0x0400010D RID: 269
	public const string VARIANT_MARKER = "{variant}";

	// Token: 0x0400010E RID: 270
	public const string HAIR_MARKER = "{hair}";

	// Token: 0x0400010F RID: 271
	[PublicizedFrom(EAccessModifier.Private)]
	public static Archetype archetype;

	// Token: 0x04000110 RID: 272
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] baseParts = new string[]
	{
		"head",
		"body",
		"hands",
		"feet"
	};
}
