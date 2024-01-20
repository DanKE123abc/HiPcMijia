namespace HiPcMijia;

public class Singleton<T> where T : new()
{
    private static T _instance = default!;

    /// <summary>
    /// 单例模式需要实例化
    /// </summary>
    public static T ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }

            return _instance;
        }
    }

}

