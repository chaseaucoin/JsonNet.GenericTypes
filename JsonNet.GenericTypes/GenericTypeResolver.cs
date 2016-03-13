using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;

namespace JsonNet.GenericTypes
{
    /// <summary>
    /// A generic type resolver for JSON.Net
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class GenericTypeResolver : JsonConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            if (objectType.IsInterface)
            {
                //any interfaces that we don't want to try to resolve need to go here. 
                Type[] ingnoredTypes = new Type[]
                {
                    typeof(IEnumerable)
                };

                var implementsIgnoredInterface = objectType
                    .GetInterfaces()
                    .Any(objectInterface => ingnoredTypes.Contains(objectInterface));

                if (implementsIgnoredInterface)
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON.
        /// </summary>
        /// <value>
        /// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can read JSON; otherwise, <c>false</c>.
        /// </value>
        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON.
        /// </summary>
        /// <value>
        /// <c>true</c> if this <see cref="T:Newtonsoft.Json.JsonConverter" /> can write JSON; otherwise, <c>false</c>.
        /// </value>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //This class is only for deserialization. Serialization should already "just work"
        }

        /// <summary>
        /// Reads the JSON representation of the object. 
        /// Makes the assumption that at least one concrete version of the object exists 
        /// in the same assembly as the interface.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type objectConreteType = null;

            foreach (Type t in objectType.Assembly.GetTypes())
            {
                // Find a type that implements the interface and is concrete.
                // Assumes that the type is found in the assembly of the interface.
                if (objectType.IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
                {
                    objectConreteType = t;
                    break;
                }
                else
                {
                    var message = string.Format(
                        "Could not find a class that is assignable from {0} in the assembly {1}",
                        objectType,
                        objectType.Assembly);

                    throw new EntryPointNotFoundException(message);
                }
            }

            var obj = serializer.Deserialize(reader, objectConreteType);

            return obj;
        }
    }
}
