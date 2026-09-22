using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000241 RID: 577
[Preserve]
public class ConsoleCmdMapData : ConsoleCmdAbstract
{
	// Token: 0x0600115A RID: 4442 RVA: 0x0006DFE4 File Offset: 0x0006C1E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"mapdata"
		};
	}

	// Token: 0x0600115B RID: 4443 RVA: 0x0006DFF4 File Offset: 0x0006C1F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		return "Usage:\nclear - reset player map data\nprefab - save prefabs to mapdata.png\nstart - save start points to mapdata.png";
	}

	// Token: 0x0600115C RID: 4444 RVA: 0x0006DFFB File Offset: 0x0006C1FB
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Writes some map data to an image";
	}

	// Token: 0x0600115D RID: 4445 RVA: 0x0006E004 File Offset: 0x0006C204
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		if (_params.Count == 0)
		{
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output(this.getHelp());
			return;
		}
		string a = _params[0].ToLower();
		if (a == "clear")
		{
			GameManager.Instance.World.GetPrimaryPlayer().ChunkObserver.mapDatabase.Clear();
			SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Map cleared");
			return;
		}
		ConsoleCmdMapData.EMode emode = ConsoleCmdMapData.EMode.Prefabs;
		if (a == "start")
		{
			emode = ConsoleCmdMapData.EMode.StartPoints;
		}
		IChunkProvider chunkProvider = GameManager.Instance.World.ChunkCache.ChunkProvider;
		Vector2i worldSize = chunkProvider.GetWorldSize();
		Texture2D texture2D = new Texture2D(worldSize.x, worldSize.y);
		Color[] pixels = texture2D.GetPixels();
		for (int i = 0; i < pixels.Length; i++)
		{
			pixels[i] = new Color(0f, 0f, 0f, 0f);
		}
		if (emode == ConsoleCmdMapData.EMode.Prefabs)
		{
			DynamicPrefabDecorator dynamicPrefabDecorator = chunkProvider.GetDynamicPrefabDecorator();
			if (dynamicPrefabDecorator == null)
			{
				return;
			}
			List<PrefabInstance> list = new List<PrefabInstance>();
			dynamicPrefabDecorator.GetAllPrefabs(list);
			if (list == null)
			{
				return;
			}
			foreach (PrefabInstance prefabInstance in list)
			{
				this.setRect(worldSize, prefabInstance.boundingBoxPosition, prefabInstance.boundingBoxSize, pixels, ConsoleCmdMapData.EColorChannel.Green);
			}
		}
		if (emode == ConsoleCmdMapData.EMode.StartPoints)
		{
			SpawnPointList spawnPointList = chunkProvider.GetSpawnPointList();
			if (spawnPointList == null)
			{
				return;
			}
			foreach (SpawnPoint spawnPoint in spawnPointList)
			{
				this.setRect(worldSize, new Vector3i(spawnPoint.spawnPosition.position) - new Vector3i(3, 0, 3), new Vector3i(7, 0, 7), pixels, ConsoleCmdMapData.EColorChannel.Blue);
			}
		}
		texture2D.SetPixels(pixels);
		texture2D.Apply();
		TextureUtils.SaveTexture(texture2D, "mapdata.png");
		UnityEngine.Object.Destroy(texture2D);
		SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Saved mapdata.png and put " + emode.ToStringCached<ConsoleCmdMapData.EMode>() + " into it");
	}

	// Token: 0x0600115E RID: 4446 RVA: 0x0006E228 File Offset: 0x0006C428
	[PublicizedFrom(EAccessModifier.Private)]
	public void setRect(Vector2i _worldSize, Vector3i _pos, Vector3i _size, Color[] _cols, ConsoleCmdMapData.EColorChannel _channel)
	{
		int num = _worldSize.x / 2 + _pos.x;
		int num2 = _worldSize.y / 2 + _pos.z;
		for (int i = 0; i < _size.x; i++)
		{
			for (int j = 0; j < _size.z; j++)
			{
				int num3 = i + num + (j + num2) * _worldSize.x;
				if (num3 >= 0 && num3 < _cols.Length)
				{
					Color color = _cols[num3];
					switch (_channel)
					{
					case ConsoleCmdMapData.EColorChannel.Red:
						color.r = 1f;
						color.a = 1f;
						break;
					case ConsoleCmdMapData.EColorChannel.Green:
						color.g = 1f;
						color.a = 1f;
						break;
					case ConsoleCmdMapData.EColorChannel.Blue:
						color.b = 1f;
						color.a = 1f;
						break;
					}
					_cols[num3] = color;
				}
			}
		}
	}

	// Token: 0x02000242 RID: 578
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EMode
	{
		// Token: 0x04000CDA RID: 3290
		Prefabs,
		// Token: 0x04000CDB RID: 3291
		StartPoints
	}

	// Token: 0x02000243 RID: 579
	[PublicizedFrom(EAccessModifier.Private)]
	public enum EColorChannel
	{
		// Token: 0x04000CDD RID: 3293
		Red,
		// Token: 0x04000CDE RID: 3294
		Green,
		// Token: 0x04000CDF RID: 3295
		Blue
	}
}
