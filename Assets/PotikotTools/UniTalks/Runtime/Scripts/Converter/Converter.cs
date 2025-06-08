using System;

namespace PotikotTools.UniTalks
{
    public static class Converter
    {
        private static readonly Type _objectType;
        private static readonly Type _stringType;
        private static readonly Type _intType;
        private static readonly Type _floatType;
        private static readonly Type _boolType;

        private static readonly string _iConvertableName;

        static Converter()
        {
            _objectType = typeof(object);
            _stringType = typeof(string);
            _intType = typeof(int);
            _floatType = typeof(float);
            _boolType = typeof(bool);

            _iConvertableName = nameof(IStringParsable);
        }

        /// <summary>
        /// T should be: int, float, bool, enum or IStringConvertible
        /// </summary>
        /// <returns> object of type T </returns>
        public static T Convert<T>(string value)
        {
            return (T)Convert(typeof(T), value);
        }

        /// <summary>
        /// targetType should be: int, float, bool, enum or IStringConvertible
        /// </summary>
        /// <returns> object of type - targetType </returns>
        public static object Convert(Type targetType, string value)
        {
            if (targetType == _stringType || targetType == _objectType)
                return value;

            if (targetType.IsEnum)
            {
                if (int.TryParse(value, out int resultInt))
                {
                    if (resultInt > 0 && Enum.GetNames(targetType).Length > resultInt)
                        return Enum.ToObject(targetType, resultInt);
                }
                else if (Enum.TryParse(targetType, value, false, out object resultEnum))
                    return resultEnum;

                return null;
            }

            Type convertableInterface = targetType.GetInterface(_iConvertableName);
            if (convertableInterface != null)
            {
                IStringParsable result = Activator.CreateInstance(targetType) as IStringParsable;
                result.FromString(value);
                return result;
            }

            if (targetType == _intType)
            {
                if (int.TryParse(value, out int result))
                    return result;

                return null;
            }

            if (targetType == _floatType)
            {
                if (float.TryParse(value, out float result))
                    return result;

                return null;
            }

            if (targetType == _boolType)
            {
                if (value == "1")
                    return true;
                if (value == "0")
                    return false;

                return null;
            }

            return null;
        }
    }
}