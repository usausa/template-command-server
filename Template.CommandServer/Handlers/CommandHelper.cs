namespace Template.CommandServer.Handlers;

using System;
using System.Buffers;
using System.Buffers.Text;

public static class CommandHelper
{
    public static void Split<T>(ref ReadOnlySequence<T> buffer, out ReadOnlySequence<T> split, T delimiter)
        where T : unmanaged, IEquatable<T>
    {
        var reader = new SequenceReader<T>(buffer);
        if (reader.TryReadTo(out split, delimiter))
        {
            buffer = buffer.Slice(reader.Position);
        }
        else
        {
            split = buffer;
            buffer = ReadOnlySequence<T>.Empty;
        }
    }

    public static bool SequentialEqual<T>(this ReadOnlySequence<T> sequence, ReadOnlySpan<T> span)
    {
        if (sequence.IsSingleSegment)
        {
            return sequence.FirstSpan.SequenceEqual(span);
        }

        foreach (var segment in sequence)
        {
            var length = segment.Length;
            if ((length > span.Length) || !segment.Span.SequenceEqual(span[..length]))
            {
                return false;
            }

            span = span[length..];
        }

        return span.Length == 0;
    }

    public static bool TryParse(this ReadOnlySequence<byte> sequence, out int value)
    {
        return sequence.IsSingleSegment
            ? Utf8Parser.TryParse(sequence.FirstSpan, out value, out _)
            : Utf8Parser.TryParse(sequence.ToArray(), out value, out _);
    }

    public static void WriteAndAdvanceOk(this IBufferWriter<byte> writer)
    {
        "ok\r\n"u8.CopyTo(writer.GetSpan(4));
        writer.Advance(4);
    }

    public static void WriteAndAdvanceOk(this IBufferWriter<byte> writer, int value)
    {
        "ok "u8.CopyTo(writer.GetSpan(3));
        writer.Advance(3);
        WriteInt32(writer, value);
        "\r\n"u8.CopyTo(writer.GetSpan(2));
        writer.Advance(2);
    }

    public static void WriteAndAdvanceNg(this IBufferWriter<byte> writer)
    {
        "ng\r\n"u8.CopyTo(writer.GetSpan(4));
        writer.Advance(4);
    }

    public static void WriteInt32(this IBufferWriter<byte> writer, int value)
    {
        var span = writer.GetSpan(11);
        Utf8Formatter.TryFormat(value, span, out var written);
        writer.Advance(written);
    }
}
