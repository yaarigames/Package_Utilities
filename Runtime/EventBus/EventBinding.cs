using System;

public interface IEventBinding<T>
{
    public Action<T> OnEvent { get; set; }
    public Action OnEventNoArgs { get; set; }
    public int Priority { get; set; }
}

public class EventBinding<T> : IEventBinding<T> where T : IEvent
{
    Action<T> onEvent = _ => { };
    Action onEventNoArgs = () => { };
    int priority;

    Action<T> IEventBinding<T>.OnEvent
    {
        get => onEvent;
        set => onEvent = value;
    }

    Action IEventBinding<T>.OnEventNoArgs
    {
        get => onEventNoArgs;
        set => onEventNoArgs = value;
    }
    public int Priority
    {
        get => priority;
        set => priority = value;
    }

    public EventBinding(Action<T> onEvent, int priority = 0)
    {
        this.onEvent = onEvent;
        this.priority = priority;
    }
    public EventBinding(Action onEventNoArgs, int priority = 0)
    {
        this.onEventNoArgs = onEventNoArgs;
        this.priority = priority;
    }

    public void Add(Action onEvent) => onEventNoArgs += onEvent;
    public void Remove(Action onEvent) => onEventNoArgs -= onEvent;

    public void Add(Action<T> onEvent) => this.onEvent += onEvent;
    public void Remove(Action<T> onEvent) => this.onEvent -= onEvent;
}