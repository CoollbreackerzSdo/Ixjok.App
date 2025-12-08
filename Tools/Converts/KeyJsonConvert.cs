using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ixjok.Tools.Converts;

/// <summary>
/// Convertidor JSON personalizado para serialización/deserialización de <see cref="KeyId"/>.
/// Convierte GUIDs a y desde formato JSON.
/// </summary>
public class KeyJsonConvert : JsonConverter<KeyId>
{
    /// <summary>
    /// Lee un GUID desde JSON y lo convierte a <see cref="KeyId"/>.
    /// </summary>
    /// <param name="reader">El lector JSON.</param>
    /// <param name="typeToConvert">El tipo destino (siempre KeyId).</param>
    /// <param name="options">Opciones de serialización JSON.</param>
    /// <returns>Un KeyId con el GUID leído, o GUID.Empty si falla la conversión.</returns>
    public override KeyId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.TryGetGuid(out var value) ? value : Guid.Empty;

    /// <summary>
    /// Escribe un <see cref="KeyId"/> en formato JSON.
    /// </summary>
    /// <param name="writer">El escritor JSON.</param>
    /// <param name="value">El KeyId a serializar.</param>
    /// <param name="options">Opciones de serialización JSON.</param>
    public override void Write(Utf8JsonWriter writer, KeyId value, JsonSerializerOptions options)
        => writer.WriteString("id", value);
}
