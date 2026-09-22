using System;
using System.Collections;
using System.Collections.Generic;
using DynamicMusic.Factories;
using MusicUtils.Enums;
using UniLinq;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001A3B RID: 6715
	[Preserve]
	public class Conductor : IUpdatable, ICleanable
	{
		// Token: 0x170018E4 RID: 6372
		// (get) Token: 0x0600CBC1 RID: 52161 RVA: 0x004AA015 File Offset: 0x004A8215
		// (set) Token: 0x0600CBC2 RID: 52162 RVA: 0x004AA01D File Offset: 0x004A821D
		public SectionType CurrentSectionType { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600CBC3 RID: 52163 RVA: 0x004AA026 File Offset: 0x004A8226
		public Conductor()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			MixerController.Instance.Init();
		}

		// Token: 0x0600CBC4 RID: 52164 RVA: 0x004AA040 File Offset: 0x004A8240
		public void Init(bool ReadyImmediate = false)
		{
			this.world = GameManager.Instance.World;
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer && this.PlayerEligibleForBloodmoonCache == null)
			{
				this.PlayerEligibleForBloodmoonCache = new Dictionary<int, bool>();
				ConnectionManager.OnClientDisconnected += this.OnClientDisconnected;
				Log.Out("Dynamic Music Initialized on Server");
			}
			if (GameManager.IsDedicatedServer)
			{
				Log.Out("Dynamic Music Initialized on Dedi");
				return;
			}
			LayeredContent.ReadyQueuesImmediate();
			this.sections = new EnumDictionary<SectionType, ISection>
			{
				{
					SectionType.Exploration,
					Factory.CreateSection<Adventure>(SectionType.Exploration)
				},
				{
					SectionType.Suspense,
					Factory.CreateSection<Adventure>(SectionType.Suspense)
				},
				{
					SectionType.Combat,
					Factory.CreateSection<Combat>(SectionType.Combat)
				},
				{
					SectionType.Bloodmoon,
					Factory.CreateSection<Bloodmoon>(SectionType.Bloodmoon)
				},
				{
					SectionType.HomeDay,
					Factory.CreateSection<Song>(SectionType.HomeDay)
				},
				{
					SectionType.HomeNight,
					Factory.CreateSection<Song>(SectionType.HomeNight)
				},
				{
					SectionType.TraderBob,
					Factory.CreateSection<Song>(SectionType.TraderBob)
				},
				{
					SectionType.TraderHugh,
					Factory.CreateSection<Song>(SectionType.TraderHugh)
				},
				{
					SectionType.TraderJen,
					Factory.CreateSection<Song>(SectionType.TraderJen)
				},
				{
					SectionType.TraderJoel,
					Factory.CreateSection<Song>(SectionType.TraderJoel)
				},
				{
					SectionType.TraderRekt,
					Factory.CreateSection<Song>(SectionType.TraderRekt)
				}
			};
			this.sectionSelector = Factory.CreateSectionSelector();
			this.wasMusicPlaying = false;
			Log.Out("Dynamic Music Initialized on Client");
		}

		// Token: 0x170018E5 RID: 6373
		// (get) Token: 0x0600CBC5 RID: 52165 RVA: 0x004AA16C File Offset: 0x004A836C
		public bool IsMusicPlaying
		{
			get
			{
				if (this.sections != null)
				{
					using (Dictionary<SectionType, ISection>.ValueCollection.Enumerator enumerator = this.sections.Values.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							if (enumerator.Current.IsPlaying)
							{
								return true;
							}
						}
					}
					return false;
				}
				return false;
			}
		}

		// Token: 0x0600CBC6 RID: 52166 RVA: 0x004AA1D4 File Offset: 0x004A83D4
		public IEnumerator PreloadRoutine()
		{
			Log.Out("Begin DMS Conductor Preload Routine");
			foreach (KeyValuePair<SectionType, ISection> keyValuePair in this.sections)
			{
				Section section = keyValuePair.Value as Section;
				if (section != null)
				{
					yield return section.PreloadRoutine();
				}
			}
			Dictionary<SectionType, ISection>.Enumerator enumerator = default(Dictionary<SectionType, ISection>.Enumerator);
			yield break;
			yield break;
		}

		// Token: 0x0600CBC7 RID: 52167 RVA: 0x004AA1E4 File Offset: 0x004A83E4
		public void Update()
		{
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				Dictionary<int, bool> playerEligibleForBloodmoonCache = this.PlayerEligibleForBloodmoonCache;
				lock (playerEligibleForBloodmoonCache)
				{
					foreach (EntityPlayer entityPlayer in this.world.Players.list)
					{
						bool flag4;
						if (entityPlayer.bloodMoonParty != null && entityPlayer.bloodMoonParty.partySpawner != null)
						{
							bool flag2 = entityPlayer.bloodMoonParty.partySpawner.partyMembers.Max((EntityPlayer p) => p.Progression.GetLevel()) > 1 && (!entityPlayer.bloodMoonParty.partySpawner.IsDone || entityPlayer.bloodMoonParty.BloodmoonZombiesRemain);
							if (entityPlayer is EntityPlayerLocal)
							{
								this.IsBloodmoonMusicEligible = flag2;
							}
							else
							{
								bool flag3;
								if (!this.PlayerEligibleForBloodmoonCache.TryGetValue(entityPlayer.entityId, out flag3))
								{
									this.PlayerEligibleForBloodmoonCache.Add(entityPlayer.entityId, flag2);
								}
								if (flag3 != flag2)
								{
									this.PlayerEligibleForBloodmoonCache[entityPlayer.entityId] = flag2;
									NetPackageBloodmoonMusic package = NetPackageManager.GetPackage<NetPackageBloodmoonMusic>().Setup(flag2);
									SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, entityPlayer.entityId, -1, -1, null, 192, false);
								}
							}
						}
						else if (entityPlayer is EntityPlayerLocal)
						{
							this.IsBloodmoonMusicEligible = false;
						}
						else if (this.PlayerEligibleForBloodmoonCache.TryGetValue(entityPlayer.entityId, out flag4) && flag4)
						{
							this.PlayerEligibleForBloodmoonCache[entityPlayer.entityId] = false;
							NetPackageBloodmoonMusic package2 = NetPackageManager.GetPackage<NetPackageBloodmoonMusic>().Setup(false);
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package2, false, entityPlayer.entityId, -1, -1, null, 192, false);
						}
						else if (!this.PlayerEligibleForBloodmoonCache.ContainsKey(entityPlayer.entityId))
						{
							this.PlayerEligibleForBloodmoonCache.Add(entityPlayer.entityId, false);
							NetPackageBloodmoonMusic package3 = NetPackageManager.GetPackage<NetPackageBloodmoonMusic>().Setup(false);
							SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package3, false, entityPlayer.entityId, -1, -1, null, 192, false);
						}
					}
				}
			}
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			bool flag5 = this.wasMusicPlaying;
			bool flag = this.wasMusicPlaying = this.IsMusicPlaying;
			if (flag5 & !flag)
			{
				this.sectionSelector.Notify(MusicActionType.Stop);
				Log.Out("Notified SectionSelector that music stopped");
			}
			SectionType sectionType = this.sectionSelector.Select();
			if (this.CurrentSectionType != sectionType)
			{
				Log.Out(string.Format("SectionType change from {0} to {1}", this.CurrentSectionType, sectionType));
				if (this.CurrentSectionType == SectionType.None)
				{
					ISection section;
					if (this.sections.TryGetValue(sectionType, out section))
					{
						section.FadeIn();
						this.CurrentSection = section;
						this.sectionSelector.Notify(MusicActionType.Play);
						Log.Out("Notified SectionSelector that music played");
					}
				}
				else
				{
					if (this.CurrentSection != null && this.CurrentSection.IsPlaying && !this.CurrentSection.IsPaused)
					{
						this.CurrentSection.FadeOut();
					}
					ISection section2;
					if (this.sections.TryGetValue(sectionType, out section2))
					{
						section2.FadeIn();
						this.sectionSelector.Notify(MusicActionType.FadeIn);
						this.CurrentSection = section2;
					}
					else
					{
						this.CurrentSection = null;
					}
				}
			}
			this.CurrentSectionType = sectionType;
			MixerController.Instance.Update();
		}

		// Token: 0x0600CBC8 RID: 52168 RVA: 0x004AA598 File Offset: 0x004A8798
		public void OnPauseGame()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			if (this.CurrentSection != null)
			{
				this.CurrentSection.Pause();
			}
			if (this.sectionSelector != null)
			{
				this.sectionSelector.Notify(MusicActionType.Pause);
			}
		}

		// Token: 0x0600CBC9 RID: 52169 RVA: 0x004AA5C9 File Offset: 0x004A87C9
		public void OnUnPauseGame()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			if (this.CurrentSection != null)
			{
				this.CurrentSection.UnPause();
			}
			if (this.sectionSelector != null)
			{
				this.sectionSelector.Notify(MusicActionType.UnPause);
			}
		}

		// Token: 0x0600CBCA RID: 52170 RVA: 0x004AA5FC File Offset: 0x004A87FC
		public void CleanUp()
		{
			if (GameManager.IsDedicatedServer)
			{
				return;
			}
			this.sectionSelector = null;
			if (this.sections != null)
			{
				foreach (ISection section in this.sections.Values)
				{
					section.CleanUp();
				}
				this.sections.Clear();
			}
			this.sections = null;
			this.CurrentSection = null;
			this.CurrentSectionType = SectionType.None;
			LayeredContent.ClearQueues();
		}

		// Token: 0x0600CBCB RID: 52171 RVA: 0x004AA690 File Offset: 0x004A8890
		public void OnWorldExit()
		{
			ConnectionManager.OnClientDisconnected -= this.OnClientDisconnected;
			if (this.PlayerEligibleForBloodmoonCache != null)
			{
				Dictionary<int, bool> playerEligibleForBloodmoonCache = this.PlayerEligibleForBloodmoonCache;
				lock (playerEligibleForBloodmoonCache)
				{
					this.PlayerEligibleForBloodmoonCache.Clear();
					this.PlayerEligibleForBloodmoonCache = null;
				}
			}
		}

		// Token: 0x0600CBCC RID: 52172 RVA: 0x004AA6F8 File Offset: 0x004A88F8
		public override string ToString()
		{
			return "Conductor:\n" + string.Format("Current Section Type: {0}\n", this.CurrentSectionType);
		}

		// Token: 0x0600CBCD RID: 52173 RVA: 0x004AA71C File Offset: 0x004A891C
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClientDisconnected(ClientInfo _ci)
		{
			if (this.PlayerEligibleForBloodmoonCache != null)
			{
				Dictionary<int, bool> playerEligibleForBloodmoonCache = this.PlayerEligibleForBloodmoonCache;
				lock (playerEligibleForBloodmoonCache)
				{
					if (!this.PlayerEligibleForBloodmoonCache.Remove(_ci.entityId))
					{
						Log.Warning(string.Format("DynamicMusic: {0} was not in Bloodmoon state cache on disconnect", _ci.entityId));
					}
					else
					{
						Log.Out(string.Format("DynamicMusic: {0} successfully removed from Bloodmoon state cache", _ci.entityId));
					}
				}
			}
		}

		// Token: 0x04009B39 RID: 39737
		[PublicizedFrom(EAccessModifier.Private)]
		public ISectionSelector sectionSelector;

		// Token: 0x04009B3A RID: 39738
		[PublicizedFrom(EAccessModifier.Private)]
		public EnumDictionary<SectionType, ISection> sections;

		// Token: 0x04009B3C RID: 39740
		[PublicizedFrom(EAccessModifier.Private)]
		public ISection CurrentSection;

		// Token: 0x04009B3D RID: 39741
		[PublicizedFrom(EAccessModifier.Private)]
		public World world;

		// Token: 0x04009B3E RID: 39742
		public Dictionary<int, bool> PlayerEligibleForBloodmoonCache;

		// Token: 0x04009B3F RID: 39743
		public bool IsBloodmoonMusicEligible;

		// Token: 0x04009B40 RID: 39744
		[PublicizedFrom(EAccessModifier.Private)]
		public bool wasMusicPlaying;
	}
}
