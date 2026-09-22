using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace Audio
{
	// Token: 0x02001B27 RID: 6951
	[Preserve]
	public class NetPackageAudio : NetPackageEntityTargeted
	{
		// Token: 0x0600D098 RID: 53400 RVA: 0x004C3DCE File Offset: 0x004C1FCE
		public NetPackageAudio Setup(int _playOnEntityId, string _soundGroupName, float _occlusion, bool _play, bool _signalOnly = false, float _volumeScale = 1f)
		{
			base.Setup(_playOnEntityId);
			this.playOnEntity = true;
			this.soundGroupName = _soundGroupName;
			this.play = _play;
			this.occlusion = _occlusion;
			this.signalOnly = _signalOnly;
			this.volumeScale = _volumeScale;
			return this;
		}

		// Token: 0x0600D099 RID: 53401 RVA: 0x004C3E05 File Offset: 0x004C2005
		public NetPackageAudio Setup(Vector3 _position, string _soundGroupName, float _occlusion, bool _play, int _entityId = -1, float _volumeScale = 1f)
		{
			base.Setup(_entityId);
			this.playOnEntity = false;
			this.position = _position;
			this.soundGroupName = _soundGroupName;
			this.play = _play;
			this.occlusion = _occlusion;
			this.volumeScale = _volumeScale;
			return this;
		}

		// Token: 0x0600D09A RID: 53402 RVA: 0x004C3E3C File Offset: 0x004C203C
		public override void read(PooledBinaryReader _reader)
		{
			base.read(_reader);
			this.soundGroupName = _reader.ReadString();
			this.play = _reader.ReadBoolean();
			float x = _reader.ReadSingle();
			float y = _reader.ReadSingle();
			float z = _reader.ReadSingle();
			this.position.x = x;
			this.position.y = y;
			this.position.z = z;
			this.playOnEntity = _reader.ReadBoolean();
			this.occlusion = _reader.ReadSingle();
			this.volumeScale = _reader.ReadSingle();
			this.signalOnly = _reader.ReadBoolean();
		}

		// Token: 0x0600D09B RID: 53403 RVA: 0x004C3ED4 File Offset: 0x004C20D4
		public override void write(PooledBinaryWriter _writer)
		{
			base.write(_writer);
			_writer.Write((this.soundGroupName != null) ? this.soundGroupName : "");
			_writer.Write(this.play);
			_writer.Write(this.position.x);
			_writer.Write(this.position.y);
			_writer.Write(this.position.z);
			_writer.Write(this.playOnEntity);
			_writer.Write(this.occlusion);
			_writer.Write(this.volumeScale);
			_writer.Write(this.signalOnly);
		}

		// Token: 0x0600D09C RID: 53404 RVA: 0x004C3F78 File Offset: 0x004C2178
		public override void ProcessPackage(World _world, GameManager _callbacks)
		{
			if (_world == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.soundGroupName))
			{
				return;
			}
			if (this.playOnEntity && this.entityId >= 0)
			{
				Entity entity = _world.GetEntity(this.entityId);
				if (entity == null)
				{
					return;
				}
				if (GameManager.IsDedicatedServer && Manager.ServerAudio != null)
				{
					if (this.play)
					{
						Manager.ServerAudio.Play(entity, this.soundGroupName, this.occlusion, this.signalOnly, this.volumeScale);
						return;
					}
					Manager.ServerAudio.Stop(this.entityId, this.soundGroupName);
					return;
				}
				else if (!GameManager.IsDedicatedServer && Manager.ServerAudio != null)
				{
					if (this.play)
					{
						Manager.Play(entity, this.soundGroupName, this.volumeScale, false);
						Manager.ServerAudio.Play(entity, this.soundGroupName, this.occlusion, this.signalOnly, this.volumeScale);
						return;
					}
					Manager.Stop(this.entityId, this.soundGroupName);
					Manager.ServerAudio.Stop(this.entityId, this.soundGroupName);
					return;
				}
				else if (Manager.ServerAudio == null)
				{
					if (!this.play)
					{
						Manager.Stop(this.entityId, this.soundGroupName);
						return;
					}
					if (!this.signalOnly)
					{
						Manager.Play(entity, this.soundGroupName, this.volumeScale, false);
						return;
					}
				}
			}
			else if (GameManager.IsDedicatedServer && Manager.ServerAudio != null)
			{
				if (this.play)
				{
					Manager.ServerAudio.Play(this.position, this.soundGroupName, this.occlusion, this.entityId, this.volumeScale);
					return;
				}
				Manager.ServerAudio.Stop(this.position, this.soundGroupName);
				return;
			}
			else if (!GameManager.IsDedicatedServer && Manager.ServerAudio != null)
			{
				if (this.play)
				{
					Manager.Play(this.position, this.soundGroupName, this.entityId, false, this.volumeScale);
					Manager.ServerAudio.Play(this.position, this.soundGroupName, this.occlusion, this.entityId, this.volumeScale);
					return;
				}
				Manager.Stop(this.position, this.soundGroupName);
				Manager.ServerAudio.Stop(this.position, this.soundGroupName);
				return;
			}
			else if (Manager.ServerAudio == null)
			{
				if (this.play)
				{
					Manager.Play(this.position, this.soundGroupName, this.entityId, false, this.volumeScale);
					return;
				}
				Manager.Stop(this.position, this.soundGroupName);
			}
		}

		// Token: 0x0600D09D RID: 53405 RVA: 0x00081502 File Offset: 0x0007F702
		public override int GetLength()
		{
			return 10;
		}

		// Token: 0x04009F3B RID: 40763
		public string soundGroupName;

		// Token: 0x04009F3C RID: 40764
		public bool play;

		// Token: 0x04009F3D RID: 40765
		public Vector3 position;

		// Token: 0x04009F3E RID: 40766
		public bool playOnEntity;

		// Token: 0x04009F3F RID: 40767
		public float occlusion;

		// Token: 0x04009F40 RID: 40768
		public bool signalOnly;

		// Token: 0x04009F41 RID: 40769
		public float volumeScale;
	}
}
