using System;
using Unity.Jobs;
using UnityEngine;

// Token: 0x02001D98 RID: 7576
[DOTSCompilerGenerated]
[PublicizedFrom(EAccessModifier.Internal)]
public class __JobReflectionRegistrationOutput__2539302986
{
	// Token: 0x0600DE8D RID: 56973 RVA: 0x004FDEB0 File Offset: 0x004FC0B0
	public static void CreateJobReflectionData()
	{
		try
		{
			IJobParallelForExtensions.EarlyJobInit<WaterSimulationApplyFlows>();
			IJobParallelForExtensions.EarlyJobInit<WaterSimulationCalcFlows>();
			IJobExtensions.EarlyJobInit<WaterSimulationPostProcess>();
			IJobExtensions.EarlyJobInit<WaterSimulationPreProcess>();
		}
		catch (Exception ex)
		{
			EarlyInitHelpers.JobReflectionDataCreationFailed(ex);
		}
	}

	// Token: 0x0600DE8E RID: 56974 RVA: 0x004FDEF4 File Offset: 0x004FC0F4
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
	public static void EarlyInit()
	{
		__JobReflectionRegistrationOutput__2539302986.CreateJobReflectionData();
	}
}
