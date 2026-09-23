using System;

// Token: 0x020003CF RID: 975
public static class FireControllerUtils
{
	// Token: 0x06001D77 RID: 7543 RVA: 0x000B208C File Offset: 0x000B028C
	public static void SpawnParticleEffect(ParticleEffect _pe, int _entityId)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			if (!GameManager.IsDedicatedServer)
			{
				GameManager.Instance.SpawnParticleEffectClient(_pe, _entityId, false, true);
			}
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, false, true), false, -1, _entityId, -1, null, 192, false);
			return;
		}
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageParticleEffect>().Setup(_pe, _entityId, false, true), false);
	}
}
