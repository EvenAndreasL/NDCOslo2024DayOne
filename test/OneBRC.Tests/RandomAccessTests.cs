using System.Reflection;

namespace OneBRC.Tests;

public class RandomAccessTests
{
    private static readonly string FilePath = Path.Combine(
        Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!,
        "million.txt"
    );
    
    [Fact]
    public void Works()
    {
        var impl = new RandomAccessImpl(FilePath);
        var actual = impl.Run().Result;
        Assert.NotEmpty(actual);
    }
}