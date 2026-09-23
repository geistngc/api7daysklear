using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200133F RID: 4927
public class GameLightManager
{
	// Token: 0x06009B9A RID: 39834 RVA: 0x003AC15F File Offset: 0x003AA35F
	public static GameLightManager Create(EntityPlayerLocal player)
	{
		GameLightManager gameLightManager = new GameLightManager();
		gameLightManager.player = player;
		gameLightManager.Init();
		return gameLightManager;
	}

	// Token: 0x06009B9B RID: 39835 RVA: 0x003AC173 File Offset: 0x003AA373
	[PublicizedFrom(EAccessModifier.Private)]
	public void Init()
	{
		GameLightManager.Instance = this;
		this.UpdateLightInit();
	}

	// Token: 0x06009B9C RID: 39836 RVA: 0x003AC181 File Offset: 0x003AA381
	public void Destroy()
	{
		this.lights.Clear();
		this.priorityLights.Clear();
		this.removeLights.Clear();
		this.UpdateLightCleanup();
		GameLightManager.Instance = null;
	}

	// Token: 0x06009B9D RID: 39837 RVA: 0x003AC1B0 File Offset: 0x003AA3B0
	public void FrameUpdate()
	{
		if (GameManager.IsDedicatedServer)
		{
			return;
		}
		if (GameManager.Instance.World == null)
		{
			return;
		}
		if (MeshDescription.bDebugStability || LightViewer.IsAllOff)
		{
			return;
		}
		this.isUpdating = true;
		Vector3 position = this.player.cameraTransform.position;
		int count = this.lights.Count;
		int num = (count + 19) / 20;
		if (this.lightUpdateIndex >= count)
		{
			this.lightUpdateIndex = 0;
		}
		for (int i = 0; i < num; i++)
		{
			LightLOD lightLOD = this.lights[this.lightUpdateIndex];
			if (lightLOD.priority <= 0f)
			{
				lightLOD.FrameUpdate(position);
				if (lightLOD.priority > 0f)
				{
					this.priorityLights.Add(lightLOD);
				}
			}
			int num2 = this.lightUpdateIndex + 1;
			this.lightUpdateIndex = num2;
			if (num2 >= count)
			{
				this.lightUpdateIndex = 0;
			}
		}
		int j = 0;
		while (j < this.priorityLights.Count)
		{
			LightLOD lightLOD2 = this.priorityLights[j];
			lightLOD2.FrameUpdate(position);
			if (lightLOD2.priority <= 0f)
			{
				this.priorityLights.RemoveAt(j);
			}
			else
			{
				j++;
			}
		}
		int count2 = this.removeLights.Count;
		if (count2 > 0)
		{
			for (int k = count2 - 1; k >= 0; k--)
			{
				LightLOD lightLOD3 = this.removeLights[k];
				this.RemoveLightFromLists(lightLOD3);
			}
			this.removeLights.Clear();
		}
		if (this.isWaterLevelChanged)
		{
			this.isWaterLevelChanged = false;
			foreach (LightLOD lightLOD4 in this.lights)
			{
				if (!lightLOD4.bWorksUnderwater)
				{
					lightLOD4.WaterLevelDirty = true;
				}
			}
		}
		this.isUpdating = false;
		this.UpdateLightFrameUpdate();
	}

	// Token: 0x06009B9E RID: 39838 RVA: 0x003AC390 File Offset: 0x003AA590
	public void AddLight(LightLOD lightLOD)
	{
		this.lights.Add(lightLOD);
		lightLOD.priority = 1f;
		this.priorityLights.Add(lightLOD);
		if (OcclusionManager.Instance.cullLights)
		{
			OcclusionManager.AddLight(lightLOD);
		}
	}

	// Token: 0x06009B9F RID: 39839 RVA: 0x003AC3C7 File Offset: 0x003AA5C7
	public void RemoveLight(LightLOD lightLOD)
	{
		if (this.isUpdating)
		{
			this.removeLights.Add(lightLOD);
		}
		else
		{
			this.RemoveLightFromLists(lightLOD);
		}
		if (OcclusionManager.Instance.cullLights)
		{
			OcclusionManager.RemoveLight(lightLOD);
		}
	}

	// Token: 0x06009BA0 RID: 39840 RVA: 0x003AC3F8 File Offset: 0x003AA5F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void RemoveLightFromLists(LightLOD lightLOD)
	{
		int num = this.lights.IndexOf(lightLOD);
		if (num < 0)
		{
			Log.Warning("RemoveLightFromLists none");
			return;
		}
		this.lights.RemoveAt(num);
		if (num < this.lightUpdateIndex)
		{
			this.lightUpdateIndex--;
		}
		this.priorityLights.Remove(lightLOD);
	}

	// Token: 0x06009BA1 RID: 39841 RVA: 0x003AC451 File Offset: 0x003AA651
	public void MakeLightAPriority(LightLOD lightLOD)
	{
		if (lightLOD.priority <= 0f)
		{
			lightLOD.priority = 1f;
			this.priorityLights.Add(lightLOD);
		}
	}

	// Token: 0x06009BA2 RID: 39842 RVA: 0x003AC477 File Offset: 0x003AA677
	public Vector3 CameraPos()
	{
		return this.player.cameraTransform.position;
	}

	// Token: 0x06009BA3 RID: 39843 RVA: 0x003AC489 File Offset: 0x003AA689
	public void HandleWaterLevelChanged()
	{
		this.isWaterLevelChanged = true;
	}

	// Token: 0x06009BA4 RID: 39844 RVA: 0x003AC494 File Offset: 0x003AA694
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLightInit()
	{
		for (int i = 0; i < this.fastULs.Length; i++)
		{
			this.fastULs[i] = new List<UpdateLight>(64);
		}
		for (int j = 0; j < this.slowULs.Length; j++)
		{
			this.slowULs[j] = new List<UpdateLight>(256);
		}
	}

	// Token: 0x06009BA5 RID: 39845 RVA: 0x003AC4E8 File Offset: 0x003AA6E8
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLightCleanup()
	{
		this.newULs.Clear();
		for (int i = 0; i < this.fastULs.Length; i++)
		{
			this.fastULs[i] = null;
		}
		for (int j = 0; j < this.slowULs.Length; j++)
		{
			this.slowULs[j] = null;
		}
	}

	// Token: 0x06009BA6 RID: 39846 RVA: 0x003AC538 File Offset: 0x003AA738
	public void AddUpdateLight(UpdateLight _ul)
	{
		this.newULs.Add(_ul);
	}

	// Token: 0x06009BA7 RID: 39847 RVA: 0x003AC548 File Offset: 0x003AA748
	public void RemoveUpdateLight(UpdateLight _ul)
	{
		bool flag;
		if (_ul.IsDynamicObject)
		{
			int num = _ul.GetHashCode() >> 2 & 3;
			flag = this.fastULs[num].Remove(_ul);
		}
		else
		{
			int num2 = _ul.GetHashCode() >> 2 & 63;
			flag = this.slowULs[num2].Remove(_ul);
		}
		if (!flag && !this.newULs.Remove(_ul))
		{
			Log.Warning("RemoveUpdateLight {0} dy{1} missing!", new object[]
			{
				_ul.transform.name,
				_ul.IsDynamicObject
			});
		}
	}

	// Token: 0x06009BA8 RID: 39848 RVA: 0x003AC5D4 File Offset: 0x003AA7D4
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateLightFrameUpdate()
	{
		if (GameManager.Instance == null || GameManager.Instance.World == null || !GameManager.Instance.gameStateManager.IsGameStarted())
		{
			return;
		}
		float step = Time.deltaTime * 4f;
		this.fastULUpdateIndex = (this.fastULUpdateIndex + 1 & 3);
		List<UpdateLight> list = this.fastULs[this.fastULUpdateIndex];
		for (int i = list.Count - 1; i >= 0; i--)
		{
			UpdateLight updateLight = list[i];
			if (updateLight)
			{
				updateLight.UpdateLighting(step);
			}
			else
			{
				list.RemoveAt(i);
			}
		}
		this.slowULUpdateIndex = (this.slowULUpdateIndex + 1 & 63);
		List<UpdateLight> list2 = this.slowULs[this.slowULUpdateIndex];
		for (int j = list2.Count - 1; j >= 0; j--)
		{
			UpdateLight updateLight2 = list2[j];
			if (updateLight2)
			{
				if (updateLight2.appliedLit < 0f)
				{
					updateLight2.UpdateLighting(1f);
				}
			}
			else
			{
				list2.RemoveAt(j);
			}
		}
		int num = Utils.FastMin(160, this.newULs.Count);
		for (int k = 0; k < num; k++)
		{
			UpdateLight updateLight3 = this.newULs[k];
			if (updateLight3)
			{
				updateLight3.ManagerFirstUpdate();
				int num2 = updateLight3.GetHashCode() >> 2;
				if (updateLight3.IsDynamicObject)
				{
					this.fastULs[num2 & 3].Add(updateLight3);
				}
				else
				{
					this.slowULs[num2 & 63].Add(updateLight3);
				}
			}
		}
		this.newULs.RemoveRange(0, num);
	}

	// Token: 0x04007550 RID: 30032
	public static GameLightManager Instance;

	// Token: 0x04007551 RID: 30033
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal player;

	// Token: 0x04007552 RID: 30034
	[PublicizedFrom(EAccessModifier.Private)]
	public List<LightLOD> lights = new List<LightLOD>();

	// Token: 0x04007553 RID: 30035
	[PublicizedFrom(EAccessModifier.Private)]
	public List<LightLOD> priorityLights = new List<LightLOD>();

	// Token: 0x04007554 RID: 30036
	[PublicizedFrom(EAccessModifier.Private)]
	public List<LightLOD> removeLights = new List<LightLOD>();

	// Token: 0x04007555 RID: 30037
	[PublicizedFrom(EAccessModifier.Private)]
	public int lightUpdateIndex;

	// Token: 0x04007556 RID: 30038
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isUpdating;

	// Token: 0x04007557 RID: 30039
	[PublicizedFrom(EAccessModifier.Private)]
	public volatile bool isWaterLevelChanged;

	// Token: 0x04007558 RID: 30040
	[PublicizedFrom(EAccessModifier.Private)]
	public List<UpdateLight> newULs = new List<UpdateLight>(512);

	// Token: 0x04007559 RID: 30041
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFastULGroups = 4;

	// Token: 0x0400755A RID: 30042
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cFastULGroupMask = 3;

	// Token: 0x0400755B RID: 30043
	[PublicizedFrom(EAccessModifier.Private)]
	public List<UpdateLight>[] fastULs = new List<UpdateLight>[4];

	// Token: 0x0400755C RID: 30044
	[PublicizedFrom(EAccessModifier.Private)]
	public int fastULUpdateIndex;

	// Token: 0x0400755D RID: 30045
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cULGroups = 64;

	// Token: 0x0400755E RID: 30046
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cSlowULGroupMask = 63;

	// Token: 0x0400755F RID: 30047
	[PublicizedFrom(EAccessModifier.Private)]
	public List<UpdateLight>[] slowULs = new List<UpdateLight>[64];

	// Token: 0x04007560 RID: 30048
	[PublicizedFrom(EAccessModifier.Private)]
	public int slowULUpdateIndex;
}
