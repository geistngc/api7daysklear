using System;
using UnityEngine;

// Token: 0x0200038C RID: 908
public class DynamicObserver
{
	// Token: 0x06001B1C RID: 6940 RVA: 0x000A1174 File Offset: 0x0009F374
	public void Start(Vector3 pos)
	{
		this.Position = pos;
		if (GameManager.Instance.World == null)
		{
			return;
		}
		if (this.Observer == null)
		{
			this.Observer = GameManager.Instance.AddChunkObserver(pos, false, DynamicObserver.ViewSize, GameManager.Instance.World.GetPrimaryPlayerId());
		}
		this.Observer.SetPosition(this.Position);
		this.StopTime = float.MaxValue;
	}

	// Token: 0x06001B1D RID: 6941 RVA: 0x000A11E0 File Offset: 0x0009F3E0
	public bool ContainsPoint(Vector3i pos)
	{
		int num = DynamicObserver.ViewSize * 16;
		return (float)pos.x >= this.Position.x - (float)num && (float)pos.x <= this.Position.x + (float)num && (float)pos.z >= this.Position.z - (float)num && (float)pos.z <= this.Position.z + (float)num;
	}

	// Token: 0x06001B1E RID: 6942 RVA: 0x000A1258 File Offset: 0x0009F458
	public bool HasFallingBlocks()
	{
		foreach (long chunkKey in this.Observer.chunksLoaded)
		{
			if (GameManager.Instance == null)
			{
				return false;
			}
			if (GameManager.Instance.World == null)
			{
				return false;
			}
			Chunk chunk = (Chunk)GameManager.Instance.World.GetChunkSync(chunkKey);
			if (chunk == null)
			{
				DynamicMeshManager.LogMsg("Observer couldn't load chunk so assuming falling");
				return true;
			}
			if (chunk.HasFallingBlocks())
			{
				DynamicMeshManager.Instance.AddUpdateData(chunk.Key, false, true, true, 3);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06001B1F RID: 6943 RVA: 0x000A1318 File Offset: 0x0009F518
	public void Stop()
	{
		if (this.Observer == null)
		{
			return;
		}
		try
		{
			GameManager.Instance.RemoveChunkObserver(this.Observer);
		}
		catch (Exception ex)
		{
			if (DynamicMeshManager.DoLog)
			{
				DynamicMeshManager.LogMsg("Observer already destroyed: " + ex.Message);
			}
		}
		this.Observer = null;
	}

	// Token: 0x0400116D RID: 4461
	public static int ViewSize = 3;

	// Token: 0x0400116E RID: 4462
	public Vector3 Position;

	// Token: 0x0400116F RID: 4463
	public ChunkManager.ChunkObserver Observer;

	// Token: 0x04001170 RID: 4464
	public float StopTime = float.MaxValue;
}
