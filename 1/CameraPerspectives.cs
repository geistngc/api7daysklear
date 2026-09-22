using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// Token: 0x0200053B RID: 1339
public class CameraPerspectives
{
	// Token: 0x06002C1A RID: 11290 RVA: 0x00116E88 File Offset: 0x00115088
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetFullFilePath()
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			return GameIO.GetSaveGameDir() + "/camerapositions.xml";
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
		{
			return GameIO.GetSaveGameLocalDir() + "/camerapositions.xml";
		}
		return null;
	}

	// Token: 0x06002C1B RID: 11291 RVA: 0x00116EC3 File Offset: 0x001150C3
	public CameraPerspectives(bool _load = true)
	{
		if (_load)
		{
			this.Load();
		}
	}

	// Token: 0x06002C1C RID: 11292 RVA: 0x00116EE8 File Offset: 0x001150E8
	public bool Load()
	{
		this.Perspectives.Clear();
		string fullFilePath = CameraPerspectives.GetFullFilePath();
		if (!SdFile.Exists(fullFilePath))
		{
			return false;
		}
		XmlDocument xmlDocument = new XmlDocument();
		try
		{
			xmlDocument.SdLoad(fullFilePath);
		}
		catch (XmlException ex)
		{
			Log.Error("Failed loading camera file: " + ex.Message);
			return false;
		}
		if (xmlDocument.DocumentElement == null)
		{
			Log.Warning("Camera file has no root XML element.");
			return false;
		}
		foreach (object obj in xmlDocument.DocumentElement.ChildNodes)
		{
			XmlNode xmlNode = (XmlNode)obj;
			if (xmlNode.NodeType == XmlNodeType.Element && !(xmlNode.Name != "position"))
			{
				CameraPerspectives.Perspective perspective = CameraPerspectives.Perspective.FromXml((XmlElement)xmlNode);
				if (perspective != null && !this.Perspectives.TryAdd(perspective.Name, perspective))
				{
					Log.Warning("Duplicate camera perspective entry '" + perspective.Name + "'");
				}
			}
		}
		return true;
	}

	// Token: 0x06002C1D RID: 11293 RVA: 0x00117014 File Offset: 0x00115214
	public void Save()
	{
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.CreateXmlDeclaration();
		XmlElement parent = xmlDocument.AddXmlElement("camerapositions");
		foreach (KeyValuePair<string, CameraPerspectives.Perspective> keyValuePair in this.Perspectives)
		{
			keyValuePair.Value.ToXml(parent);
		}
		xmlDocument.SdSave(CameraPerspectives.GetFullFilePath());
	}

	// Token: 0x040021BE RID: 8638
	public readonly SortedDictionary<string, CameraPerspectives.Perspective> Perspectives = new SortedDictionary<string, CameraPerspectives.Perspective>(StringComparer.OrdinalIgnoreCase);

	// Token: 0x0200053C RID: 1340
	public class Perspective : IEquatable<CameraPerspectives.Perspective>
	{
		// Token: 0x06002C1E RID: 11294 RVA: 0x00117090 File Offset: 0x00115290
		public Perspective(string _name, Vector3 _position, Vector3 _direction, string _comment)
		{
			this.Name = _name;
			this.Position = _position;
			this.Direction = _direction;
			this.Comment = _comment;
		}

		// Token: 0x06002C1F RID: 11295 RVA: 0x001170B8 File Offset: 0x001152B8
		public Perspective(string _name, EntityPlayerLocal _player, string _comment = null)
		{
			this.Name = _name;
			if (_player.movementInput.bDetachedCameraMove)
			{
				this.Position = _player.cameraTransform.position - Constants.cDefaultCameraPlayerOffset;
				this.Direction = _player.cameraTransform.localEulerAngles;
				this.Direction.x = -this.Direction.x;
			}
			else
			{
				this.Position = _player.GetPosition();
				this.Direction = _player.rotation;
			}
			this.Comment = _comment;
		}

		// Token: 0x06002C20 RID: 11296 RVA: 0x00117144 File Offset: 0x00115344
		public void ToPlayer(EntityPlayerLocal _player)
		{
			if (_player.movementInput.bDetachedCameraMove)
			{
				_player.cameraTransform.position = this.Position + Constants.cDefaultCameraPlayerOffset;
				Vector3 direction = this.Direction;
				direction.x = -direction.x;
				_player.cameraTransform.localEulerAngles = direction;
				return;
			}
			_player.TeleportToPosition(this.Position, false, new Vector3?(this.Direction));
		}

		// Token: 0x06002C21 RID: 11297 RVA: 0x001171B4 File Offset: 0x001153B4
		public bool Equals(CameraPerspectives.Perspective _other)
		{
			return _other != null && (this == _other || (this.Position.Equals(_other.Position) && this.Direction.Equals(_other.Direction)));
		}

		// Token: 0x06002C22 RID: 11298 RVA: 0x001171F8 File Offset: 0x001153F8
		public override bool Equals(object _obj)
		{
			return _obj != null && (this == _obj || (!(_obj.GetType() != base.GetType()) && this.Equals((CameraPerspectives.Perspective)_obj)));
		}

		// Token: 0x06002C23 RID: 11299 RVA: 0x00117228 File Offset: 0x00115428
		public override int GetHashCode()
		{
			return this.Position.GetHashCode() * 397 ^ this.Direction.GetHashCode();
		}

		// Token: 0x06002C24 RID: 11300 RVA: 0x00117264 File Offset: 0x00115464
		public void ToXml(XmlElement _parent)
		{
			XmlElement element = _parent.AddXmlElement("position").SetAttrib("name", this.Name).SetAttrib("position", this.Position.ToString()).SetAttrib("direction", this.Direction.ToString());
			if (!string.IsNullOrEmpty(this.Comment))
			{
				element.SetAttrib("comment", this.Comment);
			}
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x001172E8 File Offset: 0x001154E8
		public static CameraPerspectives.Perspective FromXml(XmlElement _lineItem)
		{
			if (!_lineItem.HasAttribute("name"))
			{
				Log.Warning("Ignoring camera-entry because of missing 'name' attribute: " + _lineItem.OuterXml);
				return null;
			}
			if (!_lineItem.HasAttribute("position"))
			{
				Log.Warning("Ignoring camera-entry because of missing 'position' attribute: " + _lineItem.OuterXml);
				return null;
			}
			if (!_lineItem.HasAttribute("direction"))
			{
				Log.Warning("Ignoring camera-entry because of missing 'direction' attribute: " + _lineItem.OuterXml);
				return null;
			}
			string attribute = _lineItem.GetAttribute("name");
			Vector3 position = StringParsers.ParseVector3(_lineItem.GetAttribute("position"), 0, -1);
			Vector3 direction = StringParsers.ParseVector3(_lineItem.GetAttribute("direction"), 0, -1);
			string comment = null;
			if (_lineItem.HasAttribute("comment"))
			{
				comment = _lineItem.GetAttribute("comment");
			}
			return new CameraPerspectives.Perspective(attribute, position, direction, comment);
		}

		// Token: 0x040021BF RID: 8639
		public readonly string Name;

		// Token: 0x040021C0 RID: 8640
		public readonly Vector3 Position;

		// Token: 0x040021C1 RID: 8641
		public readonly Vector3 Direction;

		// Token: 0x040021C2 RID: 8642
		public readonly string Comment;
	}
}
