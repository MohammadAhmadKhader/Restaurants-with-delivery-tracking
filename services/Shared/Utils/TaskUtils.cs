namespace Shared.Utils;

public static class TaskUtils
{
    public static async Task<(T1, T2)> RunAsync<T1, T2>(
        Task<T1> task1,
        Task<T2> task2
    )
    {
        await Task.WhenAll(task1, task2);
        return (task1.Result, task2.Result);
    }
    
    public static async Task<(T1, T2, T3)> RunAsync<T1, T2, T3>(
        Task<T1> task1,
        Task<T2> task2,
        Task<T3> task3
    )
    {
        await Task.WhenAll(task1, task2, task3);
        return (task1.Result, task2.Result, task3.Result);
    }
}