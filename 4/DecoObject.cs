using System;
using System.IO;
using UnityEngine;

// Token: 0x02000C1A RID: 3098
public class DecoObject : IEquatable<DecoObject>
{
	// Token: 0x06005E96 RID: 24214 RVA: 0x0024EDAA File Offset: 0x0024CFAA
	public void Init(Vector3i _pos, float _realYPos, BlockValue _bv, DecoState _state)
	{
		this.pos = _pos;
		this.realYPos = _realYPos;
		this.bv = _bv;
		this.state = _state;
		this.asyncItem = null;
		this.go = null;
	}

	// Token: 0x06005E97 RID: 24215 RVA: 0x0024EDD8 File Offset: 0x0024CFD8
	public string GetModelName()
	{
		Block block = this.bv.Block;
		if (block == null)
		{
			Log.Error(string.Format("DecoObject '{0}', no block!", this.bv));
			return null;
		}
		string value = block.Properties.GetValue("Model");
		if (string.IsNullOrEmpty(value))
		{
			Log.Error("DecoObject block '" + block.GetBlockName() + "', no model!");
			return null;
		}
		return GameIO.GetFilenameFromPathWithoutExtension(value);
	}

	// Token: 0x06005E98 RID: 24216 RVA: 0x0024EE4C File Offset: 0x0024D04C
	public void CreateGameObject(DecoChunk _decoChunk, Transform _parent)
	{
		string modelName = this.GetModelName();
		if (modelName != null)
		{
			GameObject objectForType = GameObjectPool.Instance.GetObjectForType(modelName);
			this.CreateGameObjectCallback(objectForType, _parent, false);
		}
	}

	// Token: 0x06005E99 RID: 24217 RVA: 0x0024EE78 File Offset: 0x0024D078
	public void CreateGameObjectCallback(GameObject _obj, Transform _parent, bool _isAsync)
	{
		this.go = _obj;
		if (_isAsync && this.asyncItem == null)
		{
			this.Destroy();
			return;
		}
		this.asyncItem = null;
		Block block = this.bv.Block;
		BlockShapeDistantDeco blockShapeDistantDeco = block.shape as BlockShapeDistantDeco;
		if (blockShapeDistantDeco == null)
		{
			Log.Error("Block '{0}' needs a deco shape assigned but has not!", new object[]
			{
				block.GetBlockName()
			});
			return;
		}
		Transform transform = this.go.transform;
		transform.SetParent(_parent, false);
		float y = blockShapeDistantDeco.modelOffset.y;
		transform.position = new Vector3((float)this.pos.x + DecoManager.cDecoMiddleOffset.x, this.realYPos + y, (float)this.pos.z + DecoManager.cDecoMiddleOffset.z) - Origin.position;
		int num = (int)this.bv.rotation;
		if (!blockShapeDistantDeco.Has45DegreeRotations)
		{
			num &= 3;
		}
		transform.localRotation = BlockShapeNew.GetRotationStatic(num);
		this.go.SetActive(true);
		BlockEntityData blockEntityData = new BlockEntityData();
		blockEntityData.transform = transform;
		blockShapeDistantDeco.OnBlockEntityTransformAfterActivated(null, this.pos, this.bv, blockEntityData);
	}

	// Token: 0x06005E9A RID: 24218 RVA: 0x0024EF9D File Offset: 0x0024D19D
	public void Destroy()
	{
		this.asyncItem = null;
		if (this.go)
		{
			GameObjectPool.Instance.PoolObjectAsync(this.go);
			this.go = null;
		}
	}

	// Token: 0x06005E9B RID: 24219 RVA: 0x0024EFCC File Offset: 0x0024D1CC
	public void Write(BinaryWriter _bw, NameIdMapping _blockMap = null)
	{
		_bw.Write(GameUtils.Vector3iToUInt64(this.pos));
		_bw.Write(this.realYPos);
		_bw.Write(this.bv.rawData);
		_bw.Write((byte)this.state);
		Block block = this.bv.Block;
		if (block == null)
		{
			Log.Error(string.Format("Writing DecoObject '{0}', no block!", this.bv));
			return;
		}
		if (_blockMap != null)
		{
			_blockMap.AddMapping(block.blockID, block.GetBlockName(), false);
		}
	}

	// Token: 0x06005E9C RID: 24220 RVA: 0x0024F055 File Offset: 0x0024D255
	public void Read(BinaryReader _br)
	{
		this.pos = GameUtils.UInt64ToVector3i(_br.ReadUInt64());
		this.realYPos = _br.ReadSingle();
		this.bv = new BlockValue(_br.ReadUInt32());
		this.state = (DecoState)_br.ReadByte();
	}

	// Token: 0x06005E9D RID: 24221 RVA: 0x0024F091 File Offset: 0x0024D291
	public override int GetHashCode()
	{
		return HashCode.Combine<int, DecoState, int>(this.pos.GetHashCode(), this.state, this.bv.GetHashCode());
	}

	// Token: 0x06005E9E RID: 24222 RVA: 0x0024F0C0 File Offset: 0x0024D2C0
	public override bool Equals(object _obj)
	{
		if (_obj == null)
		{
			return false;
		}
		if (this == _obj)
		{
			return true;
		}
		DecoObject decoObject = _obj as DecoObject;
		return decoObject != null && this.Equals(decoObject);
	}

	// Token: 0x06005E9F RID: 24223 RVA: 0x0024F0EC File Offset: 0x0024D2EC
	public bool Equals(DecoObject _other)
	{
		return _other != null && (this == _other || (_other.pos.Equals(this.pos) && _other.state == this.state && _other.bv.Equals(this.bv)));
	}

	// Token: 0x04004987 RID: 18823
	public Vector3i pos;

	// Token: 0x04004988 RID: 18824
	public float realYPos;

	// Token: 0x04004989 RID: 18825
	public BlockValue bv;

	// Token: 0x0400498A RID: 18826
	public DecoState state;

	// Token: 0x0400498B RID: 18827
	public GameObjectPool.AsyncItem asyncItem;

	// Token: 0x0400498C RID: 18828
	public GameObject go;
}
