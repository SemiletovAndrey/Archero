using System;
using Template.Scripts.Enums;

namespace Template.Scripts.Attributes
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class SerializableEnumAttribute : Attribute
    {
        public SerializableEnumAttribute(EnumSerializationType serializationType)
        {
            SerializationType = serializationType;
        }

        public EnumSerializationType SerializationType { get; }
    }
}