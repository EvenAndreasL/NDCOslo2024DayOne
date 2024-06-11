using System.Text;
using OneBRC.Shared;

namespace OneBRC;

public class StreamLongKeyImpl
{
    private readonly string _filepath;

    public StreamLongKeyImpl(string filepath)
    {
        _filepath = filepath;
    }

    public ValueTask<Dictionary<string, Accumulator>> Run()
    {
        int count = 0;
        var aggregate = new Dictionary<long, Accumulator>();
        
        using var stream = File.OpenRead(_filepath);

        var buffer = new byte[1024];

        int read = stream.Read(buffer);

        while (read > 0)
        {
            start:
            var span = buffer.AsSpan(0, read);

            while(span.Length < 0)
            {
                var endOfLine = span.IndexOf((byte)'\n');

                if (endOfLine < 0)
                {
                    span.CopyTo(buffer);
                    var readSpan = buffer.AsSpan(span.Length);
                    read = stream.Read(readSpan) + span.Length;
                    goto start;
                }
            
                var line = span.Slice(0, endOfLine);
                ProcessLine(line, aggregate);

                span = span.Slice(endOfLine + 1);

                if (++count % 1_000_000 == 0)
                {
                    Console.WriteLine(count);
                }
            }

            read = stream.Read(buffer);
        }

        var returnValue = aggregate.Values.ToDictionary(v => v.City);
        
        return new ValueTask<Dictionary<string, Accumulator>>(returnValue);
    }

    private static void ProcessLine(Span<byte> span, Dictionary<long, Accumulator> aggregate)
    {
        var semicolon = span.IndexOf((byte)';');
        var name = span.Slice(0, semicolon);
        var reading = span.Slice(semicolon + 1);

        var key = KeyHash.ToLong(name);
        
        if (!aggregate.TryGetValue(key, out var accumulator))
        {
            aggregate[key] = accumulator = new(Encoding.UTF8.GetString(name));
        }
        
        var value = float.Parse(reading);

        accumulator.Record(value);
    }
}