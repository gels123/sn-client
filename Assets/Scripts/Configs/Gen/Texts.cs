
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;
using SimpleJSON;


namespace cfg
{
public sealed partial class Texts : Luban.BeanBase
{
    public Texts(JSONNode _buf) 
    {
        { if(!_buf["key"].IsString) { throw new SerializationException(); }  Key = _buf["key"]; }
        { if(!_buf["zh"].IsString) { throw new SerializationException(); }  Zh = _buf["zh"]; }
        { if(!_buf["en"].IsString) { throw new SerializationException(); }  En = _buf["en"]; }
    }

    public static Texts DeserializeTexts(JSONNode _buf)
    {
        return new Texts(_buf);
    }

    /// <summary>
    /// 键值
    /// </summary>
    public readonly string Key;
    /// <summary>
    /// 中文
    /// </summary>
    public readonly string Zh;
    /// <summary>
    /// 英文
    /// </summary>
    public readonly string En;
   
    public const int __ID__ = 80703686;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "key:" + Key + ","
        + "zh:" + Zh + ","
        + "en:" + En + ","
        + "}";
    }
}

}
