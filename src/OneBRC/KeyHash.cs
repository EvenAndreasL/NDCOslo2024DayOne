using System.Runtime.InteropServices;

namespace OneBRC;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Key : IEquatable<Key>
{
    private readonly long _part1;
    private readonly long _part2;

    public Key(long part1, long part2)
    {
        _part1 = part1;
        _part2 = part2;
    }

    public bool Equals(Key other)
    {
        throw new NotImplementedException();
    }
}

public static class KeyHash
{
    public static Key ToKey(ReadOnlySpan<byte> span)
    {
        if (span.Length > 7)
        {
            if (span.Length > 15)
            {
                return new Key(
                    MemoryMarshal.Read<long>(span),
                    MemoryMarshal.Read<long>(span.Slice(8))
                );
            }

            return new Key(
                MemoryMarshal.Read<long>(span),
                Rest(span.Slice(8))
                );
        }

        return new Key(Rest(span), 0);
    }

    private static long Rest(ReadOnlySpan<byte> span)
    {
        if (span.Length > 3)
        {
            long i = MemoryMarshal.Read<int>(span);
            var rest = span.Slice(4);
            while (rest.Length > 0)
            {
                i = (i << 8) + rest[0];
                rest = rest.Slice(1);
            }

            return i;
        }
        
        {
            long i = 0;
            var rest = span.Slice(4);
            while (rest.Length > 0)
            {
                i = (i << 8) + rest[0];
                rest = rest.Slice(1);
            }

            return i;   
        }
    }
    public static long ToLong(ReadOnlySpan<byte> span)
    {
        if (span.Length > 7)
        {
            return MemoryMarshal.Read<long>(span);
        }
        
        Span<byte> local = stackalloc byte[8];
        local.Clear();
        span.CopyTo(local);
        return MemoryMarshal.Read<long>(local);
    }
    
    public static long ToLong2(ReadOnlySpan<byte> span)
    {
        if (span.Length > 7)
        {
            return MemoryMarshal.Read<long>(span);
        }

        if (span.Length > 3)
        {
            long i = MemoryMarshal.Read<int>(span);
            var rest = span.Slice(4);
            while (rest.Length > 0)
            {
                i = (i << 8) + rest[0];
                rest = rest.Slice(1);
            }

            return i;
        }

        {
            long i = 0;
            var rest = span.Slice(4);
            while (rest.Length > 0)
            {
                i = (i << 8) + rest[0];
                rest = rest.Slice(1);
            }

            return i;   
        }
    }
}