using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Platform;
using UnityEngine.Scripting;

// Token: 0x020008E7 RID: 2279
[Preserve]
public class AllyStore
{
	// Token: 0x1400004B RID: 75
	// (add) Token: 0x06004219 RID: 16921 RVA: 0x0019D740 File Offset: 0x0019B940
	// (remove) Token: 0x0600421A RID: 16922 RVA: 0x0019D778 File Offset: 0x0019B978
	public event AllyStore.AllyChangeEvent OnAllyChangeEvent;

	// Token: 0x0600421B RID: 16923 RVA: 0x0019D7B0 File Offset: 0x0019B9B0
	public void AllyUpdateRequest(PlatformUserIdentifierAbs _target, bool _addAlly)
	{
		PlatformUserIdentifierAbs internalLocalUserIdentifier = PlatformManager.InternalLocalUserIdentifier;
		if (internalLocalUserIdentifier == null || _target == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageAllyRequest>().Setup(internalLocalUserIdentifier, _target, _addAlly), false);
			return;
		}
		this.ProcessAllyRequest(internalLocalUserIdentifier, _target, _addAlly);
	}

	// Token: 0x0600421C RID: 16924 RVA: 0x0019D7F8 File Offset: 0x0019B9F8
	public void ProcessAllyRequest(PlatformUserIdentifierAbs _source, PlatformUserIdentifierAbs _target, bool _addAlly)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return;
		}
		if (_source == null || _target == null)
		{
			return;
		}
		AllyStore.AllyStatus newStatus;
		AllyStore.AllyEvent allyEventSource;
		AllyStore.AllyEvent allyEventTarget;
		AllyStore.ComputeTransition(this.GetStatus(_source, _target), _addAlly, out newStatus, out allyEventSource, out allyEventTarget);
		SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageAllyResponse>().Setup(_source, _target, newStatus, allyEventSource, allyEventTarget), false, -1, -1, -1, null, 192, false);
		this.AllyUpdateResponse(_source, _target, newStatus, allyEventSource, allyEventTarget);
	}

	// Token: 0x0600421D RID: 16925 RVA: 0x0019D864 File Offset: 0x0019BA64
	public void ApplyTransition(PlatformUserIdentifierAbs _source, PlatformUserIdentifierAbs _target, bool _addAlly)
	{
		if (_source == null || _target == null)
		{
			return;
		}
		AllyStore.AllyStatus status;
		AllyStore.AllyEvent allyEvent;
		AllyStore.AllyEvent allyEvent2;
		AllyStore.ComputeTransition(this.GetStatus(_source, _target), _addAlly, out status, out allyEvent, out allyEvent2);
		this.SetStatus(_source, _target, status);
	}

	// Token: 0x0600421E RID: 16926 RVA: 0x0019D898 File Offset: 0x0019BA98
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ComputeTransition(AllyStore.AllyStatus _oldStatus, bool addAlly, out AllyStore.AllyStatus _newStatus, out AllyStore.AllyEvent _eventSource, out AllyStore.AllyEvent _eventTarget)
	{
		_newStatus = _oldStatus;
		_eventSource = AllyStore.AllyEvent.None;
		_eventTarget = AllyStore.AllyEvent.None;
		switch (_oldStatus)
		{
		case AllyStore.AllyStatus.NotAllied:
			if (addAlly)
			{
				_newStatus = AllyStore.AllyStatus.OutgoingInvite;
				_eventSource = AllyStore.AllyEvent.OutgoingSent;
				_eventTarget = AllyStore.AllyEvent.IncomingReceived;
				return;
			}
			break;
		case AllyStore.AllyStatus.Allies:
			if (!addAlly)
			{
				_newStatus = AllyStore.AllyStatus.NotAllied;
				_eventSource = AllyStore.AllyEvent.AllyRemoved;
				_eventTarget = AllyStore.AllyEvent.RemovedByAlly;
			}
			break;
		case AllyStore.AllyStatus.OutgoingInvite:
			if (!addAlly)
			{
				_newStatus = AllyStore.AllyStatus.NotAllied;
				_eventSource = AllyStore.AllyEvent.OutgoingCanceled;
				_eventTarget = AllyStore.AllyEvent.IncomingCanceled;
				return;
			}
			break;
		case AllyStore.AllyStatus.IncomingInvite:
			if (addAlly)
			{
				_newStatus = AllyStore.AllyStatus.Allies;
				_eventSource = AllyStore.AllyEvent.IncomingAccepted;
				_eventTarget = AllyStore.AllyEvent.OutgoingAccepted;
			}
			if (!addAlly)
			{
				_newStatus = AllyStore.AllyStatus.NotAllied;
				_eventSource = AllyStore.AllyEvent.IncomingDeclined;
				_eventTarget = AllyStore.AllyEvent.OutgoingDeclined;
				return;
			}
			break;
		default:
			return;
		}
	}

	// Token: 0x0600421F RID: 16927 RVA: 0x0019D90C File Offset: 0x0019BB0C
	public void AllyUpdateResponse(PlatformUserIdentifierAbs _source, PlatformUserIdentifierAbs _target, AllyStore.AllyStatus _newStatus, AllyStore.AllyEvent _allyEventSource, AllyStore.AllyEvent _allyEventTarget)
	{
		this.SetStatus(_source, _target, _newStatus);
		AllyStore.AllyChangeEvent onAllyChangeEvent = this.OnAllyChangeEvent;
		if (onAllyChangeEvent != null)
		{
			onAllyChangeEvent(_source, _target, _allyEventSource);
		}
		AllyStore.AllyChangeEvent onAllyChangeEvent2 = this.OnAllyChangeEvent;
		if (onAllyChangeEvent2 == null)
		{
			return;
		}
		onAllyChangeEvent2(_target, _source, _allyEventTarget);
	}

	// Token: 0x06004220 RID: 16928 RVA: 0x0019D940 File Offset: 0x0019BB40
	public bool IsAlly(PlatformUserIdentifierAbs _source, PlatformUserIdentifierAbs _target)
	{
		return this.GetStatus(_source, _target) == AllyStore.AllyStatus.Allies;
	}

	// Token: 0x06004221 RID: 16929 RVA: 0x0019D94D File Offset: 0x0019BB4D
	public IEnumerable<PlatformUserIdentifierAbs> EnumerateAllies(PlatformUserIdentifierAbs _id)
	{
		Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus> dictionary;
		if (_id == null || !this.relationships.TryGetValue(_id, out dictionary))
		{
			yield break;
		}
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AllyStore.AllyStatus> keyValuePair in dictionary)
		{
			if (keyValuePair.Value == AllyStore.AllyStatus.Allies)
			{
				yield return keyValuePair.Key;
			}
		}
		Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>.Enumerator enumerator = default(Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>.Enumerator);
		yield break;
		yield break;
	}

	// Token: 0x06004222 RID: 16930 RVA: 0x0019D964 File Offset: 0x0019BB64
	public bool HasAllies(PlatformUserIdentifierAbs _id)
	{
		Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus> dictionary;
		if (_id == null || !this.relationships.TryGetValue(_id, out dictionary))
		{
			return false;
		}
		foreach (KeyValuePair<PlatformUserIdentifierAbs, AllyStore.AllyStatus> keyValuePair in dictionary)
		{
			if (keyValuePair.Value == AllyStore.AllyStatus.Allies)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06004223 RID: 16931 RVA: 0x0019D9D4 File Offset: 0x0019BBD4
	public AllyStore.AllyStatus GetStatus(PlatformUserIdentifierAbs _source, PlatformUserIdentifierAbs _target)
	{
		if (_source == null || _target == null)
		{
			return AllyStore.AllyStatus.NotAllied;
		}
		Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus> dictionary;
		if (!this.relationships.TryGetValue(_source, out dictionary))
		{
			return AllyStore.AllyStatus.NotAllied;
		}
		AllyStore.AllyStatus result;
		if (!dictionary.TryGetValue(_target, out result))
		{
			return AllyStore.AllyStatus.NotAllied;
		}
		return result;
	}

	// Token: 0x06004224 RID: 16932 RVA: 0x0019DA0C File Offset: 0x0019BC0C
	public void SetStatus(PlatformUserIdentifierAbs _source, PlatformUserIdentifierAbs _target, AllyStore.AllyStatus _status)
	{
		if (_source == null || _target == null)
		{
			return;
		}
		if (_status == AllyStore.AllyStatus.NotAllied)
		{
			this.ClearStatus(_source, _target);
			return;
		}
		if (!this.relationships.ContainsKey(_source))
		{
			this.relationships.Add(_source, new Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>());
		}
		if (!this.relationships.ContainsKey(_target))
		{
			this.relationships.Add(_target, new Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>());
		}
		if (_status == AllyStore.AllyStatus.Allies)
		{
			this.relationships[_source][_target] = AllyStore.AllyStatus.Allies;
			this.relationships[_target][_source] = AllyStore.AllyStatus.Allies;
			return;
		}
		if (_status == AllyStore.AllyStatus.OutgoingInvite)
		{
			this.relationships[_source][_target] = AllyStore.AllyStatus.OutgoingInvite;
			this.relationships[_target][_source] = AllyStore.AllyStatus.IncomingInvite;
			return;
		}
		if (_status == AllyStore.AllyStatus.IncomingInvite)
		{
			this.relationships[_source][_target] = AllyStore.AllyStatus.IncomingInvite;
			this.relationships[_target][_source] = AllyStore.AllyStatus.OutgoingInvite;
		}
	}

	// Token: 0x06004225 RID: 16933 RVA: 0x0019DAEC File Offset: 0x0019BCEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearStatus(PlatformUserIdentifierAbs _source, PlatformUserIdentifierAbs _target)
	{
		Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus> dictionary;
		if (this.relationships.TryGetValue(_source, out dictionary))
		{
			dictionary.Remove(_target);
		}
		Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus> dictionary2;
		if (this.relationships.TryGetValue(_target, out dictionary2))
		{
			dictionary2.Remove(_source);
		}
	}

	// Token: 0x06004226 RID: 16934 RVA: 0x0019DB29 File Offset: 0x0019BD29
	public void ClearAll()
	{
		this.relationships.Clear();
	}

	// Token: 0x06004227 RID: 16935 RVA: 0x0019DB38 File Offset: 0x0019BD38
	public void CopyFrom(AllyStore _other)
	{
		this.relationships.Clear();
		if (_other == null)
		{
			return;
		}
		foreach (KeyValuePair<PlatformUserIdentifierAbs, Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>> keyValuePair in _other.relationships)
		{
			foreach (KeyValuePair<PlatformUserIdentifierAbs, AllyStore.AllyStatus> keyValuePair2 in keyValuePair.Value)
			{
				this.SetStatus(keyValuePair.Key, keyValuePair2.Key, keyValuePair2.Value);
			}
		}
	}

	// Token: 0x06004228 RID: 16936 RVA: 0x0019DBEC File Offset: 0x0019BDEC
	public void ReadXml(XmlElement _alliesElement, int _readVersion)
	{
		if (_readVersion == 0)
		{
			return;
		}
		foreach (object obj in _alliesElement.ChildNodes)
		{
			XmlElement xmlElement = ((XmlNode)obj) as XmlElement;
			if (xmlElement != null && xmlElement.Name == "ally")
			{
				PlatformUserIdentifierAbs platformUserIdentifierAbs = PlatformUserIdentifierAbs.FromXml(xmlElement, true, "a");
				PlatformUserIdentifierAbs platformUserIdentifierAbs2 = PlatformUserIdentifierAbs.FromXml(xmlElement, true, "b");
				if (platformUserIdentifierAbs != null && platformUserIdentifierAbs2 != null)
				{
					if (xmlElement.GetAttribute("status") == "allies")
					{
						this.SetStatus(platformUserIdentifierAbs, platformUserIdentifierAbs2, AllyStore.AllyStatus.Allies);
					}
					else if (xmlElement.GetAttribute("status") == "pending")
					{
						this.SetStatus(platformUserIdentifierAbs, platformUserIdentifierAbs2, AllyStore.AllyStatus.OutgoingInvite);
					}
				}
			}
		}
	}

	// Token: 0x06004229 RID: 16937 RVA: 0x0019DCC8 File Offset: 0x0019BEC8
	public void WriteXml(XmlElement _root)
	{
		bool flag = false;
		foreach (KeyValuePair<PlatformUserIdentifierAbs, Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>> keyValuePair in this.relationships)
		{
			if (keyValuePair.Value.Count > 0)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		XmlElement node = _root.AddXmlElement("allies");
		foreach (KeyValuePair<PlatformUserIdentifierAbs, Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>> keyValuePair2 in this.relationships)
		{
			foreach (KeyValuePair<PlatformUserIdentifierAbs, AllyStore.AllyStatus> keyValuePair3 in keyValuePair2.Value)
			{
				PlatformUserIdentifierAbs key = keyValuePair2.Key;
				PlatformUserIdentifierAbs key2 = keyValuePair3.Key;
				AllyStore.AllyStatus value = keyValuePair3.Value;
				if (value != AllyStore.AllyStatus.NotAllied && value != AllyStore.AllyStatus.IncomingInvite && (value != AllyStore.AllyStatus.Allies || string.CompareOrdinal(key.CombinedString, key2.CombinedString) <= 0))
				{
					XmlElement xmlElement = node.AddXmlElement("ally");
					key.ToXml(xmlElement, "a");
					key2.ToXml(xmlElement, "b");
					if (value == AllyStore.AllyStatus.Allies)
					{
						xmlElement.SetAttribute("status", "allies");
					}
					else
					{
						xmlElement.SetAttribute("status", "pending");
					}
				}
			}
		}
	}

	// Token: 0x0600422A RID: 16938 RVA: 0x0019DE50 File Offset: 0x0019C050
	public void Read(BinaryReader _br)
	{
		this.relationships.Clear();
		int num = _br.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			PlatformUserIdentifierAbs source = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			PlatformUserIdentifierAbs target = PlatformUserIdentifierAbs.FromStream(_br, false, false);
			AllyStore.AllyStatus status = (AllyStore.AllyStatus)_br.ReadByte();
			this.SetStatus(source, target, status);
		}
	}

	// Token: 0x0600422B RID: 16939 RVA: 0x0019DEA0 File Offset: 0x0019C0A0
	public void Write(BinaryWriter _bw)
	{
		int num = 0;
		foreach (KeyValuePair<PlatformUserIdentifierAbs, Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>> keyValuePair in this.relationships)
		{
			foreach (KeyValuePair<PlatformUserIdentifierAbs, AllyStore.AllyStatus> keyValuePair2 in keyValuePair.Value)
			{
				num++;
			}
		}
		num /= 2;
		_bw.Write(num);
		foreach (KeyValuePair<PlatformUserIdentifierAbs, Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>> keyValuePair3 in this.relationships)
		{
			foreach (KeyValuePair<PlatformUserIdentifierAbs, AllyStore.AllyStatus> keyValuePair4 in keyValuePair3.Value)
			{
				if (string.CompareOrdinal(keyValuePair3.Key.CombinedString, keyValuePair4.Key.CombinedString) <= 0)
				{
					keyValuePair3.Key.ToStream(_bw, false);
					keyValuePair4.Key.ToStream(_bw, false);
					_bw.Write((byte)keyValuePair4.Value);
				}
			}
		}
	}

	// Token: 0x04003594 RID: 13716
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<PlatformUserIdentifierAbs, Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>> relationships = new Dictionary<PlatformUserIdentifierAbs, Dictionary<PlatformUserIdentifierAbs, AllyStore.AllyStatus>>();

	// Token: 0x020008E8 RID: 2280
	public enum AllyStatus : byte
	{
		// Token: 0x04003596 RID: 13718
		NotAllied,
		// Token: 0x04003597 RID: 13719
		Allies,
		// Token: 0x04003598 RID: 13720
		OutgoingInvite,
		// Token: 0x04003599 RID: 13721
		IncomingInvite
	}

	// Token: 0x020008E9 RID: 2281
	public enum AllyEvent : byte
	{
		// Token: 0x0400359B RID: 13723
		None,
		// Token: 0x0400359C RID: 13724
		OutgoingSent,
		// Token: 0x0400359D RID: 13725
		OutgoingCanceled,
		// Token: 0x0400359E RID: 13726
		IncomingAccepted,
		// Token: 0x0400359F RID: 13727
		IncomingDeclined,
		// Token: 0x040035A0 RID: 13728
		AllyRemoved,
		// Token: 0x040035A1 RID: 13729
		OutgoingAccepted,
		// Token: 0x040035A2 RID: 13730
		OutgoingDeclined,
		// Token: 0x040035A3 RID: 13731
		IncomingReceived,
		// Token: 0x040035A4 RID: 13732
		IncomingCanceled,
		// Token: 0x040035A5 RID: 13733
		RemovedByAlly
	}

	// Token: 0x020008EA RID: 2282
	// (Invoke) Token: 0x0600422E RID: 16942
	public delegate void AllyChangeEvent(PlatformUserIdentifierAbs _source, PlatformUserIdentifierAbs _target, AllyStore.AllyEvent _allyEventSource);
}
