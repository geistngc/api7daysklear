using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Audio;
using InControl;
using Unity.Profiling;
using UnityEngine;

// Token: 0x0200114E RID: 4430
public abstract class XUiView : IXUiElement
{
	// Token: 0x17001037 RID: 4151
	// (get) Token: 0x06008C1F RID: 35871 RVA: 0x00354567 File Offset: 0x00352767
	// (set) Token: 0x06008C20 RID: 35872 RVA: 0x0035456F File Offset: 0x0035276F
	public XUiController Controller
	{
		get
		{
			return this.controller;
		}
		set
		{
			this.controller = value;
			if (this.controller.ViewComponent != this)
			{
				this.controller.ViewComponent = this;
			}
		}
	}

	// Token: 0x17001038 RID: 4152
	// (get) Token: 0x06008C21 RID: 35873
	public abstract UIRect UiRect { [PublicizedFrom(EAccessModifier.Protected)] get; }

	// Token: 0x17001039 RID: 4153
	// (get) Token: 0x06008C22 RID: 35874 RVA: 0x00354592 File Offset: 0x00352792
	public Transform UiTransform
	{
		get
		{
			return this.uiTransform;
		}
	}

	// Token: 0x1700103A RID: 4154
	// (get) Token: 0x06008C23 RID: 35875 RVA: 0x0035459A File Offset: 0x0035279A
	public bool ColliderEnabled
	{
		get
		{
			return !this.destroyed && this.collider.enabled;
		}
	}

	// Token: 0x1700103B RID: 4155
	// (get) Token: 0x06008C24 RID: 35876 RVA: 0x003545B4 File Offset: 0x003527B4
	public Vector3 ColliderCenter
	{
		get
		{
			if (!this.destroyed)
			{
				return this.collider.bounds.center;
			}
			return default(Vector3);
		}
	}

	// Token: 0x1700103C RID: 4156
	// (get) Token: 0x06008C25 RID: 35877 RVA: 0x003545E8 File Offset: 0x003527E8
	public float ColliderHeightExtent
	{
		get
		{
			if (!this.destroyed)
			{
				return this.collider.bounds.size.y / 2f;
			}
			return 0f;
		}
	}

	// Token: 0x1700103D RID: 4157
	// (get) Token: 0x06008C26 RID: 35878 RVA: 0x00354624 File Offset: 0x00352824
	public float ColliderWidthExtent
	{
		get
		{
			if (!this.destroyed)
			{
				return this.collider.bounds.size.x / 2f;
			}
			return 0f;
		}
	}

	// Token: 0x1700103E RID: 4158
	// (get) Token: 0x06008C27 RID: 35879 RVA: 0x00354660 File Offset: 0x00352860
	public Bounds ColliderBounds
	{
		get
		{
			if (!this.destroyed)
			{
				return this.collider.bounds;
			}
			return default(Bounds);
		}
	}

	// Token: 0x1700103F RID: 4159
	// (get) Token: 0x06008C28 RID: 35880 RVA: 0x0035468C File Offset: 0x0035288C
	public virtual Vector3 LocalCenter
	{
		get
		{
			return (this.position + this.size / 2).AsVector2();
		}
	}

	// Token: 0x17001040 RID: 4160
	// (get) Token: 0x06008C29 RID: 35881
	public abstract Vector3[] WorldCorners { get; }

	// Token: 0x17001041 RID: 4161
	// (get) Token: 0x06008C2A RID: 35882 RVA: 0x003546BD File Offset: 0x003528BD
	// (set) Token: 0x06008C2B RID: 35883 RVA: 0x003546C5 File Offset: 0x003528C5
	[XuiXmlAttribute("repeat_content", false)]
	public bool RepeatContent { get; set; }

	// Token: 0x17001042 RID: 4162
	// (get) Token: 0x06008C2C RID: 35884 RVA: 0x003546CE File Offset: 0x003528CE
	// (set) Token: 0x06008C2D RID: 35885 RVA: 0x003546D6 File Offset: 0x003528D6
	[XuiXmlAttribute("repeat_count", false)]
	public virtual int RepeatCount { get; set; }

	// Token: 0x17001043 RID: 4163
	// (get) Token: 0x06008C2E RID: 35886 RVA: 0x003546DF File Offset: 0x003528DF
	public bool UiTransformIsHovered
	{
		get
		{
			return UICamera.hoveredObject == this.uiTransform.gameObject;
		}
	}

	// Token: 0x17001044 RID: 4164
	// (get) Token: 0x06008C2F RID: 35887 RVA: 0x003546F6 File Offset: 0x003528F6
	public bool IsActiveInHierarchy
	{
		get
		{
			if (!(this.uiTransform != null))
			{
				return this.isVisible;
			}
			return this.uiTransform.gameObject.activeInHierarchy;
		}
	}

	// Token: 0x17001045 RID: 4165
	// (get) Token: 0x06008C30 RID: 35888 RVA: 0x0035471D File Offset: 0x0035291D
	public string ID
	{
		get
		{
			return this.id;
		}
	}

	// Token: 0x06008C31 RID: 35889 RVA: 0x00354725 File Offset: 0x00352925
	[XuiXmlAttribute("name", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeName(string _value)
	{
		this.id = _value;
	}

	// Token: 0x17001046 RID: 4166
	// (get) Token: 0x06008C32 RID: 35890 RVA: 0x0035472E File Offset: 0x0035292E
	// (set) Token: 0x06008C33 RID: 35891 RVA: 0x00354738 File Offset: 0x00352938
	[XuiXmlAttribute("visible", false)]
	public bool IsVisible
	{
		get
		{
			return this.isVisible;
		}
		set
		{
			if (this.isVisible == value)
			{
				return;
			}
			this.isVisible = value;
			if (this.uiTransform != null && this.isVisible != this.uiTransform.gameObject.activeSelf)
			{
				this.uiTransform.gameObject.SetActive(this.isVisible);
			}
			this.Controller.OnVisibilityChanged(this.IsActiveInHierarchy);
			if (this.xui.playerUI.CursorController.navigationTarget == this)
			{
				this.xui.playerUI.RefreshNavigationTarget();
			}
			this.SetDirty();
		}
	}

	// Token: 0x17001047 RID: 4167
	// (get) Token: 0x06008C34 RID: 35892 RVA: 0x003547D1 File Offset: 0x003529D1
	// (set) Token: 0x06008C35 RID: 35893 RVA: 0x003547DC File Offset: 0x003529DC
	[XuiXmlAttribute("enabled", false)]
	public virtual bool Enabled
	{
		get
		{
			return this.enabled;
		}
		set
		{
			if (value == this.enabled)
			{
				return;
			}
			this.enabled = value;
			if (!this.enabled && this.xui.playerUI.CursorController.navigationTarget == this)
			{
				this.xui.playerUI.RefreshNavigationTarget();
			}
			if (!value && this.isOver)
			{
				this.OnHover(true);
			}
			this.Controller.OnEnabledChanged(this.enabled);
			this.SetDirty();
		}
	}

	// Token: 0x17001048 RID: 4168
	// (get) Token: 0x06008C36 RID: 35894 RVA: 0x00354853 File Offset: 0x00352A53
	// (set) Token: 0x06008C37 RID: 35895 RVA: 0x0035485B File Offset: 0x00352A5B
	[XuiXmlAttribute("collider_scale", false)]
	public float ColliderScale { [PublicizedFrom(EAccessModifier.Protected)] get; [PublicizedFrom(EAccessModifier.Protected)] set; } = 1f;

	// Token: 0x17001049 RID: 4169
	// (get) Token: 0x06008C38 RID: 35896 RVA: 0x00354864 File Offset: 0x00352A64
	// (set) Token: 0x06008C39 RID: 35897 RVA: 0x0035486C File Offset: 0x00352A6C
	[XuiXmlAttribute("collider_padding", false)]
	public Vector2i ColliderPadding { [PublicizedFrom(EAccessModifier.Protected)] get; [PublicizedFrom(EAccessModifier.Protected)] set; } = Vector2i.zero;

	// Token: 0x1700104A RID: 4170
	// (get) Token: 0x06008C3A RID: 35898 RVA: 0x00354875 File Offset: 0x00352A75
	// (set) Token: 0x06008C3B RID: 35899 RVA: 0x0035487D File Offset: 0x00352A7D
	[XuiXmlAttribute("size", false)]
	public Vector2i Size
	{
		get
		{
			return this.size;
		}
		set
		{
			if (this.size == value)
			{
				return;
			}
			this.size = value;
			this.SetDirty();
		}
	}

	// Token: 0x1700104B RID: 4171
	// (get) Token: 0x06008C3C RID: 35900 RVA: 0x0035489B File Offset: 0x00352A9B
	// (set) Token: 0x06008C3D RID: 35901 RVA: 0x003548A8 File Offset: 0x00352AA8
	[XuiXmlAttribute("width", false)]
	public int Width
	{
		get
		{
			return this.Size.x;
		}
		set
		{
			if (this.Width == value)
			{
				return;
			}
			Vector2i vector2i = this.Size;
			vector2i.x = value;
			this.Size = vector2i;
		}
	}

	// Token: 0x1700104C RID: 4172
	// (get) Token: 0x06008C3E RID: 35902 RVA: 0x003548D5 File Offset: 0x00352AD5
	// (set) Token: 0x06008C3F RID: 35903 RVA: 0x003548E4 File Offset: 0x00352AE4
	[XuiXmlAttribute("height", false)]
	public int Height
	{
		get
		{
			return this.Size.y;
		}
		set
		{
			if (this.Height == value)
			{
				return;
			}
			Vector2i vector2i = this.Size;
			vector2i.y = value;
			this.Size = vector2i;
		}
	}

	// Token: 0x1700104D RID: 4173
	// (get) Token: 0x06008C40 RID: 35904 RVA: 0x00354911 File Offset: 0x00352B11
	// (set) Token: 0x06008C41 RID: 35905 RVA: 0x00354919 File Offset: 0x00352B19
	[XuiXmlAttribute("pos", false)]
	public Vector2i Position
	{
		get
		{
			return this.position;
		}
		set
		{
			if (this.position == value)
			{
				return;
			}
			this.position = value;
			this.SetDirty();
			this.positionDirty = true;
		}
	}

	// Token: 0x1700104E RID: 4174
	// (get) Token: 0x06008C42 RID: 35906 RVA: 0x0035493E File Offset: 0x00352B3E
	// (set) Token: 0x06008C43 RID: 35907 RVA: 0x00354946 File Offset: 0x00352B46
	[XuiXmlAttribute("ignoreparentpadding", false)]
	public bool IgnoreParentPadding
	{
		get
		{
			return this.ignoreParentPadding;
		}
		set
		{
			if (this.ignoreParentPadding == value)
			{
				return;
			}
			this.ignoreParentPadding = value;
			this.SetDirty();
		}
	}

	// Token: 0x1700104F RID: 4175
	// (get) Token: 0x06008C44 RID: 35908 RVA: 0x00354960 File Offset: 0x00352B60
	public Vector2i PaddedPosition
	{
		get
		{
			Vector2i one = this.position;
			Vector2i other;
			if (!this.ignoreParentPadding)
			{
				XUiController parent = this.Controller.Parent;
				Vector2i? vector2i;
				if (parent == null)
				{
					vector2i = null;
				}
				else
				{
					XUiView viewComponent = parent.ViewComponent;
					vector2i = ((viewComponent != null) ? new Vector2i?(viewComponent.InnerPosition) : null);
				}
				other = (vector2i ?? Vector2i.zero);
			}
			else
			{
				other = Vector2i.zero;
			}
			return one + other;
		}
	}

	// Token: 0x17001050 RID: 4176
	// (get) Token: 0x06008C45 RID: 35909 RVA: 0x003549D7 File Offset: 0x00352BD7
	public virtual Vector2i InnerSize
	{
		get
		{
			return new Vector2i(this.size.x - this.padding.SumLeftRight, this.size.y - this.padding.SumTopBottom);
		}
	}

	// Token: 0x17001051 RID: 4177
	// (get) Token: 0x06008C46 RID: 35910 RVA: 0x00354A0C File Offset: 0x00352C0C
	public Vector2i InnerPosition
	{
		get
		{
			return new Vector2i(this.padding.Left, -this.padding.Top);
		}
	}

	// Token: 0x17001052 RID: 4178
	// (get) Token: 0x06008C47 RID: 35911 RVA: 0x00354A2A File Offset: 0x00352C2A
	// (set) Token: 0x06008C48 RID: 35912 RVA: 0x00354A32 File Offset: 0x00352C32
	[XuiXmlAttribute("rotation", false)]
	public float Rotation
	{
		get
		{
			return this.rotation;
		}
		set
		{
			if (Mathf.Approximately(this.rotation, value))
			{
				return;
			}
			this.rotation = value;
			this.SetDirty();
			this.rotationDirty = true;
		}
	}

	// Token: 0x17001053 RID: 4179
	// (get) Token: 0x06008C49 RID: 35913 RVA: 0x00354A57 File Offset: 0x00352C57
	// (set) Token: 0x06008C4A RID: 35914 RVA: 0x00354A5F File Offset: 0x00352C5F
	public int Depth
	{
		get
		{
			return this.depth;
		}
		set
		{
			if (this.depth == value)
			{
				return;
			}
			this.depth = value;
			this.SetDirty();
		}
	}

	// Token: 0x06008C4B RID: 35915 RVA: 0x00354A78 File Offset: 0x00352C78
	[XuiXmlAttribute("depth", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeDepth(int _value)
	{
		XUiView viewComponent = this.controller.Parent.ViewComponent;
		this.Depth = _value + ((viewComponent != null) ? viewComponent.Depth : 0);
	}

	// Token: 0x06008C4C RID: 35916 RVA: 0x00354A9E File Offset: 0x00352C9E
	[XuiXmlAttribute("padding_left", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributePaddingLeft(int _value)
	{
		this.padding = this.padding.SetLeft(_value);
	}

	// Token: 0x06008C4D RID: 35917 RVA: 0x00354AB2 File Offset: 0x00352CB2
	[XuiXmlAttribute("padding_right", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributePaddingRight(int _value)
	{
		this.padding = this.padding.SetRight(_value);
	}

	// Token: 0x06008C4E RID: 35918 RVA: 0x00354AC6 File Offset: 0x00352CC6
	[XuiXmlAttribute("padding_top", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributePaddingTop(int _value)
	{
		this.padding = this.padding.SetTop(_value);
	}

	// Token: 0x06008C4F RID: 35919 RVA: 0x00354ADA File Offset: 0x00352CDA
	[XuiXmlAttribute("padding_bottom", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributePaddingBottom(int _value)
	{
		this.padding = this.padding.SetBottom(_value);
	}

	// Token: 0x06008C50 RID: 35920 RVA: 0x00354AEE File Offset: 0x00352CEE
	[XuiXmlAttribute("padding", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributePadding(string _value)
	{
		XUiSideSizes.TryParse(_value, out this.padding, "padding");
	}

	// Token: 0x06008C51 RID: 35921 RVA: 0x00354B02 File Offset: 0x00352D02
	[XuiXmlAttribute("sound", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeSound(string _value)
	{
		this.xui.LoadData<AudioClip>(_value, delegate(AudioClip _o)
		{
			this.xuiSound = _o;
		});
	}

	// Token: 0x06008C52 RID: 35922 RVA: 0x00354B1C File Offset: 0x00352D1C
	[XuiXmlAttribute("sound_play_on_hover", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeSoundOnHover(string _value)
	{
		this.xui.LoadData<AudioClip>(_value, delegate(AudioClip _o)
		{
			this.xuiHoverSound = _o;
		});
	}

	// Token: 0x17001054 RID: 4180
	// (get) Token: 0x06008C53 RID: 35923 RVA: 0x00354B36 File Offset: 0x00352D36
	// (set) Token: 0x06008C54 RID: 35924 RVA: 0x00354B3E File Offset: 0x00352D3E
	[XuiXmlAttribute("sound_play_on_press", false)]
	public bool SoundPlayOnClick { get; set; }

	// Token: 0x17001055 RID: 4181
	// (get) Token: 0x06008C55 RID: 35925 RVA: 0x00354B47 File Offset: 0x00352D47
	// (set) Token: 0x06008C56 RID: 35926 RVA: 0x00354B4F File Offset: 0x00352D4F
	public bool SoundPlayOnHover { get; set; }

	// Token: 0x17001056 RID: 4182
	// (get) Token: 0x06008C57 RID: 35927 RVA: 0x00354B58 File Offset: 0x00352D58
	// (set) Token: 0x06008C58 RID: 35928 RVA: 0x00354B60 File Offset: 0x00352D60
	[XuiXmlAttribute("sound_play_on_open", false)]
	public bool SoundPlayOnOpen { get; set; }

	// Token: 0x17001057 RID: 4183
	// (get) Token: 0x06008C59 RID: 35929 RVA: 0x00354B69 File Offset: 0x00352D69
	// (set) Token: 0x06008C5A RID: 35930 RVA: 0x00354B71 File Offset: 0x00352D71
	[XuiXmlAttribute("sound_volume", false)]
	public float SoundVolume { get; set; }

	// Token: 0x17001058 RID: 4184
	// (get) Token: 0x06008C5B RID: 35931 RVA: 0x00354B7A File Offset: 0x00352D7A
	public bool HasHoverSound
	{
		get
		{
			return this.xuiHoverSound != null;
		}
	}

	// Token: 0x17001059 RID: 4185
	// (get) Token: 0x06008C5C RID: 35932 RVA: 0x00354B88 File Offset: 0x00352D88
	// (set) Token: 0x06008C5D RID: 35933 RVA: 0x00354B90 File Offset: 0x00352D90
	[XuiXmlAttribute("hold_delay", false)]
	public float HoldDelay { [PublicizedFrom(EAccessModifier.Private)] get; [PublicizedFrom(EAccessModifier.Private)] set; } = 0.5f;

	// Token: 0x1700105A RID: 4186
	// (get) Token: 0x06008C5E RID: 35934 RVA: 0x00354B99 File Offset: 0x00352D99
	// (set) Token: 0x06008C5F RID: 35935 RVA: 0x00354BA1 File Offset: 0x00352DA1
	[XuiXmlAttribute("hold_timed_initial_interval", false)]
	public float HoldEventIntervalInitial { [PublicizedFrom(EAccessModifier.Private)] get; [PublicizedFrom(EAccessModifier.Private)] set; } = 0.5f;

	// Token: 0x1700105B RID: 4187
	// (get) Token: 0x06008C60 RID: 35936 RVA: 0x00354BAA File Offset: 0x00352DAA
	// (set) Token: 0x06008C61 RID: 35937 RVA: 0x00354BB2 File Offset: 0x00352DB2
	[XuiXmlAttribute("hold_timed_final_interval", false)]
	public float HoldEventIntervalFinal { [PublicizedFrom(EAccessModifier.Private)] get; [PublicizedFrom(EAccessModifier.Private)] set; } = 0.06f;

	// Token: 0x1700105C RID: 4188
	// (get) Token: 0x06008C62 RID: 35938 RVA: 0x00354BBB File Offset: 0x00352DBB
	// (set) Token: 0x06008C63 RID: 35939 RVA: 0x00354BC3 File Offset: 0x00352DC3
	[XuiXmlAttribute("hold_timed_step_acceleration", false)]
	public float HoldEventIntervalAcceleration { [PublicizedFrom(EAccessModifier.Private)] get; [PublicizedFrom(EAccessModifier.Private)] set; } = 0.015f;

	// Token: 0x1700105D RID: 4189
	// (get) Token: 0x06008C64 RID: 35940 RVA: 0x00354BCC File Offset: 0x00352DCC
	// (set) Token: 0x06008C65 RID: 35941 RVA: 0x00354BD4 File Offset: 0x00352DD4
	[XuiXmlAttribute("on_hover", false)]
	public bool EventOnHover
	{
		get
		{
			return this.eventOnHover;
		}
		set
		{
			if (this.eventOnHover == value)
			{
				return;
			}
			this.eventOnHover = value;
			this.SetDirty();
		}
	}

	// Token: 0x1700105E RID: 4190
	// (get) Token: 0x06008C66 RID: 35942 RVA: 0x00354BED File Offset: 0x00352DED
	// (set) Token: 0x06008C67 RID: 35943 RVA: 0x00354BF5 File Offset: 0x00352DF5
	[XuiXmlAttribute("on_press", false)]
	public bool EventOnPress
	{
		get
		{
			return this.eventOnPress;
		}
		set
		{
			if (this.eventOnPress == value)
			{
				return;
			}
			this.eventOnPress = value;
			this.SetDirty();
		}
	}

	// Token: 0x1700105F RID: 4191
	// (get) Token: 0x06008C68 RID: 35944 RVA: 0x00354C0E File Offset: 0x00352E0E
	// (set) Token: 0x06008C69 RID: 35945 RVA: 0x00354C16 File Offset: 0x00352E16
	[XuiXmlAttribute("on_doubleclick", false)]
	public bool EventOnDoubleClick
	{
		get
		{
			return this.eventOnDoubleClick;
		}
		set
		{
			if (this.eventOnDoubleClick == value)
			{
				return;
			}
			this.eventOnDoubleClick = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001060 RID: 4192
	// (get) Token: 0x06008C6A RID: 35946 RVA: 0x00354C2F File Offset: 0x00352E2F
	// (set) Token: 0x06008C6B RID: 35947 RVA: 0x00354C37 File Offset: 0x00352E37
	[XuiXmlAttribute("on_held", false)]
	public bool EventOnHeld
	{
		get
		{
			return this.eventOnHeld;
		}
		set
		{
			if (this.eventOnHeld == value)
			{
				return;
			}
			this.eventOnHeld = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001061 RID: 4193
	// (get) Token: 0x06008C6C RID: 35948 RVA: 0x00354C50 File Offset: 0x00352E50
	// (set) Token: 0x06008C6D RID: 35949 RVA: 0x00354C58 File Offset: 0x00352E58
	[XuiXmlAttribute("on_scroll", false)]
	public bool EventOnScroll
	{
		get
		{
			return this.eventOnScroll;
		}
		set
		{
			if (this.eventOnScroll == value)
			{
				return;
			}
			this.eventOnScroll = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001062 RID: 4194
	// (get) Token: 0x06008C6E RID: 35950 RVA: 0x00354C71 File Offset: 0x00352E71
	// (set) Token: 0x06008C6F RID: 35951 RVA: 0x00354C79 File Offset: 0x00352E79
	[XuiXmlAttribute("on_drag", false)]
	public bool EventOnDrag
	{
		get
		{
			return this.eventOnDrag;
		}
		set
		{
			if (this.eventOnDrag == value)
			{
				return;
			}
			this.eventOnDrag = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001063 RID: 4195
	// (get) Token: 0x06008C70 RID: 35952 RVA: 0x00354C92 File Offset: 0x00352E92
	// (set) Token: 0x06008C71 RID: 35953 RVA: 0x00354C9A File Offset: 0x00352E9A
	[XuiXmlAttribute("on_select", false)]
	public bool EventOnSelect
	{
		get
		{
			return this.eventOnSelect;
		}
		set
		{
			if (this.eventOnSelect == value)
			{
				return;
			}
			this.eventOnSelect = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001064 RID: 4196
	// (get) Token: 0x06008C72 RID: 35954 RVA: 0x00354CB4 File Offset: 0x00352EB4
	public virtual bool HasAnyEvent
	{
		get
		{
			return this.EventOnPress || this.EventOnDoubleClick || this.EventOnHover || this.EventOnHeld || this.EventOnDrag || this.EventOnScroll || this.EventOnSelect || !string.IsNullOrEmpty(this.ToolTip);
		}
	}

	// Token: 0x17001065 RID: 4197
	// (get) Token: 0x06008C73 RID: 35955 RVA: 0x00354D09 File Offset: 0x00352F09
	public bool IsHovered
	{
		get
		{
			return this.isOver;
		}
	}

	// Token: 0x17001066 RID: 4198
	// (get) Token: 0x06008C74 RID: 35956 RVA: 0x00354D11 File Offset: 0x00352F11
	// (set) Token: 0x06008C75 RID: 35957 RVA: 0x00354D33 File Offset: 0x00352F33
	public bool IsNavigatable
	{
		get
		{
			return this.isNavigatable && this.Enabled && this.IsVisible && this.IsActiveInHierarchy;
		}
		set
		{
			this.isNavigatable = value;
		}
	}

	// Token: 0x06008C76 RID: 35958 RVA: 0x00354D3C File Offset: 0x00352F3C
	[XuiXmlAttribute("gamepad_selectable", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeGamepadSelectable(bool _value)
	{
		this.IsNavigatable = _value;
		this.gamepadSelectableSetFromAttributes = true;
	}

	// Token: 0x17001067 RID: 4199
	// (get) Token: 0x06008C77 RID: 35959 RVA: 0x00354D4C File Offset: 0x00352F4C
	// (set) Token: 0x06008C78 RID: 35960 RVA: 0x00354D54 File Offset: 0x00352F54
	public bool IsSnappable { get; set; } = true;

	// Token: 0x06008C79 RID: 35961 RVA: 0x00354D5D File Offset: 0x00352F5D
	[XuiXmlAttribute("snap", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeGamepadSnappable(bool _value)
	{
		this.IsSnappable = _value;
		this.gamepadSnappableSetFromAttributes = true;
	}

	// Token: 0x17001068 RID: 4200
	// (get) Token: 0x06008C7A RID: 35962 RVA: 0x00354D6D File Offset: 0x00352F6D
	// (set) Token: 0x06008C7B RID: 35963 RVA: 0x00354D75 File Offset: 0x00352F75
	[XuiXmlAttribute("use_selection_box", false)]
	public bool UseSelectionBox { get; set; } = true;

	// Token: 0x17001069 RID: 4201
	// (get) Token: 0x06008C7C RID: 35964 RVA: 0x00354D7E File Offset: 0x00352F7E
	// (set) Token: 0x06008C7D RID: 35965 RVA: 0x00354D86 File Offset: 0x00352F86
	[XuiXmlAttribute("nav_up", false)]
	public string NavUpTargetString
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.navUpTargetString;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.navUpTargetString == value)
			{
				return;
			}
			this.navUpTargetString = value;
			this.SetDirty();
		}
	}

	// Token: 0x1700106A RID: 4202
	// (get) Token: 0x06008C7E RID: 35966 RVA: 0x00354DA4 File Offset: 0x00352FA4
	// (set) Token: 0x06008C7F RID: 35967 RVA: 0x00354DAC File Offset: 0x00352FAC
	public XUiView NavUpTarget
	{
		get
		{
			return this.navUpTarget;
		}
		set
		{
			this.navUpTarget = value;
			foreach (XUiController xuiController in this.Controller.Children)
			{
				xuiController.ViewComponent.NavUpTarget = value;
			}
		}
	}

	// Token: 0x1700106B RID: 4203
	// (get) Token: 0x06008C80 RID: 35968 RVA: 0x00354E10 File Offset: 0x00353010
	// (set) Token: 0x06008C81 RID: 35969 RVA: 0x00354E18 File Offset: 0x00353018
	[XuiXmlAttribute("nav_down", false)]
	public string NavDownTargetString
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.navDownTargetString;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.navDownTargetString == value)
			{
				return;
			}
			this.navDownTargetString = value;
			this.SetDirty();
		}
	}

	// Token: 0x1700106C RID: 4204
	// (get) Token: 0x06008C82 RID: 35970 RVA: 0x00354E36 File Offset: 0x00353036
	// (set) Token: 0x06008C83 RID: 35971 RVA: 0x00354E40 File Offset: 0x00353040
	public XUiView NavDownTarget
	{
		get
		{
			return this.navDownTarget;
		}
		set
		{
			this.navDownTarget = value;
			foreach (XUiController xuiController in this.Controller.Children)
			{
				xuiController.ViewComponent.navDownTarget = value;
			}
		}
	}

	// Token: 0x1700106D RID: 4205
	// (get) Token: 0x06008C84 RID: 35972 RVA: 0x00354EA4 File Offset: 0x003530A4
	// (set) Token: 0x06008C85 RID: 35973 RVA: 0x00354EAC File Offset: 0x003530AC
	[XuiXmlAttribute("nav_left", false)]
	public string NavLeftTargetString
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.navLeftTargetString;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.navLeftTargetString == value)
			{
				return;
			}
			this.navLeftTargetString = value;
			this.SetDirty();
		}
	}

	// Token: 0x1700106E RID: 4206
	// (get) Token: 0x06008C86 RID: 35974 RVA: 0x00354ECA File Offset: 0x003530CA
	// (set) Token: 0x06008C87 RID: 35975 RVA: 0x00354ED4 File Offset: 0x003530D4
	public XUiView NavLeftTarget
	{
		get
		{
			return this.navLeftTarget;
		}
		set
		{
			this.navLeftTarget = value;
			foreach (XUiController xuiController in this.Controller.Children)
			{
				xuiController.ViewComponent.navLeftTarget = value;
			}
		}
	}

	// Token: 0x1700106F RID: 4207
	// (get) Token: 0x06008C88 RID: 35976 RVA: 0x00354F38 File Offset: 0x00353138
	// (set) Token: 0x06008C89 RID: 35977 RVA: 0x00354F40 File Offset: 0x00353140
	[XuiXmlAttribute("nav_right", false)]
	public string NavRightTargetString
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.navRightTargetString;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.navRightTargetString == value)
			{
				return;
			}
			this.navRightTargetString = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001070 RID: 4208
	// (get) Token: 0x06008C8A RID: 35978 RVA: 0x00354F5E File Offset: 0x0035315E
	// (set) Token: 0x06008C8B RID: 35979 RVA: 0x00354F68 File Offset: 0x00353168
	public XUiView NavRightTarget
	{
		get
		{
			return this.navRightTarget;
		}
		set
		{
			this.navRightTarget = value;
			foreach (XUiController xuiController in this.Controller.Children)
			{
				xuiController.ViewComponent.navRightTarget = value;
			}
		}
	}

	// Token: 0x17001071 RID: 4209
	// (get) Token: 0x06008C8C RID: 35980 RVA: 0x00354FCC File Offset: 0x003531CC
	// (set) Token: 0x06008C8D RID: 35981 RVA: 0x00354FD4 File Offset: 0x003531D4
	[XuiXmlAttribute("tooltip", false)]
	public string ToolTip
	{
		get
		{
			return this.toolTip;
		}
		set
		{
			if (this.toolTip == value)
			{
				return;
			}
			if (GameManager.Instance.GameIsFocused && this.enabled && this.isOver && this.xui.ToolTipWindow != null && this.xui.ToolTipWindow.ToolTip == this.toolTip)
			{
				this.xui.ToolTipWindow.ToolTip = value;
			}
			this.toolTip = value;
		}
	}

	// Token: 0x17001072 RID: 4210
	// (get) Token: 0x06008C8E RID: 35982 RVA: 0x0035504E File Offset: 0x0035324E
	// (set) Token: 0x06008C8F RID: 35983 RVA: 0x00355058 File Offset: 0x00353258
	[XuiXmlAttribute("disabled_tooltip", false)]
	public string DisabledToolTip
	{
		get
		{
			return this.disabledToolTip;
		}
		set
		{
			if (this.disabledToolTip == value)
			{
				return;
			}
			if (GameManager.Instance.GameIsFocused && this.enabled && this.isOver && this.xui.ToolTipWindow != null && this.xui.ToolTipWindow.ToolTip == this.disabledToolTip)
			{
				this.xui.ToolTipWindow.ToolTip = value;
			}
			this.disabledToolTip = value;
		}
	}

	// Token: 0x06008C90 RID: 35984 RVA: 0x003550D2 File Offset: 0x003532D2
	[XuiXmlAttribute("tooltip_key", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeTooltipKey(string _value)
	{
		this.ToolTip = Localization.Get(_value, false, null);
	}

	// Token: 0x06008C91 RID: 35985 RVA: 0x003550E2 File Offset: 0x003532E2
	[XuiXmlAttribute("disabled_tooltip_key", false)]
	[PublicizedFrom(EAccessModifier.Private)]
	public void attributeDisabledTooltipKey(string _value)
	{
		this.DisabledToolTip = Localization.Get(_value, false, null);
	}

	// Token: 0x06008C92 RID: 35986 RVA: 0x003550F4 File Offset: 0x003532F4
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiView(XUi _xui, string _id)
	{
		this.xui = _xui;
		this.id = _id;
	}

	// Token: 0x06008C93 RID: 35987 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void createComponents(GameObject _go)
	{
	}

	// Token: 0x06008C94 RID: 35988 RVA: 0x003551B8 File Offset: 0x003533B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void buildView()
	{
		Type type = base.GetType();
		GameObject original;
		if (XUiView.componentTemplates.TryGetValue(type, out original))
		{
			this.uiTransform = UnityEngine.Object.Instantiate<GameObject>(original).transform;
			return;
		}
		if (XUiView.templatesParent == null)
		{
			Transform transform = this.xui.playerUI.uiCamera.transform;
			GameObject gameObject = new GameObject("_ViewTemplates");
			gameObject.SetActive(false);
			XUiView.templatesParent = gameObject.transform;
			XUiView.templatesParent.parent = transform;
		}
		GameObject gameObject2 = new GameObject(type.Name)
		{
			layer = 12,
			transform = 
			{
				parent = XUiView.templatesParent
			}
		};
		gameObject2.AddComponent<BoxCollider>().enabled = false;
		UIEventListener.Get(gameObject2);
		this.createComponents(gameObject2);
		XUiView.componentTemplates[type] = gameObject2;
		this.uiTransform = UnityEngine.Object.Instantiate<GameObject>(gameObject2).transform;
	}

	// Token: 0x06008C95 RID: 35989 RVA: 0x00355291 File Offset: 0x00353491
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void captureComponents()
	{
		this.collider = this.uiTransform.gameObject.GetComponent<BoxCollider>();
	}

	// Token: 0x06008C96 RID: 35990 RVA: 0x003552AC File Offset: 0x003534AC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void addTweeners()
	{
		for (int i = 0; i < this.Tweeners.Count; i++)
		{
			this.Tweeners[i].CreateTween(this.uiTransform.gameObject);
		}
	}

	// Token: 0x06008C97 RID: 35991 RVA: 0x003552EC File Offset: 0x003534EC
	public virtual void InitView()
	{
		this.buildView();
		this.uiTransform.name = this.id;
		this.captureComponents();
		this.addTweeners();
		XUiController parent = this.controller.Parent;
		if (((parent != null) ? parent.ViewComponent : null) != null)
		{
			XUiView viewComponent = this.controller.Parent.ViewComponent;
			this.uiTransform.parent = viewComponent.uiTransform;
			this.uiTransform.localScale = Vector3.one;
			this.uiTransform.localPosition = new Vector3((float)this.PaddedPosition.x, (float)this.PaddedPosition.y, 0f);
			this.uiTransform.localEulerAngles = new Vector3(0f, 0f, this.rotation);
		}
		if (this.HasAnyEvent)
		{
			this.collider.enabled = true;
			this.refreshBoxCollider();
			UIEventListener uieventListener = UIEventListener.Get(this.uiTransform.gameObject);
			uieventListener.onHover = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener.onHover, new UIEventListener.BoolDelegate(delegate(GameObject _, bool _state)
			{
				this.OnHover(_state);
			}));
			uieventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onClick, new UIEventListener.VoidDelegate(this.OnClick));
			uieventListener.onDoubleClick = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onDoubleClick, new UIEventListener.VoidDelegate(this.OnDoubleClick));
			uieventListener.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener.onDrag, new UIEventListener.VectorDelegate(this.OnDrag));
			uieventListener.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener.onPress, new UIEventListener.BoolDelegate(this.OnPress));
			uieventListener.onScroll = (UIEventListener.FloatDelegate)Delegate.Combine(uieventListener.onScroll, new UIEventListener.FloatDelegate(this.OnScroll));
			uieventListener.onSelect = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener.onSelect, new UIEventListener.BoolDelegate(this.OnSelect));
			uieventListener.onDragOut = (UIEventListener.VoidDelegate)Delegate.Combine(uieventListener.onDragOut, new UIEventListener.VoidDelegate(this.OnHeldDragOut));
		}
		if (this.uiTransform.gameObject.activeSelf != this.isVisible)
		{
			this.uiTransform.gameObject.SetActive(this.isVisible);
		}
		if (!this.gamepadSelectableSetFromAttributes)
		{
			this.IsNavigatable = this.EventOnPress;
		}
		if (!this.gamepadSnappableSetFromAttributes)
		{
			this.IsSnappable = this.EventOnPress;
		}
	}

	// Token: 0x06008C98 RID: 35992 RVA: 0x000027FC File Offset: 0x000009FC
	public virtual void SetRepeatContentTemplateParams(Dictionary<string, object> _templateParams, int _curRepeatNum)
	{
	}

	// Token: 0x06008C99 RID: 35993 RVA: 0x00355546 File Offset: 0x00353746
	public virtual void Cleanup()
	{
		this.destroyed = true;
	}

	// Token: 0x06008C9A RID: 35994 RVA: 0x00355550 File Offset: 0x00353750
	public virtual void Update(float _dt)
	{
		if (this.isOver && !this.UiTransformIsHovered)
		{
			using (this.pmUpdateHover.Auto())
			{
				this.OnHover(false);
			}
		}
		if (this.isPressed && this.enabled)
		{
			using (this.pmUpdatePress.Auto())
			{
				float unscaledTime = Time.unscaledTime;
				if (!this.isHold)
				{
					this.isHold = (unscaledTime - this.pressStartTime >= this.HoldDelay);
					if (this.isHold)
					{
						this.holdStartTime = unscaledTime;
						this.controller.Held(EHoldType.HoldStart, 0f, -1f);
						this.holdEventNextTime = 0f;
						this.holdEventLastTime = unscaledTime;
						this.holdEventIntervalChangeSpeed = 0f;
						this.holdEventIntervalCurrent = this.HoldEventIntervalInitial;
					}
				}
				else
				{
					this.controller.Held(EHoldType.Hold, unscaledTime - this.holdStartTime, -1f);
				}
				if (this.isHold && unscaledTime >= this.holdEventNextTime)
				{
					this.holdEventIntervalCurrent = Mathf.SmoothDamp(this.holdEventIntervalCurrent, this.HoldEventIntervalFinal, ref this.holdEventIntervalChangeSpeed, this.HoldEventIntervalAcceleration, float.PositiveInfinity, _dt);
					this.holdEventNextTime = unscaledTime + this.holdEventIntervalCurrent;
					this.controller.Held(EHoldType.HoldTimed, unscaledTime - this.holdStartTime, unscaledTime - this.holdEventLastTime);
					this.holdEventLastTime = unscaledTime;
				}
			}
		}
		if (this.isDirty)
		{
			using (this.pmUpdateData.Auto())
			{
				this.updateData();
				this.isDirty = false;
			}
		}
	}

	// Token: 0x06008C9B RID: 35995 RVA: 0x00355718 File Offset: 0x00353918
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void updateData()
	{
		if (this.positionDirty)
		{
			this.uiTransform.localPosition = new Vector3((float)this.PaddedPosition.x, (float)this.PaddedPosition.y, this.uiTransform.localPosition.z);
			this.positionDirty = false;
		}
		if (this.rotationDirty)
		{
			this.uiTransform.localEulerAngles = new Vector3(0f, 0f, this.rotation);
			this.rotationDirty = false;
		}
		this.parseAnchors();
		this.parseNavigationTargets();
	}

	// Token: 0x06008C9C RID: 35996 RVA: 0x003557A8 File Offset: 0x003539A8
	public void TryUpdatePosition()
	{
		if (this.positionDirty)
		{
			this.uiTransform.localPosition = new Vector3((float)this.PaddedPosition.x, (float)this.PaddedPosition.y, this.uiTransform.localPosition.z);
		}
	}

	// Token: 0x06008C9D RID: 35997 RVA: 0x003557F8 File Offset: 0x003539F8
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void refreshBoxCollider()
	{
		this.collider.center = new Vector3((float)this.size.x * 0.5f, 0f - (float)this.size.y * 0.5f, 0f);
		this.collider.size = new Vector3((float)this.size.x * this.ColliderScale + (float)(2 * this.ColliderPadding.x), (float)this.size.y * this.ColliderScale + (float)(2 * this.ColliderPadding.y), 0f);
	}

	// Token: 0x06008C9E RID: 35998 RVA: 0x0035589D File Offset: 0x00353A9D
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetDirty()
	{
		this.isDirty = true;
	}

	// Token: 0x17001073 RID: 4211
	// (get) Token: 0x06008C9F RID: 35999 RVA: 0x003558A6 File Offset: 0x00353AA6
	// (set) Token: 0x06008CA0 RID: 36000 RVA: 0x003558AE File Offset: 0x00353AAE
	[XuiXmlAttribute("anchor_left", false)]
	public string AnchorLeft
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.anchorLeft;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.anchorLeft == value)
			{
				return;
			}
			this.anchorLeft = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001074 RID: 4212
	// (get) Token: 0x06008CA1 RID: 36001 RVA: 0x003558CC File Offset: 0x00353ACC
	// (set) Token: 0x06008CA2 RID: 36002 RVA: 0x003558D4 File Offset: 0x00353AD4
	[XuiXmlAttribute("anchor_right", false)]
	public string AnchorRight
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.anchorRight;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.anchorRight == value)
			{
				return;
			}
			this.anchorRight = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001075 RID: 4213
	// (get) Token: 0x06008CA3 RID: 36003 RVA: 0x003558F2 File Offset: 0x00353AF2
	// (set) Token: 0x06008CA4 RID: 36004 RVA: 0x003558FA File Offset: 0x00353AFA
	[XuiXmlAttribute("anchor_bottom", false)]
	public string AnchorBottom
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.anchorBottom;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.anchorBottom == value)
			{
				return;
			}
			this.anchorBottom = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001076 RID: 4214
	// (get) Token: 0x06008CA5 RID: 36005 RVA: 0x00355918 File Offset: 0x00353B18
	// (set) Token: 0x06008CA6 RID: 36006 RVA: 0x00355920 File Offset: 0x00353B20
	[XuiXmlAttribute("anchor_top", false)]
	public string AnchorTop
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.anchorTop;
		}
		[PublicizedFrom(EAccessModifier.Private)]
		set
		{
			if (this.anchorTop == value)
			{
				return;
			}
			this.anchorTop = value;
			this.SetDirty();
		}
	}

	// Token: 0x17001077 RID: 4215
	// (get) Token: 0x06008CA7 RID: 36007 RVA: 0x0035593E File Offset: 0x00353B3E
	public bool HasAnchorStringLeft
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return !string.IsNullOrEmpty(this.anchorLeft);
		}
	}

	// Token: 0x17001078 RID: 4216
	// (get) Token: 0x06008CA8 RID: 36008 RVA: 0x0035594E File Offset: 0x00353B4E
	public bool HasAnchorStringRight
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return !string.IsNullOrEmpty(this.anchorRight);
		}
	}

	// Token: 0x17001079 RID: 4217
	// (get) Token: 0x06008CA9 RID: 36009 RVA: 0x0035595E File Offset: 0x00353B5E
	public bool HasAnchorStringTop
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return !string.IsNullOrEmpty(this.anchorTop);
		}
	}

	// Token: 0x1700107A RID: 4218
	// (get) Token: 0x06008CAA RID: 36010 RVA: 0x0035596E File Offset: 0x00353B6E
	public bool HasAnchorStringBottom
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return !string.IsNullOrEmpty(this.anchorBottom);
		}
	}

	// Token: 0x1700107B RID: 4219
	// (get) Token: 0x06008CAB RID: 36011 RVA: 0x0035597E File Offset: 0x00353B7E
	public bool HasAnchorStringLeftAndRight
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.HasAnchorStringLeft && this.HasAnchorStringRight;
		}
	}

	// Token: 0x1700107C RID: 4220
	// (get) Token: 0x06008CAC RID: 36012 RVA: 0x00355990 File Offset: 0x00353B90
	public bool HasAnchorStringTopAndBottom
	{
		[PublicizedFrom(EAccessModifier.Protected)]
		get
		{
			return this.HasAnchorStringTop && this.HasAnchorStringBottom;
		}
	}

	// Token: 0x06008CAD RID: 36013 RVA: 0x003559A4 File Offset: 0x00353BA4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void parseAnchors()
	{
		UIRect uiRect = this.UiRect;
		if (uiRect == null)
		{
			return;
		}
		if (this.<parseAnchors>g__ParseAnchorString|281_0(this.anchorLeft, ref this.anchorLeftParsed, uiRect, uiRect.leftAnchor) | this.<parseAnchors>g__ParseAnchorString|281_0(this.anchorRight, ref this.anchorRightParsed, uiRect, uiRect.rightAnchor) | this.<parseAnchors>g__ParseAnchorString|281_0(this.anchorBottom, ref this.anchorBottomParsed, uiRect, uiRect.bottomAnchor) | this.<parseAnchors>g__ParseAnchorString|281_0(this.anchorTop, ref this.anchorTopParsed, uiRect, uiRect.topAnchor))
		{
			this.anchoredLeftAndRight = (uiRect.leftAnchor.target && uiRect.rightAnchor.target);
			this.anchoredTopAndBottom = (uiRect.bottomAnchor.target && uiRect.topAnchor.target);
			this.SetDirty();
			uiRect.ResetAnchors();
		}
		this.anchorsParsed();
	}

	// Token: 0x06008CAE RID: 36014 RVA: 0x000027FC File Offset: 0x000009FC
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void anchorsParsed()
	{
	}

	// Token: 0x06008CAF RID: 36015 RVA: 0x00355A93 File Offset: 0x00353C93
	public virtual void OnOpen()
	{
		if (this.xuiSound != null && this.SoundPlayOnOpen)
		{
			Manager.PlayXUiSound(this.xuiSound, this.SoundVolume);
		}
		this.isPressed = false;
		this.isHold = false;
	}

	// Token: 0x06008CB0 RID: 36016 RVA: 0x00355ACC File Offset: 0x00353CCC
	public virtual void OnClose()
	{
		this.isPressed = false;
		this.isHold = false;
		if (!GameManager.Instance.IsQuitting)
		{
			CursorControllerAbs cursorController = this.xui.playerUI.CursorController;
			if (cursorController.HoverTarget == this)
			{
				cursorController.HoverTarget = null;
			}
			if (cursorController.navigationTarget == this)
			{
				this.controller.Hovered(false);
				cursorController.SetNavigationTarget(null);
			}
			if (cursorController.lockNavigationToView == this)
			{
				cursorController.SetNavigationLockView(null, null);
			}
		}
	}

	// Token: 0x06008CB1 RID: 36017 RVA: 0x00355B41 File Offset: 0x00353D41
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnScroll(GameObject _go, float _delta)
	{
		if (this.EventOnScroll)
		{
			this.controller.Scrolled(_delta);
		}
	}

	// Token: 0x06008CB2 RID: 36018 RVA: 0x00355B57 File Offset: 0x00353D57
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnSelect(GameObject _go, bool _selected)
	{
		if (this.EventOnSelect && this.enabled)
		{
			this.controller.Selected(_selected);
		}
	}

	// Token: 0x06008CB3 RID: 36019 RVA: 0x00355B78 File Offset: 0x00353D78
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDrag(GameObject _go, Vector2 _delta)
	{
		if (this.EventOnDrag && this.enabled)
		{
			EDragType dragType = this.wasDragging ? EDragType.Dragging : EDragType.DragStart;
			this.wasDragging = true;
			this.controller.Dragged(_delta, dragType);
		}
	}

	// Token: 0x06008CB4 RID: 36020 RVA: 0x00355BB8 File Offset: 0x00353DB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnPress(GameObject _go, bool _pressed)
	{
		if (this.EventOnPress && this.enabled)
		{
			this.controller.MouseUpDown(_pressed);
		}
		if (this.EventOnDrag && this.enabled && !_pressed && this.wasDragging)
		{
			this.wasDragging = false;
			this.controller.Dragged(default(Vector2), EDragType.DragEnd);
		}
		if (this.EventOnHeld)
		{
			if (_pressed && !this.isPressed && this.enabled)
			{
				this.isPressed = true;
				this.isHold = false;
				this.pressStartTime = Time.unscaledTime;
				this.holdStartTime = -1f;
				return;
			}
			if (!_pressed)
			{
				this.isPressed = false;
				bool flag = this.isHold;
				this.isHold = false;
				if (flag)
				{
					this.controller.Held(EHoldType.HoldEnd, Time.unscaledTime - this.holdStartTime, -1f);
				}
			}
		}
	}

	// Token: 0x06008CB5 RID: 36021 RVA: 0x00355C90 File Offset: 0x00353E90
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnHeldDragOut(GameObject _go)
	{
		if (this.EventOnHeld && this.isPressed)
		{
			this.isPressed = false;
			bool flag = this.isHold;
			this.isHold = false;
			if (flag)
			{
				this.controller.Held(EHoldType.HoldEnd, Time.unscaledTime - this.holdStartTime, -1f);
			}
		}
	}

	// Token: 0x06008CB6 RID: 36022 RVA: 0x00355CE0 File Offset: 0x00353EE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnClick(GameObject _go)
	{
		if (this.EventOnPress && this.enabled)
		{
			if (this.xuiSound != null && this.SoundPlayOnClick && UICamera.currentTouchID == -1)
			{
				Manager.PlayXUiSound(this.xuiSound, this.SoundVolume);
			}
			this.controller.Pressed(UICamera.currentTouchID);
		}
	}

	// Token: 0x06008CB7 RID: 36023 RVA: 0x00355D3C File Offset: 0x00353F3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDoubleClick(GameObject _go)
	{
		if (this.EventOnDoubleClick && this.enabled)
		{
			if (this.xuiSound != null && this.SoundPlayOnClick && UICamera.currentTouchID == -1)
			{
				Manager.PlayXUiSound(this.xuiSound, this.SoundVolume);
			}
			this.controller.DoubleClicked(UICamera.currentTouchID);
		}
	}

	// Token: 0x06008CB8 RID: 36024 RVA: 0x00355D98 File Offset: 0x00353F98
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnHover(bool _isOver)
	{
		PlayerActionsLocal playerInput = this.xui.playerUI.playerInput;
		if (playerInput != null && playerInput.LastDeviceClass == InputDeviceClass.Keyboard && !Cursor.visible)
		{
			_isOver = false;
		}
		bool flag = _isOver;
		bool flag2 = _isOver && !this.enabled && !string.IsNullOrEmpty(this.DisabledToolTip);
		_isOver &= this.enabled;
		bool flag3 = _isOver && !string.IsNullOrEmpty(this.ToolTip);
		if (_isOver != this.isOver && _isOver)
		{
			this.PlayHoverSound();
		}
		this.isOver = _isOver;
		if (this.EventOnHover)
		{
			this.controller.Hovered(_isOver);
		}
		if (this.xui.ToolTipWindow != null)
		{
			if (flag3)
			{
				this.xui.ToolTipWindow.ToolTip = this.ToolTip;
			}
			else if (flag2)
			{
				this.xui.ToolTipWindow.ToolTip = this.DisabledToolTip;
			}
			else
			{
				this.xui.ToolTipWindow.ToolTip = "";
			}
		}
		this.xui.playerUI.CursorController.HoverTarget = (flag ? this : null);
	}

	// Token: 0x06008CB9 RID: 36025 RVA: 0x00355EB3 File Offset: 0x003540B3
	public void PlayHoverSound()
	{
		if (this.xuiHoverSound != null && this.SoundPlayOnHover && this.enabled && GameManager.Instance.GameIsFocused)
		{
			Manager.PlayXUiSound(this.xuiHoverSound, this.SoundVolume);
		}
	}

	// Token: 0x06008CBA RID: 36026 RVA: 0x00355EF0 File Offset: 0x003540F0
	public void PlayClickSound()
	{
		if (this.xuiSound != null)
		{
			Manager.PlayXUiSound(this.xuiSound, this.SoundVolume);
		}
	}

	// Token: 0x06008CBB RID: 36027 RVA: 0x00355F11 File Offset: 0x00354111
	public Rect GetXUiRect()
	{
		return this.GetXUiRect(Vector2.zero);
	}

	// Token: 0x06008CBC RID: 36028 RVA: 0x00355F20 File Offset: 0x00354120
	public Rect GetXUiRect(Vector2 _padding)
	{
		XUiController parent = this.controller;
		Vector3 vector = this.uiTransform.localPosition;
		vector.x -= _padding.x;
		vector.y += _padding.y;
		while (parent.Parent != null && parent.Parent.ViewComponent != null)
		{
			parent = parent.Parent;
			vector += parent.ViewComponent.uiTransform.localPosition;
		}
		vector += parent.ViewComponent.uiTransform.parent.localPosition;
		Vector2 vector2 = this.Size.AsVector2() + 2f * _padding;
		XUiV_Window xuiV_Window = parent.ViewComponent as XUiV_Window;
		if (xuiV_Window != null && xuiV_Window.IsInStackPanel)
		{
			Transform parent2 = xuiV_Window.uiTransform.parent.parent;
			vector *= parent2.localScale.x;
			vector2 *= parent2.localScale.x;
			vector += parent2.localPosition;
		}
		return new Rect(new Vector2(vector.x, vector.y - vector2.y), vector2);
	}

	// Token: 0x06008CBD RID: 36029 RVA: 0x0035604E File Offset: 0x0035424E
	public string GetXuiHierarchy()
	{
		return XUiUtils.GetXuiHierarchy(this.Controller);
	}

	// Token: 0x06008CBE RID: 36030 RVA: 0x0035605B File Offset: 0x0035425B
	public Vector2 GetClosestPoint(Vector3 _point)
	{
		return this.collider.ClosestPointOnBounds(_point);
	}

	// Token: 0x06008CBF RID: 36031 RVA: 0x00356070 File Offset: 0x00354270
	public void ClearNavigationTargets()
	{
		this.NavUpTarget = (this.NavDownTarget = (this.NavLeftTarget = (this.NavRightTarget = null)));
	}

	// Token: 0x06008CC0 RID: 36032 RVA: 0x003560A0 File Offset: 0x003542A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void parseNavigationTargets()
	{
		if (!string.IsNullOrEmpty(this.navUpTargetString))
		{
			this.NavUpTarget = this.<parseNavigationTargets>g__FindView|310_0(this.navUpTargetString);
		}
		if (!string.IsNullOrEmpty(this.navDownTargetString))
		{
			this.NavDownTarget = this.<parseNavigationTargets>g__FindView|310_0(this.navDownTargetString);
		}
		if (!string.IsNullOrEmpty(this.navLeftTargetString))
		{
			this.NavLeftTarget = this.<parseNavigationTargets>g__FindView|310_0(this.navLeftTargetString);
		}
		if (!string.IsNullOrEmpty(this.navRightTargetString))
		{
			this.NavRightTarget = this.<parseNavigationTargets>g__FindView|310_0(this.navRightTargetString);
		}
	}

	// Token: 0x06008CC1 RID: 36033 RVA: 0x0035612C File Offset: 0x0035432C
	public virtual void SetDefaults(XUiController _parent)
	{
		this.RepeatContent = false;
		this.RepeatCount = 1;
		this.Size = Vector2i.min;
		int? num;
		if (_parent == null)
		{
			num = null;
		}
		else
		{
			XUiView viewComponent = _parent.ViewComponent;
			num = ((viewComponent != null) ? new int?(viewComponent.Depth) : null);
		}
		int? num2 = num;
		this.Depth = num2.GetValueOrDefault();
		this.ToolTip = "";
		this.SoundPlayOnClick = true;
		this.SoundPlayOnOpen = false;
		this.SoundPlayOnHover = true;
		this.SoundVolume = 1f;
	}

	// Token: 0x06008CC2 RID: 36034 RVA: 0x003561B8 File Offset: 0x003543B8
	public virtual void SetPostParsingDefaults(XUiController _parent)
	{
		XUiView xuiView = (_parent != null) ? _parent.ViewComponent : null;
		Vector2i vector2i = this.Size;
		if (vector2i.x == -2147483648)
		{
			vector2i.x = (this.ignoreParentPadding ? ((xuiView != null) ? xuiView.Size.x : 0) : ((xuiView != null) ? xuiView.InnerSize.x : 0));
		}
		if (vector2i.y == -2147483648)
		{
			vector2i.y = (this.ignoreParentPadding ? ((xuiView != null) ? xuiView.Size.y : 0) : ((xuiView != null) ? xuiView.InnerSize.y : 0));
		}
		this.Size = vector2i;
	}

	// Token: 0x06008CC3 RID: 36035 RVA: 0x00356260 File Offset: 0x00354460
	public void ParseInitialAttributeValue(string _attribute, string _value)
	{
		if (_value.Contains("{"))
		{
			BindingsManager.CreateBinding(this, _attribute, _value);
			return;
		}
		if (ParsingMethodCache.Instance.TryParseDirect(this, _attribute, _value))
		{
			return;
		}
		if (this.Controller != null && ParsingMethodCache.Instance.TryParseDirect(this.Controller, _attribute, _value))
		{
			return;
		}
		this.ParseAttributeViewAndController(_attribute, _value);
	}

	// Token: 0x06008CC4 RID: 36036 RVA: 0x003562BC File Offset: 0x003544BC
	public bool ParseAttributeViewAndController(string _attribute, string _value)
	{
		if (_value.Contains("{") && XUiFromXml.DebugXuiLoading == XUiFromXml.DebugLevel.Verbose)
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] Refreshed binding contained '{': ",
				_attribute,
				"='",
				_value,
				"' on ",
				this.id
			}));
		}
		if (this.Controller != null)
		{
			if (!this.Controller.ParseAttribute(_attribute, _value))
			{
				this.Controller.CustomAttributes[_attribute] = _value;
			}
			return true;
		}
		return false;
	}

	// Token: 0x06008CC5 RID: 36037 RVA: 0x00356343 File Offset: 0x00354543
	public virtual void OnVisibilityChanged(bool _visibleInScene)
	{
		if (this.isOver && !_visibleInScene)
		{
			this.OnHover(false);
		}
	}

	// Token: 0x06008CCA RID: 36042 RVA: 0x00356380 File Offset: 0x00354580
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public bool <parseAnchors>g__ParseAnchorString|281_0(string _anchorString, ref string _parsedString, UIRect _uiRect, UIRect.AnchorPoint _anchor)
	{
		if (string.IsNullOrEmpty(_anchorString))
		{
			return false;
		}
		if (_anchorString == _parsedString && _anchor.target != null)
		{
			return false;
		}
		_parsedString = _anchorString;
		int num = _anchorString.IndexOf(',');
		if (num < 0)
		{
			throw new ArgumentException("Invalid anchor string '" + _anchorString + "', expected '<target>,<relative>,<absolute>'");
		}
		string text = _anchorString.Substring(0, num);
		int num2 = _anchorString.IndexOf(',', num + 1);
		if (num2 < 0)
		{
			throw new ArgumentException("Invalid anchor string '" + _anchorString + "', expected '<target>,<relative>,<absolute>'");
		}
		float relative = StringParsers.ParseFloat(_anchorString, num + 1, num2 - 1, NumberStyles.Any);
		int absolute = StringParsers.ParseSInt32(_anchorString, num2 + 1, -1, NumberStyles.Integer);
		if (text.Length == 0)
		{
			throw new ArgumentException("Invalid anchor string '" + _anchorString + "', expected '<target>,<relative>,<absolute>'");
		}
		if (text.EqualsCaseInsensitive("#parent"))
		{
			_anchor.target = this.uiTransform.parent;
		}
		else if (text.EqualsCaseInsensitive("#cam"))
		{
			UICamera componentInParent = this.uiTransform.gameObject.GetComponentInParent<UICamera>();
			if (componentInParent == null)
			{
				throw new Exception("UICamera not found");
			}
			_anchor.target = componentInParent.transform;
		}
		else if (text[0] == '#')
		{
			string text2 = text.Substring(1);
			UIAnchor.Side anchorSide;
			if (!EnumUtils.TryParse<UIAnchor.Side>(text2, out anchorSide, true))
			{
				throw new ArgumentException("Invalid anchor side name '" + text2 + "', expected any of '\tBottomLeft,Left,TopLeft,Top,TopRight,Right,BottomRight,Bottom,Center'");
			}
			_anchor.target = this.xui.GetAnchor(anchorSide).transform;
		}
		else
		{
			XUiView xuiView = XUiUtils.FindHierarchyClosestView(this.Controller, text);
			if (xuiView == null)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Invalid anchor string '",
					_anchorString,
					"', view component with name '",
					text,
					"' not found.\nOn: ",
					this.GetXuiHierarchy()
				}));
			}
			_anchor.target = xuiView.UiTransform;
			XUiV_Grid xuiV_Grid = xuiView as XUiV_Grid;
			if (xuiV_Grid != null)
			{
				xuiV_Grid.OnSizeChanged += delegate(Vector2Int _, Vector2 _)
				{
					_uiRect.UpdateAnchors();
				};
			}
		}
		_anchor.relative = relative;
		_anchor.absolute = absolute;
		return true;
	}

	// Token: 0x06008CCB RID: 36043 RVA: 0x0035659C File Offset: 0x0035479C
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView <parseNavigationTargets>g__FindView|310_0(string _name)
	{
		if (string.IsNullOrEmpty(_name))
		{
			return null;
		}
		XUiView xuiView = XUiUtils.FindHierarchyClosestView(this.Controller, _name);
		if (xuiView != null)
		{
			return xuiView;
		}
		XUiController childById = this.controller.WindowGroup.Controller.GetChildById(_name);
		xuiView = ((childById != null) ? childById.ViewComponent : null);
		if (xuiView != null)
		{
			return xuiView;
		}
		Log.Error("Invalid navigation target, view component with name '" + _name + "' not found.\nOn: " + this.GetXuiHierarchy());
		return null;
	}

	// Token: 0x04006779 RID: 26489
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<Type, GameObject> componentTemplates = new Dictionary<Type, GameObject>();

	// Token: 0x0400677A RID: 26490
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform templatesParent;

	// Token: 0x0400677B RID: 26491
	public readonly XUi xui;

	// Token: 0x0400677C RID: 26492
	[PublicizedFrom(EAccessModifier.Private)]
	public bool destroyed;

	// Token: 0x0400677D RID: 26493
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiController controller;

	// Token: 0x0400677E RID: 26494
	[PublicizedFrom(EAccessModifier.Protected)]
	public Transform uiTransform;

	// Token: 0x0400677F RID: 26495
	[PublicizedFrom(EAccessModifier.Protected)]
	public BoxCollider collider;

	// Token: 0x04006780 RID: 26496
	public readonly List<XUiTweenAbs> Tweeners = new List<XUiTweenAbs>();

	// Token: 0x04006783 RID: 26499
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04006784 RID: 26500
	[PublicizedFrom(EAccessModifier.Protected)]
	public string id;

	// Token: 0x04006785 RID: 26501
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isVisible = true;

	// Token: 0x04006786 RID: 26502
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool enabled = true;

	// Token: 0x04006789 RID: 26505
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector2i size;

	// Token: 0x0400678A RID: 26506
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector2i position;

	// Token: 0x0400678B RID: 26507
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool positionDirty;

	// Token: 0x0400678C RID: 26508
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool ignoreParentPadding;

	// Token: 0x0400678D RID: 26509
	[PublicizedFrom(EAccessModifier.Protected)]
	public XUiSideSizes padding;

	// Token: 0x0400678E RID: 26510
	[PublicizedFrom(EAccessModifier.Protected)]
	public float rotation;

	// Token: 0x0400678F RID: 26511
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool rotationDirty;

	// Token: 0x04006790 RID: 26512
	[PublicizedFrom(EAccessModifier.Protected)]
	public int depth;

	// Token: 0x04006791 RID: 26513
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip xuiSound;

	// Token: 0x04006792 RID: 26514
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip xuiHoverSound;

	// Token: 0x0400679B RID: 26523
	[PublicizedFrom(EAccessModifier.Private)]
	public bool eventOnHover;

	// Token: 0x0400679C RID: 26524
	[PublicizedFrom(EAccessModifier.Private)]
	public bool eventOnPress;

	// Token: 0x0400679D RID: 26525
	[PublicizedFrom(EAccessModifier.Private)]
	public bool eventOnDoubleClick;

	// Token: 0x0400679E RID: 26526
	[PublicizedFrom(EAccessModifier.Private)]
	public bool eventOnHeld;

	// Token: 0x0400679F RID: 26527
	[PublicizedFrom(EAccessModifier.Private)]
	public bool eventOnScroll;

	// Token: 0x040067A0 RID: 26528
	[PublicizedFrom(EAccessModifier.Private)]
	public bool eventOnDrag;

	// Token: 0x040067A1 RID: 26529
	[PublicizedFrom(EAccessModifier.Private)]
	public bool eventOnSelect;

	// Token: 0x040067A2 RID: 26530
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isNavigatable = true;

	// Token: 0x040067A5 RID: 26533
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool gamepadSelectableSetFromAttributes;

	// Token: 0x040067A6 RID: 26534
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool gamepadSnappableSetFromAttributes;

	// Token: 0x040067A7 RID: 26535
	[PublicizedFrom(EAccessModifier.Private)]
	public string navUpTargetString;

	// Token: 0x040067A8 RID: 26536
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView navUpTarget;

	// Token: 0x040067A9 RID: 26537
	[PublicizedFrom(EAccessModifier.Private)]
	public string navDownTargetString;

	// Token: 0x040067AA RID: 26538
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView navDownTarget;

	// Token: 0x040067AB RID: 26539
	[PublicizedFrom(EAccessModifier.Private)]
	public string navLeftTargetString;

	// Token: 0x040067AC RID: 26540
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView navLeftTarget;

	// Token: 0x040067AD RID: 26541
	[PublicizedFrom(EAccessModifier.Private)]
	public string navRightTargetString;

	// Token: 0x040067AE RID: 26542
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiView navRightTarget;

	// Token: 0x040067AF RID: 26543
	[PublicizedFrom(EAccessModifier.Private)]
	public string toolTip;

	// Token: 0x040067B0 RID: 26544
	[PublicizedFrom(EAccessModifier.Private)]
	public string disabledToolTip;

	// Token: 0x040067B1 RID: 26545
	[PublicizedFrom(EAccessModifier.Private)]
	public ProfilerMarker pmUpdateHover = new ProfilerMarker("XV.U-Hover");

	// Token: 0x040067B2 RID: 26546
	[PublicizedFrom(EAccessModifier.Private)]
	public ProfilerMarker pmUpdatePress = new ProfilerMarker("XV.U-Press");

	// Token: 0x040067B3 RID: 26547
	[PublicizedFrom(EAccessModifier.Private)]
	public ProfilerMarker pmUpdateData = new ProfilerMarker("XV.U-UpdateData");

	// Token: 0x040067B4 RID: 26548
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchorLeft;

	// Token: 0x040067B5 RID: 26549
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchorRight;

	// Token: 0x040067B6 RID: 26550
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchorBottom;

	// Token: 0x040067B7 RID: 26551
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchorTop;

	// Token: 0x040067B8 RID: 26552
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchorLeftParsed;

	// Token: 0x040067B9 RID: 26553
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchorRightParsed;

	// Token: 0x040067BA RID: 26554
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchorBottomParsed;

	// Token: 0x040067BB RID: 26555
	[PublicizedFrom(EAccessModifier.Private)]
	public string anchorTopParsed;

	// Token: 0x040067BC RID: 26556
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool anchoredLeftAndRight;

	// Token: 0x040067BD RID: 26557
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool anchoredTopAndBottom;

	// Token: 0x040067BE RID: 26558
	[PublicizedFrom(EAccessModifier.Private)]
	public bool wasDragging;

	// Token: 0x040067BF RID: 26559
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPressed;

	// Token: 0x040067C0 RID: 26560
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isHold;

	// Token: 0x040067C1 RID: 26561
	[PublicizedFrom(EAccessModifier.Private)]
	public float pressStartTime;

	// Token: 0x040067C2 RID: 26562
	[PublicizedFrom(EAccessModifier.Private)]
	public float holdStartTime;

	// Token: 0x040067C3 RID: 26563
	[PublicizedFrom(EAccessModifier.Private)]
	public float holdEventIntervalCurrent;

	// Token: 0x040067C4 RID: 26564
	[PublicizedFrom(EAccessModifier.Private)]
	public float holdEventIntervalChangeSpeed;

	// Token: 0x040067C5 RID: 26565
	[PublicizedFrom(EAccessModifier.Private)]
	public float holdEventNextTime;

	// Token: 0x040067C6 RID: 26566
	[PublicizedFrom(EAccessModifier.Private)]
	public float holdEventLastTime;

	// Token: 0x040067C7 RID: 26567
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool isOver;
}
