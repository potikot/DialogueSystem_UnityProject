using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace PotikotTools.UniTalks
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            var obj = new JObject
            {
                ["x"] = value.x,
                ["y"] = value.y
            };

            obj.WriteTo(writer);
        }

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject obj = JObject.Load(reader);

            float x = (float)obj.GetValue("x");
            float y = (float)obj.GetValue("y");

            return new Vector2(x, y);
        }
    }
}