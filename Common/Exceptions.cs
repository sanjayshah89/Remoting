using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

    [Serializable]
    public class CustomSinkException : RemotingException
    {
        public CustomSinkException()
        {
        }

        public CustomSinkException(string message)
            : base(message)
        {
        }

        protected CustomSinkException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public CustomSinkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public class ExcludeMeException : Exception
    {
        private bool excludeMePermanently = false;

        public ExcludeMeException()
        {
        }

        public ExcludeMeException(bool excludeMePermanently)
        {
            this.excludeMePermanently = excludeMePermanently;
        }

        public bool ExcludeMePermanently
        {
            get { return this.excludeMePermanently; }
        }
    }
}