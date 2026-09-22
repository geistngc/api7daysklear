using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

// Token: 0x020010F8 RID: 4344
public class BindingMethodCache
{
	// Token: 0x1700100A RID: 4106
	// (get) Token: 0x060089E8 RID: 35304 RVA: 0x00349701 File Offset: 0x00347901
	public static BindingMethodCache Instance
	{
		get
		{
			BindingMethodCache result;
			if ((result = BindingMethodCache.instance) == null)
			{
				result = (BindingMethodCache.instance = new BindingMethodCache());
			}
			return result;
		}
	}

	// Token: 0x060089E9 RID: 35305 RVA: 0x00349718 File Offset: 0x00347918
	[PublicizedFrom(EAccessModifier.Private)]
	public BindingMethodCache.BindingMethodTypeCache getCacheForType(Type _type)
	{
		BindingMethodCache.BindingMethodTypeCache bindingMethodTypeCache;
		if (!this.cachePerType.TryGetValue(_type, out bindingMethodTypeCache))
		{
			this.mswInit.Start();
			bindingMethodTypeCache = new BindingMethodCache.BindingMethodTypeCache(_type, this);
			this.mswInit.Stop();
			this.cachePerType[_type] = bindingMethodTypeCache;
		}
		return bindingMethodTypeCache;
	}

	// Token: 0x060089EA RID: 35306 RVA: 0x00349764 File Offset: 0x00347964
	public void RegisterCustomBindingMethod(string _bindingName, MethodInfo _method)
	{
		if (string.IsNullOrEmpty(_bindingName))
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Registering custom binding method for '",
				_bindingName,
				"' failed: Binding name has to be non-empty. Method: ",
				_method.DeclaringType.FullName,
				".",
				_method.Name
			}));
		}
		if (!_method.IsStatic)
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Registering custom binding method for '",
				_bindingName,
				"' failed: Method has to be static. Method: ",
				_method.DeclaringType.FullName,
				".",
				_method.Name
			}));
			return;
		}
		ParameterInfo[] parameters = _method.GetParameters();
		if (parameters.Length != 1 || !typeof(XUiController).IsAssignableFrom(parameters[0].ParameterType))
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Registering custom binding method for '",
				_bindingName,
				"' failed: Methods have to take exactly one parameter with the type inheriting from XUiController. Method: ",
				_method.DeclaringType.FullName,
				".",
				_method.Name
			}));
			return;
		}
		Type parameterType = parameters[0].ParameterType;
		XuiBindingDelegate value;
		if (!BindingMethodCache.tryGetBindingObjectDelegate(_method, out value))
		{
			return;
		}
		Dictionary<string, XuiBindingDelegate> dictionary;
		if (!this.customBindingDelegates.TryGetValue(parameterType, out dictionary))
		{
			dictionary = new Dictionary<string, XuiBindingDelegate>();
			this.customBindingDelegates[parameterType] = dictionary;
		}
		if (!dictionary.TryAdd(_bindingName, value))
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] Registering custom binding method for '",
				_bindingName,
				"' failed: Custom binding with that name already registered. Method: ",
				_method.DeclaringType.FullName,
				".",
				_method.Name
			}));
		}
	}

	// Token: 0x060089EB RID: 35307 RVA: 0x003498F6 File Offset: 0x00347AF6
	public bool TryGetBindingDelegate(XUiController _controller, string _bindingName, out XuiBindingDelegate _bindingDelegate)
	{
		return this.getCacheForType(_controller.GetType()).TryGetBindingDelegate(_bindingName, out _bindingDelegate);
	}

	// Token: 0x060089EC RID: 35308 RVA: 0x0034990B File Offset: 0x00347B0B
	public void LogInitTotal()
	{
		Log.Out(string.Format("[XUi] BindingMethodCache total init time: {0:F3} ms", (float)this.mswInit.ElapsedMicroseconds / 1000f));
	}

	// Token: 0x060089ED RID: 35309 RVA: 0x00349934 File Offset: 0x00347B34
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool tryGetBindingObjectDelegate(MethodInfo _method, out XuiBindingDelegate _func)
	{
		_func = null;
		if (_method == null)
		{
			Log.Error("[XUi] Failed creating binding method wrapper: MethodInfo null");
			return false;
		}
		Type returnType = _method.ReturnType;
		if (returnType == typeof(void))
		{
			Log.Error(string.Concat(new string[]
			{
				"[XUi] Failed creating binding method wrapper: Method has no return value (",
				_method.DeclaringType.FullName,
				".",
				_method.Name,
				")"
			}));
			return false;
		}
		Type type = _method.IsStatic ? _method.GetParameters()[0].ParameterType : _method.DeclaringType;
		MethodInfo methodInfo;
		if (returnType == typeof(bool))
		{
			methodInfo = new Func<MethodInfo, XuiBindingDelegate>(BindingMethodCache.<tryGetBindingObjectDelegate>g__GetBindingObjectDelegateCoreBool|16_1<XUiController>).Method.GetGenericMethodDefinition().MakeGenericMethod(new Type[]
			{
				type
			});
		}
		else if (returnType == typeof(int))
		{
			methodInfo = new Func<MethodInfo, XuiBindingDelegate>(BindingMethodCache.<tryGetBindingObjectDelegate>g__GetBindingObjectDelegateCoreInt|16_2<XUiController>).Method.GetGenericMethodDefinition().MakeGenericMethod(new Type[]
			{
				type
			});
		}
		else
		{
			methodInfo = new Func<MethodInfo, XuiBindingDelegate>(BindingMethodCache.<tryGetBindingObjectDelegate>g__GetBindingObjectDelegateCoreGeneric|16_0<XUiController, object>).Method.GetGenericMethodDefinition().MakeGenericMethod(new Type[]
			{
				type,
				returnType
			});
		}
		_func = (XuiBindingDelegate)methodInfo.Invoke(null, new object[]
		{
			_method
		});
		return true;
	}

	// Token: 0x060089F0 RID: 35312 RVA: 0x00349AE5 File Offset: 0x00347CE5
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static XuiBindingDelegate <tryGetBindingObjectDelegate>g__GetBindingObjectDelegateCoreGeneric|16_0<TController, TValue>(MethodInfo _methodInner) where TController : XUiController
	{
		Func<TController, TValue> typedFunc = (Func<TController, TValue>)Delegate.CreateDelegate(typeof(Func<TController, TValue>), _methodInner);
		return (XUiController _instance) => typedFunc((TController)((object)_instance));
	}

	// Token: 0x060089F1 RID: 35313 RVA: 0x00349B12 File Offset: 0x00347D12
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static XuiBindingDelegate <tryGetBindingObjectDelegate>g__GetBindingObjectDelegateCoreBool|16_1<TController>(MethodInfo _methodInner) where TController : XUiController
	{
		Func<TController, bool> typedFunc = (Func<TController, bool>)Delegate.CreateDelegate(typeof(Func<TController, bool>), _methodInner);
		return delegate(XUiController _instance)
		{
			if (!typedFunc((TController)((object)_instance)))
			{
				return BindingMethodCache.boxedFalse;
			}
			return BindingMethodCache.boxedTrue;
		};
	}

	// Token: 0x060089F2 RID: 35314 RVA: 0x00349B3F File Offset: 0x00347D3F
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static XuiBindingDelegate <tryGetBindingObjectDelegate>g__GetBindingObjectDelegateCoreInt|16_2<TController>(MethodInfo _methodInner) where TController : XUiController
	{
		Func<TController, int> typedFunc = (Func<TController, int>)Delegate.CreateDelegate(typeof(Func<TController, int>), _methodInner);
		return delegate(XUiController _instance)
		{
			int num = typedFunc((TController)((object)_instance));
			object result;
			if (num != 0)
			{
				if (num != 1)
				{
					result = num;
				}
				else
				{
					result = BindingMethodCache.boxed1;
				}
			}
			else
			{
				result = BindingMethodCache.boxed0;
			}
			return result;
		};
	}

	// Token: 0x0400669B RID: 26267
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly MicroStopwatch mswInit = new MicroStopwatch(false);

	// Token: 0x0400669C RID: 26268
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<Type, BindingMethodCache.BindingMethodTypeCache> cachePerType = new Dictionary<Type, BindingMethodCache.BindingMethodTypeCache>(128);

	// Token: 0x0400669D RID: 26269
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<Type, Dictionary<string, XuiBindingDelegate>> customBindingDelegates = new Dictionary<Type, Dictionary<string, XuiBindingDelegate>>();

	// Token: 0x0400669E RID: 26270
	[PublicizedFrom(EAccessModifier.Private)]
	public static BindingMethodCache instance;

	// Token: 0x0400669F RID: 26271
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object boxed0 = 0;

	// Token: 0x040066A0 RID: 26272
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object boxed1 = 1;

	// Token: 0x040066A1 RID: 26273
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object boxedFalse = false;

	// Token: 0x040066A2 RID: 26274
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly object boxedTrue = true;

	// Token: 0x020010F9 RID: 4345
	public class BindingMethodData
	{
		// Token: 0x060089F3 RID: 35315 RVA: 0x00349B6C File Offset: 0x00347D6C
		public BindingMethodData(string _definingController, XuiBindingDelegate _delegate)
		{
			this.DefiningController = _definingController;
			this.Delegate = _delegate;
		}

		// Token: 0x040066A3 RID: 26275
		public readonly string DefiningController;

		// Token: 0x040066A4 RID: 26276
		public readonly XuiBindingDelegate Delegate;
	}

	// Token: 0x020010FA RID: 4346
	public class BindingMethodTypeCache
	{
		// Token: 0x060089F4 RID: 35316 RVA: 0x00349B82 File Offset: 0x00347D82
		public BindingMethodTypeCache(Type _controllerType, BindingMethodCache _owner)
		{
			this.controllerType = _controllerType;
			this.initCacheForController(_owner);
		}

		// Token: 0x060089F5 RID: 35317 RVA: 0x00349BAC File Offset: 0x00347DAC
		[PublicizedFrom(EAccessModifier.Private)]
		public void initCacheForController(BindingMethodCache _owner)
		{
			if (this.controllerType.BaseType != null)
			{
				_owner.getCacheForType(this.controllerType.BaseType).methods.CopyTo(this.methods, false);
			}
			ReflectionHelpers.GetMethodsWithAttribute<XuiXmlBindingAttribute>(this.controllerType, new Action<MethodInfo, bool, XuiXmlBindingAttribute>(this.<initCacheForController>g__MethodFoundCallback|3_1), true, true, true);
			ReflectionHelpers.GetPropertiesWithAttribute<XuiXmlBindingAttribute>(this.controllerType, new Action<PropertyInfo, bool, XuiXmlBindingAttribute>(this.<initCacheForController>g__PropertyFoundCallback|3_0), true, true, true);
			Dictionary<string, XuiBindingDelegate> dictionary;
			if (_owner.customBindingDelegates.TryGetValue(this.controllerType, out dictionary))
			{
				foreach (KeyValuePair<string, XuiBindingDelegate> keyValuePair in dictionary)
				{
					string text;
					XuiBindingDelegate xuiBindingDelegate;
					keyValuePair.Deconstruct(out text, out xuiBindingDelegate);
					string text2 = text;
					XuiBindingDelegate @delegate = xuiBindingDelegate;
					BindingMethodCache.BindingMethodData bindingMethodData;
					if (this.methods.TryGetValue(text2, out bindingMethodData))
					{
						Log.Warning(string.Concat(new string[]
						{
							"[XUi] Custom binding overriding overriding existing (",
							bindingMethodData.DefiningController,
							") binding definition of '",
							text2,
							"'"
						}));
					}
					this.methods[text2] = new BindingMethodCache.BindingMethodData("<Custom>", @delegate);
				}
			}
		}

		// Token: 0x060089F6 RID: 35318 RVA: 0x00349CE4 File Offset: 0x00347EE4
		public bool TryGetBindingDelegate(string _bindingName, out XuiBindingDelegate _bindingDelegate)
		{
			BindingMethodCache.BindingMethodData bindingMethodData;
			if (this.methods.TryGetValue(_bindingName, out bindingMethodData))
			{
				_bindingDelegate = bindingMethodData.Delegate;
				return true;
			}
			_bindingDelegate = null;
			return false;
		}

		// Token: 0x060089F7 RID: 35319 RVA: 0x00349D10 File Offset: 0x00347F10
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <initCacheForController>g__PropertyFoundCallback|3_0(PropertyInfo _property, bool _hasMultiple, XuiXmlBindingAttribute _bindingAttribute)
		{
			MethodInfo getMethod = _property.GetGetMethod(true);
			if (getMethod == null)
			{
				Log.Error(string.Concat(new string[]
				{
					"[XUi] Failed creating binding property wrapper: Property has no getter (",
					_property.DeclaringType.FullName,
					".",
					_property.Name,
					")"
				}));
			}
			this.<initCacheForController>g__MethodFoundCallback|3_1(getMethod, _hasMultiple, _bindingAttribute);
		}

		// Token: 0x060089F8 RID: 35320 RVA: 0x00349D78 File Offset: 0x00347F78
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <initCacheForController>g__MethodFoundCallback|3_1(MethodInfo _method, bool _hasMultiple, XuiXmlBindingAttribute _bindingAttribute)
		{
			if (_method.IsStatic)
			{
				Log.Error("[XUi] XML binding has to be non-static! " + _method.DeclaringType.FullName + "." + _method.Name);
				return;
			}
			if (_method.GetParameters().Length != 0)
			{
				Log.Error(string.Concat(new string[]
				{
					"[XUi] Failed creating binding method wrapper: Method has parameters (",
					_method.DeclaringType.FullName,
					".",
					_method.Name,
					")"
				}));
				return;
			}
			XuiBindingDelegate @delegate;
			if (!BindingMethodCache.tryGetBindingObjectDelegate(_method, out @delegate))
			{
				return;
			}
			BindingMethodCache.BindingMethodData bindingMethodData = new BindingMethodCache.BindingMethodData(_method.DeclaringType.FullName, @delegate);
			BindingMethodCache.BindingMethodData bindingMethodData2;
			if (this.methods.TryGetValue(_bindingAttribute.BindingName, out bindingMethodData2))
			{
				Log.Warning(string.Concat(new string[]
				{
					"[XUi] Child class (",
					bindingMethodData.DefiningController,
					") overriding parent (",
					bindingMethodData2.DefiningController,
					") binding definition of '",
					_bindingAttribute.BindingName,
					"'"
				}));
			}
			this.methods[_bindingAttribute.BindingName] = bindingMethodData;
		}

		// Token: 0x040066A5 RID: 26277
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Type controllerType;

		// Token: 0x040066A6 RID: 26278
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Dictionary<string, BindingMethodCache.BindingMethodData> methods = new Dictionary<string, BindingMethodCache.BindingMethodData>(64, StringComparer.Ordinal);
	}
}
