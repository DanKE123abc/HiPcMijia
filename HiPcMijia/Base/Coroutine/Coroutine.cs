using System.Collections;

namespace HiPcMijia.Coroutine;

public sealed class Coroutine
{
    private IEnumerator _routine;
    public Coroutine(IEnumerator routine)
    {
        _routine = routine;
    }

    public bool MoveNext()
    {
        if (_routine == null)
            return false;

        return _routine.MoveNext();
    }

    public void Stop()
    {
        _routine = null;
    }
}