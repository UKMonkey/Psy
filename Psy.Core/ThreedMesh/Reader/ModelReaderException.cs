using System;
using System.Runtime.Serialization;

namespace Psy.Core.ThreedMesh.Reader
{
    [Serializable]
    public class ModelReaderException : Exception
    {
        public ModelReaderException()
        {
        }

        public ModelReaderException(string message) : base(message)
        {
        }

        public ModelReaderException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ModelReaderException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}