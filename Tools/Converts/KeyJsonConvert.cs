using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ixjok.Tools.Converts;

public class KeyJsonConvert : JsonConverter<KeyId>
{
    public override KeyId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TryGetGuid(out var value) ? value : Guid.Empty;
    public override void Write(Utf8JsonWriter writer, KeyId value, JsonSerializerOptions options)
        => writer.WriteString("id", value);
}
