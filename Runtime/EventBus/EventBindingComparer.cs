using System.Collections.Generic;
using System;

public class EventBindingComparer<T> : IComparer<IEventBinding<T>> where T : IEvent
{
    public int Compare(IEventBinding<T> x, IEventBinding<T> y)
    {
        // Compare by priority first (higher priority first)
        int priorityComparison = y.Priority.CompareTo(x.Priority);
        if (priorityComparison != 0)
            return priorityComparison;

        // If priorities are the same, compare by the OnEvent delegate (ensure uniqueness)
        int onEventComparison = Comparer<Action<T>>.Default.Compare(x.OnEvent, y.OnEvent);
        if (onEventComparison != 0)
            return onEventComparison;

        // If OnEvent is the same, compare OnEventNoArgs as a tie-breaker
        return Comparer<Action>.Default.Compare(x.OnEventNoArgs, y.OnEventNoArgs);
    }
}
