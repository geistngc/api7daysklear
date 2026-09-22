using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace mumblelib
{
	// Token: 0x020016AF RID: 5807
	[PublicizedFrom(EAccessModifier.Internal)]
	public class WindowsLinkFile : ILinkFile, IDisposable
	{
		// Token: 0x0600B5BE RID: 46526 RVA: 0x0043BE94 File Offset: 0x0043A094
		public unsafe WindowsLinkFile()
		{
			this.memoryMappedFile = MemoryMappedFile.CreateOrOpen("MumbleLink", (long)Marshal.SizeOf<WindowsLinkFile.WindowsLinkMemory>());
			byte* ptr = null;
			this.memoryMappedFile.CreateViewAccessor().SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
			this.ptr = (WindowsLinkFile.WindowsLinkMemory*)ptr;
		}

		// Token: 0x1700161E RID: 5662
		// (get) Token: 0x0600B5BF RID: 46527 RVA: 0x0043BEDE File Offset: 0x0043A0DE
		// (set) Token: 0x0600B5C0 RID: 46528 RVA: 0x0043BEEB File Offset: 0x0043A0EB
		public unsafe uint UIVersion
		{
			get
			{
				return this.ptr->uiVersion;
			}
			set
			{
				this.ptr->uiVersion = value;
			}
		}

		// Token: 0x0600B5C1 RID: 46529 RVA: 0x0043BEF9 File Offset: 0x0043A0F9
		public unsafe void Tick()
		{
			this.ptr->uiTick += 1U;
		}

		// Token: 0x1700161F RID: 5663
		// (set) Token: 0x0600B5C2 RID: 46530 RVA: 0x0043BF0B File Offset: 0x0043A10B
		public unsafe Vector3 AvatarPosition
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarPosition.FixedElementField, value);
			}
		}

		// Token: 0x17001620 RID: 5664
		// (set) Token: 0x0600B5C3 RID: 46531 RVA: 0x0043BF24 File Offset: 0x0043A124
		public unsafe Vector3 AvatarForward
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarFront.FixedElementField, value);
			}
		}

		// Token: 0x17001621 RID: 5665
		// (set) Token: 0x0600B5C4 RID: 46532 RVA: 0x0043BF3D File Offset: 0x0043A13D
		public unsafe Vector3 AvatarTop
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarTop.FixedElementField, value);
			}
		}

		// Token: 0x17001622 RID: 5666
		// (set) Token: 0x0600B5C5 RID: 46533 RVA: 0x0043BF56 File Offset: 0x0043A156
		public unsafe string Name
		{
			set
			{
				Util.SetString<ushort>(&this.ptr->name.FixedElementField, value, 256, Encoding.Unicode);
			}
		}

		// Token: 0x17001623 RID: 5667
		// (set) Token: 0x0600B5C6 RID: 46534 RVA: 0x0043BF79 File Offset: 0x0043A179
		public unsafe Vector3 CameraPosition
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraPosition.FixedElementField, value);
			}
		}

		// Token: 0x17001624 RID: 5668
		// (set) Token: 0x0600B5C7 RID: 46535 RVA: 0x0043BF92 File Offset: 0x0043A192
		public unsafe Vector3 CameraForward
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraFront.FixedElementField, value);
			}
		}

		// Token: 0x17001625 RID: 5669
		// (set) Token: 0x0600B5C8 RID: 46536 RVA: 0x0043BFAB File Offset: 0x0043A1AB
		public unsafe Vector3 CameraTop
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraTop.FixedElementField, value);
			}
		}

		// Token: 0x17001626 RID: 5670
		// (set) Token: 0x0600B5C9 RID: 46537 RVA: 0x0043BFC4 File Offset: 0x0043A1C4
		public unsafe string Identity
		{
			set
			{
				Util.SetString<ushort>(&this.ptr->identity.FixedElementField, value, 256, Encoding.Unicode);
			}
		}

		// Token: 0x17001627 RID: 5671
		// (set) Token: 0x0600B5CA RID: 46538 RVA: 0x0043BFE7 File Offset: 0x0043A1E7
		public unsafe string Context
		{
			set
			{
				Util.SetContext(&this.ptr->context.FixedElementField, &this.ptr->context_len, value);
			}
		}

		// Token: 0x17001628 RID: 5672
		// (set) Token: 0x0600B5CB RID: 46539 RVA: 0x0043C00C File Offset: 0x0043A20C
		public unsafe string Description
		{
			set
			{
				Util.SetString<ushort>(&this.ptr->description.FixedElementField, value, 2048, Encoding.Unicode);
			}
		}

		// Token: 0x0600B5CC RID: 46540 RVA: 0x0043C02F File Offset: 0x0043A22F
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0600B5CD RID: 46541 RVA: 0x0043C040 File Offset: 0x0043A240
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~WindowsLinkFile()
		{
			this.Dispose();
		}

		// Token: 0x0600B5CE RID: 46542 RVA: 0x0043C06C File Offset: 0x0043A26C
		[PublicizedFrom(EAccessModifier.Private)]
		public void Dispose(bool _disposing)
		{
			Log.Out("[MumbleLF] Disposing shm");
			if (!this.disposed)
			{
				if (_disposing)
				{
					this.memoryMappedFile.Dispose();
				}
				this.disposed = true;
			}
		}

		// Token: 0x0400884C RID: 34892
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly MemoryMappedFile memoryMappedFile;

		// Token: 0x0400884D RID: 34893
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe readonly WindowsLinkFile.WindowsLinkMemory* ptr;

		// Token: 0x0400884E RID: 34894
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposed;

		// Token: 0x020016B0 RID: 5808
		[PublicizedFrom(EAccessModifier.Private)]
		public struct WindowsLinkMemory
		{
			// Token: 0x0400884F RID: 34895
			public uint uiVersion;

			// Token: 0x04008850 RID: 34896
			public uint uiTick;

			// Token: 0x04008851 RID: 34897
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fAvatarPosition>e__FixedBuffer fAvatarPosition;

			// Token: 0x04008852 RID: 34898
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fAvatarFront>e__FixedBuffer fAvatarFront;

			// Token: 0x04008853 RID: 34899
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fAvatarTop>e__FixedBuffer fAvatarTop;

			// Token: 0x04008854 RID: 34900
			[FixedBuffer(typeof(ushort), 256)]
			public WindowsLinkFile.WindowsLinkMemory.<name>e__FixedBuffer name;

			// Token: 0x04008855 RID: 34901
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fCameraPosition>e__FixedBuffer fCameraPosition;

			// Token: 0x04008856 RID: 34902
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fCameraFront>e__FixedBuffer fCameraFront;

			// Token: 0x04008857 RID: 34903
			[FixedBuffer(typeof(float), 3)]
			public WindowsLinkFile.WindowsLinkMemory.<fCameraTop>e__FixedBuffer fCameraTop;

			// Token: 0x04008858 RID: 34904
			[FixedBuffer(typeof(ushort), 256)]
			public WindowsLinkFile.WindowsLinkMemory.<identity>e__FixedBuffer identity;

			// Token: 0x04008859 RID: 34905
			public uint context_len;

			// Token: 0x0400885A RID: 34906
			[FixedBuffer(typeof(byte), 256)]
			public WindowsLinkFile.WindowsLinkMemory.<context>e__FixedBuffer context;

			// Token: 0x0400885B RID: 34907
			[FixedBuffer(typeof(ushort), 2048)]
			public WindowsLinkFile.WindowsLinkMemory.<description>e__FixedBuffer description;

			// Token: 0x020016B1 RID: 5809
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 256)]
			public struct <context>e__FixedBuffer
			{
				// Token: 0x0400885C RID: 34908
				public byte FixedElementField;
			}

			// Token: 0x020016B2 RID: 5810
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 4096)]
			public struct <description>e__FixedBuffer
			{
				// Token: 0x0400885D RID: 34909
				public ushort FixedElementField;
			}

			// Token: 0x020016B3 RID: 5811
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarFront>e__FixedBuffer
			{
				// Token: 0x0400885E RID: 34910
				public float FixedElementField;
			}

			// Token: 0x020016B4 RID: 5812
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarPosition>e__FixedBuffer
			{
				// Token: 0x0400885F RID: 34911
				public float FixedElementField;
			}

			// Token: 0x020016B5 RID: 5813
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarTop>e__FixedBuffer
			{
				// Token: 0x04008860 RID: 34912
				public float FixedElementField;
			}

			// Token: 0x020016B6 RID: 5814
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraFront>e__FixedBuffer
			{
				// Token: 0x04008861 RID: 34913
				public float FixedElementField;
			}

			// Token: 0x020016B7 RID: 5815
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraPosition>e__FixedBuffer
			{
				// Token: 0x04008862 RID: 34914
				public float FixedElementField;
			}

			// Token: 0x020016B8 RID: 5816
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraTop>e__FixedBuffer
			{
				// Token: 0x04008863 RID: 34915
				public float FixedElementField;
			}

			// Token: 0x020016B9 RID: 5817
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 512)]
			public struct <identity>e__FixedBuffer
			{
				// Token: 0x04008864 RID: 34916
				public ushort FixedElementField;
			}

			// Token: 0x020016BA RID: 5818
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 512)]
			public struct <name>e__FixedBuffer
			{
				// Token: 0x04008865 RID: 34917
				public ushort FixedElementField;
			}
		}
	}
}
