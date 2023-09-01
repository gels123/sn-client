
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg.item
{
public sealed partial class Item : Luban.BeanBase
{
    public Item(JSONNode _buf) 
    {
        { if(!_buf["id"].IsNumber) { throw new SerializationException(); }  Id = _buf["id"]; }
        { if(!_buf["name"].IsString) { throw new SerializationException(); }  Name = _buf["name"]; }
        { if(!_buf["desc"].IsString) { throw new SerializationException(); }  Desc = _buf["desc"]; }
        { if(!_buf["price"].IsNumber) { throw new SerializationException(); }  Price = _buf["price"]; }
        { if(!_buf["upgrade_to_item_id"].IsNumber) { throw new SerializationException(); }  UpgradeToItemId = _buf["upgrade_to_item_id"]; }
        UpgradeToItemId_Ref = null;
        { var _j = _buf["expire_time"]; if (_j.Tag != JSONNodeType.None && _j.Tag != JSONNodeType.NullValue) { { if(!_j.IsNumber) { throw new SerializationException(); }  ExpireTime = _j; } } else { ExpireTime = null; } }
        { if(!_buf["batch_useable"].IsBoolean) { throw new SerializationException(); }  BatchUseable = _buf["batch_useable"]; }
        { if(!_buf["quality"].IsNumber) { throw new SerializationException(); }  Quality = (item.EQuality)_buf["quality"].AsInt; }
        { if(!_buf["exchange1"].IsObject) { throw new SerializationException(); }  Exchange1 = item.ItemCell.DeserializeItemCell(_buf["exchange1"]);  }
        { var __json0 = _buf["exchange2"]; if(!__json0.IsArray) { throw new SerializationException(); } Exchange2 = new System.Collections.Generic.List<item.ItemCell>(__json0.Count); foreach(JSONNode __e0 in __json0.Children) { item.ItemCell __v0;  { if(!__e0.IsObject) { throw new SerializationException(); }  __v0 = item.ItemCell.DeserializeItemCell(__e0);  }  Exchange2.Add(__v0); }   }
        { if(!_buf["exchange3"].IsObject) { throw new SerializationException(); }  Exchange3 = item.ItemCell.DeserializeItemCell(_buf["exchange3"]);  }
    }

    public static Item DeserializeItem(JSONNode _buf)
    {
        return new item.Item(_buf);
    }

    /// <summary>
    /// 这是id
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 名字
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// 描述
    /// </summary>
    public readonly string Desc;
    /// <summary>
    /// 价格
    /// </summary>
    public readonly int Price;
    /// <summary>
    /// 引用当前表
    /// </summary>
    public readonly int UpgradeToItemId;
    public item.Item UpgradeToItemId_Ref;
    /// <summary>
    /// 过期时间
    /// </summary>
    public readonly long? ExpireTime;
    /// <summary>
    /// 能否批量使用
    /// </summary>
    public readonly bool BatchUseable;
    /// <summary>
    /// 品质
    /// </summary>
    public readonly item.EQuality Quality;
    /// <summary>
    /// 道具兑换配置
    /// </summary>
    public readonly item.ItemCell Exchange1;
    public readonly System.Collections.Generic.List<item.ItemCell> Exchange2;
    /// <summary>
    /// 道具兑换配置
    /// </summary>
    public readonly item.ItemCell Exchange3;
   
    public const int __ID__ = 2107285806;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
        
        UpgradeToItemId_Ref = tables.TbItem.GetOrDefault(UpgradeToItemId);
        
        
        
        Exchange1?.ResolveRef(tables);
        foreach (var _e in Exchange2) { _e?.ResolveRef(tables); }
        Exchange3?.ResolveRef(tables);
    }

    public override string ToString()
    {
        return "{ "
        + "id:" + Id + ","
        + "name:" + Name + ","
        + "desc:" + Desc + ","
        + "price:" + Price + ","
        + "upgradeToItemId:" + UpgradeToItemId + ","
        + "expireTime:" + ExpireTime + ","
        + "batchUseable:" + BatchUseable + ","
        + "quality:" + Quality + ","
        + "exchange1:" + Exchange1 + ","
        + "exchange2:" + Luban.StringUtil.CollectionToString(Exchange2) + ","
        + "exchange3:" + Exchange3 + ","
        + "}";
    }
}

}