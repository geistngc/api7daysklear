using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000DD RID: 221
public class DismembermentManager
{
	// Token: 0x06000597 RID: 1431 RVA: 0x00027F9B File Offset: 0x0002619B
	public static string GetAssetBundlePath(string prefabPath)
	{
		return "@:Entities/Zombies/" + prefabPath + ".prefab";
	}

	// Token: 0x06000598 RID: 1432 RVA: 0x00027FB0 File Offset: 0x000261B0
	public static bool IsDefaultGib(string matName)
	{
		for (int i = 0; i < DismembermentManager.DefaultBundleGibs.Length; i++)
		{
			if (DismembermentManager.DefaultBundleGibs[i] == matName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x1700005B RID: 91
	// (get) Token: 0x06000599 RID: 1433 RVA: 0x00027FE4 File Offset: 0x000261E4
	public Material GibCapsMaterial
	{
		get
		{
			if (!this.zombieGibCapsMaterial)
			{
				Material material = DataLoader.LoadAsset<Material>("@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps.mat", false);
				this.zombieGibCapsMaterial = UnityEngine.Object.Instantiate<Material>(material);
				this.zombieGibCapsMaterial.name = material.name + "(global)";
				if (DismembermentManager.DebugLogEnabled)
				{
					Log.Out("{0} material: {1}", new object[]
					{
						this.zombieGibCapsMaterial ? "load" : "load failed",
						"@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps.mat"
					});
				}
			}
			return this.zombieGibCapsMaterial;
		}
	}

	// Token: 0x1700005C RID: 92
	// (get) Token: 0x0600059A RID: 1434 RVA: 0x00028074 File Offset: 0x00026274
	public Material GibCapsRadMaterial
	{
		get
		{
			if (!this.zombieGibCapsMaterialRadiated)
			{
				Material material = DataLoader.LoadAsset<Material>("@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps_IsRadiated.mat", false);
				this.zombieGibCapsMaterialRadiated = UnityEngine.Object.Instantiate<Material>(material);
				this.zombieGibCapsMaterialRadiated.name = material.name + "(global)";
				if (DismembermentManager.DebugLogEnabled)
				{
					Log.Out("{0} material: {1}", new object[]
					{
						this.zombieGibCapsMaterialRadiated ? "load" : "load failed",
						"@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps_IsRadiated.mat"
					});
				}
			}
			return this.zombieGibCapsMaterialRadiated;
		}
	}

	// Token: 0x0600059B RID: 1435 RVA: 0x00028102 File Offset: 0x00026302
	public static Texture GetShaderTexture(Material _mat)
	{
		if (_mat.HasTexture("_ZombieColor"))
		{
			return _mat.GetTexture("_ZombieColor");
		}
		if (_mat.HasTexture("_Albedo"))
		{
			return _mat.GetTexture("_Albedo");
		}
		return null;
	}

	// Token: 0x0600059C RID: 1436 RVA: 0x00028137 File Offset: 0x00026337
	public static void SetShaderTexture(Material _mat, Texture _altColor)
	{
		if (_mat.HasTexture("_ZombieColor"))
		{
			_mat.SetTexture("_ZombieColor", _altColor);
		}
		if (_mat.HasTexture("_Albedo"))
		{
			_mat.SetTexture("_Albedo", _altColor);
		}
	}

	// Token: 0x1700005D RID: 93
	// (get) Token: 0x0600059D RID: 1437 RVA: 0x0002816B File Offset: 0x0002636B
	public static DismembermentManager Instance
	{
		get
		{
			return DismembermentManager.instance;
		}
	}

	// Token: 0x0600059F RID: 1439 RVA: 0x00028185 File Offset: 0x00026385
	public static void Init()
	{
		DismembermentManager.instance = new DismembermentManager();
		if (DismembermentManager.DebugLogEnabled)
		{
			Log.Out("DismembermentManager Init");
		}
	}

	// Token: 0x060005A0 RID: 1440 RVA: 0x000281A4 File Offset: 0x000263A4
	public static void Cleanup()
	{
		if (DismembermentManager.instance != null)
		{
			List<DismemberedPart> list = DismembermentManager.instance.parts;
			for (int i = 0; i < list.Count; i++)
			{
				list[i].CleanupDetached();
			}
			list.Clear();
		}
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x000281E6 File Offset: 0x000263E6
	public void AddPart(DismemberedPart part)
	{
		this.parts.Add(part);
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x000281F4 File Offset: 0x000263F4
	public void Update()
	{
		for (int i = 0; i < this.parts.Count; i++)
		{
			DismemberedPart dismemberedPart = this.parts[i];
			dismemberedPart.Update();
			if (dismemberedPart.ReadyForCleanup)
			{
				dismemberedPart.CleanupDetached();
				this.parts.RemoveAt(i);
				i--;
			}
		}
		if (this.parts.Count > 25)
		{
			int num = this.parts.Count - 25;
			for (int j = 0; j < num; j++)
			{
				this.parts[j].ReadyForCleanup = true;
			}
		}
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00028284 File Offset: 0x00026484
	public static float GetImpactForce(ItemClass ic, float strength)
	{
		if (ic != null)
		{
			if (ic.HasAnyTags(DismembermentManager.shotgunTags))
			{
				return 1.5f;
			}
			if (ic.HasAnyTags(DismembermentManager.sledgeTags))
			{
				return Mathf.Clamp(1f + Mathf.Abs(strength), 1f, 1.5f);
			}
			if (ic.HasAnyTags(DismembermentManager.knifeTags))
			{
				return Mathf.Abs(1f * strength) * 0.67f;
			}
		}
		return 1f;
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x000282F8 File Offset: 0x000264F8
	public static EnumBodyPartHit GetBodyPartHit(uint bodyDamageFlag)
	{
		if (bodyDamageFlag == 1U)
		{
			return EnumBodyPartHit.Head;
		}
		if (bodyDamageFlag == 2U)
		{
			return EnumBodyPartHit.LeftUpperArm;
		}
		if (bodyDamageFlag == 4U)
		{
			return EnumBodyPartHit.LeftLowerArm;
		}
		if (bodyDamageFlag == 8U)
		{
			return EnumBodyPartHit.RightUpperArm;
		}
		if (bodyDamageFlag == 16U)
		{
			return EnumBodyPartHit.RightLowerArm;
		}
		if (bodyDamageFlag == 32U)
		{
			return EnumBodyPartHit.LeftUpperLeg;
		}
		if (bodyDamageFlag == 64U)
		{
			return EnumBodyPartHit.LeftLowerLeg;
		}
		if (bodyDamageFlag == 128U)
		{
			return EnumBodyPartHit.RightUpperLeg;
		}
		if (bodyDamageFlag == 256U)
		{
			return EnumBodyPartHit.RightLowerLeg;
		}
		return EnumBodyPartHit.None;
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00028358 File Offset: 0x00026558
	public static EnumBodyPartHit GetBodyPartHit(string _propKey)
	{
		if (_propKey.ContainsCaseInsensitive("L_HeadGore"))
		{
			return EnumBodyPartHit.Head;
		}
		if (_propKey.ContainsCaseInsensitive("L_LeftUpperArmGore"))
		{
			return EnumBodyPartHit.LeftUpperArm;
		}
		if (_propKey.ContainsCaseInsensitive("L_LeftLowerArmGore"))
		{
			return EnumBodyPartHit.LeftLowerArm;
		}
		if (_propKey.ContainsCaseInsensitive("L_RightUpperArmGore"))
		{
			return EnumBodyPartHit.RightUpperArm;
		}
		if (_propKey.ContainsCaseInsensitive("L_RightLowerArmGore"))
		{
			return EnumBodyPartHit.RightLowerArm;
		}
		if (_propKey.ContainsCaseInsensitive("L_LeftUpperLegGore"))
		{
			return EnumBodyPartHit.LeftUpperLeg;
		}
		if (_propKey.ContainsCaseInsensitive("L_LeftLowerLegGore"))
		{
			return EnumBodyPartHit.LeftLowerLeg;
		}
		if (_propKey.ContainsCaseInsensitive("L_RightUpperLegGore"))
		{
			return EnumBodyPartHit.RightUpperLeg;
		}
		if (_propKey.ContainsCaseInsensitive("L_RightLowerLegGore"))
		{
			return EnumBodyPartHit.RightLowerLeg;
		}
		return EnumBodyPartHit.None;
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x000283FC File Offset: 0x000265FC
	public static string GetDamageTag(EnumDamageTypes _damageType, bool lastHitRanged)
	{
		if (_damageType == EnumDamageTypes.Piercing && lastHitRanged)
		{
			return "blunt";
		}
		switch (_damageType)
		{
		case EnumDamageTypes.Piercing:
		case EnumDamageTypes.Slashing:
			return "blade";
		case EnumDamageTypes.Bashing:
		case EnumDamageTypes.Crushing:
			return "blunt";
		case EnumDamageTypes.Heat:
			return "blunt";
		}
		return null;
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x0002844C File Offset: 0x0002664C
	public static DismemberedPartData DismemberPart(uint bodyDamageFlag, EnumDamageTypes damageType, EntityAlive _entity, bool isBiped, bool useLegacy = false)
	{
		return DismembermentManager.dismemberPart(DismembermentManager.GetBodyPartHit(bodyDamageFlag), damageType, _entity, isBiped, useLegacy);
	}

	// Token: 0x060005A8 RID: 1448 RVA: 0x00028460 File Offset: 0x00026660
	[PublicizedFrom(EAccessModifier.Private)]
	public static DismemberedPartData dismemberPart(EnumBodyPartHit partHit, EnumDamageTypes damageType, EntityAlive _entity, bool isBiped, bool useLegacy = false)
	{
		if (!DismembermentManager.hasDismemberedPart(partHit, isBiped))
		{
			return null;
		}
		DismemberedPartData dismemberedPartData = new DismemberedPartData();
		string[] dismemberedPart = DismembermentManager.getDismemberedPart(partHit, isBiped);
		dismemberedPartData.propertyKey = dismemberedPart[0];
		dismemberedPartData.prefabPath = dismemberedPart[1];
		dismemberedPartData.damageTypeKey = DismembermentManager.GetDamageTag(damageType, _entity.lastHitRanged);
		DynamicProperties properties = _entity.EntityClass.Properties;
		if (useLegacy || !properties.Contains(dismemberedPartData.propertyKey) || string.IsNullOrEmpty(properties.GetValue(dismemberedPartData.propertyKey)))
		{
			return dismemberedPartData;
		}
		if (properties.Data.ContainsKey(dismemberedPartData.propertyKey))
		{
			string[] array = properties.Values[dismemberedPartData.propertyKey].Split(';', StringSplitOptions.None);
			string[] array2 = properties.Data[dismemberedPartData.propertyKey].Split(';', StringSplitOptions.None);
			if (array[0].ContainsCaseInsensitive("linked"))
			{
				string key = array2[0].Replace("target=", "");
				array = properties.Values[key].Split(';', StringSplitOptions.None);
				array2 = properties.Data[key].Split(';', StringSplitOptions.None);
				dismemberedPartData.isLinked = true;
			}
			DismemberedPartData dismemberedPartData2 = DismembermentManager.readRandomPart(array, array2, dismemberedPartData.damageTypeKey);
			if (dismemberedPartData2 == null && dismemberedPartData.damageTypeKey == "blunt" && !dismemberedPartData.prefabPath.ContainsCaseInsensitive("blunt"))
			{
				DismemberedPartData dismemberedPartData3 = DismembermentManager.readRandomPart(array, array2, "blade");
				if (dismemberedPartData3 != null && (dismemberedPartData3.useMask || dismemberedPartData3.scaleOutLimb))
				{
					dismemberedPartData2 = dismemberedPartData3;
				}
			}
			if (dismemberedPartData2 != null)
			{
				if (!dismemberedPartData2.isLinked && dismemberedPartData2.Invalid)
				{
					return dismemberedPartData;
				}
				if (!string.IsNullOrEmpty(dismemberedPartData2.prefabPath))
				{
					dismemberedPartData.prefabPath = dismemberedPartData2.prefabPath;
				}
				dismemberedPartData.scale = dismemberedPartData2.scale;
				if (dismemberedPartData2.hasRotOffset)
				{
					dismemberedPartData.SetRot(dismemberedPartData2.rot);
				}
				dismemberedPartData.targetBone = dismemberedPartData2.targetBone;
				dismemberedPartData.attachToParent = dismemberedPartData2.attachToParent;
				dismemberedPartData.particlePaths = dismemberedPartData2.particlePaths;
				dismemberedPartData.isDetachable = dismemberedPartData2.isDetachable;
				dismemberedPartData.offset = dismemberedPartData2.offset;
				dismemberedPartData.useMask = dismemberedPartData2.useMask;
				dismemberedPartData.scaleOutLimb = dismemberedPartData2.scaleOutLimb;
				dismemberedPartData.solTarget = dismemberedPartData2.solTarget;
				dismemberedPartData.solScale = dismemberedPartData2.solScale;
				dismemberedPartData.hasSolScale = dismemberedPartData2.hasSolScale;
				dismemberedPartData.childTargetObj = dismemberedPartData2.childTargetObj;
				dismemberedPartData.insertBoneObj = dismemberedPartData2.insertBoneObj;
				dismemberedPartData.addScalePoint = dismemberedPartData2.addScalePoint;
				dismemberedPartData.maskScaleBlend = dismemberedPartData2.maskScaleBlend;
				dismemberedPartData.setFixedValues = dismemberedPartData2.setFixedValues;
				if (properties.Contains("DismemberMaterial"))
				{
					dismemberedPartData.dismemberMatPath = properties.GetValue("DismemberMaterial");
				}
			}
		}
		if (DismembermentManager.DebugLogEnabled)
		{
			Log.Out("[{0}.DismemberPart] - entityClass: {1}{2}", new object[]
			{
				"DismembermentManager",
				EntityClass.list[_entity.entityClass].entityClassName,
				dismemberedPartData.Log()
			});
		}
		return dismemberedPartData;
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x0002875C File Offset: 0x0002695C
	public static void ActivateDetachable(Transform rootT, string targetPart)
	{
		Transform transform = rootT;
		Transform transform2 = transform.Find("Physics");
		if (transform2)
		{
			transform = transform2;
		}
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			bool active = child.name.ContainsCaseInsensitive(targetPart);
			child.gameObject.SetActive(active);
		}
	}

	// Token: 0x060005AA RID: 1450 RVA: 0x000287B4 File Offset: 0x000269B4
	public static DismemberedPartData GetPartData(EntityAlive _entity)
	{
		string[] dismemberedPart = DismembermentManager.getDismemberedPart(_entity.bodyDamage.bodyPartHit, true);
		if (dismemberedPart != null)
		{
			string key = dismemberedPart[0];
			DynamicProperties properties = _entity.EntityClass.Properties;
			if (properties.Data.ContainsKey(key))
			{
				string[] array = properties.Values[key].Split(';', StringSplitOptions.None);
				string a = array[0];
				string[] array2 = properties.Data[key].Split(';', StringSplitOptions.None);
				string text = array2[0].Replace("target=", "");
				bool flag = a.ContainsCaseInsensitive("linked");
				if (flag)
				{
					array = properties.Values[text].Split(';', StringSplitOptions.None);
					array2 = properties.Data[text].Split(';', StringSplitOptions.None);
				}
				DismemberedPartData dismemberedPartData = DismembermentManager.readPart(array2);
				if (dismemberedPartData != null)
				{
					dismemberedPartData.propertyKey = text.Trim();
					dismemberedPartData.prefabPath = array[0].Trim();
					dismemberedPartData.isLinked = flag;
					return dismemberedPartData;
				}
			}
		}
		return null;
	}

	// Token: 0x060005AB RID: 1451 RVA: 0x000288B1 File Offset: 0x00026AB1
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool hasDismemberedPart(EnumBodyPartHit part, bool isBiped = true)
	{
		if (isBiped)
		{
			return DismembermentManager.BipedDismemberments.ContainsKey(part);
		}
		return DismembermentManager.QuadrupedDismemberments.ContainsKey(part);
	}

	// Token: 0x060005AC RID: 1452 RVA: 0x000288D0 File Offset: 0x00026AD0
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] getDismemberedPart(EnumBodyPartHit part, bool isBiped = true)
	{
		string[] result = null;
		if (isBiped)
		{
			DismembermentManager.BipedDismemberments.TryGetValue(part, out result);
		}
		else
		{
			DismembermentManager.QuadrupedDismemberments.TryGetValue(part, out result);
		}
		return result;
	}

	// Token: 0x060005AD RID: 1453 RVA: 0x00028904 File Offset: 0x00026B04
	[PublicizedFrom(EAccessModifier.Private)]
	public static void readString(string rawString, DismemberedPartData data)
	{
		string[] array = rawString.Split('=', StringSplitOptions.None);
		string text = array[0].Trim();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1412654217U)
		{
			if (num <= 845187144U)
			{
				if (num <= 660706664U)
				{
					if (num != 91623701U)
					{
						if (num == 660706664U)
						{
							if (text == "atp")
							{
								bool.TryParse(array[1], out data.attachToParent);
								return;
							}
						}
					}
					else if (text == "solscale")
					{
						string[] array2 = array[1].Split(',', StringSplitOptions.None);
						float.TryParse(array2[0], out data.solScale.x);
						float.TryParse(array2[1], out data.solScale.y);
						float.TryParse(array2[2], out data.solScale.z);
						data.hasSolScale = true;
						return;
					}
				}
				else if (num != 829615687U)
				{
					if (num == 845187144U)
					{
						if (text == "target")
						{
							data.targetBone = array[1].Trim();
							return;
						}
					}
				}
				else if (text == "asp")
				{
					data.addScalePoint = array[1].Trim();
					return;
				}
			}
			else if (num <= 1213057714U)
			{
				if (num != 1158553358U)
				{
					if (num == 1213057714U)
					{
						if (text == "detach")
						{
							bool.TryParse(array[1], out data.isDetachable);
							return;
						}
					}
				}
				else if (text == "rot")
				{
					string[] array3 = array[1].Split(',', StringSplitOptions.None);
					if (array3.Length != 3)
					{
						return;
					}
					Vector3 zero = Vector3.zero;
					float.TryParse(array3[0], out zero.x);
					float.TryParse(array3[1], out zero.y);
					float.TryParse(array3[2], out zero.z);
					if (zero != Vector3.zero)
					{
						data.SetRot(zero);
						return;
					}
					return;
				}
			}
			else if (num != 1325231910U)
			{
				if (num != 1361572173U)
				{
					if (num == 1412654217U)
					{
						if (text == "pos")
						{
							string[] array4 = array[1].Split(',', StringSplitOptions.None);
							float.TryParse(array4[0], out data.pos.x);
							float.TryParse(array4[1], out data.pos.y);
							float.TryParse(array4[2], out data.pos.z);
							return;
						}
					}
				}
				else if (text == "type")
				{
					string a = array[1].Trim();
					if (a == "blunt" || a == "blade" || a == "bullet" || a == "explosive")
					{
						data.damageTypeKey = array[1].Trim();
						return;
					}
					return;
				}
			}
			else if (text == "oset")
			{
				string[] array5 = array[1].Split(',', StringSplitOptions.None);
				float.TryParse(array5[0], out data.offset.x);
				float.TryParse(array5[1], out data.offset.y);
				float.TryParse(array5[2], out data.offset.z);
				return;
			}
		}
		else if (num <= 2631859207U)
		{
			if (num <= 2190941297U)
			{
				if (num != 2095122494U)
				{
					if (num == 2190941297U)
					{
						if (text == "scale")
						{
							string[] array6 = array[1].Split(',', StringSplitOptions.None);
							float.TryParse(array6[0], out data.scale.x);
							float.TryParse(array6[1], out data.scale.y);
							float.TryParse(array6[2], out data.scale.z);
							return;
						}
					}
				}
				else if (text == "ico")
				{
					data.childTargetObj = array[1].Trim();
					return;
				}
			}
			else if (num != 2531611380U)
			{
				if (num == 2631859207U)
				{
					if (text == "ibo")
					{
						data.insertBoneObj = array[1].Trim();
						return;
					}
				}
			}
			else if (text == "soltarget")
			{
				data.solTarget = array[1].Trim();
				return;
			}
		}
		else if (num <= 3716176457U)
		{
			if (num != 3592343918U)
			{
				if (num == 3716176457U)
				{
					if (text == "msb")
					{
						data.maskScaleBlend = array[1].Trim();
						return;
					}
				}
			}
			else if (text == "sfv")
			{
				data.setFixedValues = array[1].Trim();
				return;
			}
		}
		else if (num != 3740252708U)
		{
			if (num != 3795205537U)
			{
				if (num == 3883353449U)
				{
					if (text == "mask")
					{
						bool.TryParse(array[1], out data.useMask);
						return;
					}
				}
			}
			else if (text == "sol")
			{
				bool.TryParse(array[1], out data.scaleOutLimb);
				return;
			}
		}
		else if (text == "particles")
		{
			data.particlePaths = array[1].Split(',', StringSplitOptions.None);
			return;
		}
		data.Invalid = true;
		if (DismembermentManager.DebugLogEnabled)
		{
			Log.Warning("[{0}.readString] entityclasses.xml unknown key:{1} in raw:{2}", new object[]
			{
				"DismembermentManager",
				text,
				rawString
			});
		}
	}

	// Token: 0x060005AE RID: 1454 RVA: 0x00028E94 File Offset: 0x00027094
	[PublicizedFrom(EAccessModifier.Private)]
	public static DismemberedPartData readRandomPart(string[] prefabs, string[] data, string tag)
	{
		List<DismemberedPartData> list = new List<DismemberedPartData>();
		for (int i = 0; i < data.Length; i++)
		{
			DismemberedPartData dismemberedPartData = new DismemberedPartData();
			if (i < prefabs.Length)
			{
				string prefabPath = prefabs[i].Trim();
				dismemberedPartData.prefabPath = prefabPath;
			}
			if (data[i].Contains('+'.ToString()))
			{
				string[] array = data[i].Split('+', StringSplitOptions.None);
				for (int j = 0; j < array.Length; j++)
				{
					DismembermentManager.readString(array[j], dismemberedPartData);
				}
				if (!string.IsNullOrEmpty(dismemberedPartData.damageTypeKey) && dismemberedPartData.damageTypeKey == tag)
				{
					list.Add(dismemberedPartData);
				}
			}
			else
			{
				DismembermentManager.readString(data[i], dismemberedPartData);
				if (!string.IsNullOrEmpty(dismemberedPartData.damageTypeKey) && dismemberedPartData.damageTypeKey == tag)
				{
					list.Add(dismemberedPartData);
				}
			}
		}
		if (list.Count > 0)
		{
			int index = UnityEngine.Random.Range(0, list.Count);
			return list[index];
		}
		return null;
	}

	// Token: 0x060005AF RID: 1455 RVA: 0x00028F88 File Offset: 0x00027188
	[PublicizedFrom(EAccessModifier.Private)]
	public static DismemberedPartData readPart(string[] data)
	{
		List<DismemberedPartData> list = new List<DismemberedPartData>();
		for (int i = 0; i < data.Length; i++)
		{
			DismemberedPartData dismemberedPartData = new DismemberedPartData();
			string text = data[i];
			if (text.Contains('+'.ToString()))
			{
				string[] array = text.Split('+', StringSplitOptions.None);
				for (int j = 0; j < array.Length; j++)
				{
					DismembermentManager.readString(array[j], dismemberedPartData);
				}
				list.Add(dismemberedPartData);
			}
			else
			{
				DismembermentManager.readString(text, dismemberedPartData);
				list.Add(dismemberedPartData);
			}
		}
		if (list.Count > 0)
		{
			return list[0];
		}
		return null;
	}

	// Token: 0x060005B0 RID: 1456 RVA: 0x00029018 File Offset: 0x00027218
	public static void SpawnParticleEffect(ParticleEffect _pe, int _entityId = -1)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (!GameManager.IsDedicatedServer)
			{
				GameManager.Instance.SpawnParticleEffectClient(_pe, _entityId, false, true);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, false, false), false, -1, _entityId, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, false, false), false);
	}

	// Token: 0x060005B1 RID: 1457 RVA: 0x00029090 File Offset: 0x00027290
	public static void AddDebugArmObjects(Transform partT, Transform parentT)
	{
		if (partT && parentT && partT.name.ContainsCaseInsensitive("arm"))
		{
			GameObject gameObject = DataLoader.LoadAsset<GameObject>("@:Entities/Zombies/Gibs/Debug/debugAxisObj.prefab", false);
			if (gameObject)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
				gameObject2.transform.SetParent(parentT);
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				if (partT.name.ContainsCaseInsensitive("right"))
				{
					gameObject2.transform.localRotation = Quaternion.Euler(180f, 0f, 0f);
				}
				gameObject2.transform.localScale = Vector3.one * 0.5f;
				Transform transform = parentT.FindRecursive("rot");
				if (transform)
				{
					GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
					gameObject3.transform.SetParent(transform);
					gameObject3.transform.localPosition = Vector3.zero;
					gameObject3.transform.localRotation = Quaternion.identity;
					gameObject3.transform.localScale = Vector3.one * 0.33f;
					gameObject3.GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
				}
			}
		}
	}

	// Token: 0x040005D1 RID: 1489
	public const string cClassName = "DismembermentManager";

	// Token: 0x040005D2 RID: 1490
	public const string cManagedLimbsParentName = "DismemberedLimbs";

	// Token: 0x040005D3 RID: 1491
	public static bool DebugLogEnabled;

	// Token: 0x040005D4 RID: 1492
	public static bool DebugShowArmRotations;

	// Token: 0x040005D5 RID: 1493
	public static bool DebugDismemberExplosions;

	// Token: 0x040005D6 RID: 1494
	public static bool DebugBulletTime;

	// Token: 0x040005D7 RID: 1495
	public static bool DebugBloodParticles;

	// Token: 0x040005D8 RID: 1496
	public static bool DebugDontCreateParts;

	// Token: 0x040005D9 RID: 1497
	public static EnumBodyPartHit DebugBodyPartHit;

	// Token: 0x040005DA RID: 1498
	public static bool DebugUseLegacy;

	// Token: 0x040005DB RID: 1499
	public static bool DebugExplosiveCleanup;

	// Token: 0x040005DC RID: 1500
	public const string DestroyedCapRoot = "pos";

	// Token: 0x040005DD RID: 1501
	public const string PhysicsRootName = "Physics";

	// Token: 0x040005DE RID: 1502
	public const string DetachableRootName = "Detachable";

	// Token: 0x040005DF RID: 1503
	public const string cSubTagHeadAccessories = "HeadAccessories";

	// Token: 0x040005E0 RID: 1504
	public const string DetachableArmName = "HalfArm";

	// Token: 0x040005E1 RID: 1505
	public const string DetachableLegName = "HalfLeg";

	// Token: 0x040005E2 RID: 1506
	public const string ZombieSkinPrefix = "HD_";

	// Token: 0x040005E3 RID: 1507
	public const string MatPropRadiated = "_IsRadiated";

	// Token: 0x040005E4 RID: 1508
	public const string MatPropIrradiated = "_Irradiated";

	// Token: 0x040005E5 RID: 1509
	public const string MatPropFade = "_Fade";

	// Token: 0x040005E6 RID: 1510
	public static readonly FastTags<TagGroup.Global> radiatedTag = FastTags<TagGroup.Global>.Parse("radiated");

	// Token: 0x040005E7 RID: 1511
	public static readonly FastTags<TagGroup.Global> chargedTag = FastTags<TagGroup.Global>.Parse("charged");

	// Token: 0x040005E8 RID: 1512
	public static readonly FastTags<TagGroup.Global> infernalTag = FastTags<TagGroup.Global>.Parse("infernal");

	// Token: 0x040005E9 RID: 1513
	public static readonly FastTags<TagGroup.Global> radOrChargedTag = DismembermentManager.radiatedTag | DismembermentManager.chargedTag;

	// Token: 0x040005EA RID: 1514
	public static readonly FastTags<TagGroup.Global> specialTypeTags = DismembermentManager.radiatedTag | DismembermentManager.chargedTag | DismembermentManager.infernalTag;

	// Token: 0x040005EB RID: 1515
	public const string cCensorGoreSearch = "_CGore";

	// Token: 0x040005EC RID: 1516
	public static readonly List<string> BluntCensors = new List<string>
	{
		"zombieLab",
		"zombieUtilityWorker"
	};

	// Token: 0x040005ED RID: 1517
	public const string cAssetBundleZombies = "@:Entities/Zombies/";

	// Token: 0x040005EE RID: 1518
	public const string cAssetBundleSearchName = "Dismemberment";

	// Token: 0x040005EF RID: 1519
	public const string cAssetBundleFolder = "@:Entities/Zombies/";

	// Token: 0x040005F0 RID: 1520
	public const string cPrefabExt = ".prefab";

	// Token: 0x040005F1 RID: 1521
	public const string cLOD0 = "LOD0";

	// Token: 0x040005F2 RID: 1522
	public const string cLOD1 = "LOD1";

	// Token: 0x040005F3 RID: 1523
	public const string cLOD2 = "LOD2";

	// Token: 0x040005F4 RID: 1524
	public const string MatPropLeftLowerLeg = "_LeftLowerLeg";

	// Token: 0x040005F5 RID: 1525
	public const string MatPropRightLowerLeg = "_RightLowerLeg";

	// Token: 0x040005F6 RID: 1526
	public const string MatPropLeftUpperLeg = "_LeftUpperLeg";

	// Token: 0x040005F7 RID: 1527
	public const string MatPropRightUpperLeg = "_RightUpperLeg";

	// Token: 0x040005F8 RID: 1528
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cAssetBundleGibMats = "Common/Gibs/Materials";

	// Token: 0x040005F9 RID: 1529
	[PublicizedFrom(EAccessModifier.Private)]
	public const string CAssetBundleDefaultGib = "gib_dismemberment";

	// Token: 0x040005FA RID: 1530
	[PublicizedFrom(EAccessModifier.Private)]
	public const string CAssetBundleDefaultGibBlood = "gib_bloodcap";

	// Token: 0x040005FB RID: 1531
	public const string CAssetBundleDefaultGibChunk = "ZombieGibs_caps";

	// Token: 0x040005FC RID: 1532
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cDismemberMatXmlProp = "DismemberMaterial";

	// Token: 0x040005FD RID: 1533
	public static string[] DefaultBundleGibs = new string[]
	{
		"gib_dismemberment",
		"gib_bloodcap",
		"ZombieGibs_caps"
	};

	// Token: 0x040005FE RID: 1534
	public const string cGlobalMatName = "(global)";

	// Token: 0x040005FF RID: 1535
	public const string cLocalMatName = "(local)";

	// Token: 0x04000600 RID: 1536
	public const string cInstanceMatName = "(Instance)";

	// Token: 0x04000601 RID: 1537
	public const string cMatExt = ".mat";

	// Token: 0x04000602 RID: 1538
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cGibCapsMatPath = "@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps.mat";

	// Token: 0x04000603 RID: 1539
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cGibCapsMatRadPath = "@:Entities/Zombies/Common/Gibs/Materials/ZombieGibs_caps_IsRadiated.mat";

	// Token: 0x04000604 RID: 1540
	[PublicizedFrom(EAccessModifier.Private)]
	public Material zombieGibCapsMaterial;

	// Token: 0x04000605 RID: 1541
	[PublicizedFrom(EAccessModifier.Private)]
	public Material zombieGibCapsMaterialRadiated;

	// Token: 0x04000606 RID: 1542
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cDefaultMatBaseTex = "_ZombieColor";

	// Token: 0x04000607 RID: 1543
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cDefaultMatBaseTexAlt = "_Albedo";

	// Token: 0x04000608 RID: 1544
	public const float cDefaultDetachLimbLifeTime = 10f;

	// Token: 0x04000609 RID: 1545
	public const int cDefaultDetachLimbMax = 25;

	// Token: 0x0400060A RID: 1546
	public const int cDefaultDetachLimbCleanupCount = 5;

	// Token: 0x0400060B RID: 1547
	public const int cMaxLimbsFromExplosiveDeath = 3;

	// Token: 0x0400060C RID: 1548
	public List<DismemberedPart> parts = new List<DismemberedPart>();

	// Token: 0x0400060D RID: 1549
	[PublicizedFrom(EAccessModifier.Private)]
	public static DismembermentManager instance;

	// Token: 0x0400060E RID: 1550
	public const string cDynamicGore = "DynamicGore";

	// Token: 0x0400060F RID: 1551
	public static FastTags<TagGroup.Global> rangedTags = FastTags<TagGroup.Global>.Parse("ranged");

	// Token: 0x04000610 RID: 1552
	public static FastTags<TagGroup.Global> launcherTags = FastTags<TagGroup.Global>.Parse("launcher");

	// Token: 0x04000611 RID: 1553
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> shotgunTags = FastTags<TagGroup.Global>.Parse("shotgun");

	// Token: 0x04000612 RID: 1554
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> sledgeTags = FastTags<TagGroup.Global>.Parse("sledge");

	// Token: 0x04000613 RID: 1555
	[PublicizedFrom(EAccessModifier.Private)]
	public static FastTags<TagGroup.Global> knifeTags = FastTags<TagGroup.Global>.Parse("knife");

	// Token: 0x04000614 RID: 1556
	[PublicizedFrom(EAccessModifier.Private)]
	public const float MaxForce = 1.5f;

	// Token: 0x04000615 RID: 1557
	public const string cXmlTag = "DismemberTag_";

	// Token: 0x04000616 RID: 1558
	public const char cParamSplit = ';';

	// Token: 0x04000617 RID: 1559
	public const char cRawSplit = '+';

	// Token: 0x04000618 RID: 1560
	public const char cDataSplit = '=';

	// Token: 0x04000619 RID: 1561
	public const char cCommaDel = ',';

	// Token: 0x0400061A RID: 1562
	public static readonly Dictionary<EnumBodyPartHit, string[]> BipedDismemberments = new Dictionary<EnumBodyPartHit, string[]>
	{
		{
			EnumBodyPartHit.Head,
			new string[]
			{
				"DismemberTag_L_HeadGore",
				"Common/Dismemberment/HeadGore"
			}
		},
		{
			EnumBodyPartHit.LeftUpperLeg,
			new string[]
			{
				"DismemberTag_L_LeftUpperLegGore",
				"Common/Dismemberment/UpperLegGore"
			}
		},
		{
			EnumBodyPartHit.LeftLowerLeg,
			new string[]
			{
				"DismemberTag_L_LeftLowerLegGore",
				"Common/Dismemberment/LowerLegGore"
			}
		},
		{
			EnumBodyPartHit.RightUpperLeg,
			new string[]
			{
				"DismemberTag_L_RightUpperLegGore",
				"Common/Dismemberment/UpperLegGore"
			}
		},
		{
			EnumBodyPartHit.RightLowerLeg,
			new string[]
			{
				"DismemberTag_L_RightLowerLegGore",
				"Common/Dismemberment/LowerLegGore"
			}
		},
		{
			EnumBodyPartHit.LeftUpperArm,
			new string[]
			{
				"DismemberTag_L_LeftUpperArmGore",
				"Common/Dismemberment/UpperArmGore"
			}
		},
		{
			EnumBodyPartHit.LeftLowerArm,
			new string[]
			{
				"DismemberTag_L_LeftLowerArmGore",
				"Common/Dismemberment/LowerArmGore"
			}
		},
		{
			EnumBodyPartHit.RightUpperArm,
			new string[]
			{
				"DismemberTag_L_RightUpperArmGore",
				"Common/Dismemberment/UpperArmGore"
			}
		},
		{
			EnumBodyPartHit.RightLowerArm,
			new string[]
			{
				"DismemberTag_L_RightLowerArmGore",
				"Common/Dismemberment/LowerArmGore"
			}
		}
	};

	// Token: 0x0400061B RID: 1563
	public static readonly Dictionary<EnumBodyPartHit, string[]> QuadrupedDismemberments = new Dictionary<EnumBodyPartHit, string[]>
	{
		{
			EnumBodyPartHit.Head,
			new string[]
			{
				"DismemberTag_L_HeadGore",
				"Common/Dismemberment/HeadGore"
			}
		},
		{
			EnumBodyPartHit.LeftUpperLeg,
			new string[]
			{
				"DismemberTag_L_LeftUpperLegGore",
				"Common/Dismemberment/UpperLegGore"
			}
		},
		{
			EnumBodyPartHit.LeftLowerLeg,
			new string[]
			{
				"DismemberTag_L_LeftLowerLegGore",
				"Common/Dismemberment/LowerLegGore"
			}
		},
		{
			EnumBodyPartHit.RightUpperLeg,
			new string[]
			{
				"DismemberTag_L_RightUpperLegGore",
				"Common/Dismemberment/UpperLegGore"
			}
		},
		{
			EnumBodyPartHit.RightLowerLeg,
			new string[]
			{
				"DismemberTag_L_RightLowerLegGore",
				"Common/Dismemberment/LowerLegGore"
			}
		},
		{
			EnumBodyPartHit.LeftUpperArm,
			new string[]
			{
				"DismemberTag_L_LeftUpperArmGore",
				"Common/Dismemberment/UpperArmGore"
			}
		},
		{
			EnumBodyPartHit.LeftLowerArm,
			new string[]
			{
				"DismemberTag_L_LeftLowerArmGore",
				"Common/Dismemberment/LowerArmGore"
			}
		},
		{
			EnumBodyPartHit.RightUpperArm,
			new string[]
			{
				"DismemberTag_L_RightUpperArmGore",
				"Common/Dismemberment/UpperArmGore"
			}
		},
		{
			EnumBodyPartHit.RightLowerArm,
			new string[]
			{
				"DismemberTag_L_RightLowerArmGore",
				"Common/Dismemberment/LowerArmGore"
			}
		}
	};

	// Token: 0x0400061C RID: 1564
	[PublicizedFrom(EAccessModifier.Private)]
	public const string cDebugAxisPath = "@:Entities/Zombies/Gibs/Debug/debugAxisObj.prefab";

	// Token: 0x020000DE RID: 222
	public static class DamageKeys
	{
		// Token: 0x0400061D RID: 1565
		public const string blade = "blade";

		// Token: 0x0400061E RID: 1566
		public const string blunt = "blunt";

		// Token: 0x0400061F RID: 1567
		public const string bullet = "bullet";

		// Token: 0x04000620 RID: 1568
		public const string exlosive = "explosive";
	}

	// Token: 0x020000DF RID: 223
	public enum DamageTags
	{
		// Token: 0x04000622 RID: 1570
		none,
		// Token: 0x04000623 RID: 1571
		blade,
		// Token: 0x04000624 RID: 1572
		blunt,
		// Token: 0x04000625 RID: 1573
		any
	}

	// Token: 0x020000E0 RID: 224
	public static class ParseKeys
	{
		// Token: 0x04000626 RID: 1574
		public const string cType = "type";

		// Token: 0x04000627 RID: 1575
		public const string cTarget = "target";

		// Token: 0x04000628 RID: 1576
		public const string cAttachToParent = "atp";

		// Token: 0x04000629 RID: 1577
		public const string cDetach = "detach";

		// Token: 0x0400062A RID: 1578
		public const string cMask = "mask";

		// Token: 0x0400062B RID: 1579
		public const string cScaleOutLimb = "sol";

		// Token: 0x0400062C RID: 1580
		public const string cSolTarget = "soltarget";

		// Token: 0x0400062D RID: 1581
		public const string cSolScale = "solscale";

		// Token: 0x0400062E RID: 1582
		public const string cInsertChildObj = "ico";

		// Token: 0x0400062F RID: 1583
		public const string cInsertBoneObj = "ibo";

		// Token: 0x04000630 RID: 1584
		public const string cAddScalePoint = "asp";

		// Token: 0x04000631 RID: 1585
		public const string cMaskScaleBlend = "msb";

		// Token: 0x04000632 RID: 1586
		public const string cSetFixedValues = "sfv";
	}
}
