using System.Collections.Generic;
using System.IO;

namespace Psy.Core.Serialization
{
    class StringDictionarySerializer
    {
        public byte[] Serialize(IDictionary<string, string> dictionary)
        {
            var stream = new MemoryStream();

            stream.Write(dictionary.Count);
            foreach (var item in dictionary)
            {
                stream.Write(item.Key);
                stream.Write(item.Value);
            }

            return stream.ToArray();
        }
    }
}