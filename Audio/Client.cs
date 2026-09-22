using System;
using UnityEngine;

namespace Audio
{
	// Token: 0x02001B29 RID: 6953
	public class Client : IDisposable
	{
		// Token: 0x0600D0A8 RID: 53416 RVA: 0x004C4534 File Offset: 0x004C2734
		public Client(int _entityId)
		{
			this.entityId = _entityId;
		}

		// Token: 0x0600D0A9 RID: 53417 RVA: 0x000027FC File Offset: 0x000009FC
		public void Dispose()
		{
		}

		// Token: 0x0600D0AA RID: 53418 RVA: 0x004C4544 File Offset: 0x004C2744
		public void Play(int playOnEntityId, string soundGroupName, float occlusion, float volumeScale = 1f)
		{
			NetPackageAudio package = NetPackageManager.GetPackage<NetPackageAudio>().Setup(playOnEntityId, soundGroupName, occlusion, true, false, volumeScale);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x0600D0AB RID: 53419 RVA: 0x004C4588 File Offset: 0x004C2788
		public void Play(Vector3 position, string soundGroupName, float occlusion, int entityId = -1, float volumeScale = 1f)
		{
			NetPackageAudio package = NetPackageManager.GetPackage<NetPackageAudio>().Setup(position, soundGroupName, occlusion, true, entityId, volumeScale);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x0600D0AC RID: 53420 RVA: 0x004C45CC File Offset: 0x004C27CC
		public void Stop(int stopOnEntityId, string soundGroupName)
		{
			NetPackageAudio package = NetPackageManager.GetPackage<NetPackageAudio>().Setup(stopOnEntityId, soundGroupName, 0f, false, false, 1f);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x0600D0AD RID: 53421 RVA: 0x004C4618 File Offset: 0x004C2818
		public void Stop(Vector3 position, string soundGroupName)
		{
			NetPackageAudio package = NetPackageManager.GetPackage<NetPackageAudio>().Setup(position, soundGroupName, 0f, false, -1, 1f);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x04009F44 RID: 40772
		public int entityId;
	}
}
