using GameFramework;

/// <summary>
/// ��¼�������
/// </summary>
public class ViewRefData : IReference
{
    public string imagelocation;

    public void Clear()
    {
        
    }
    //...

    public static ViewRefData Create()
    {
        ViewRefData data = ReferencePool.Acquire<ViewRefData>();

        return data;
    }
}
