using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PCL.Core.Minecraft.Java.UserPreference;

/// <summary>
/// Preserves the Java preference discriminator format while accepting it in any property order.
/// </summary>
public sealed class JavaPreferenceJsonConverter : JsonConverter<JavaPreference>
{
    public override JavaPreference Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;
        var hasKind = false;
        var kindElement = default(JsonElement);
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in root.EnumerateObject())
            {
                if (!string.Equals(property.Name, "kind", StringComparison.OrdinalIgnoreCase)) continue;
                kindElement = property.Value;
                hasKind = true;
            }
        }

        if (root.ValueKind != JsonValueKind.Object ||
            !hasKind ||
            kindElement.ValueKind != JsonValueKind.String)
        {
            throw new JsonException("Java preference is missing its kind discriminator.");
        }

        var concreteType = kindElement.GetString() switch
        {
            "exist" => typeof(ExistingJava),
            "global" => typeof(UseGlobalPreference),
            "relative" => typeof(UseRelativePath),
            "auto" => typeof(AutoSelect),
            _ => throw new JsonException("Unknown Java preference kind.")
        };

        return (JavaPreference?)JsonSerializer.Deserialize(root.GetRawText(), concreteType, options)
            ?? throw new JsonException("Java preference could not be deserialized.");
    }

    public override void Write(Utf8JsonWriter writer, JavaPreference value, JsonSerializerOptions options)
    {
        var (kind, concreteType) = value switch
        {
            ExistingJava => ("exist", typeof(ExistingJava)),
            UseGlobalPreference => ("global", typeof(UseGlobalPreference)),
            UseRelativePath => ("relative", typeof(UseRelativePath)),
            AutoSelect => ("auto", typeof(AutoSelect)),
            _ => throw new JsonException("Unknown Java preference type.")
        };

        var payload = JsonSerializer.SerializeToElement(value, concreteType, options);
        if (payload.ValueKind != JsonValueKind.Object)
            throw new JsonException("Java preference payload must be an object.");

        writer.WriteStartObject();
        writer.WriteString("kind", kind);
        foreach (var property in payload.EnumerateObject())
        {
            if (property.NameEquals("kind"))
                throw new JsonException("Java preference payload cannot contain a kind property.");
            property.WriteTo(writer);
        }
        writer.WriteEndObject();
    }
}
