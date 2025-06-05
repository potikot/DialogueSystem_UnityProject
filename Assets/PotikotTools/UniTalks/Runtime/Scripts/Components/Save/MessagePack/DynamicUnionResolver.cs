using System;
using System.Collections.Generic;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;

namespace PotikotTools.UniTalks
{
    public class DynamicUnionResolver<TBase> : IFormatterResolver
    {
        private readonly Dictionary<Type, int> _typeToKey;
        private readonly Dictionary<int, Type> _keyToType;

        public DynamicUnionResolver(Dictionary<Type, int> map)
        {
            _typeToKey = map;
            _keyToType = map.ToDictionary(kv => kv.Value, kv => kv.Key);
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            if (typeof(T) != typeof(TBase)) return null;

            return (IMessagePackFormatter<T>)new UnionFormatter<TBase>(_typeToKey, _keyToType);
        }
    }

    public class UnionFormatter<TBase> : IMessagePackFormatter<TBase>
    {
        private readonly Dictionary<Type, int> _typeToKey;
        private readonly Dictionary<int, Type> _keyToType;

        public UnionFormatter(Dictionary<Type, int> typeToKey, Dictionary<int, Type> keyToType)
        {
            _typeToKey = typeToKey;
            _keyToType = keyToType;
        }

        public void Serialize(ref MessagePackWriter writer, TBase value, MessagePackSerializerOptions options)
        {
            if (!_typeToKey.TryGetValue(value.GetType(), out var key))
                throw new Exception($"Type {value.GetType()} not registered in Union.");

            writer.WriteArrayHeader(2);
            writer.Write(key);
            MessagePackSerializer.Serialize(value.GetType(), ref writer, value, options);
        }

        public TBase Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {
            // var count = reader.ReadArrayHeader();
            var key = reader.ReadInt32();

            if (!_keyToType.TryGetValue(key, out var type))
                throw new Exception($"Unknown key {key} in Union.");

            var obj = MessagePackSerializer.Deserialize(type, ref reader, options);
            return (TBase)obj;
        }
    }
}