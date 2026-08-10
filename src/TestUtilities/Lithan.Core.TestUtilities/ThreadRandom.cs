namespace Lithan.Core.TestUtilities;

/// <summary> 
/// Convenience class for dealing with randomness. 
/// </summary> 
public static class ThreadLocalRandom
{
    private static readonly Random GlobalRandom = new();
    private static readonly SemaphoreSlim GlobalLock = new(1,1);

    /// <summary> 
    /// Random number generator 
    /// </summary> 
    private static readonly ThreadLocal<Random> ThreadRandom = new(NewRandom);

    /// <summary> 
    /// Creates a new instance of Random. The seed is derived 
    /// from a global (static) instance of Random, rather 
    /// than time. 
    /// </summary> 
    public static Random NewRandom()
    {
        GlobalLock.Wait(TimeSpan.FromMilliseconds(100));
        try
        { 
            return new Random(GlobalRandom.Next());
        }
        finally
        {
            GlobalLock.Release();
        }
    }

    /// <summary> 
    /// Returns an instance of Random which can be used freely 
    /// within the current thread. 
    /// </summary> 
    public static Random Instance => ThreadRandom.Value!;

    /// <summary>See <see cref="Random.Next()" /></summary> 
    public static int Next()
    {
        return Instance.Next();
    }

    /// <summary>See <see cref="Random.Next(int)" /></summary> 
    public static int Next(int maxValue)
    {
        return Instance.Next(maxValue);
    }

    /// <summary>See <see cref="Random.Next(int, int)" /></summary> 
    public static int Next(int minValue, int maxValue)
    {
        return Instance.Next(minValue, maxValue);
    }

    /// <summary>See <see cref="Random.NextDouble()" /></summary> 
    public static double NextDouble()
    {
        return Instance.NextDouble();
    }

    /// <summary>See <see cref="Random.NextBytes(byte[])" /></summary> 
    public static void NextBytes(byte[] buffer)
    {
        Instance.NextBytes(buffer);
    }
}