namespace HiPcMijia;

public interface IEventInfo
{
}

public class EventInfo<T> : IEventInfo
{

    public Action<T> Actions;

    public EventInfo(Action<T> action)
    {
        Actions += action;
    }
}

public class EventInfo : IEventInfo
{
    public Action Actions;

    public EventInfo(Action action)
    {
        Actions += action;
    }
}

public class EventCenter : Singleton<EventCenter>
{
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    ///添加事件监听 
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">准备用来处理事件的委托函数</param>
    public void AddEventListener(string name, Action action)
    {
        //判断字典里有没有对应这个事件，有就执行，没有就加进去。
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).Actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo(action));
        }
    }

    public void AddEventListener<T>(string name, Action<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).Actions += action;
        }
        else
        {
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    /// <summary>
    /// 移除对应的实践监听
    /// </summary>
    /// <param name="name">事件的名字</param>
    /// <param name="action">对应之前添加的委托函数</param>
    public void RemoveEventListener(string name, Action action)
    {
        if (eventDic.ContainsKey(name))
        {
            //移除这个委托
            (eventDic[name] as EventInfo).Actions -= action;
        }
    }

    public void RemoveEventListener<T>(string name, Action<T> action)
    {
        if (eventDic.ContainsKey(name))
        {
            //移除这个委托
            (eventDic[name] as EventInfo<T>).Actions -= action;
        }
    }

    /// <summary>
    /// 事件触发
    /// </summary>
    /// <param name="name">哪一个名字的事件触发了</param>
    public void EventTrigger(string name)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo).Actions?.Invoke();
        }

    }

    public void EventTrigger<T>(string name, T info)
    {
        if (eventDic.ContainsKey(name))
        {
            (eventDic[name] as EventInfo<T>).Actions?.Invoke(info);
        }

        Debug.Log($"{name}被触发");
    }

    /// <summary>
    /// 清空事件中心
    /// </summary>
    public void Clear()
    {
        eventDic.Clear();
    }
}