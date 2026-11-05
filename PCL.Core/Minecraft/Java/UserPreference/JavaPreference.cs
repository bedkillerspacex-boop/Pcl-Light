using System.Text.Json.Serialization;

namespace PCL.Core.Minecraft.Java.UserPreference;

[JsonConverter(typeof(JavaPreferenceJsonConverter))]
public abstract record JavaPreference;
