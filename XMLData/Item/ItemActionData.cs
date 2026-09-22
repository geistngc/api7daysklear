using System;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using UnityEngine.Scripting;
using XMLData.Exceptions;
using XMLData.Parsers;

namespace XMLData.Item
{
	// Token: 0x02001647 RID: 5703
	[Preserve]
	public class ItemActionData : IXMLData
	{
		// Token: 0x17001520 RID: 5408
		// (get) Token: 0x0600B2EC RID: 45804 RVA: 0x0042DB87 File Offset: 0x0042BD87
		// (set) Token: 0x0600B2ED RID: 45805 RVA: 0x0042DB8F File Offset: 0x0042BD8F
		public DataItem<float> Delay
		{
			get
			{
				return this.pDelay;
			}
			set
			{
				this.pDelay = value;
			}
		}

		// Token: 0x17001521 RID: 5409
		// (get) Token: 0x0600B2EE RID: 45806 RVA: 0x0042DB98 File Offset: 0x0042BD98
		// (set) Token: 0x0600B2EF RID: 45807 RVA: 0x0042DBA0 File Offset: 0x0042BDA0
		public DataItem<float> Range
		{
			get
			{
				return this.pRange;
			}
			set
			{
				this.pRange = value;
			}
		}

		// Token: 0x17001522 RID: 5410
		// (get) Token: 0x0600B2F0 RID: 45808 RVA: 0x0042DBA9 File Offset: 0x0042BDA9
		// (set) Token: 0x0600B2F1 RID: 45809 RVA: 0x0042DBB1 File Offset: 0x0042BDB1
		public DataItem<string> SoundStart
		{
			get
			{
				return this.pSoundStart;
			}
			set
			{
				this.pSoundStart = value;
			}
		}

		// Token: 0x17001523 RID: 5411
		// (get) Token: 0x0600B2F2 RID: 45810 RVA: 0x0042DBBA File Offset: 0x0042BDBA
		// (set) Token: 0x0600B2F3 RID: 45811 RVA: 0x0042DBC2 File Offset: 0x0042BDC2
		public DataItem<string> SoundRepeat
		{
			get
			{
				return this.pSoundRepeat;
			}
			set
			{
				this.pSoundRepeat = value;
			}
		}

		// Token: 0x17001524 RID: 5412
		// (get) Token: 0x0600B2F4 RID: 45812 RVA: 0x0042DBCB File Offset: 0x0042BDCB
		// (set) Token: 0x0600B2F5 RID: 45813 RVA: 0x0042DBD3 File Offset: 0x0042BDD3
		public DataItem<string> SoundEnd
		{
			get
			{
				return this.pSoundEnd;
			}
			set
			{
				this.pSoundEnd = value;
			}
		}

		// Token: 0x17001525 RID: 5413
		// (get) Token: 0x0600B2F6 RID: 45814 RVA: 0x0042DBDC File Offset: 0x0042BDDC
		// (set) Token: 0x0600B2F7 RID: 45815 RVA: 0x0042DBE4 File Offset: 0x0042BDE4
		public DataItem<string> SoundEmpty
		{
			get
			{
				return this.pSoundEmpty;
			}
			set
			{
				this.pSoundEmpty = value;
			}
		}

		// Token: 0x17001526 RID: 5414
		// (get) Token: 0x0600B2F8 RID: 45816 RVA: 0x0042DBED File Offset: 0x0042BDED
		// (set) Token: 0x0600B2F9 RID: 45817 RVA: 0x0042DBF5 File Offset: 0x0042BDF5
		public DataItem<string> SoundReload
		{
			get
			{
				return this.pSoundReload;
			}
			set
			{
				this.pSoundReload = value;
			}
		}

		// Token: 0x17001527 RID: 5415
		// (get) Token: 0x0600B2FA RID: 45818 RVA: 0x0042DBFE File Offset: 0x0042BDFE
		// (set) Token: 0x0600B2FB RID: 45819 RVA: 0x0042DC06 File Offset: 0x0042BE06
		public DataItem<string> SoundWarning
		{
			get
			{
				return this.pSoundWarning;
			}
			set
			{
				this.pSoundWarning = value;
			}
		}

		// Token: 0x17001528 RID: 5416
		// (get) Token: 0x0600B2FC RID: 45820 RVA: 0x0042DC0F File Offset: 0x0042BE0F
		// (set) Token: 0x0600B2FD RID: 45821 RVA: 0x0042DC17 File Offset: 0x0042BE17
		public DataItem<string> StaminaUsage
		{
			get
			{
				return this.pStaminaUsage;
			}
			set
			{
				this.pStaminaUsage = value;
			}
		}

		// Token: 0x17001529 RID: 5417
		// (get) Token: 0x0600B2FE RID: 45822 RVA: 0x0042DC20 File Offset: 0x0042BE20
		// (set) Token: 0x0600B2FF RID: 45823 RVA: 0x0042DC28 File Offset: 0x0042BE28
		public DataItem<string> UseTime
		{
			get
			{
				return this.pUseTime;
			}
			set
			{
				this.pUseTime = value;
			}
		}

		// Token: 0x1700152A RID: 5418
		// (get) Token: 0x0600B300 RID: 45824 RVA: 0x0042DC31 File Offset: 0x0042BE31
		// (set) Token: 0x0600B301 RID: 45825 RVA: 0x0042DC39 File Offset: 0x0042BE39
		public DataItem<string> FocusedBlockname1
		{
			get
			{
				return this.pFocusedBlockname1;
			}
			set
			{
				this.pFocusedBlockname1 = value;
			}
		}

		// Token: 0x1700152B RID: 5419
		// (get) Token: 0x0600B302 RID: 45826 RVA: 0x0042DC42 File Offset: 0x0042BE42
		// (set) Token: 0x0600B303 RID: 45827 RVA: 0x0042DC4A File Offset: 0x0042BE4A
		public DataItem<string> FocusedBlockname2
		{
			get
			{
				return this.pFocusedBlockname2;
			}
			set
			{
				this.pFocusedBlockname2 = value;
			}
		}

		// Token: 0x1700152C RID: 5420
		// (get) Token: 0x0600B304 RID: 45828 RVA: 0x0042DC53 File Offset: 0x0042BE53
		// (set) Token: 0x0600B305 RID: 45829 RVA: 0x0042DC5B File Offset: 0x0042BE5B
		public DataItem<string> FocusedBlockname3
		{
			get
			{
				return this.pFocusedBlockname3;
			}
			set
			{
				this.pFocusedBlockname3 = value;
			}
		}

		// Token: 0x1700152D RID: 5421
		// (get) Token: 0x0600B306 RID: 45830 RVA: 0x0042DC64 File Offset: 0x0042BE64
		// (set) Token: 0x0600B307 RID: 45831 RVA: 0x0042DC6C File Offset: 0x0042BE6C
		public DataItem<string> FocusedBlockname4
		{
			get
			{
				return this.pFocusedBlockname4;
			}
			set
			{
				this.pFocusedBlockname4 = value;
			}
		}

		// Token: 0x1700152E RID: 5422
		// (get) Token: 0x0600B308 RID: 45832 RVA: 0x0042DC75 File Offset: 0x0042BE75
		// (set) Token: 0x0600B309 RID: 45833 RVA: 0x0042DC7D File Offset: 0x0042BE7D
		public DataItem<string> FocusedBlockname5
		{
			get
			{
				return this.pFocusedBlockname5;
			}
			set
			{
				this.pFocusedBlockname5 = value;
			}
		}

		// Token: 0x1700152F RID: 5423
		// (get) Token: 0x0600B30A RID: 45834 RVA: 0x0042DC86 File Offset: 0x0042BE86
		// (set) Token: 0x0600B30B RID: 45835 RVA: 0x0042DC8E File Offset: 0x0042BE8E
		public DataItem<string> FocusedBlockname6
		{
			get
			{
				return this.pFocusedBlockname6;
			}
			set
			{
				this.pFocusedBlockname6 = value;
			}
		}

		// Token: 0x17001530 RID: 5424
		// (get) Token: 0x0600B30C RID: 45836 RVA: 0x0042DC97 File Offset: 0x0042BE97
		// (set) Token: 0x0600B30D RID: 45837 RVA: 0x0042DC9F File Offset: 0x0042BE9F
		public DataItem<string> FocusedBlockname7
		{
			get
			{
				return this.pFocusedBlockname7;
			}
			set
			{
				this.pFocusedBlockname7 = value;
			}
		}

		// Token: 0x17001531 RID: 5425
		// (get) Token: 0x0600B30E RID: 45838 RVA: 0x0042DCA8 File Offset: 0x0042BEA8
		// (set) Token: 0x0600B30F RID: 45839 RVA: 0x0042DCB0 File Offset: 0x0042BEB0
		public DataItem<string> FocusedBlockname8
		{
			get
			{
				return this.pFocusedBlockname8;
			}
			set
			{
				this.pFocusedBlockname8 = value;
			}
		}

		// Token: 0x17001532 RID: 5426
		// (get) Token: 0x0600B310 RID: 45840 RVA: 0x0042DCB9 File Offset: 0x0042BEB9
		// (set) Token: 0x0600B311 RID: 45841 RVA: 0x0042DCC1 File Offset: 0x0042BEC1
		public DataItem<string> FocusedBlockname9
		{
			get
			{
				return this.pFocusedBlockname9;
			}
			set
			{
				this.pFocusedBlockname9 = value;
			}
		}

		// Token: 0x17001533 RID: 5427
		// (get) Token: 0x0600B312 RID: 45842 RVA: 0x0042DCCA File Offset: 0x0042BECA
		// (set) Token: 0x0600B313 RID: 45843 RVA: 0x0042DCD2 File Offset: 0x0042BED2
		public DataItem<string> ChangeItemTo
		{
			get
			{
				return this.pChangeItemTo;
			}
			set
			{
				this.pChangeItemTo = value;
			}
		}

		// Token: 0x17001534 RID: 5428
		// (get) Token: 0x0600B314 RID: 45844 RVA: 0x0042DCDB File Offset: 0x0042BEDB
		// (set) Token: 0x0600B315 RID: 45845 RVA: 0x0042DCE3 File Offset: 0x0042BEE3
		public DataItem<string> ChangeBlockTo
		{
			get
			{
				return this.pChangeBlockTo;
			}
			set
			{
				this.pChangeBlockTo = value;
			}
		}

		// Token: 0x17001535 RID: 5429
		// (get) Token: 0x0600B316 RID: 45846 RVA: 0x0042DCEC File Offset: 0x0042BEEC
		// (set) Token: 0x0600B317 RID: 45847 RVA: 0x0042DCF4 File Offset: 0x0042BEF4
		public DataItem<string> DoBlockAction
		{
			get
			{
				return this.pDoBlockAction;
			}
			set
			{
				this.pDoBlockAction = value;
			}
		}

		// Token: 0x17001536 RID: 5430
		// (get) Token: 0x0600B318 RID: 45848 RVA: 0x0042DCFD File Offset: 0x0042BEFD
		// (set) Token: 0x0600B319 RID: 45849 RVA: 0x0042DD05 File Offset: 0x0042BF05
		public DataItem<float> GainHealth
		{
			get
			{
				return this.pGainHealth;
			}
			set
			{
				this.pGainHealth = value;
			}
		}

		// Token: 0x17001537 RID: 5431
		// (get) Token: 0x0600B31A RID: 45850 RVA: 0x0042DD0E File Offset: 0x0042BF0E
		// (set) Token: 0x0600B31B RID: 45851 RVA: 0x0042DD16 File Offset: 0x0042BF16
		public DataItem<float> GainFood
		{
			get
			{
				return this.pGainFood;
			}
			set
			{
				this.pGainFood = value;
			}
		}

		// Token: 0x17001538 RID: 5432
		// (get) Token: 0x0600B31C RID: 45852 RVA: 0x0042DD1F File Offset: 0x0042BF1F
		// (set) Token: 0x0600B31D RID: 45853 RVA: 0x0042DD27 File Offset: 0x0042BF27
		public DataItem<float> GainWater
		{
			get
			{
				return this.pGainWater;
			}
			set
			{
				this.pGainWater = value;
			}
		}

		// Token: 0x17001539 RID: 5433
		// (get) Token: 0x0600B31E RID: 45854 RVA: 0x0042DD30 File Offset: 0x0042BF30
		// (set) Token: 0x0600B31F RID: 45855 RVA: 0x0042DD38 File Offset: 0x0042BF38
		public DataItem<float> GainStamina
		{
			get
			{
				return this.pGainStamina;
			}
			set
			{
				this.pGainStamina = value;
			}
		}

		// Token: 0x1700153A RID: 5434
		// (get) Token: 0x0600B320 RID: 45856 RVA: 0x0042DD41 File Offset: 0x0042BF41
		// (set) Token: 0x0600B321 RID: 45857 RVA: 0x0042DD49 File Offset: 0x0042BF49
		public DataItem<float> GainSickness
		{
			get
			{
				return this.pGainSickness;
			}
			set
			{
				this.pGainSickness = value;
			}
		}

		// Token: 0x1700153B RID: 5435
		// (get) Token: 0x0600B322 RID: 45858 RVA: 0x0042DD52 File Offset: 0x0042BF52
		// (set) Token: 0x0600B323 RID: 45859 RVA: 0x0042DD5A File Offset: 0x0042BF5A
		public DataItem<float> GainWellness
		{
			get
			{
				return this.pGainWellness;
			}
			set
			{
				this.pGainWellness = value;
			}
		}

		// Token: 0x1700153C RID: 5436
		// (get) Token: 0x0600B324 RID: 45860 RVA: 0x0042DD63 File Offset: 0x0042BF63
		// (set) Token: 0x0600B325 RID: 45861 RVA: 0x0042DD6B File Offset: 0x0042BF6B
		public DataItem<string> Buff
		{
			get
			{
				return this.pBuff;
			}
			set
			{
				this.pBuff = value;
			}
		}

		// Token: 0x1700153D RID: 5437
		// (get) Token: 0x0600B326 RID: 45862 RVA: 0x0042DD74 File Offset: 0x0042BF74
		// (set) Token: 0x0600B327 RID: 45863 RVA: 0x0042DD7C File Offset: 0x0042BF7C
		public DataItem<string> BuffChance
		{
			get
			{
				return this.pBuffChance;
			}
			set
			{
				this.pBuffChance = value;
			}
		}

		// Token: 0x1700153E RID: 5438
		// (get) Token: 0x0600B328 RID: 45864 RVA: 0x0042DD85 File Offset: 0x0042BF85
		// (set) Token: 0x0600B329 RID: 45865 RVA: 0x0042DD8D File Offset: 0x0042BF8D
		public DataItem<string> Debuff
		{
			get
			{
				return this.pDebuff;
			}
			set
			{
				this.pDebuff = value;
			}
		}

		// Token: 0x1700153F RID: 5439
		// (get) Token: 0x0600B32A RID: 45866 RVA: 0x0042DD96 File Offset: 0x0042BF96
		// (set) Token: 0x0600B32B RID: 45867 RVA: 0x0042DD9E File Offset: 0x0042BF9E
		public DataItem<string> CreateItem
		{
			get
			{
				return this.pCreateItem;
			}
			set
			{
				this.pCreateItem = value;
			}
		}

		// Token: 0x17001540 RID: 5440
		// (get) Token: 0x0600B32C RID: 45868 RVA: 0x0042DDA7 File Offset: 0x0042BFA7
		// (set) Token: 0x0600B32D RID: 45869 RVA: 0x0042DDAF File Offset: 0x0042BFAF
		public DataItem<int> ConditionRaycastBlock
		{
			get
			{
				return this.pConditionRaycastBlock;
			}
			set
			{
				this.pConditionRaycastBlock = value;
			}
		}

		// Token: 0x17001541 RID: 5441
		// (get) Token: 0x0600B32E RID: 45870 RVA: 0x0042DDB8 File Offset: 0x0042BFB8
		// (set) Token: 0x0600B32F RID: 45871 RVA: 0x0042DDC0 File Offset: 0x0042BFC0
		public DataItem<int> GainGas
		{
			get
			{
				return this.pGainGas;
			}
			set
			{
				this.pGainGas = value;
			}
		}

		// Token: 0x17001542 RID: 5442
		// (get) Token: 0x0600B330 RID: 45872 RVA: 0x0042DDC9 File Offset: 0x0042BFC9
		// (set) Token: 0x0600B331 RID: 45873 RVA: 0x0042DDD1 File Offset: 0x0042BFD1
		public DataItem<bool> Consume
		{
			get
			{
				return this.pConsume;
			}
			set
			{
				this.pConsume = value;
			}
		}

		// Token: 0x17001543 RID: 5443
		// (get) Token: 0x0600B332 RID: 45874 RVA: 0x0042DDDA File Offset: 0x0042BFDA
		// (set) Token: 0x0600B333 RID: 45875 RVA: 0x0042DDE2 File Offset: 0x0042BFE2
		public DataItem<string> Blockname
		{
			get
			{
				return this.pBlockname;
			}
			set
			{
				this.pBlockname = value;
			}
		}

		// Token: 0x17001544 RID: 5444
		// (get) Token: 0x0600B334 RID: 45876 RVA: 0x0042DDEB File Offset: 0x0042BFEB
		// (set) Token: 0x0600B335 RID: 45877 RVA: 0x0042DDF3 File Offset: 0x0042BFF3
		public DataItem<float> ThrowStrengthDefault
		{
			get
			{
				return this.pThrowStrengthDefault;
			}
			set
			{
				this.pThrowStrengthDefault = value;
			}
		}

		// Token: 0x17001545 RID: 5445
		// (get) Token: 0x0600B336 RID: 45878 RVA: 0x0042DDFC File Offset: 0x0042BFFC
		// (set) Token: 0x0600B337 RID: 45879 RVA: 0x0042DE04 File Offset: 0x0042C004
		public DataItem<float> ThrowStrengthMax
		{
			get
			{
				return this.pThrowStrengthMax;
			}
			set
			{
				this.pThrowStrengthMax = value;
			}
		}

		// Token: 0x17001546 RID: 5446
		// (get) Token: 0x0600B338 RID: 45880 RVA: 0x0042DE0D File Offset: 0x0042C00D
		// (set) Token: 0x0600B339 RID: 45881 RVA: 0x0042DE15 File Offset: 0x0042C015
		public DataItem<float> MaxStrainTime
		{
			get
			{
				return this.pMaxStrainTime;
			}
			set
			{
				this.pMaxStrainTime = value;
			}
		}

		// Token: 0x17001547 RID: 5447
		// (get) Token: 0x0600B33A RID: 45882 RVA: 0x0042DE1E File Offset: 0x0042C01E
		// (set) Token: 0x0600B33B RID: 45883 RVA: 0x0042DE26 File Offset: 0x0042C026
		public DataItem<int> MagazineSize
		{
			get
			{
				return this.pMagazineSize;
			}
			set
			{
				this.pMagazineSize = value;
			}
		}

		// Token: 0x17001548 RID: 5448
		// (get) Token: 0x0600B33C RID: 45884 RVA: 0x0042DE2F File Offset: 0x0042C02F
		// (set) Token: 0x0600B33D RID: 45885 RVA: 0x0042DE37 File Offset: 0x0042C037
		public DataItem<string> MagazineItem
		{
			get
			{
				return this.pMagazineItem;
			}
			set
			{
				this.pMagazineItem = value;
			}
		}

		// Token: 0x17001549 RID: 5449
		// (get) Token: 0x0600B33E RID: 45886 RVA: 0x0042DE40 File Offset: 0x0042C040
		// (set) Token: 0x0600B33F RID: 45887 RVA: 0x0042DE48 File Offset: 0x0042C048
		public DataItem<float> ReloadTime
		{
			get
			{
				return this.pReloadTime;
			}
			set
			{
				this.pReloadTime = value;
			}
		}

		// Token: 0x1700154A RID: 5450
		// (get) Token: 0x0600B340 RID: 45888 RVA: 0x0042DE51 File Offset: 0x0042C051
		// (set) Token: 0x0600B341 RID: 45889 RVA: 0x0042DE59 File Offset: 0x0042C059
		public DataItem<string> BulletIcon
		{
			get
			{
				return this.pBulletIcon;
			}
			set
			{
				this.pBulletIcon = value;
			}
		}

		// Token: 0x1700154B RID: 5451
		// (get) Token: 0x0600B342 RID: 45890 RVA: 0x0042DE62 File Offset: 0x0042C062
		// (set) Token: 0x0600B343 RID: 45891 RVA: 0x0042DE6A File Offset: 0x0042C06A
		public DataItem<int> RaysPerShot
		{
			get
			{
				return this.pRaysPerShot;
			}
			set
			{
				this.pRaysPerShot = value;
			}
		}

		// Token: 0x1700154C RID: 5452
		// (get) Token: 0x0600B344 RID: 45892 RVA: 0x0042DE73 File Offset: 0x0042C073
		// (set) Token: 0x0600B345 RID: 45893 RVA: 0x0042DE7B File Offset: 0x0042C07B
		public DataItem<float> RaysSpread
		{
			get
			{
				return this.pRaysSpread;
			}
			set
			{
				this.pRaysSpread = value;
			}
		}

		// Token: 0x1700154D RID: 5453
		// (get) Token: 0x0600B346 RID: 45894 RVA: 0x0042DE84 File Offset: 0x0042C084
		// (set) Token: 0x0600B347 RID: 45895 RVA: 0x0042DE8C File Offset: 0x0042C08C
		public DataItem<float> Sphere
		{
			get
			{
				return this.pSphere;
			}
			set
			{
				this.pSphere = value;
			}
		}

		// Token: 0x1700154E RID: 5454
		// (get) Token: 0x0600B348 RID: 45896 RVA: 0x0042DE95 File Offset: 0x0042C095
		// (set) Token: 0x0600B349 RID: 45897 RVA: 0x0042DE9D File Offset: 0x0042C09D
		public DataItem<int> CrosshairMinDistance
		{
			get
			{
				return this.pCrosshairMinDistance;
			}
			set
			{
				this.pCrosshairMinDistance = value;
			}
		}

		// Token: 0x1700154F RID: 5455
		// (get) Token: 0x0600B34A RID: 45898 RVA: 0x0042DEA6 File Offset: 0x0042C0A6
		// (set) Token: 0x0600B34B RID: 45899 RVA: 0x0042DEAE File Offset: 0x0042C0AE
		public DataItem<int> CrosshairMaxDistance
		{
			get
			{
				return this.pCrosshairMaxDistance;
			}
			set
			{
				this.pCrosshairMaxDistance = value;
			}
		}

		// Token: 0x17001550 RID: 5456
		// (get) Token: 0x0600B34C RID: 45900 RVA: 0x0042DEB7 File Offset: 0x0042C0B7
		// (set) Token: 0x0600B34D RID: 45901 RVA: 0x0042DEBF File Offset: 0x0042C0BF
		public DataItem<int> DamageEntity
		{
			get
			{
				return this.pDamageEntity;
			}
			set
			{
				this.pDamageEntity = value;
			}
		}

		// Token: 0x17001551 RID: 5457
		// (get) Token: 0x0600B34E RID: 45902 RVA: 0x0042DEC8 File Offset: 0x0042C0C8
		// (set) Token: 0x0600B34F RID: 45903 RVA: 0x0042DED0 File Offset: 0x0042C0D0
		public DataItem<float> DamageBlock
		{
			get
			{
				return this.pDamageBlock;
			}
			set
			{
				this.pDamageBlock = value;
			}
		}

		// Token: 0x17001552 RID: 5458
		// (get) Token: 0x0600B350 RID: 45904 RVA: 0x0042DED9 File Offset: 0x0042C0D9
		// (set) Token: 0x0600B351 RID: 45905 RVA: 0x0042DEE1 File Offset: 0x0042C0E1
		public DataItem<string> ParticlesMuzzleFire
		{
			get
			{
				return this.pParticlesMuzzleFire;
			}
			set
			{
				this.pParticlesMuzzleFire = value;
			}
		}

		// Token: 0x17001553 RID: 5459
		// (get) Token: 0x0600B352 RID: 45906 RVA: 0x0042DEEA File Offset: 0x0042C0EA
		// (set) Token: 0x0600B353 RID: 45907 RVA: 0x0042DEF2 File Offset: 0x0042C0F2
		public DataItem<string> ParticlesMuzzleSmoke
		{
			get
			{
				return this.pParticlesMuzzleSmoke;
			}
			set
			{
				this.pParticlesMuzzleSmoke = value;
			}
		}

		// Token: 0x17001554 RID: 5460
		// (get) Token: 0x0600B354 RID: 45908 RVA: 0x0042DEFB File Offset: 0x0042C0FB
		// (set) Token: 0x0600B355 RID: 45909 RVA: 0x0042DF03 File Offset: 0x0042C103
		public DataItem<float> BlockRange
		{
			get
			{
				return this.pBlockRange;
			}
			set
			{
				this.pBlockRange = value;
			}
		}

		// Token: 0x17001555 RID: 5461
		// (get) Token: 0x0600B356 RID: 45910 RVA: 0x0042DF0C File Offset: 0x0042C10C
		// (set) Token: 0x0600B357 RID: 45911 RVA: 0x0042DF14 File Offset: 0x0042C114
		public DataItem<bool> AutoFire
		{
			get
			{
				return this.pAutoFire;
			}
			set
			{
				this.pAutoFire = value;
			}
		}

		// Token: 0x17001556 RID: 5462
		// (get) Token: 0x0600B358 RID: 45912 RVA: 0x0042DF1D File Offset: 0x0042C11D
		// (set) Token: 0x0600B359 RID: 45913 RVA: 0x0042DF25 File Offset: 0x0042C125
		public DataItem<float> HordeMeterRate
		{
			get
			{
				return this.pHordeMeterRate;
			}
			set
			{
				this.pHordeMeterRate = value;
			}
		}

		// Token: 0x17001557 RID: 5463
		// (get) Token: 0x0600B35A RID: 45914 RVA: 0x0042DF2E File Offset: 0x0042C12E
		// (set) Token: 0x0600B35B RID: 45915 RVA: 0x0042DF36 File Offset: 0x0042C136
		public DataItem<float> HordeMeterDistance
		{
			get
			{
				return this.pHordeMeterDistance;
			}
			set
			{
				this.pHordeMeterDistance = value;
			}
		}

		// Token: 0x17001558 RID: 5464
		// (get) Token: 0x0600B35C RID: 45916 RVA: 0x0042DF3F File Offset: 0x0042C13F
		// (set) Token: 0x0600B35D RID: 45917 RVA: 0x0042DF47 File Offset: 0x0042C147
		public DataItem<string> HitmaskOverride
		{
			get
			{
				return this.pHitmaskOverride;
			}
			set
			{
				this.pHitmaskOverride = value;
			}
		}

		// Token: 0x17001559 RID: 5465
		// (get) Token: 0x0600B35E RID: 45918 RVA: 0x0042DF50 File Offset: 0x0042C150
		// (set) Token: 0x0600B35F RID: 45919 RVA: 0x0042DF58 File Offset: 0x0042C158
		public DataItem<bool> SingleMagazineUsage
		{
			get
			{
				return this.pSingleMagazineUsage;
			}
			set
			{
				this.pSingleMagazineUsage = value;
			}
		}

		// Token: 0x1700155A RID: 5466
		// (get) Token: 0x0600B360 RID: 45920 RVA: 0x0042DF61 File Offset: 0x0042C161
		// (set) Token: 0x0600B361 RID: 45921 RVA: 0x0042DF69 File Offset: 0x0042C169
		public DataItem<string> BulletMaterial
		{
			get
			{
				return this.pBulletMaterial;
			}
			set
			{
				this.pBulletMaterial = value;
			}
		}

		// Token: 0x1700155B RID: 5467
		// (get) Token: 0x0600B362 RID: 45922 RVA: 0x0042DF72 File Offset: 0x0042C172
		// (set) Token: 0x0600B363 RID: 45923 RVA: 0x0042DF7A File Offset: 0x0042C17A
		public DataItem<bool> InfiniteAmmo
		{
			get
			{
				return this.pInfiniteAmmo;
			}
			set
			{
				this.pInfiniteAmmo = value;
			}
		}

		// Token: 0x1700155C RID: 5468
		// (get) Token: 0x0600B364 RID: 45924 RVA: 0x0042DF83 File Offset: 0x0042C183
		// (set) Token: 0x0600B365 RID: 45925 RVA: 0x0042DF8B File Offset: 0x0042C18B
		public DataItem<float> ZoomMaxOut
		{
			get
			{
				return this.pZoomMaxOut;
			}
			set
			{
				this.pZoomMaxOut = value;
			}
		}

		// Token: 0x1700155D RID: 5469
		// (get) Token: 0x0600B366 RID: 45926 RVA: 0x0042DF94 File Offset: 0x0042C194
		// (set) Token: 0x0600B367 RID: 45927 RVA: 0x0042DF9C File Offset: 0x0042C19C
		public DataItem<float> ZoomMaxIn
		{
			get
			{
				return this.pZoomMaxIn;
			}
			set
			{
				this.pZoomMaxIn = value;
			}
		}

		// Token: 0x1700155E RID: 5470
		// (get) Token: 0x0600B368 RID: 45928 RVA: 0x0042DFA5 File Offset: 0x0042C1A5
		// (set) Token: 0x0600B369 RID: 45929 RVA: 0x0042DFAD File Offset: 0x0042C1AD
		public DataItem<string> ZoomOverlay
		{
			get
			{
				return this.pZoomOverlay;
			}
			set
			{
				this.pZoomOverlay = value;
			}
		}

		// Token: 0x1700155F RID: 5471
		// (get) Token: 0x0600B36A RID: 45930 RVA: 0x0042DFB6 File Offset: 0x0042C1B6
		// (set) Token: 0x0600B36B RID: 45931 RVA: 0x0042DFBE File Offset: 0x0042C1BE
		public DataItem<int> Velocity
		{
			get
			{
				return this.pVelocity;
			}
			set
			{
				this.pVelocity = value;
			}
		}

		// Token: 0x17001560 RID: 5472
		// (get) Token: 0x0600B36C RID: 45932 RVA: 0x0042DFC7 File Offset: 0x0042C1C7
		// (set) Token: 0x0600B36D RID: 45933 RVA: 0x0042DFCF File Offset: 0x0042C1CF
		public DataItem<float> FlyTime
		{
			get
			{
				return this.pFlyTime;
			}
			set
			{
				this.pFlyTime = value;
			}
		}

		// Token: 0x17001561 RID: 5473
		// (get) Token: 0x0600B36E RID: 45934 RVA: 0x0042DFD8 File Offset: 0x0042C1D8
		// (set) Token: 0x0600B36F RID: 45935 RVA: 0x0042DFE0 File Offset: 0x0042C1E0
		public DataItem<float> LifeTime
		{
			get
			{
				return this.pLifeTime;
			}
			set
			{
				this.pLifeTime = value;
			}
		}

		// Token: 0x17001562 RID: 5474
		// (get) Token: 0x0600B370 RID: 45936 RVA: 0x0042DFE9 File Offset: 0x0042C1E9
		// (set) Token: 0x0600B371 RID: 45937 RVA: 0x0042DFF1 File Offset: 0x0042C1F1
		public DataItem<float> CollisionRadius
		{
			get
			{
				return this.pCollisionRadius;
			}
			set
			{
				this.pCollisionRadius = value;
			}
		}

		// Token: 0x17001563 RID: 5475
		// (get) Token: 0x0600B372 RID: 45938 RVA: 0x0042DFFA File Offset: 0x0042C1FA
		// (set) Token: 0x0600B373 RID: 45939 RVA: 0x0042E002 File Offset: 0x0042C202
		public DataItem<int> ProjectileInitialVelocity
		{
			get
			{
				return this.pProjectileInitialVelocity;
			}
			set
			{
				this.pProjectileInitialVelocity = value;
			}
		}

		// Token: 0x17001564 RID: 5476
		// (get) Token: 0x0600B374 RID: 45940 RVA: 0x0042E00B File Offset: 0x0042C20B
		// (set) Token: 0x0600B375 RID: 45941 RVA: 0x0042E013 File Offset: 0x0042C213
		public DataItem<string> Fertileblock
		{
			get
			{
				return this.pFertileblock;
			}
			set
			{
				this.pFertileblock = value;
			}
		}

		// Token: 0x17001565 RID: 5477
		// (get) Token: 0x0600B376 RID: 45942 RVA: 0x0042E01C File Offset: 0x0042C21C
		// (set) Token: 0x0600B377 RID: 45943 RVA: 0x0042E024 File Offset: 0x0042C224
		public DataItem<string> Adjacentblock
		{
			get
			{
				return this.pAdjacentblock;
			}
			set
			{
				this.pAdjacentblock = value;
			}
		}

		// Token: 0x17001566 RID: 5478
		// (get) Token: 0x0600B378 RID: 45944 RVA: 0x0042E02D File Offset: 0x0042C22D
		// (set) Token: 0x0600B379 RID: 45945 RVA: 0x0042E035 File Offset: 0x0042C235
		public DataItem<int> RepairAmount
		{
			get
			{
				return this.pRepairAmount;
			}
			set
			{
				this.pRepairAmount = value;
			}
		}

		// Token: 0x17001567 RID: 5479
		// (get) Token: 0x0600B37A RID: 45946 RVA: 0x0042E03E File Offset: 0x0042C23E
		// (set) Token: 0x0600B37B RID: 45947 RVA: 0x0042E046 File Offset: 0x0042C246
		public DataItem<int> UpgradeHitOffset
		{
			get
			{
				return this.pUpgradeHitOffset;
			}
			set
			{
				this.pUpgradeHitOffset = value;
			}
		}

		// Token: 0x17001568 RID: 5480
		// (get) Token: 0x0600B37C RID: 45948 RVA: 0x0042E04F File Offset: 0x0042C24F
		// (set) Token: 0x0600B37D RID: 45949 RVA: 0x0042E057 File Offset: 0x0042C257
		public DataItem<string> AllowedUpgradeItems
		{
			get
			{
				return this.pAllowedUpgradeItems;
			}
			set
			{
				this.pAllowedUpgradeItems = value;
			}
		}

		// Token: 0x17001569 RID: 5481
		// (get) Token: 0x0600B37E RID: 45950 RVA: 0x0042E060 File Offset: 0x0042C260
		// (set) Token: 0x0600B37F RID: 45951 RVA: 0x0042E068 File Offset: 0x0042C268
		public DataItem<string> RestrictedUpgradeItems
		{
			get
			{
				return this.pRestrictedUpgradeItems;
			}
			set
			{
				this.pRestrictedUpgradeItems = value;
			}
		}

		// Token: 0x1700156A RID: 5482
		// (get) Token: 0x0600B380 RID: 45952 RVA: 0x0042E071 File Offset: 0x0042C271
		// (set) Token: 0x0600B381 RID: 45953 RVA: 0x0042E079 File Offset: 0x0042C279
		public DataItem<string> UpgradeActionSound
		{
			get
			{
				return this.pUpgradeActionSound;
			}
			set
			{
				this.pUpgradeActionSound = value;
			}
		}

		// Token: 0x1700156B RID: 5483
		// (get) Token: 0x0600B382 RID: 45954 RVA: 0x0042E082 File Offset: 0x0042C282
		// (set) Token: 0x0600B383 RID: 45955 RVA: 0x0042E08A File Offset: 0x0042C28A
		public DataItem<string> RepairActionSound
		{
			get
			{
				return this.pRepairActionSound;
			}
			set
			{
				this.pRepairActionSound = value;
			}
		}

		// Token: 0x1700156C RID: 5484
		// (get) Token: 0x0600B384 RID: 45956 RVA: 0x0042E093 File Offset: 0x0042C293
		// (set) Token: 0x0600B385 RID: 45957 RVA: 0x0042E09B File Offset: 0x0042C29B
		public DataItem<string> ReferenceItem
		{
			get
			{
				return this.pReferenceItem;
			}
			set
			{
				this.pReferenceItem = value;
			}
		}

		// Token: 0x1700156D RID: 5485
		// (get) Token: 0x0600B386 RID: 45958 RVA: 0x0042E0A4 File Offset: 0x0042C2A4
		// (set) Token: 0x0600B387 RID: 45959 RVA: 0x0042E0AC File Offset: 0x0042C2AC
		public DataItem<string> Mesh
		{
			get
			{
				return this.pMesh;
			}
			set
			{
				this.pMesh = value;
			}
		}

		// Token: 0x1700156E RID: 5486
		// (get) Token: 0x0600B388 RID: 45960 RVA: 0x0042E0B5 File Offset: 0x0042C2B5
		// (set) Token: 0x0600B389 RID: 45961 RVA: 0x0042E0BD File Offset: 0x0042C2BD
		public DataItem<int> ActionIdx
		{
			get
			{
				return this.pActionIdx;
			}
			set
			{
				this.pActionIdx = value;
			}
		}

		// Token: 0x1700156F RID: 5487
		// (get) Token: 0x0600B38A RID: 45962 RVA: 0x0042E0C6 File Offset: 0x0042C2C6
		// (set) Token: 0x0600B38B RID: 45963 RVA: 0x0042E0CE File Offset: 0x0042C2CE
		public DataItem<string> Title
		{
			get
			{
				return this.pTitle;
			}
			set
			{
				this.pTitle = value;
			}
		}

		// Token: 0x17001570 RID: 5488
		// (get) Token: 0x0600B38C RID: 45964 RVA: 0x0042E0D7 File Offset: 0x0042C2D7
		// (set) Token: 0x0600B38D RID: 45965 RVA: 0x0042E0DF File Offset: 0x0042C2DF
		public DataItem<string> Description
		{
			get
			{
				return this.pDescription;
			}
			set
			{
				this.pDescription = value;
			}
		}

		// Token: 0x17001571 RID: 5489
		// (get) Token: 0x0600B38E RID: 45966 RVA: 0x0042E0E8 File Offset: 0x0042C2E8
		// (set) Token: 0x0600B38F RID: 45967 RVA: 0x0042E0F0 File Offset: 0x0042C2F0
		public DataItem<string> RecipesToLearn
		{
			get
			{
				return this.pRecipesToLearn;
			}
			set
			{
				this.pRecipesToLearn = value;
			}
		}

		// Token: 0x17001572 RID: 5490
		// (get) Token: 0x0600B390 RID: 45968 RVA: 0x0042E0F9 File Offset: 0x0042C2F9
		// (set) Token: 0x0600B391 RID: 45969 RVA: 0x0042E101 File Offset: 0x0042C301
		public DataItem<string> InstantiateOnLoad
		{
			get
			{
				return this.pInstantiateOnLoad;
			}
			set
			{
				this.pInstantiateOnLoad = value;
			}
		}

		// Token: 0x17001573 RID: 5491
		// (get) Token: 0x0600B392 RID: 45970 RVA: 0x0042E10A File Offset: 0x0042C30A
		// (set) Token: 0x0600B393 RID: 45971 RVA: 0x0042E112 File Offset: 0x0042C312
		public DataItem<string> SoundDraw
		{
			get
			{
				return this.pSoundDraw;
			}
			set
			{
				this.pSoundDraw = value;
			}
		}

		// Token: 0x17001574 RID: 5492
		// (get) Token: 0x0600B394 RID: 45972 RVA: 0x0042E11B File Offset: 0x0042C31B
		// (set) Token: 0x0600B395 RID: 45973 RVA: 0x0042E123 File Offset: 0x0042C323
		public DataItem<DamageBonusData> DamageBonus
		{
			get
			{
				return this.pDamageBonus;
			}
			set
			{
				this.pDamageBonus = value;
			}
		}

		// Token: 0x17001575 RID: 5493
		// (get) Token: 0x0600B396 RID: 45974 RVA: 0x0042E12C File Offset: 0x0042C32C
		// (set) Token: 0x0600B397 RID: 45975 RVA: 0x0042E134 File Offset: 0x0042C334
		public DataItem<ExplosionData> Explosion
		{
			get
			{
				return this.pExplosion;
			}
			set
			{
				this.pExplosion = value;
			}
		}

		// Token: 0x0600B398 RID: 45976 RVA: 0x0042E140 File Offset: 0x0042C340
		public List<IDataItem> GetDisplayValues(bool _recursive = true)
		{
			List<IDataItem> list = new List<IDataItem>();
			if (_recursive && this.pDamageBonus != null)
			{
				list.AddRange(this.pDamageBonus.Value.GetDisplayValues(true));
			}
			if (_recursive && this.pExplosion != null)
			{
				list.AddRange(this.pExplosion.Value.GetDisplayValues(true));
			}
			return list;
		}

		// Token: 0x040086AC RID: 34476
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pDelay;

		// Token: 0x040086AD RID: 34477
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pRange;

		// Token: 0x040086AE RID: 34478
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundStart;

		// Token: 0x040086AF RID: 34479
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundRepeat;

		// Token: 0x040086B0 RID: 34480
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundEnd;

		// Token: 0x040086B1 RID: 34481
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundEmpty;

		// Token: 0x040086B2 RID: 34482
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundReload;

		// Token: 0x040086B3 RID: 34483
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundWarning;

		// Token: 0x040086B4 RID: 34484
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pStaminaUsage;

		// Token: 0x040086B5 RID: 34485
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pUseTime;

		// Token: 0x040086B6 RID: 34486
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname1;

		// Token: 0x040086B7 RID: 34487
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname2;

		// Token: 0x040086B8 RID: 34488
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname3;

		// Token: 0x040086B9 RID: 34489
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname4;

		// Token: 0x040086BA RID: 34490
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname5;

		// Token: 0x040086BB RID: 34491
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname6;

		// Token: 0x040086BC RID: 34492
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname7;

		// Token: 0x040086BD RID: 34493
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname8;

		// Token: 0x040086BE RID: 34494
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFocusedBlockname9;

		// Token: 0x040086BF RID: 34495
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pChangeItemTo;

		// Token: 0x040086C0 RID: 34496
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pChangeBlockTo;

		// Token: 0x040086C1 RID: 34497
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDoBlockAction;

		// Token: 0x040086C2 RID: 34498
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainHealth;

		// Token: 0x040086C3 RID: 34499
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainFood;

		// Token: 0x040086C4 RID: 34500
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainWater;

		// Token: 0x040086C5 RID: 34501
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainStamina;

		// Token: 0x040086C6 RID: 34502
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainSickness;

		// Token: 0x040086C7 RID: 34503
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pGainWellness;

		// Token: 0x040086C8 RID: 34504
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBuff;

		// Token: 0x040086C9 RID: 34505
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBuffChance;

		// Token: 0x040086CA RID: 34506
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDebuff;

		// Token: 0x040086CB RID: 34507
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pCreateItem;

		// Token: 0x040086CC RID: 34508
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pConditionRaycastBlock;

		// Token: 0x040086CD RID: 34509
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pGainGas;

		// Token: 0x040086CE RID: 34510
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pConsume;

		// Token: 0x040086CF RID: 34511
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBlockname;

		// Token: 0x040086D0 RID: 34512
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pThrowStrengthDefault;

		// Token: 0x040086D1 RID: 34513
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pThrowStrengthMax;

		// Token: 0x040086D2 RID: 34514
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pMaxStrainTime;

		// Token: 0x040086D3 RID: 34515
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pMagazineSize;

		// Token: 0x040086D4 RID: 34516
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pMagazineItem;

		// Token: 0x040086D5 RID: 34517
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pReloadTime;

		// Token: 0x040086D6 RID: 34518
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBulletIcon;

		// Token: 0x040086D7 RID: 34519
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pRaysPerShot;

		// Token: 0x040086D8 RID: 34520
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pRaysSpread;

		// Token: 0x040086D9 RID: 34521
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pSphere;

		// Token: 0x040086DA RID: 34522
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pCrosshairMinDistance;

		// Token: 0x040086DB RID: 34523
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pCrosshairMaxDistance;

		// Token: 0x040086DC RID: 34524
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pDamageEntity;

		// Token: 0x040086DD RID: 34525
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pDamageBlock;

		// Token: 0x040086DE RID: 34526
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pParticlesMuzzleFire;

		// Token: 0x040086DF RID: 34527
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pParticlesMuzzleSmoke;

		// Token: 0x040086E0 RID: 34528
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pBlockRange;

		// Token: 0x040086E1 RID: 34529
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pAutoFire;

		// Token: 0x040086E2 RID: 34530
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pHordeMeterRate;

		// Token: 0x040086E3 RID: 34531
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pHordeMeterDistance;

		// Token: 0x040086E4 RID: 34532
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pHitmaskOverride;

		// Token: 0x040086E5 RID: 34533
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pSingleMagazineUsage;

		// Token: 0x040086E6 RID: 34534
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pBulletMaterial;

		// Token: 0x040086E7 RID: 34535
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<bool> pInfiniteAmmo;

		// Token: 0x040086E8 RID: 34536
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pZoomMaxOut;

		// Token: 0x040086E9 RID: 34537
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pZoomMaxIn;

		// Token: 0x040086EA RID: 34538
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pZoomOverlay;

		// Token: 0x040086EB RID: 34539
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pVelocity;

		// Token: 0x040086EC RID: 34540
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pFlyTime;

		// Token: 0x040086ED RID: 34541
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pLifeTime;

		// Token: 0x040086EE RID: 34542
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<float> pCollisionRadius;

		// Token: 0x040086EF RID: 34543
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pProjectileInitialVelocity;

		// Token: 0x040086F0 RID: 34544
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pFertileblock;

		// Token: 0x040086F1 RID: 34545
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pAdjacentblock;

		// Token: 0x040086F2 RID: 34546
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pRepairAmount;

		// Token: 0x040086F3 RID: 34547
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pUpgradeHitOffset;

		// Token: 0x040086F4 RID: 34548
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pAllowedUpgradeItems;

		// Token: 0x040086F5 RID: 34549
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pRestrictedUpgradeItems;

		// Token: 0x040086F6 RID: 34550
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pUpgradeActionSound;

		// Token: 0x040086F7 RID: 34551
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pRepairActionSound;

		// Token: 0x040086F8 RID: 34552
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pReferenceItem;

		// Token: 0x040086F9 RID: 34553
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pMesh;

		// Token: 0x040086FA RID: 34554
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<int> pActionIdx;

		// Token: 0x040086FB RID: 34555
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pTitle;

		// Token: 0x040086FC RID: 34556
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pDescription;

		// Token: 0x040086FD RID: 34557
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pRecipesToLearn;

		// Token: 0x040086FE RID: 34558
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pInstantiateOnLoad;

		// Token: 0x040086FF RID: 34559
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<string> pSoundDraw;

		// Token: 0x04008700 RID: 34560
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<DamageBonusData> pDamageBonus;

		// Token: 0x04008701 RID: 34561
		[PublicizedFrom(EAccessModifier.Private)]
		public DataItem<ExplosionData> pExplosion;

		// Token: 0x02001648 RID: 5704
		public static class Parser
		{
			// Token: 0x0600B39A RID: 45978 RVA: 0x0042E1A4 File Offset: 0x0042C3A4
			[PublicizedFrom(EAccessModifier.Private)]
			public static DataItem<string> ParseItem(string _string, PositionXmlElement _elem)
			{
				string startValue;
				try
				{
					startValue = stringParser.Parse(ParserUtils.ParseStringAttribute(_elem, "value", true, null));
				}
				catch (Exception innerException)
				{
					throw new InvalidValueException(string.Concat(new string[]
					{
						"Could not parse attribute \"",
						_elem.Name,
						"\" value \"",
						ParserUtils.ParseStringAttribute(_elem, "value", true, null),
						"\""
					}), _elem.LineNumber, innerException);
				}
				return new DataItem<string>(_string, startValue);
			}

			// Token: 0x0600B39B RID: 45979 RVA: 0x0042E228 File Offset: 0x0042C428
			public static ItemAction Parse(PositionXmlElement _elem, Dictionary<PositionXmlElement, DataItem<ItemClass>> _updateLater)
			{
				string text = _elem.HasAttribute("class") ? _elem.GetAttribute("class") : "ItemAction";
				Type type = Type.GetType(typeof(ItemActionData.Parser).Namespace + "." + text);
				if (type == null)
				{
					type = Type.GetType(text);
					if (type == null)
					{
						throw new InvalidValueException("Specified class \"" + text + "\" not found", _elem.LineNumber);
					}
				}
				ItemAction itemAction = (ItemAction)Activator.CreateInstance(type);
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (object obj in _elem.ChildNodes)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlNodeType nodeType = xmlNode.NodeType;
					if (nodeType != XmlNodeType.Element)
					{
						if (nodeType != XmlNodeType.Comment)
						{
							throw new UnexpectedElementException("Unknown node \"" + xmlNode.NodeType.ToString() + "\" found while parsing ItemAction", ((IXmlLineInfo)xmlNode).LineNumber);
						}
					}
					else
					{
						PositionXmlElement positionXmlElement = (PositionXmlElement)xmlNode;
						if (!ItemActionData.Parser.knownAttributesMultiplicity.ContainsKey(positionXmlElement.Name))
						{
							throw new UnexpectedElementException("Unknown element \"" + xmlNode.Name + "\" found while parsing ItemAction", ((IXmlLineInfo)xmlNode).LineNumber);
						}
						string name = positionXmlElement.Name;
						uint num = <PrivateImplementationDetails>.ComputeStringHash(name);
						if (num <= 2050383924U)
						{
							if (num <= 973038343U)
							{
								if (num <= 477934085U)
								{
									if (num <= 264004213U)
									{
										if (num <= 91459346U)
										{
											if (num != 32426346U)
											{
												if (num == 91459346U)
												{
													if (name == "CrosshairMinDistance")
													{
														int startValue;
														try
														{
															startValue = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
														}
														catch (Exception innerException)
														{
															throw new InvalidValueException(string.Concat(new string[]
															{
																"Could not parse attribute \"",
																positionXmlElement.Name,
																"\" value \"",
																ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
																"\""
															}), positionXmlElement.LineNumber, innerException);
														}
														DataItem<int> pCrosshairMinDistance = new DataItem<int>("CrosshairMinDistance", startValue);
														itemAction.pCrosshairMinDistance = pCrosshairMinDistance;
													}
												}
											}
											else if (name == "HitmaskOverride")
											{
												string startValue2;
												try
												{
													startValue2 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException2)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException2);
												}
												DataItem<string> pHitmaskOverride = new DataItem<string>("HitmaskOverride", startValue2);
												itemAction.pHitmaskOverride = pHitmaskOverride;
											}
										}
										else if (num != 132142556U)
										{
											if (num != 248841283U)
											{
												if (num == 264004213U)
												{
													if (name == "ProjectileInitialVelocity")
													{
														int startValue3;
														try
														{
															startValue3 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
														}
														catch (Exception innerException3)
														{
															throw new InvalidValueException(string.Concat(new string[]
															{
																"Could not parse attribute \"",
																positionXmlElement.Name,
																"\" value \"",
																ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
																"\""
															}), positionXmlElement.LineNumber, innerException3);
														}
														DataItem<int> pProjectileInitialVelocity = new DataItem<int>("ProjectileInitialVelocity", startValue3);
														itemAction.pProjectileInitialVelocity = pProjectileInitialVelocity;
													}
												}
											}
											else if (name == "RaysSpread")
											{
												float startValue4;
												try
												{
													startValue4 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException4)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException4);
												}
												DataItem<float> pRaysSpread = new DataItem<float>("RaysSpread", startValue4);
												itemAction.pRaysSpread = pRaysSpread;
											}
										}
										else if (name == "Mesh")
										{
											string startValue5;
											try
											{
												startValue5 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException5)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException5);
											}
											DataItem<string> pMesh = new DataItem<string>("Mesh", startValue5);
											itemAction.pMesh = pMesh;
										}
									}
									else if (num <= 388984571U)
									{
										if (num != 383713930U)
										{
											if (num == 388984571U)
											{
												if (name == "DamageEntity")
												{
													int startValue6;
													try
													{
														startValue6 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException6)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException6);
													}
													DataItem<int> pDamageEntity = new DataItem<int>("DamageEntity", startValue6);
													itemAction.pDamageEntity = pDamageEntity;
												}
											}
										}
										else if (name == "AutoFire")
										{
											bool startValue7;
											try
											{
												startValue7 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException7)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException7);
											}
											DataItem<bool> pAutoFire = new DataItem<bool>("AutoFire", startValue7);
											itemAction.pAutoFire = pAutoFire;
										}
									}
									else if (num != 422257095U)
									{
										if (num != 441323083U)
										{
											if (num == 477934085U)
											{
												if (name == "MaxStrainTime")
												{
													float startValue8;
													try
													{
														startValue8 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException8)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException8);
													}
													DataItem<float> pMaxStrainTime = new DataItem<float>("MaxStrainTime", startValue8);
													itemAction.pMaxStrainTime = pMaxStrainTime;
												}
											}
										}
										else if (name == "RaysPerShot")
										{
											int startValue9;
											try
											{
												startValue9 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException9)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException9);
											}
											DataItem<int> pRaysPerShot = new DataItem<int>("RaysPerShot", startValue9);
											itemAction.pRaysPerShot = pRaysPerShot;
										}
									}
									else if (name == "GainGas")
									{
										int startValue10;
										try
										{
											startValue10 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException10)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException10);
										}
										DataItem<int> pGainGas = new DataItem<int>("GainGas", startValue10);
										itemAction.pGainGas = pGainGas;
									}
								}
								else if (num <= 678358751U)
								{
									if (num <= 573162709U)
									{
										if (num != 550270072U)
										{
											if (num == 573162709U)
											{
												if (name == "SoundEmpty")
												{
													string startValue11;
													try
													{
														startValue11 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException11)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException11);
													}
													DataItem<string> pSoundEmpty = new DataItem<string>("SoundEmpty", startValue11);
													itemAction.pSoundEmpty = pSoundEmpty;
												}
											}
										}
										else if (name == "Delay")
										{
											float startValue12;
											try
											{
												startValue12 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException12)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException12);
											}
											DataItem<float> pDelay = new DataItem<float>("Delay", startValue12);
											itemAction.pDelay = pDelay;
										}
									}
									else if (num != 573416248U)
									{
										if (num != 617902505U)
										{
											if (num == 678358751U)
											{
												if (name == "GainStamina")
												{
													float startValue13;
													try
													{
														startValue13 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException13)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException13);
													}
													DataItem<float> pGainStamina = new DataItem<float>("GainStamina", startValue13);
													itemAction.pGainStamina = pGainStamina;
												}
											}
										}
										else if (name == "Title")
										{
											string startValue14;
											try
											{
												startValue14 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException14)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException14);
											}
											DataItem<string> pTitle = new DataItem<string>("Title", startValue14);
											itemAction.pTitle = pTitle;
										}
									}
									else if (name == "Sphere")
									{
										float startValue15;
										try
										{
											startValue15 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException15)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException15);
										}
										DataItem<float> pSphere = new DataItem<float>("Sphere", startValue15);
										itemAction.pSphere = pSphere;
									}
								}
								else if (num <= 859861643U)
								{
									if (num != 738624775U)
									{
										if (num != 843848237U)
										{
											if (num == 859861643U)
											{
												if (name == "SoundRepeat")
												{
													string startValue16;
													try
													{
														startValue16 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException16)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException16);
													}
													DataItem<string> pSoundRepeat = new DataItem<string>("SoundRepeat", startValue16);
													itemAction.pSoundRepeat = pSoundRepeat;
												}
											}
										}
										else if (name == "CollisionRadius")
										{
											float startValue17;
											try
											{
												startValue17 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException17)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException17);
											}
											DataItem<float> pCollisionRadius = new DataItem<float>("CollisionRadius", startValue17);
											itemAction.pCollisionRadius = pCollisionRadius;
										}
									}
									else if (name == "ThrowStrengthDefault")
									{
										float startValue18;
										try
										{
											startValue18 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException18)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException18);
										}
										DataItem<float> pThrowStrengthDefault = new DataItem<float>("ThrowStrengthDefault", startValue18);
										itemAction.pThrowStrengthDefault = pThrowStrengthDefault;
									}
								}
								else if (num != 883311634U)
								{
									if (num != 906214847U)
									{
										if (num == 973038343U)
										{
											if (name == "InfiniteAmmo")
											{
												bool startValue19;
												try
												{
													startValue19 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException19)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException19);
												}
												DataItem<bool> pInfiniteAmmo = new DataItem<bool>("InfiniteAmmo", startValue19);
												itemAction.pInfiniteAmmo = pInfiniteAmmo;
											}
										}
									}
									else if (name == "ChangeBlockTo")
									{
										string startValue20;
										try
										{
											startValue20 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException20)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException20);
										}
										DataItem<string> pChangeBlockTo = new DataItem<string>("ChangeBlockTo", startValue20);
										itemAction.pChangeBlockTo = pChangeBlockTo;
									}
								}
								else if (name == "ZoomOverlay")
								{
									string startValue21;
									try
									{
										startValue21 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException21)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException21);
									}
									DataItem<string> pZoomOverlay = new DataItem<string>("ZoomOverlay", startValue21);
									itemAction.pZoomOverlay = pZoomOverlay;
								}
							}
							else if (num <= 1725856265U)
							{
								if (num <= 1237474039U)
								{
									if (num <= 1173181999U)
									{
										if (num != 1157581142U)
										{
											if (num == 1173181999U)
											{
												if (name == "BlockRange")
												{
													float startValue22;
													try
													{
														startValue22 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException22)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException22);
													}
													DataItem<float> pBlockRange = new DataItem<float>("BlockRange", startValue22);
													itemAction.pBlockRange = pBlockRange;
												}
											}
										}
										else if (name == "SingleMagazineUsage")
										{
											bool startValue23;
											try
											{
												startValue23 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException23)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException23);
											}
											DataItem<bool> pSingleMagazineUsage = new DataItem<bool>("SingleMagazineUsage", startValue23);
											itemAction.pSingleMagazineUsage = pSingleMagazineUsage;
										}
									}
									else if (num != 1180803064U)
									{
										if (num != 1221226493U)
										{
											if (num == 1237474039U)
											{
												if (name == "DamageBonus")
												{
													DamageBonusData startValue24 = DamageBonusData.Parser.Parse(positionXmlElement, _updateLater);
													DataItem<DamageBonusData> pDamageBonus = new DataItem<DamageBonusData>("DamageBonus", startValue24);
													itemAction.pDamageBonus = pDamageBonus;
												}
											}
										}
										else if (name == "RecipesToLearn")
										{
											string startValue25;
											try
											{
												startValue25 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException24)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException24);
											}
											DataItem<string> pRecipesToLearn = new DataItem<string>("RecipesToLearn", startValue25);
											itemAction.pRecipesToLearn = pRecipesToLearn;
										}
									}
									else if (name == "SoundStart")
									{
										string startValue26;
										try
										{
											startValue26 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException25)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException25);
										}
										DataItem<string> pSoundStart = new DataItem<string>("SoundStart", startValue26);
										itemAction.pSoundStart = pSoundStart;
									}
								}
								else if (num <= 1330610628U)
								{
									if (num != 1259223448U)
									{
										if (num != 1292404073U)
										{
											if (num == 1330610628U)
											{
												if (name == "CrosshairMaxDistance")
												{
													int startValue27;
													try
													{
														startValue27 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException26)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException26);
													}
													DataItem<int> pCrosshairMaxDistance = new DataItem<int>("CrosshairMaxDistance", startValue27);
													itemAction.pCrosshairMaxDistance = pCrosshairMaxDistance;
												}
											}
										}
										else if (name == "SoundEnd")
										{
											string startValue28;
											try
											{
												startValue28 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException27)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException27);
											}
											DataItem<string> pSoundEnd = new DataItem<string>("SoundEnd", startValue28);
											itemAction.pSoundEnd = pSoundEnd;
										}
									}
									else if (name == "ZoomMaxOut")
									{
										float startValue29;
										try
										{
											startValue29 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException28)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException28);
										}
										DataItem<float> pZoomMaxOut = new DataItem<float>("ZoomMaxOut", startValue29);
										itemAction.pZoomMaxOut = pZoomMaxOut;
									}
								}
								else if (num != 1601156781U)
								{
									if (num != 1660679176U)
									{
										if (num == 1725856265U)
										{
											if (name == "Description")
											{
												string startValue30;
												try
												{
													startValue30 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException29)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException29);
												}
												DataItem<string> pDescription = new DataItem<string>("Description", startValue30);
												itemAction.pDescription = pDescription;
											}
										}
									}
									else if (name == "BuffChance")
									{
										string startValue31;
										try
										{
											startValue31 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException30)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException30);
										}
										DataItem<string> pBuffChance = new DataItem<string>("BuffChance", startValue31);
										itemAction.pBuffChance = pBuffChance;
									}
								}
								else if (name == "ReloadTime")
								{
									float startValue32;
									try
									{
										startValue32 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException31)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException31);
									}
									DataItem<float> pReloadTime = new DataItem<float>("ReloadTime", startValue32);
									itemAction.pReloadTime = pReloadTime;
								}
							}
							else if (num <= 1811911295U)
							{
								if (num <= 1744800819U)
								{
									if (num != 1728023200U)
									{
										if (num == 1744800819U)
										{
											if (name == "FocusedBlockname5")
											{
												string startValue33;
												try
												{
													startValue33 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException32)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException32);
												}
												DataItem<string> pFocusedBlockname = new DataItem<string>("FocusedBlockname5", startValue33);
												itemAction.pFocusedBlockname5 = pFocusedBlockname;
											}
										}
									}
									else if (name == "FocusedBlockname4")
									{
										string startValue34;
										try
										{
											startValue34 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException33)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException33);
										}
										DataItem<string> pFocusedBlockname2 = new DataItem<string>("FocusedBlockname4", startValue34);
										itemAction.pFocusedBlockname4 = pFocusedBlockname2;
									}
								}
								else if (num != 1761578438U)
								{
									if (num != 1778356057U)
									{
										if (num == 1811911295U)
										{
											if (name == "FocusedBlockname1")
											{
												string startValue35;
												try
												{
													startValue35 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException34)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException34);
												}
												DataItem<string> pFocusedBlockname3 = new DataItem<string>("FocusedBlockname1", startValue35);
												itemAction.pFocusedBlockname1 = pFocusedBlockname3;
											}
										}
									}
									else if (name == "FocusedBlockname7")
									{
										string startValue36;
										try
										{
											startValue36 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException35)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException35);
										}
										DataItem<string> pFocusedBlockname4 = new DataItem<string>("FocusedBlockname7", startValue36);
										itemAction.pFocusedBlockname7 = pFocusedBlockname4;
									}
								}
								else if (name == "FocusedBlockname6")
								{
									string startValue37;
									try
									{
										startValue37 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException36)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException36);
									}
									DataItem<string> pFocusedBlockname5 = new DataItem<string>("FocusedBlockname6", startValue37);
									itemAction.pFocusedBlockname6 = pFocusedBlockname5;
								}
							}
							else if (num <= 1929345774U)
							{
								if (num != 1828688914U)
								{
									if (num != 1845466533U)
									{
										if (num == 1929345774U)
										{
											if (name == "LifeTime")
											{
												float startValue38;
												try
												{
													startValue38 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException37)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException37);
												}
												DataItem<float> pLifeTime = new DataItem<float>("LifeTime", startValue38);
												itemAction.pLifeTime = pLifeTime;
											}
										}
									}
									else if (name == "FocusedBlockname3")
									{
										string startValue39;
										try
										{
											startValue39 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException38)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException38);
										}
										DataItem<string> pFocusedBlockname6 = new DataItem<string>("FocusedBlockname3", startValue39);
										itemAction.pFocusedBlockname3 = pFocusedBlockname6;
									}
								}
								else if (name == "FocusedBlockname2")
								{
									string startValue40;
									try
									{
										startValue40 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException39)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException39);
									}
									DataItem<string> pFocusedBlockname7 = new DataItem<string>("FocusedBlockname2", startValue40);
									itemAction.pFocusedBlockname2 = pFocusedBlockname7;
								}
							}
							else if (num != 1929354628U)
							{
								if (num != 1946132247U)
								{
									if (num == 2050383924U)
									{
										if (name == "RestrictedUpgradeItems")
										{
											string startValue41;
											try
											{
												startValue41 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException40)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException40);
											}
											DataItem<string> pRestrictedUpgradeItems = new DataItem<string>("RestrictedUpgradeItems", startValue41);
											itemAction.pRestrictedUpgradeItems = pRestrictedUpgradeItems;
										}
									}
								}
								else if (name == "FocusedBlockname9")
								{
									string startValue42;
									try
									{
										startValue42 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException41)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException41);
									}
									DataItem<string> pFocusedBlockname8 = new DataItem<string>("FocusedBlockname9", startValue42);
									itemAction.pFocusedBlockname9 = pFocusedBlockname8;
								}
							}
							else if (name == "FocusedBlockname8")
							{
								string startValue43;
								try
								{
									startValue43 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException42)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException42);
								}
								DataItem<string> pFocusedBlockname9 = new DataItem<string>("FocusedBlockname8", startValue43);
								itemAction.pFocusedBlockname8 = pFocusedBlockname9;
							}
						}
						else if (num <= 3213271394U)
						{
							if (num <= 2731875518U)
							{
								if (num <= 2292684416U)
								{
									if (num <= 2079371571U)
									{
										if (num != 2054944794U)
										{
											if (num == 2079371571U)
											{
												if (name == "Fertileblock")
												{
													string startValue44;
													try
													{
														startValue44 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
													}
													catch (Exception innerException43)
													{
														throw new InvalidValueException(string.Concat(new string[]
														{
															"Could not parse attribute \"",
															positionXmlElement.Name,
															"\" value \"",
															ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
															"\""
														}), positionXmlElement.LineNumber, innerException43);
													}
													DataItem<string> pFertileblock = new DataItem<string>("Fertileblock", startValue44);
													itemAction.pFertileblock = pFertileblock;
												}
											}
										}
										else if (name == "ParticlesMuzzleSmoke")
										{
											string startValue45;
											try
											{
												startValue45 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException44)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException44);
											}
											DataItem<string> pParticlesMuzzleSmoke = new DataItem<string>("ParticlesMuzzleSmoke", startValue45);
											itemAction.pParticlesMuzzleSmoke = pParticlesMuzzleSmoke;
										}
									}
									else if (num != 2205678605U)
									{
										if (num != 2214691755U)
										{
											if (num == 2292684416U)
											{
												if (name == "Explosion")
												{
													ExplosionData startValue46 = ExplosionData.Parser.Parse(positionXmlElement, _updateLater);
													DataItem<ExplosionData> pExplosion = new DataItem<ExplosionData>("Explosion", startValue46);
													itemAction.pExplosion = pExplosion;
												}
											}
										}
										else if (name == "GainSickness")
										{
											float startValue47;
											try
											{
												startValue47 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException45)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException45);
											}
											DataItem<float> pGainSickness = new DataItem<float>("GainSickness", startValue47);
											itemAction.pGainSickness = pGainSickness;
										}
									}
									else if (name == "ParticlesMuzzleFire")
									{
										string startValue48;
										try
										{
											startValue48 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException46)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException46);
										}
										DataItem<string> pParticlesMuzzleFire = new DataItem<string>("ParticlesMuzzleFire", startValue48);
										itemAction.pParticlesMuzzleFire = pParticlesMuzzleFire;
									}
								}
								else if (num <= 2391273097U)
								{
									if (num != 2296213333U)
									{
										if (num == 2391273097U)
										{
											if (name == "GainWater")
											{
												float startValue49;
												try
												{
													startValue49 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException47)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException47);
												}
												DataItem<float> pGainWater = new DataItem<float>("GainWater", startValue49);
												itemAction.pGainWater = pGainWater;
											}
										}
									}
									else if (name == "UseTime")
									{
										string startValue50;
										try
										{
											startValue50 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException48)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException48);
										}
										DataItem<string> pUseTime = new DataItem<string>("UseTime", startValue50);
										itemAction.pUseTime = pUseTime;
									}
								}
								else if (num != 2508081957U)
								{
									if (num != 2654093974U)
									{
										if (num == 2731875518U)
										{
											if (name == "ActionIdx")
											{
												int startValue51;
												try
												{
													startValue51 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException49)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException49);
												}
												DataItem<int> pActionIdx = new DataItem<int>("ActionIdx", startValue51);
												itemAction.pActionIdx = pActionIdx;
											}
										}
									}
									else if (name == "SoundDraw")
									{
										string startValue52;
										try
										{
											startValue52 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException50)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException50);
										}
										DataItem<string> pSoundDraw = new DataItem<string>("SoundDraw", startValue52);
										itemAction.pSoundDraw = pSoundDraw;
									}
								}
								else if (name == "RepairActionSound")
								{
									string startValue53;
									try
									{
										startValue53 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException51)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException51);
									}
									DataItem<string> pRepairActionSound = new DataItem<string>("RepairActionSound", startValue53);
									itemAction.pRepairActionSound = pRepairActionSound;
								}
							}
							else if (num <= 2951397452U)
							{
								if (num <= 2776146097U)
								{
									if (num != 2735859570U)
									{
										if (num == 2776146097U)
										{
											if (name == "GainWellness")
											{
												float startValue54;
												try
												{
													startValue54 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException52)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException52);
												}
												DataItem<float> pGainWellness = new DataItem<float>("GainWellness", startValue54);
												itemAction.pGainWellness = pGainWellness;
											}
										}
									}
									else if (name == "Range")
									{
										float startValue55;
										try
										{
											startValue55 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException53)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException53);
										}
										DataItem<float> pRange = new DataItem<float>("Range", startValue55);
										itemAction.pRange = pRange;
									}
								}
								else if (num != 2889970889U)
								{
									if (num != 2916596296U)
									{
										if (num == 2951397452U)
										{
											if (name == "ThrowStrengthMax")
											{
												float startValue56;
												try
												{
													startValue56 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException54)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException54);
												}
												DataItem<float> pThrowStrengthMax = new DataItem<float>("ThrowStrengthMax", startValue56);
												itemAction.pThrowStrengthMax = pThrowStrengthMax;
											}
										}
									}
									else if (name == "SoundWarning")
									{
										string startValue57;
										try
										{
											startValue57 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException55)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException55);
										}
										DataItem<string> pSoundWarning = new DataItem<string>("SoundWarning", startValue57);
										itemAction.pSoundWarning = pSoundWarning;
									}
								}
								else if (name == "Blockname")
								{
									string startValue58;
									try
									{
										startValue58 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException56)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException56);
									}
									DataItem<string> pBlockname = new DataItem<string>("Blockname", startValue58);
									itemAction.pBlockname = pBlockname;
								}
							}
							else if (num <= 3036802414U)
							{
								if (num != 2970340076U)
								{
									if (num != 3027266612U)
									{
										if (num == 3036802414U)
										{
											if (name == "GainFood")
											{
												float startValue59;
												try
												{
													startValue59 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException57)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException57);
												}
												DataItem<float> pGainFood = new DataItem<float>("GainFood", startValue59);
												itemAction.pGainFood = pGainFood;
											}
										}
									}
									else if (name == "HordeMeterRate")
									{
										float startValue60;
										try
										{
											startValue60 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException58)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException58);
										}
										DataItem<float> pHordeMeterRate = new DataItem<float>("HordeMeterRate", startValue60);
										itemAction.pHordeMeterRate = pHordeMeterRate;
									}
								}
								else if (name == "Buff")
								{
									string startValue61;
									try
									{
										startValue61 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException59)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException59);
									}
									DataItem<string> pBuff = new DataItem<string>("Buff", startValue61);
									itemAction.pBuff = pBuff;
								}
							}
							else if (num != 3118731031U)
							{
								if (num != 3124789842U)
								{
									if (num == 3213271394U)
									{
										if (name == "MagazineSize")
										{
											int startValue62;
											try
											{
												startValue62 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException60)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException60);
											}
											DataItem<int> pMagazineSize = new DataItem<int>("MagazineSize", startValue62);
											itemAction.pMagazineSize = pMagazineSize;
										}
									}
								}
								else if (name == "Velocity")
								{
									int startValue63;
									try
									{
										startValue63 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException61)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException61);
									}
									DataItem<int> pVelocity = new DataItem<int>("Velocity", startValue63);
									itemAction.pVelocity = pVelocity;
								}
							}
							else if (name == "DoBlockAction")
							{
								itemAction.pDoBlockAction = ItemActionData.Parser.ParseItem("DoBlockAction", positionXmlElement);
							}
						}
						else if (num <= 3880205310U)
						{
							if (num <= 3448204316U)
							{
								if (num <= 3261865014U)
								{
									if (num != 3239575924U)
									{
										if (num == 3261865014U)
										{
											if (name == "ConditionRaycastBlock")
											{
												int startValue64;
												try
												{
													startValue64 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException62)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException62);
												}
												DataItem<int> pConditionRaycastBlock = new DataItem<int>("ConditionRaycastBlock", startValue64);
												itemAction.pConditionRaycastBlock = pConditionRaycastBlock;
											}
										}
									}
									else if (name == "BulletMaterial")
									{
										string startValue65;
										try
										{
											startValue65 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException63)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException63);
										}
										DataItem<string> pBulletMaterial = new DataItem<string>("BulletMaterial", startValue65);
										itemAction.pBulletMaterial = pBulletMaterial;
									}
								}
								else if (num != 3297724363U)
								{
									if (num != 3370626489U)
									{
										if (num == 3448204316U)
										{
											if (name == "GainHealth")
											{
												float startValue66;
												try
												{
													startValue66 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException64)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException64);
												}
												DataItem<float> pGainHealth = new DataItem<float>("GainHealth", startValue66);
												itemAction.pGainHealth = pGainHealth;
											}
										}
									}
									else if (name == "SoundReload")
									{
										string startValue67;
										try
										{
											startValue67 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException65)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException65);
										}
										DataItem<string> pSoundReload = new DataItem<string>("SoundReload", startValue67);
										itemAction.pSoundReload = pSoundReload;
									}
								}
								else if (name == "ZoomMaxIn")
								{
									float startValue68;
									try
									{
										startValue68 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException66)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException66);
									}
									DataItem<float> pZoomMaxIn = new DataItem<float>("ZoomMaxIn", startValue68);
									itemAction.pZoomMaxIn = pZoomMaxIn;
								}
							}
							else if (num <= 3781194609U)
							{
								if (num != 3646476408U)
								{
									if (num != 3699885409U)
									{
										if (num == 3781194609U)
										{
											if (name == "ReferenceItem")
											{
												string startValue69;
												try
												{
													startValue69 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
												}
												catch (Exception innerException67)
												{
													throw new InvalidValueException(string.Concat(new string[]
													{
														"Could not parse attribute \"",
														positionXmlElement.Name,
														"\" value \"",
														ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
														"\""
													}), positionXmlElement.LineNumber, innerException67);
												}
												DataItem<string> pReferenceItem = new DataItem<string>("ReferenceItem", startValue69);
												itemAction.pReferenceItem = pReferenceItem;
											}
										}
									}
									else if (name == "Consume")
									{
										bool startValue70;
										try
										{
											startValue70 = boolParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException68)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException68);
										}
										DataItem<bool> pConsume = new DataItem<bool>("Consume", startValue70);
										itemAction.pConsume = pConsume;
									}
								}
								else if (name == "RepairAmount")
								{
									int startValue71;
									try
									{
										startValue71 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException69)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException69);
									}
									DataItem<int> pRepairAmount = new DataItem<int>("RepairAmount", startValue71);
									itemAction.pRepairAmount = pRepairAmount;
								}
							}
							else if (num != 3788546643U)
							{
								if (num != 3860496195U)
								{
									if (num == 3880205310U)
									{
										if (name == "CreateItem")
										{
											string startValue72;
											try
											{
												startValue72 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException70)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException70);
											}
											DataItem<string> pCreateItem = new DataItem<string>("CreateItem", startValue72);
											itemAction.pCreateItem = pCreateItem;
										}
									}
								}
								else if (name == "ChangeItemTo")
								{
									string startValue73;
									try
									{
										startValue73 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException71)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException71);
									}
									DataItem<string> pChangeItemTo = new DataItem<string>("ChangeItemTo", startValue73);
									itemAction.pChangeItemTo = pChangeItemTo;
								}
							}
							else if (name == "AllowedUpgradeItems")
							{
								string startValue74;
								try
								{
									startValue74 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException72)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException72);
								}
								DataItem<string> pAllowedUpgradeItems = new DataItem<string>("AllowedUpgradeItems", startValue74);
								itemAction.pAllowedUpgradeItems = pAllowedUpgradeItems;
							}
						}
						else if (num <= 4005067140U)
						{
							if (num <= 3894590787U)
							{
								if (num != 3893059105U)
								{
									if (num == 3894590787U)
									{
										if (name == "FlyTime")
										{
											float startValue75;
											try
											{
												startValue75 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException73)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException73);
											}
											DataItem<float> pFlyTime = new DataItem<float>("FlyTime", startValue75);
											itemAction.pFlyTime = pFlyTime;
										}
									}
								}
								else if (name == "StaminaUsage")
								{
									string startValue76;
									try
									{
										startValue76 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException74)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException74);
									}
									DataItem<string> pStaminaUsage = new DataItem<string>("StaminaUsage", startValue76);
									itemAction.pStaminaUsage = pStaminaUsage;
								}
							}
							else if (num != 3922517809U)
							{
								if (num != 3965826732U)
								{
									if (num == 4005067140U)
									{
										if (name == "MagazineItem")
										{
											string startValue77;
											try
											{
												startValue77 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException75)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException75);
											}
											DataItem<string> pMagazineItem = new DataItem<string>("MagazineItem", startValue77);
											itemAction.pMagazineItem = pMagazineItem;
										}
									}
								}
								else if (name == "UpgradeActionSound")
								{
									string startValue78;
									try
									{
										startValue78 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException76)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException76);
									}
									DataItem<string> pUpgradeActionSound = new DataItem<string>("UpgradeActionSound", startValue78);
									itemAction.pUpgradeActionSound = pUpgradeActionSound;
								}
							}
							else if (name == "DamageBlock")
							{
								float startValue79;
								try
								{
									startValue79 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException77)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException77);
								}
								DataItem<float> pDamageBlock = new DataItem<float>("DamageBlock", startValue79);
								itemAction.pDamageBlock = pDamageBlock;
							}
						}
						else if (num <= 4124220918U)
						{
							if (num != 4013882141U)
							{
								if (num != 4069726011U)
								{
									if (num == 4124220918U)
									{
										if (name == "InstantiateOnLoad")
										{
											string startValue80;
											try
											{
												startValue80 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
											}
											catch (Exception innerException78)
											{
												throw new InvalidValueException(string.Concat(new string[]
												{
													"Could not parse attribute \"",
													positionXmlElement.Name,
													"\" value \"",
													ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
													"\""
												}), positionXmlElement.LineNumber, innerException78);
											}
											DataItem<string> pInstantiateOnLoad = new DataItem<string>("InstantiateOnLoad", startValue80);
											itemAction.pInstantiateOnLoad = pInstantiateOnLoad;
										}
									}
								}
								else if (name == "UpgradeHitOffset")
								{
									int startValue81;
									try
									{
										startValue81 = intParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
									}
									catch (Exception innerException79)
									{
										throw new InvalidValueException(string.Concat(new string[]
										{
											"Could not parse attribute \"",
											positionXmlElement.Name,
											"\" value \"",
											ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
											"\""
										}), positionXmlElement.LineNumber, innerException79);
									}
									DataItem<int> pUpgradeHitOffset = new DataItem<int>("UpgradeHitOffset", startValue81);
									itemAction.pUpgradeHitOffset = pUpgradeHitOffset;
								}
							}
							else if (name == "HordeMeterDistance")
							{
								float startValue82;
								try
								{
									startValue82 = floatParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException80)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException80);
								}
								DataItem<float> pHordeMeterDistance = new DataItem<float>("HordeMeterDistance", startValue82);
								itemAction.pHordeMeterDistance = pHordeMeterDistance;
							}
						}
						else if (num != 4129641136U)
						{
							if (num != 4139408471U)
							{
								if (num == 4265352596U)
								{
									if (name == "BulletIcon")
									{
										string startValue83;
										try
										{
											startValue83 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
										}
										catch (Exception innerException81)
										{
											throw new InvalidValueException(string.Concat(new string[]
											{
												"Could not parse attribute \"",
												positionXmlElement.Name,
												"\" value \"",
												ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
												"\""
											}), positionXmlElement.LineNumber, innerException81);
										}
										DataItem<string> pBulletIcon = new DataItem<string>("BulletIcon", startValue83);
										itemAction.pBulletIcon = pBulletIcon;
									}
								}
							}
							else if (name == "Debuff")
							{
								string startValue84;
								try
								{
									startValue84 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
								}
								catch (Exception innerException82)
								{
									throw new InvalidValueException(string.Concat(new string[]
									{
										"Could not parse attribute \"",
										positionXmlElement.Name,
										"\" value \"",
										ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
										"\""
									}), positionXmlElement.LineNumber, innerException82);
								}
								DataItem<string> pDebuff = new DataItem<string>("Debuff", startValue84);
								itemAction.pDebuff = pDebuff;
							}
						}
						else if (name == "Adjacentblock")
						{
							string startValue85;
							try
							{
								startValue85 = stringParser.Parse(ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null));
							}
							catch (Exception innerException83)
							{
								throw new InvalidValueException(string.Concat(new string[]
								{
									"Could not parse attribute \"",
									positionXmlElement.Name,
									"\" value \"",
									ParserUtils.ParseStringAttribute(positionXmlElement, "value", true, null),
									"\""
								}), positionXmlElement.LineNumber, innerException83);
							}
							DataItem<string> pAdjacentblock = new DataItem<string>("Adjacentblock", startValue85);
							itemAction.pAdjacentblock = pAdjacentblock;
						}
						if (!dictionary.ContainsKey(positionXmlElement.Name))
						{
							dictionary[positionXmlElement.Name] = 0;
						}
						Dictionary<string, int> dictionary2 = dictionary;
						name = positionXmlElement.Name;
						int num2 = dictionary2[name];
						dictionary2[name] = num2 + 1;
					}
				}
				foreach (KeyValuePair<string, Range<int>> keyValuePair in ItemActionData.Parser.knownAttributesMultiplicity)
				{
					int num3 = dictionary.ContainsKey(keyValuePair.Key) ? dictionary[keyValuePair.Key] : 0;
					if ((keyValuePair.Value.hasMin && num3 < keyValuePair.Value.min) || (keyValuePair.Value.hasMax && num3 > keyValuePair.Value.max))
					{
						throw new IncorrectAttributeOccurrenceException(string.Concat(new string[]
						{
							"Element has incorrect number of \"",
							keyValuePair.Key,
							"\" attribute instances, found ",
							num3.ToString(),
							", expected ",
							keyValuePair.Value.ToString()
						}), _elem.LineNumber);
					}
				}
				return itemAction;
			}

			// Token: 0x04008702 RID: 34562
			[PublicizedFrom(EAccessModifier.Private)]
			public static Dictionary<string, Range<int>> knownAttributesMultiplicity = new Dictionary<string, Range<int>>
			{
				{
					"Delay",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Range",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundStart",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundRepeat",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundEnd",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundEmpty",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundReload",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundWarning",
					new Range<int>(true, 0, true, 1)
				},
				{
					"StaminaUsage",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UseTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname1",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname2",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname3",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname4",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname5",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname6",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname7",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname8",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FocusedBlockname9",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ChangeItemTo",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ChangeBlockTo",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DoBlockAction",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainHealth",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainFood",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainWater",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainStamina",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainSickness",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainWellness",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Buff",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BuffChance",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Debuff",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CreateItem",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ConditionRaycastBlock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"GainGas",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Consume",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Blockname",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ThrowStrengthDefault",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ThrowStrengthMax",
					new Range<int>(true, 0, true, 1)
				},
				{
					"MaxStrainTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"MagazineSize",
					new Range<int>(true, 0, true, 1)
				},
				{
					"MagazineItem",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ReloadTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BulletIcon",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RaysPerShot",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RaysSpread",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Sphere",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CrosshairMinDistance",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CrosshairMaxDistance",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DamageEntity",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DamageBlock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ParticlesMuzzleFire",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ParticlesMuzzleSmoke",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BlockRange",
					new Range<int>(true, 0, true, 1)
				},
				{
					"AutoFire",
					new Range<int>(true, 0, true, 1)
				},
				{
					"HordeMeterRate",
					new Range<int>(true, 0, true, 1)
				},
				{
					"HordeMeterDistance",
					new Range<int>(true, 0, true, 1)
				},
				{
					"HitmaskOverride",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SingleMagazineUsage",
					new Range<int>(true, 0, true, 1)
				},
				{
					"BulletMaterial",
					new Range<int>(true, 0, true, 1)
				},
				{
					"InfiniteAmmo",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ZoomMaxOut",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ZoomMaxIn",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ZoomOverlay",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Velocity",
					new Range<int>(true, 0, true, 1)
				},
				{
					"FlyTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"LifeTime",
					new Range<int>(true, 0, true, 1)
				},
				{
					"CollisionRadius",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ProjectileInitialVelocity",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Fertileblock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Adjacentblock",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RepairAmount",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UpgradeHitOffset",
					new Range<int>(true, 0, true, 1)
				},
				{
					"AllowedUpgradeItems",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RestrictedUpgradeItems",
					new Range<int>(true, 0, true, 1)
				},
				{
					"UpgradeActionSound",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RepairActionSound",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ReferenceItem",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Mesh",
					new Range<int>(true, 0, true, 1)
				},
				{
					"ActionIdx",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Title",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Description",
					new Range<int>(true, 0, true, 1)
				},
				{
					"RecipesToLearn",
					new Range<int>(true, 0, true, 1)
				},
				{
					"InstantiateOnLoad",
					new Range<int>(true, 0, true, 1)
				},
				{
					"SoundDraw",
					new Range<int>(true, 0, true, 1)
				},
				{
					"DamageBonus",
					new Range<int>(true, 0, true, 1)
				},
				{
					"Explosion",
					new Range<int>(true, 0, true, 1)
				}
			};
		}
	}
}
