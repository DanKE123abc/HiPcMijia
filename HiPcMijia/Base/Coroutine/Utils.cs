namespace HiPcMijia.Coroutine;

public class Time
{
    public const float deltaTime = 0.02f;
    public const int deltaMilTime = 20;
}

public interface IWait
{
    bool Tick();
}

public class WaitForSeconds : IWait
{
    public float waitTime = 0;

    public WaitForSeconds(float time)
    {
        waitTime = time;
    }

    bool IWait.Tick()
    {
        waitTime -= Time.deltaTime;
        Console.WriteLine("[WaitForSeconds] now left:" + waitTime);
        return waitTime <= 0;
    }
}

public class WaitForFrame : IWait
{
    public int waitFrame = 0;

    public WaitForFrame(int frame)
    {
        waitFrame = frame;
    }

    bool IWait.Tick()
    {
        waitFrame--;
        Console.WriteLine("[WaitForFrame] now left:" + waitFrame);
        return waitFrame <= 0;
    }
}