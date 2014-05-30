using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace FinApps.SSO.RestClient_Base.Model
{
    [Serializable]
    public class ValueProviderResult
    {
        private static readonly CultureInfo staticCulture = CultureInfo.InvariantCulture;
        private CultureInfo _instanceCulture;

        protected ValueProviderResult()
        {
        }

        public ValueProviderResult(object rawValue, string attemptedValue, CultureInfo culture)
        {
            RawValue = rawValue;
            AttemptedValue = attemptedValue;
            Culture = culture;
        }

        public string AttemptedValue { get; protected set; }

        public CultureInfo Culture
        {
            get { return _instanceCulture ?? (_instanceCulture = staticCulture); }
            protected set { _instanceCulture = value; }
        }

        public object RawValue { get; protected set; }

        private static object ConvertSimpleType(CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType.IsInstanceOfType(value))
            {
                return value;
            }
            var text = value as string;
            if (text != null && string.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            Type underlyingType = Nullable.GetUnderlyingType(destinationType);
            if (underlyingType != null)
            {
                destinationType = underlyingType;
            }
            object result;
            if (text == null)
            {
                var convertible = value as IConvertible;
                if (convertible != null)
                {
                    try
                    {
                        result = convertible.ToType(destinationType, culture);
                        return result;
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception)
                    {
                    }
                }
            }
            TypeConverter converter = TypeDescriptor.GetConverter(destinationType);
            bool flag = converter.CanConvertFrom(value.GetType());
            if (!flag)
            {
                converter = TypeDescriptor.GetConverter(value.GetType());
            }
            if (!flag && !converter.CanConvertTo(destinationType))
            {
                if (destinationType.IsEnum && value is int)
                {
                    return Enum.ToObject(destinationType, (int)value);
                }
                string message = string.Format(CultureInfo.CurrentCulture,
                    "The parameter conversion from type &apos;{0}&apos; to type &apos;{1}&apos; failed because no type converter can convert between these types.",
                    new object[]
                    {
                        value.GetType().FullName,
                        destinationType.FullName
                    });
                throw new InvalidOperationException(message);
            }
            try
            {
                object obj = flag
                    ? converter.ConvertFrom(context: null, culture: culture, value: value)
                    : converter.ConvertTo(context: null, culture: culture, value: value, destinationType: destinationType);
                result = obj;
            }
            catch (Exception innerException)
            {
                string message2 = string.Format(CultureInfo.CurrentCulture,
                    "The parameter conversion from type &apos;{0}&apos; to type &apos;{1}&apos; failed. See the inner exception for more information.",
                    new object[]
                    {
                        value.GetType().FullName,
                        destinationType.FullName
                    });
                throw new InvalidOperationException(message2, innerException);
            }
            return result;
        }

        public object ConvertTo(Type type)
        {
            return ConvertTo(type, null);
        }

        public virtual object ConvertTo(Type type, CultureInfo culture)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            CultureInfo culture2 = culture ?? Culture;
            return UnwrapPossibleArrayType(culture2, RawValue, type);
        }

        private static object UnwrapPossibleArrayType(CultureInfo culture, object value, Type destinationType)
        {
            if (value == null || destinationType.IsInstanceOfType(value))
            {
                return value;
            }
            var array = value as Array;
            if (destinationType.IsArray)
            {
                Type elementType = destinationType.GetElementType();
                if (array != null)
                {
                    IList list = Array.CreateInstance(elementType, array.Length);
                    for (int i = 0; i < array.Length; i++)
                    {
                        list[i] = ConvertSimpleType(culture, array.GetValue(i), elementType);
                    }
                    return list;
                }
                object value2 = ConvertSimpleType(culture, value, elementType);
                IList list2 = Array.CreateInstance(elementType, 1);
                list2[0] = value2;
                return list2;
            }
            if (array == null)
            {
                return ConvertSimpleType(culture, value, destinationType);
            }
            if (array.Length > 0)
            {
                value = array.GetValue(0);
                return ConvertSimpleType(culture, value, destinationType);
            }
            return null;
        }
    }
}