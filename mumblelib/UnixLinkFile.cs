using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace mumblelib
{
	// Token: 0x020016A3 RID: 5795
	[PublicizedFrom(EAccessModifier.Internal)]
	public class UnixLinkFile : ILinkFile, IDisposable
	{
		// Token: 0x0600B5A8 RID: 46504
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public static extern int shm_open([MarshalAs(UnmanagedType.LPStr)] string name, int oflag, uint mode);

		// Token: 0x0600B5A9 RID: 46505
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public static extern uint getuid();

		// Token: 0x0600B5AA RID: 46506
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public static extern int ftruncate(int fd, long length);

		// Token: 0x0600B5AB RID: 46507
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public unsafe static extern void* mmap(void* addr, long length, int prot, int flags, int fd, long off);

		// Token: 0x0600B5AC RID: 46508
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public unsafe static extern void* munmap(void* addr, long length);

		// Token: 0x0600B5AD RID: 46509
		[PublicizedFrom(EAccessModifier.Private)]
		[DllImport("libc")]
		public static extern int close(int fd);

		// Token: 0x0600B5AE RID: 46510 RVA: 0x0043BC40 File Offset: 0x00439E40
		public unsafe UnixLinkFile()
		{
			this.fd = UnixLinkFile.shm_open("/MumbleLink." + UnixLinkFile.getuid().ToString(), 66, 384U);
			if (this.fd < 0)
			{
				throw new Exception("[MumbleLF] Failed to open shm");
			}
			Log.Out("[MumbleLF] FD opened");
			int num = Marshal.SizeOf<UnixLinkFile.LinuxLinkMemory>();
			if (UnixLinkFile.ftruncate(this.fd, (long)num) != 0)
			{
				Log.Error("[MumbleLF] Failed resizing shm");
				return;
			}
			Log.Out("[MumbleLF] Resized");
			this.ptr = (UnixLinkFile.LinuxLinkMemory*)UnixLinkFile.mmap(null, (long)num, 3, 1, this.fd, 0L);
			Log.Out("[MumbleLF] MemMapped");
		}

		// Token: 0x17001613 RID: 5651
		// (get) Token: 0x0600B5AF RID: 46511 RVA: 0x0043BCE7 File Offset: 0x00439EE7
		// (set) Token: 0x0600B5B0 RID: 46512 RVA: 0x0043BCF4 File Offset: 0x00439EF4
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

		// Token: 0x0600B5B1 RID: 46513 RVA: 0x0043BD02 File Offset: 0x00439F02
		public unsafe void Tick()
		{
			this.ptr->uiTick += 1U;
		}

		// Token: 0x17001614 RID: 5652
		// (set) Token: 0x0600B5B2 RID: 46514 RVA: 0x0043BD14 File Offset: 0x00439F14
		public unsafe Vector3 AvatarPosition
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarPosition.FixedElementField, value);
			}
		}

		// Token: 0x17001615 RID: 5653
		// (set) Token: 0x0600B5B3 RID: 46515 RVA: 0x0043BD2D File Offset: 0x00439F2D
		public unsafe Vector3 AvatarForward
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarFront.FixedElementField, value);
			}
		}

		// Token: 0x17001616 RID: 5654
		// (set) Token: 0x0600B5B4 RID: 46516 RVA: 0x0043BD46 File Offset: 0x00439F46
		public unsafe Vector3 AvatarTop
		{
			set
			{
				Util.SetVector3(&this.ptr->fAvatarTop.FixedElementField, value);
			}
		}

		// Token: 0x17001617 RID: 5655
		// (set) Token: 0x0600B5B5 RID: 46517 RVA: 0x0043BD5F File Offset: 0x00439F5F
		public unsafe string Name
		{
			set
			{
				Util.SetString<uint>(&this.ptr->name.FixedElementField, value, 256, Encoding.UTF32);
			}
		}

		// Token: 0x17001618 RID: 5656
		// (set) Token: 0x0600B5B6 RID: 46518 RVA: 0x0043BD82 File Offset: 0x00439F82
		public unsafe Vector3 CameraPosition
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraPosition.FixedElementField, value);
			}
		}

		// Token: 0x17001619 RID: 5657
		// (set) Token: 0x0600B5B7 RID: 46519 RVA: 0x0043BD9B File Offset: 0x00439F9B
		public unsafe Vector3 CameraForward
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraFront.FixedElementField, value);
			}
		}

		// Token: 0x1700161A RID: 5658
		// (set) Token: 0x0600B5B8 RID: 46520 RVA: 0x0043BDB4 File Offset: 0x00439FB4
		public unsafe Vector3 CameraTop
		{
			set
			{
				Util.SetVector3(&this.ptr->fCameraTop.FixedElementField, value);
			}
		}

		// Token: 0x1700161B RID: 5659
		// (set) Token: 0x0600B5B9 RID: 46521 RVA: 0x0043BDCD File Offset: 0x00439FCD
		public unsafe string Identity
		{
			set
			{
				Util.SetString<uint>(&this.ptr->identity.FixedElementField, value, 256, Encoding.UTF32);
			}
		}

		// Token: 0x1700161C RID: 5660
		// (set) Token: 0x0600B5BA RID: 46522 RVA: 0x0043BDF0 File Offset: 0x00439FF0
		public unsafe string Context
		{
			set
			{
				Util.SetContext(&this.ptr->context.FixedElementField, &this.ptr->context_len, value);
			}
		}

		// Token: 0x1700161D RID: 5661
		// (set) Token: 0x0600B5BB RID: 46523 RVA: 0x0043BE15 File Offset: 0x0043A015
		public unsafe string Description
		{
			set
			{
				Util.SetString<uint>(&this.ptr->description.FixedElementField, value, 2048, Encoding.UTF32);
			}
		}

		// Token: 0x0600B5BC RID: 46524 RVA: 0x0043BE38 File Offset: 0x0043A038
		public unsafe void Dispose()
		{
			if (!this.disposed)
			{
				UnixLinkFile.munmap((void*)this.ptr, (long)Marshal.SizeOf<UnixLinkFile.LinuxLinkMemory>());
				UnixLinkFile.close(this.fd);
				this.disposed = true;
			}
		}

		// Token: 0x0600B5BD RID: 46525 RVA: 0x0043BE68 File Offset: 0x0043A068
		[PublicizedFrom(EAccessModifier.Protected)]
		public ~UnixLinkFile()
		{
			this.Dispose();
		}

		// Token: 0x04008825 RID: 34853
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_RDONLY = 0;

		// Token: 0x04008826 RID: 34854
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_WRONLY = 1;

		// Token: 0x04008827 RID: 34855
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_RDWR = 2;

		// Token: 0x04008828 RID: 34856
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_CREAT = 64;

		// Token: 0x04008829 RID: 34857
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_EXCL = 128;

		// Token: 0x0400882A RID: 34858
		[PublicizedFrom(EAccessModifier.Private)]
		public const int O_TRUNC = 512;

		// Token: 0x0400882B RID: 34859
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PROT_READ = 1;

		// Token: 0x0400882C RID: 34860
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PROT_WRITE = 2;

		// Token: 0x0400882D RID: 34861
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PROT_EXEC = 4;

		// Token: 0x0400882E RID: 34862
		[PublicizedFrom(EAccessModifier.Private)]
		public const int PROT_NONE = 0;

		// Token: 0x0400882F RID: 34863
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MAP_SHARED = 1;

		// Token: 0x04008830 RID: 34864
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MAP_PRIVATE = 2;

		// Token: 0x04008831 RID: 34865
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MAP_SHARED_VALIDATE = 3;

		// Token: 0x04008832 RID: 34866
		[PublicizedFrom(EAccessModifier.Private)]
		public bool disposed;

		// Token: 0x04008833 RID: 34867
		[PublicizedFrom(EAccessModifier.Private)]
		public unsafe readonly UnixLinkFile.LinuxLinkMemory* ptr;

		// Token: 0x04008834 RID: 34868
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int fd;

		// Token: 0x020016A4 RID: 5796
		[PublicizedFrom(EAccessModifier.Private)]
		public struct LinuxLinkMemory
		{
			// Token: 0x04008835 RID: 34869
			public uint uiVersion;

			// Token: 0x04008836 RID: 34870
			public uint uiTick;

			// Token: 0x04008837 RID: 34871
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fAvatarPosition>e__FixedBuffer fAvatarPosition;

			// Token: 0x04008838 RID: 34872
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fAvatarFront>e__FixedBuffer fAvatarFront;

			// Token: 0x04008839 RID: 34873
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fAvatarTop>e__FixedBuffer fAvatarTop;

			// Token: 0x0400883A RID: 34874
			[FixedBuffer(typeof(uint), 256)]
			public UnixLinkFile.LinuxLinkMemory.<name>e__FixedBuffer name;

			// Token: 0x0400883B RID: 34875
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fCameraPosition>e__FixedBuffer fCameraPosition;

			// Token: 0x0400883C RID: 34876
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fCameraFront>e__FixedBuffer fCameraFront;

			// Token: 0x0400883D RID: 34877
			[FixedBuffer(typeof(float), 3)]
			public UnixLinkFile.LinuxLinkMemory.<fCameraTop>e__FixedBuffer fCameraTop;

			// Token: 0x0400883E RID: 34878
			[FixedBuffer(typeof(uint), 256)]
			public UnixLinkFile.LinuxLinkMemory.<identity>e__FixedBuffer identity;

			// Token: 0x0400883F RID: 34879
			public uint context_len;

			// Token: 0x04008840 RID: 34880
			[FixedBuffer(typeof(byte), 256)]
			public UnixLinkFile.LinuxLinkMemory.<context>e__FixedBuffer context;

			// Token: 0x04008841 RID: 34881
			[FixedBuffer(typeof(uint), 2048)]
			public UnixLinkFile.LinuxLinkMemory.<description>e__FixedBuffer description;

			// Token: 0x020016A5 RID: 5797
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 256)]
			public struct <context>e__FixedBuffer
			{
				// Token: 0x04008842 RID: 34882
				public byte FixedElementField;
			}

			// Token: 0x020016A6 RID: 5798
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 8192)]
			public struct <description>e__FixedBuffer
			{
				// Token: 0x04008843 RID: 34883
				public uint FixedElementField;
			}

			// Token: 0x020016A7 RID: 5799
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarFront>e__FixedBuffer
			{
				// Token: 0x04008844 RID: 34884
				public float FixedElementField;
			}

			// Token: 0x020016A8 RID: 5800
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarPosition>e__FixedBuffer
			{
				// Token: 0x04008845 RID: 34885
				public float FixedElementField;
			}

			// Token: 0x020016A9 RID: 5801
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fAvatarTop>e__FixedBuffer
			{
				// Token: 0x04008846 RID: 34886
				public float FixedElementField;
			}

			// Token: 0x020016AA RID: 5802
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraFront>e__FixedBuffer
			{
				// Token: 0x04008847 RID: 34887
				public float FixedElementField;
			}

			// Token: 0x020016AB RID: 5803
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraPosition>e__FixedBuffer
			{
				// Token: 0x04008848 RID: 34888
				public float FixedElementField;
			}

			// Token: 0x020016AC RID: 5804
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 12)]
			public struct <fCameraTop>e__FixedBuffer
			{
				// Token: 0x04008849 RID: 34889
				public float FixedElementField;
			}

			// Token: 0x020016AD RID: 5805
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 1024)]
			public struct <identity>e__FixedBuffer
			{
				// Token: 0x0400884A RID: 34890
				public uint FixedElementField;
			}

			// Token: 0x020016AE RID: 5806
			[CompilerGenerated]
			[UnsafeValueType]
			[StructLayout(LayoutKind.Sequential, Size = 1024)]
			public struct <name>e__FixedBuffer
			{
				// Token: 0x0400884B RID: 34891
				public uint FixedElementField;
			}
		}
	}
}
