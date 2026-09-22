using System;
using System.Threading;

// Token: 0x02000399 RID: 921
public class DynamicMeshRegionBuilder
{
	// Token: 0x17000345 RID: 837
	// (get) Token: 0x06001B82 RID: 7042 RVA: 0x000A338B File Offset: 0x000A158B
	public static World world
	{
		get
		{
			return GameManager.Instance.World;
		}
	}

	// Token: 0x06001B83 RID: 7043 RVA: 0x000A3397 File Offset: 0x000A1597
	public bool AddNewItem(DynamicMeshRegion region)
	{
		if (this.Status != DynamicMeshBuilderStatus.Ready)
		{
			Log.Warning("Builder thread tried to start when not ready. Current Status: " + this.Status.ToString());
			return false;
		}
		this.Region = region;
		this.Status = DynamicMeshBuilderStatus.StartingExport;
		return true;
	}

	// Token: 0x06001B84 RID: 7044 RVA: 0x000A33D4 File Offset: 0x000A15D4
	public void RequestStop(bool forceStop = false)
	{
		this.StopRequested = true;
		if (forceStop)
		{
			try
			{
				Thread thread = this.thread;
				if (thread != null)
				{
					thread.Abort();
				}
			}
			catch (Exception)
			{
			}
			this.Status = DynamicMeshBuilderStatus.Stopped;
		}
	}

	// Token: 0x06001B85 RID: 7045 RVA: 0x000A3418 File Offset: 0x000A1618
	public void StartThread()
	{
		this.thread = new Thread(delegate()
		{
			try
			{
				while (!this.StopRequested)
				{
					if (GameManager.Instance == null)
					{
						return;
					}
					if (GameManager.Instance.World == null)
					{
						return;
					}
					if (this.Status == DynamicMeshBuilderStatus.Ready || this.Status == DynamicMeshBuilderStatus.Complete)
					{
						Thread.Sleep(100);
					}
					else
					{
						if (this.Status != DynamicMeshBuilderStatus.StartingExport)
						{
							Log.Error("Builder thread and wrong state: " + this.Status.ToString());
							this.Status = DynamicMeshBuilderStatus.Error;
							return;
						}
						throw new NotImplementedException("No build method");
					}
				}
			}
			catch (Exception ex)
			{
				this.Error = "Builder error: " + ex.Message;
				Log.Error(this.Error);
			}
			this.Status = DynamicMeshBuilderStatus.Stopped;
		});
		this.thread.Start();
	}

	// Token: 0x040011B1 RID: 4529
	public string Error;

	// Token: 0x040011B2 RID: 4530
	public bool StopRequested;

	// Token: 0x040011B3 RID: 4531
	public DynamicMeshBuilderStatus Status;

	// Token: 0x040011B4 RID: 4532
	public ExportMeshResult Result = ExportMeshResult.Missing;

	// Token: 0x040011B5 RID: 4533
	public DynamicMeshRegion Region;

	// Token: 0x040011B6 RID: 4534
	[PublicizedFrom(EAccessModifier.Private)]
	public Thread thread;
}
