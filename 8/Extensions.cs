using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x02001401 RID: 5121
public static class Extensions
{
	// Token: 0x0600A0A3 RID: 41123 RVA: 0x003C7D40 File Offset: 0x003C5F40
	public static Transform FindInChildren(this Transform _t, string _name)
	{
		int childCount = _t.childCount;
		if (childCount == 0)
		{
			return null;
		}
		Transform transform = _t.Find(_name);
		if (!transform)
		{
			for (int i = 0; i < childCount; i++)
			{
				transform = _t.GetChild(i).FindInChildren(_name);
				if (transform)
				{
					break;
				}
			}
		}
		return transform;
	}

	// Token: 0x0600A0A4 RID: 41124 RVA: 0x003C7D8C File Offset: 0x003C5F8C
	public static Transform FindTagInChildren(this Transform _t, string _tag)
	{
		if (!_t)
		{
			return null;
		}
		if (_t.CompareTag(_tag))
		{
			return _t;
		}
		int childCount = _t.childCount;
		if (childCount == 0)
		{
			return null;
		}
		for (int i = 0; i < childCount; i++)
		{
			Transform transform = _t.GetChild(i).FindTagInChildren(_tag);
			if (transform)
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x0600A0A5 RID: 41125 RVA: 0x003C7DE0 File Offset: 0x003C5FE0
	public static Transform FindInChilds(this Transform target, string name, bool onlyActive = false)
	{
		if (!target || name == null)
		{
			return null;
		}
		if (onlyActive && (!target.gameObject || !target.gameObject.activeSelf))
		{
			return null;
		}
		if (target.name == name)
		{
			return target;
		}
		for (int i = 0; i < target.childCount; i++)
		{
			Transform transform = target.GetChild(i).FindInChilds(name, onlyActive);
			if (transform != null)
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x0600A0A6 RID: 41126 RVA: 0x003C7E55 File Offset: 0x003C6055
	public static T GetComponentInChildren<T>(this GameObject o, bool searchInactive, bool avoidGC = false) where T : Component
	{
		return o.transform.GetComponentInChildren(searchInactive, avoidGC);
	}

	// Token: 0x0600A0A7 RID: 41127 RVA: 0x003C7E64 File Offset: 0x003C6064
	public static T GetComponentInChildren<T>(this Component c, bool searchInactive, bool avoidGC = false) where T : Component
	{
		return c.transform.GetComponentInChildren(searchInactive, avoidGC);
	}

	// Token: 0x0600A0A8 RID: 41128 RVA: 0x003C7E74 File Offset: 0x003C6074
	public static T GetComponentInChildren<T>(this Transform t, bool searchInactive, bool avoidGC = false) where T : Component
	{
		if (!searchInactive)
		{
			return t.GetComponentInChildren<T>();
		}
		if (avoidGC)
		{
			T t2 = t.GetComponent<T>();
			if (t2 == null)
			{
				for (int i = 0; i < t.childCount; i++)
				{
					t2 = t.GetChild(i).GetComponentInChildren(searchInactive, avoidGC);
					if (t2 != null)
					{
						break;
					}
				}
			}
			return t2;
		}
		T[] componentsInChildren = t.GetComponentsInChildren<T>(true);
		if (componentsInChildren.Length == 0)
		{
			return default(T);
		}
		return componentsInChildren[0];
	}

	// Token: 0x0600A0A9 RID: 41129 RVA: 0x003C7EF0 File Offset: 0x003C60F0
	public static T GetOrAddComponent<T>(this GameObject go) where T : Component
	{
		T t = go.GetComponent<T>();
		if (t == null)
		{
			t = go.AddComponent<T>();
		}
		return t;
	}

	// Token: 0x0600A0AA RID: 41130 RVA: 0x003C7F1C File Offset: 0x003C611C
	public static string GetGameObjectPath(this GameObject _obj)
	{
		string text = "/" + _obj.name;
		while (_obj.transform.parent)
		{
			_obj = _obj.transform.parent.gameObject;
			text = "/" + _obj.name + text;
		}
		return text;
	}

	// Token: 0x0600A0AB RID: 41131 RVA: 0x003C7F74 File Offset: 0x003C6174
	public static bool ContainsWithComparer<T>(this List<T> _list, T _item, IEqualityComparer<T> _comparer)
	{
		if (_list == null)
		{
			throw new ArgumentNullException("_list");
		}
		if (_comparer == null)
		{
			_comparer = EqualityComparer<T>.Default;
		}
		for (int i = 0; i < _list.Count; i++)
		{
			if (_comparer.Equals(_list[i], _item))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600A0AC RID: 41132 RVA: 0x003C7FC0 File Offset: 0x003C61C0
	public static bool ContainsCaseInsensitive(this IList<string> _list, string _item)
	{
		if (_item == null)
		{
			for (int i = 0; i < _list.Count; i++)
			{
				if (_list[i] == null)
				{
					return true;
				}
			}
			return false;
		}
		for (int j = 0; j < _list.Count; j++)
		{
			if (StringComparer.OrdinalIgnoreCase.Equals(_list[j], _item))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600A0AD RID: 41133 RVA: 0x003C8018 File Offset: 0x003C6218
	public static void CopyTo<T>(this IList<T> _srcList, IList<T> _dest)
	{
		foreach (T item in _srcList)
		{
			_dest.Add(item);
		}
	}

	// Token: 0x0600A0AE RID: 41134 RVA: 0x003C8060 File Offset: 0x003C6260
	public static void Shuffle<T>(this List<T> _list)
	{
		_list.Shuffle(GameManager.Instance.World.GetGameRandom());
	}

	// Token: 0x0600A0AF RID: 41135 RVA: 0x003C8078 File Offset: 0x003C6278
	public static void Shuffle<T>(this List<T> _list, int _seed)
	{
		GameRandom gameRandom = GameRandomManager.Instance.CreateGameRandom(_seed);
		_list.Shuffle(gameRandom);
		GameRandomManager.Instance.FreeGameRandom(gameRandom);
	}

	// Token: 0x0600A0B0 RID: 41136 RVA: 0x003C80A4 File Offset: 0x003C62A4
	public static void Shuffle<T>(this List<T> _list, GameRandom _rand)
	{
		int i = _list.Count;
		while (i > 1)
		{
			int num = _rand.RandomRange(0, i--);
			int index = num;
			int index2 = i;
			T value = _list[i];
			T value2 = _list[num];
			_list[index] = value;
			_list[index2] = value2;
		}
	}

	// Token: 0x0600A0B1 RID: 41137 RVA: 0x003C80FD File Offset: 0x003C62FD
	public static bool ColorEquals(this Color32 _a, Color32 _b)
	{
		return _a.r == _b.r && _a.g == _b.g && _a.b == _b.b && _a.a == _b.a;
	}

	// Token: 0x0600A0B2 RID: 41138 RVA: 0x003C8139 File Offset: 0x003C6339
	public static string ToHexCode(this Color _color, bool _includeAlpha = false)
	{
		return _color.ToHexCode(_includeAlpha);
	}

	// Token: 0x0600A0B3 RID: 41139 RVA: 0x003C8147 File Offset: 0x003C6347
	public static Color WithAlpha(this Color _color, float _alpha)
	{
		return new Color(_color.r, _color.g, _color.b, _alpha);
	}

	// Token: 0x0600A0B4 RID: 41140 RVA: 0x003C8164 File Offset: 0x003C6364
	public static string ToHexCode(this Color32 _color, bool _includeAlpha = false)
	{
		if (!_includeAlpha)
		{
			return string.Format("{0:X02}{1:X02}{2:X02}", _color.r, _color.g, _color.b);
		}
		return string.Format("{0:X02}{1:X02}{2:X02}{3:X02}", new object[]
		{
			_color.r,
			_color.g,
			_color.b,
			_color.a
		});
	}

	// Token: 0x0600A0B5 RID: 41141 RVA: 0x003C81E8 File Offset: 0x003C63E8
	public unsafe static void WriteToBuffer(this Guid _value, byte[] _dest, int _offset)
	{
		if (_dest.Length - _offset < 16)
		{
			throw new ArgumentException("buffer too small");
		}
		fixed (byte[] array = _dest)
		{
			byte* ptr;
			if (_dest == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			long* ptr2 = (long*)(&_value);
			long* ptr3 = (long*)(ptr + _offset);
			*ptr3 = *ptr2;
			ptr3[1] = ptr2[1];
		}
	}

	// Token: 0x0600A0B6 RID: 41142 RVA: 0x003C8237 File Offset: 0x003C6437
	public static Bounds Offset(this Bounds _bounds, Vector3 _offset)
	{
		return new Bounds(_bounds.center + _offset, _bounds.size);
	}

	// Token: 0x0600A0B7 RID: 41143 RVA: 0x003C8254 File Offset: 0x003C6454
	public static bool ContainsInclusive(this BoundsInt bounds, Vector3Int position)
	{
		return position.x >= bounds.min.x && position.y >= bounds.min.y && position.z >= bounds.min.z && position.x <= bounds.max.x && position.y <= bounds.max.y && position.z <= bounds.max.z;
	}

	// Token: 0x0600A0B8 RID: 41144 RVA: 0x003C82F8 File Offset: 0x003C64F8
	public static BoundsInt InitFromPoints(IEnumerable<Vector3i> points)
	{
		Vector3i vector3i = new Vector3i(int.MaxValue, int.MaxValue, int.MaxValue);
		Vector3i vector3i2 = new Vector3i(int.MinValue, int.MinValue, int.MinValue);
		foreach (Vector3i v in points)
		{
			vector3i = Vector3i.Min(vector3i, v);
			vector3i2 = Vector3i.Max(vector3i2, v);
		}
		BoundsInt result = default(BoundsInt);
		result.SetMinMax(vector3i, vector3i2);
		return result;
	}

	// Token: 0x0600A0B9 RID: 41145 RVA: 0x003C8394 File Offset: 0x003C6594
	public static void CalculatePersistableHash(this Bounds _bounds, IncrementalHash calcHash)
	{
		calcHash.AppendDataNoAlloc(_bounds.center.x);
		calcHash.AppendDataNoAlloc(_bounds.center.y);
		calcHash.AppendDataNoAlloc(_bounds.center.z);
		calcHash.AppendDataNoAlloc(_bounds.size.x);
		calcHash.AppendDataNoAlloc(_bounds.size.y);
		calcHash.AppendDataNoAlloc(_bounds.size.z);
	}

	// Token: 0x0600A0BA RID: 41146 RVA: 0x000063DF File Offset: 0x000045DF
	public static bool EqualsCaseInsensitive(this string _a, string _b)
	{
		return string.Equals(_a, _b, StringComparison.OrdinalIgnoreCase);
	}

	// Token: 0x0600A0BB RID: 41147 RVA: 0x003C840D File Offset: 0x003C660D
	public static bool ContainsCaseInsensitive(this string _a, string _b)
	{
		return _a.IndexOf(_b, StringComparison.OrdinalIgnoreCase) >= 0;
	}

	// Token: 0x0600A0BC RID: 41148 RVA: 0x003C841D File Offset: 0x003C661D
	public static string SeparateCamelCase(this string _value)
	{
		return Extensions.StringSeparationRegex.Replace(_value, " $1").Trim();
	}

	// Token: 0x0600A0BD RID: 41149 RVA: 0x003C8434 File Offset: 0x003C6634
	public static string UppercaseFirst(this string _s)
	{
		if (string.IsNullOrEmpty(_s))
		{
			return string.Empty;
		}
		return char.ToUpper(_s[0]).ToString() + _s.Substring(1);
	}

	// Token: 0x0600A0BE RID: 41150 RVA: 0x003C846F File Offset: 0x003C666F
	public unsafe static string ToHexString(this ReadOnlyMemory<byte> bytes)
	{
		if (bytes.Length <= 0)
		{
			return string.Empty;
		}
		return string.Create<ReadOnlyMemory<byte>>(bytes.Length * 2, bytes, delegate(Span<char> dest, ReadOnlyMemory<byte> srcMemory)
		{
			ReadOnlySpan<byte> span = srcMemory.Span;
			for (int i = 0; i < span.Length; i++)
			{
				int num = i * 2;
				*dest[num] = Extensions.HexLookup[*span[i] >> 4];
				*dest[num + 1] = Extensions.HexLookup[(int)(*span[i] & 15)];
			}
		});
	}

	// Token: 0x0600A0BF RID: 41151 RVA: 0x003C84AF File Offset: 0x003C66AF
	public static string ToHexString(this byte[] bytes)
	{
		return bytes.ToHexString();
	}

	// Token: 0x0600A0C0 RID: 41152 RVA: 0x003C84BC File Offset: 0x003C66BC
	public static string ToUnicodeCodepoints(this string _value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (char c in _value)
		{
			stringBuilder.AppendFormat("\\u{0:X4} ", (int)c);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600A0C1 RID: 41153 RVA: 0x003C8502 File Offset: 0x003C6702
	public static string RemoveLineBreaks(this string _value)
	{
		return _value.Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ");
	}

	// Token: 0x0600A0C2 RID: 41154 RVA: 0x003C8532 File Offset: 0x003C6732
	public static int GetStableHashCode(this string _str)
	{
		return _str.AsSpan().GetStableHashCode();
	}

	// Token: 0x0600A0C3 RID: 41155 RVA: 0x003C8540 File Offset: 0x003C6740
	public unsafe static int GetStableHashCode(this ReadOnlySpan<char> _str)
	{
		int num = 5381;
		int num2 = num;
		int num3 = 0;
		while (num3 < _str.Length && *_str[num3] != 0)
		{
			num = ((num << 5) + num ^ (int)(*_str[num3]));
			if (num3 == _str.Length - 1 || *_str[num3 + 1] == 0)
			{
				break;
			}
			num2 = ((num2 << 5) + num2 ^ (int)(*_str[num3 + 1]));
			num3 += 2;
		}
		return num + num2 * 1566083941;
	}

	// Token: 0x0600A0C4 RID: 41156 RVA: 0x003C85B8 File Offset: 0x003C67B8
	public static string Unindent(this string _indented, bool _trimEmptyLines = true)
	{
		if (_trimEmptyLines)
		{
			_indented = Extensions.unindentEmptyBeginning.Replace(_indented, string.Empty);
			_indented = Extensions.unindentEmptyEnd.Replace(_indented, string.Empty);
		}
		_indented = Extensions.unindentIndentationNoLinebreak.Replace(_indented, " $1");
		_indented = Extensions.unindentIndentationRegularLinebreak.Replace(_indented, string.Empty);
		return _indented;
	}

	// Token: 0x0600A0C5 RID: 41157 RVA: 0x003C8614 File Offset: 0x003C6814
	public static StringBuilder TrimEnd(this StringBuilder _sb)
	{
		if (_sb == null || _sb.Length == 0)
		{
			return _sb;
		}
		int num = _sb.Length - 1;
		while (num >= 0 && char.IsWhiteSpace(_sb[num]))
		{
			num--;
		}
		if (num < _sb.Length - 1)
		{
			_sb.Length = num + 1;
		}
		return _sb;
	}

	// Token: 0x0600A0C6 RID: 41158 RVA: 0x003C8664 File Offset: 0x003C6864
	public static StringBuilder TrimStart(this StringBuilder _sb)
	{
		if (_sb == null || _sb.Length == 0)
		{
			return _sb;
		}
		int num = 0;
		while (num < _sb.Length && char.IsWhiteSpace(_sb[num]))
		{
			num++;
		}
		if (num > 0)
		{
			_sb.Remove(0, num);
		}
		return _sb;
	}

	// Token: 0x0600A0C7 RID: 41159 RVA: 0x003C86AB File Offset: 0x003C68AB
	public static StringBuilder Trim(this StringBuilder _sb)
	{
		if (_sb == null || _sb.Length == 0)
		{
			return _sb;
		}
		return _sb.TrimEnd().TrimStart();
	}

	// Token: 0x0600A0C8 RID: 41160 RVA: 0x003C86C5 File Offset: 0x003C68C5
	public static string ToCultureInvariantString(this float _value)
	{
		return _value.ToString(Utils.StandardCulture);
	}

	// Token: 0x0600A0C9 RID: 41161 RVA: 0x003C86D3 File Offset: 0x003C68D3
	public static string ToCultureInvariantString(this double _value)
	{
		return _value.ToString(Utils.StandardCulture);
	}

	// Token: 0x0600A0CA RID: 41162 RVA: 0x003C86E1 File Offset: 0x003C68E1
	public static string ToCultureInvariantString(this float _value, string _format)
	{
		return _value.ToString(_format, Utils.StandardCulture);
	}

	// Token: 0x0600A0CB RID: 41163 RVA: 0x003C86F0 File Offset: 0x003C68F0
	public static string ToCultureInvariantString(this double _value, string _format)
	{
		return _value.ToString(_format, Utils.StandardCulture);
	}

	// Token: 0x0600A0CC RID: 41164 RVA: 0x003C86FF File Offset: 0x003C68FF
	public static string ToCultureInvariantString(this decimal _value)
	{
		return _value.ToString(Utils.StandardCulture);
	}

	// Token: 0x0600A0CD RID: 41165 RVA: 0x003C870D File Offset: 0x003C690D
	public static string ToCultureInvariantString(this decimal _value, string _format)
	{
		return _value.ToString(_format, Utils.StandardCulture);
	}

	// Token: 0x0600A0CE RID: 41166 RVA: 0x003C871C File Offset: 0x003C691C
	public static string ToCultureInvariantString(this DateTime _value)
	{
		return _value.ToString(Utils.StandardCulture);
	}

	// Token: 0x0600A0CF RID: 41167 RVA: 0x003C872C File Offset: 0x003C692C
	public static string ToCultureInvariantString(this Vector2 _value)
	{
		return string.Concat(new string[]
		{
			"(",
			_value.x.ToCultureInvariantString("F1"),
			", ",
			_value.y.ToCultureInvariantString("F1"),
			")"
		});
	}

	// Token: 0x0600A0D0 RID: 41168 RVA: 0x003C8784 File Offset: 0x003C6984
	public static string ToCultureInvariantString(this Vector2 _value, string _format)
	{
		return string.Concat(new string[]
		{
			"(",
			_value.x.ToCultureInvariantString(_format),
			", ",
			_value.y.ToCultureInvariantString(_format),
			")"
		});
	}

	// Token: 0x0600A0D1 RID: 41169 RVA: 0x003C87D4 File Offset: 0x003C69D4
	public static string ToCultureInvariantString(this Vector3 _value)
	{
		return string.Concat(new string[]
		{
			"(",
			_value.x.ToCultureInvariantString("F1"),
			", ",
			_value.y.ToCultureInvariantString("F1"),
			", ",
			_value.z.ToCultureInvariantString("F1"),
			")"
		});
	}

	// Token: 0x0600A0D2 RID: 41170 RVA: 0x003C8848 File Offset: 0x003C6A48
	public static string ToCultureInvariantString(this Vector3 _value, string _format)
	{
		return string.Concat(new string[]
		{
			"(",
			_value.x.ToCultureInvariantString(_format),
			", ",
			_value.y.ToCultureInvariantString(_format),
			", ",
			_value.z.ToCultureInvariantString(_format),
			")"
		});
	}

	// Token: 0x0600A0D3 RID: 41171 RVA: 0x003C88B0 File Offset: 0x003C6AB0
	public static string ToCultureInvariantString(this Vector4 _value)
	{
		return string.Concat(new string[]
		{
			"(",
			_value.x.ToCultureInvariantString("F1"),
			", ",
			_value.y.ToCultureInvariantString("F1"),
			", ",
			_value.z.ToCultureInvariantString("F1"),
			", ",
			_value.w.ToCultureInvariantString("F1"),
			")"
		});
	}

	// Token: 0x0600A0D4 RID: 41172 RVA: 0x003C8940 File Offset: 0x003C6B40
	public static string ToCultureInvariantString(this Vector4 _value, string _format)
	{
		return string.Concat(new string[]
		{
			"(",
			_value.x.ToCultureInvariantString(_format),
			", ",
			_value.y.ToCultureInvariantString(_format),
			", ",
			_value.z.ToCultureInvariantString(_format),
			", ",
			_value.w.ToCultureInvariantString(_format),
			")"
		});
	}

	// Token: 0x0600A0D5 RID: 41173 RVA: 0x003C89BD File Offset: 0x003C6BBD
	public static string ToCultureInvariantString(this Bounds _value)
	{
		return "Center: " + _value.center.ToCultureInvariantString() + ", Extents: " + _value.extents.ToCultureInvariantString();
	}

	// Token: 0x0600A0D6 RID: 41174 RVA: 0x003C89E8 File Offset: 0x003C6BE8
	public static string ToCultureInvariantString(this Rect _value)
	{
		return string.Concat(new string[]
		{
			"(x:",
			_value.x.ToCultureInvariantString("F2"),
			", y:",
			_value.y.ToCultureInvariantString("F2"),
			", width:",
			_value.width.ToCultureInvariantString("F2"),
			", height:",
			_value.height.ToCultureInvariantString("F2"),
			")"
		});
	}

	// Token: 0x0600A0D7 RID: 41175 RVA: 0x003C8A7C File Offset: 0x003C6C7C
	public static string ToCultureInvariantString(this Quaternion _value)
	{
		return string.Concat(new string[]
		{
			"(",
			_value.x.ToCultureInvariantString("F1"),
			", ",
			_value.y.ToCultureInvariantString("F1"),
			", ",
			_value.z.ToCultureInvariantString("F1"),
			", ",
			_value.w.ToCultureInvariantString("F1"),
			")"
		});
	}

	// Token: 0x0600A0D8 RID: 41176 RVA: 0x003C8B0C File Offset: 0x003C6D0C
	public static string ToCultureInvariantString(this Matrix4x4 _value)
	{
		return string.Concat(new string[]
		{
			_value.m00.ToCultureInvariantString("F5"),
			"\t",
			_value.m01.ToCultureInvariantString("F5"),
			"\t",
			_value.m02.ToCultureInvariantString("F5"),
			"\t",
			_value.m03.ToCultureInvariantString("F5"),
			"\n",
			_value.m10.ToCultureInvariantString("F5"),
			"\t",
			_value.m11.ToCultureInvariantString("F5"),
			"\t",
			_value.m12.ToCultureInvariantString("F5"),
			"\t",
			_value.m13.ToCultureInvariantString("F5"),
			"\n",
			_value.m20.ToCultureInvariantString("F5"),
			"\t",
			_value.m21.ToCultureInvariantString("F5"),
			"\t",
			_value.m22.ToCultureInvariantString("F5"),
			"\t",
			_value.m23.ToCultureInvariantString("F5"),
			"\n",
			_value.m30.ToCultureInvariantString("F5"),
			"\t",
			_value.m31.ToCultureInvariantString("F5"),
			"\t",
			_value.m32.ToCultureInvariantString("F5"),
			"\t",
			_value.m33.ToCultureInvariantString("F5"),
			"\n"
		});
	}

	// Token: 0x0600A0D9 RID: 41177 RVA: 0x003C8CEC File Offset: 0x003C6EEC
	public static string ToCultureInvariantString(this Color _value)
	{
		return string.Concat(new string[]
		{
			"RGBA(",
			_value.r.ToCultureInvariantString("F3"),
			", ",
			_value.g.ToCultureInvariantString("F3"),
			", ",
			_value.b.ToCultureInvariantString("F3"),
			", ",
			_value.a.ToCultureInvariantString("F3"),
			")"
		});
	}

	// Token: 0x0600A0DA RID: 41178 RVA: 0x003C8D7C File Offset: 0x003C6F7C
	public static string ToCultureInvariantString(this Plane _value)
	{
		return string.Concat(new string[]
		{
			"(normal:(",
			_value.normal.x.ToCultureInvariantString("F1"),
			", ",
			_value.normal.y.ToCultureInvariantString("F1"),
			", ",
			_value.normal.z.ToCultureInvariantString("F1"),
			"), distance:",
			_value.distance.ToCultureInvariantString("F1"),
			")"
		});
	}

	// Token: 0x0600A0DB RID: 41179 RVA: 0x003C8E1C File Offset: 0x003C701C
	public static string ToCultureInvariantString(this Ray _value)
	{
		return "Origin: " + _value.origin.ToCultureInvariantString() + ", Dir: " + _value.direction.ToCultureInvariantString();
	}

	// Token: 0x0600A0DC RID: 41180 RVA: 0x003C8E45 File Offset: 0x003C7045
	public static string ToCultureInvariantString(this Ray2D _value)
	{
		return "Origin: " + _value.origin.ToCultureInvariantString() + ", Dir: " + _value.direction.ToCultureInvariantString();
	}

	// Token: 0x0600A0DD RID: 41181 RVA: 0x003C8E6E File Offset: 0x003C706E
	public static Vector3 NormalizeReturnMagnitude(this Vector3 _value, out float magnitude)
	{
		magnitude = Vector3.Magnitude(_value);
		if ((double)magnitude > 9.999999747378752E-06)
		{
			_value /= magnitude;
		}
		else
		{
			_value = Vector3.zero;
		}
		return _value;
	}

	// Token: 0x0600A0DE RID: 41182 RVA: 0x003C8E9A File Offset: 0x003C709A
	public static Vector3 Abs(this Vector3 _value)
	{
		return new Vector3(Math.Abs(_value.x), Math.Abs(_value.y), Math.Abs(_value.z));
	}

	// Token: 0x0600A0DF RID: 41183 RVA: 0x003C8EC4 File Offset: 0x003C70C4
	public static Vector3 Max(this Vector3 _value, params Vector3[] _others)
	{
		Vector3 vector = _value;
		foreach (Vector3 rhs in _others)
		{
			vector = Vector3.Max(vector, rhs);
		}
		return vector;
	}

	// Token: 0x04007980 RID: 31104
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex StringSeparationRegex = new Regex("((?<=\\p{Ll})\\p{Lu}|\\p{Lu}(?=\\p{Ll}))", RegexOptions.IgnorePatternWhitespace);

	// Token: 0x04007981 RID: 31105
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly char[] HexLookup = new char[]
	{
		'0',
		'1',
		'2',
		'3',
		'4',
		'5',
		'6',
		'7',
		'8',
		'9',
		'a',
		'b',
		'c',
		'd',
		'e',
		'f'
	};

	// Token: 0x04007982 RID: 31106
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex unindentEmptyBeginning = new Regex("^\\s*\r?\n", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

	// Token: 0x04007983 RID: 31107
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex unindentEmptyEnd = new Regex("\r?\n\\s*$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant);

	// Token: 0x04007984 RID: 31108
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex unindentIndentationNoLinebreak = new Regex("\\s*\r?\n\\s*([^\\s|])", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);

	// Token: 0x04007985 RID: 31109
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex unindentIndentationRegularLinebreak = new Regex("^\\s*\\|", RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.CultureInvariant);
}
