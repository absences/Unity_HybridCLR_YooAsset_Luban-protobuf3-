using Newtonsoft.Json;

/// <summary>
/// Json 工具 ， 对 Newtonsoft.Json 的封装
/// </summary>
public class JsonTool
{
    /// <summary>
    /// 对象 转为Json字符串
    /// </summary>
    /// <param name="obj">需要转化的对象</param>
    /// <param name="prettyPrint">是否格式化 ，占用空间更大</param>
    /// <returns></returns>
    public static string ToJson(object obj, bool prettyPrint = false)
    {
        return JsonConvert.SerializeObject(obj, prettyPrint ? Formatting.Indented : Formatting.None);
    }

    /// <summary>
    /// Json字符串 转为对象
    /// </summary>
    /// <typeparam name="T">需要转化为的类型</typeparam>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public static T FromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}
