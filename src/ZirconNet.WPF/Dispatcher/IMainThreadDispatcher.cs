namespace ZirconNet.WPF.Dispatcher;

public interface IMainThreadDispatcher
{
    void Invoke(Action action);
    ValueTask InvokeAsync(Action action);
    ValueTask<T> InvokeAsync<T>(Func<T> func);
}