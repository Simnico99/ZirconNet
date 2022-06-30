namespace ZirconNet.WPF.Dispatcher;

public interface IMainThreadDispatcher
{
    void Invoke(Action action);
    Task InvokeAsync(Action action);
    Task<T> InvokeAsync<T>(Func<T> func);
}