using System;
using System.Text;

/// <summary>
/// 自定义序列化和反序列化的Data，需要继承此类。
/// 例如，网络通信中的数据传输不能通过BinaryFormatter序列化，因为不能被其他语言反序列化。
/// </summary>
public abstract class HDataBase
{
    /// <summary>
    /// 获取序列化后的总字节数
    /// </summary>
    /// <returns></returns>
    public abstract int GetBytesCount();
    
    /// <summary>
    /// 序列化
    /// </summary>
    /// <returns></returns>
    public abstract byte[] Serialize();

    /// <summary>
    /// 反序列化
    /// </summary>
    public abstract int Deserialize(byte[] bytes, int startIndex = 0);

    #region 序列化成员重载

    protected void WriteInfo<T>(byte[] bytes, T value, ref int index) where T : struct
    {
        switch (value)
        {
            case bool b:
                BitConverter.GetBytes(b).CopyTo(bytes, index);
                index += sizeof(bool);
                break;
            case char c:
                BitConverter.GetBytes(c).CopyTo(bytes, index);
                index += sizeof(char);
                break;
            case double d:
                BitConverter.GetBytes(d).CopyTo(bytes, index);
                index += sizeof(double);
                break;
            case float f:
                BitConverter.GetBytes(f).CopyTo(bytes, index);
                index += sizeof(float);
                break;
            case int i:
                BitConverter.GetBytes(i).CopyTo(bytes, index);
                index += sizeof(int);
                break;
            case long l:
                BitConverter.GetBytes(l).CopyTo(bytes, index);
                index += sizeof(long);
                break;
            default:
                throw new Exception("Need to expand.");
        }
    }

    protected void WriteInfo(byte[] bytes, string value, ref int index)
    {
        byte[] data = Encoding.UTF8.GetBytes(value);
        WriteInfo(bytes, data.Length, ref index);
        data.CopyTo(bytes, index);
        index += data.Length;
    }
    
    protected void WriteInfo(byte[] bytes, HDataBase value, ref int index)
    {
        byte[] data = value.Serialize();
        WriteInfo(bytes, data.Length, ref index);
        data.CopyTo(bytes, index);
        index += data.Length;
    }

    #endregion

    #region 反序列化成员重载
    
    protected bool ReadBool(byte[] bytes, ref int index)
    {
        var val = BitConverter.ToBoolean(bytes, index);
        index += sizeof(bool);
        return val;
    }
    
    protected char ReadChar(byte[] bytes, ref int index)
    {
        var val = BitConverter.ToChar(bytes, index);
        index += sizeof(char);
        return val;
    }
    
    protected double ReadDouble(byte[] bytes, ref int index)
    {
        var val = BitConverter.ToDouble(bytes, index);
        index += sizeof(double);
        return val;
    }
    
    protected float ReadFloat(byte[] bytes, ref int index)
    {
        var val = BitConverter.ToSingle(bytes, index);
        index += sizeof(float);
        return val;
    }

    protected int ReadInt(byte[] bytes, ref int index)
    {
        var val = BitConverter.ToInt32(bytes, index);
        index += sizeof(int);
        return val;
    }
    
    protected long ReadLong(byte[] bytes, ref int index)
    {
        var val = BitConverter.ToInt64(bytes, index);
        index += sizeof(long);
        return val;
    }
    
    protected string ReadString(byte[] bytes, ref int index)
    {
        int len = ReadInt(bytes, ref index);
        var val = Encoding.UTF8.GetString(bytes, index, len);
        return val;
    }
    
    protected T ReadDataBase<T>(byte[] bytes, ref int index) where T : HDataBase, new()
    {
        T val = new T();
        index += val.Deserialize(bytes, index);
        return val;
    }

    #endregion
}