using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.IO;

namespace Common
{
    public abstract class BaseCustomSink : BaseChannelObjectWithProperties, IClientChannelSink, IServerChannelSink, IMessageSink
    {
        #region General

        // there are 3 references to the next sink (one for each interface type),
        // that is 3 references to the same object,
        // to avoid multiple castings in runtime
        private IClientChannelSink nextClientChannelSink;
        private IServerChannelSink nextServerChannelSink;
        private IMessageSink nextMessageSink;


        public BaseCustomSink()
        {
           
        }


        // called by the provider to set the next sink

        public void SetNextSink(object nextSink)
        {
            this.nextClientChannelSink = nextSink as IClientChannelSink;
            this.nextServerChannelSink = nextSink as IServerChannelSink;
            this.nextMessageSink = nextSink as IMessageSink;
        }
              
        #endregion

        #region IClientChannelSink implementation

        IClientChannelSink IClientChannelSink.NextChannelSink
        {
            get
            {
                return this.nextClientChannelSink;
            }
        }

        void IClientChannelSink.AsyncProcessRequest(IClientChannelSinkStack sinkStack,
            IMessage msg, ITransportHeaders headers, Stream stream)
        {
            // process request
            object state = null;
            ProcessRequest(msg, headers, ref stream, ref state);

            // push to stack (to get a call to handle response)
            // and forward to the next
            sinkStack.Push(this, state);
            this.nextClientChannelSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
        }

        void IClientChannelSink.AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state,
            ITransportHeaders headers, Stream stream)
        {
            // process response
            ProcessResponse(null, headers, ref stream, state);

            // forward to the next
            sinkStack.AsyncProcessResponse(headers, stream);
        }

        Stream IClientChannelSink.GetRequestStream(IMessage msg, ITransportHeaders headers)
        {
            return this.nextClientChannelSink.GetRequestStream(msg, headers);
        }

        void IClientChannelSink.ProcessMessage(IMessage msg, ITransportHeaders requestHeaders,
            Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            // process request
            object state = null;
            ProcessRequest(msg, requestHeaders, ref requestStream, ref state);

            // forward to the next
            this.nextClientChannelSink.ProcessMessage(msg, requestHeaders, requestStream,
                out responseHeaders, out responseStream);

            // process response
            ProcessResponse(null, responseHeaders, ref responseStream, state);
        }

        #endregion

        #region IServerChannelSink implementation

        IServerChannelSink IServerChannelSink.NextChannelSink
        {
            get
            {
                return this.nextServerChannelSink;
            }
        }

        void IServerChannelSink.AsyncProcessResponse(IServerResponseChannelSinkStack sinkStack,
            object state, IMessage msg, ITransportHeaders headers, Stream stream)
        {
            // process response
            ProcessResponse(msg, headers, ref stream, state);

            // forward to the next
            sinkStack.AsyncProcessResponse(msg, headers, stream);
        }

        Stream IServerChannelSink.GetResponseStream(IServerResponseChannelSinkStack sinkStack,
            object state, IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        ServerProcessing IServerChannelSink.ProcessMessage(IServerChannelSinkStack sinkStack,
            IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream,
            out IMessage responseMsg, out ITransportHeaders responseHeaders,
            out Stream responseStream)
        {
            // process request
            object state = null;
            ProcessRequest(requestMsg, requestHeaders, ref requestStream, ref state);

            sinkStack.Push(this, state);

            ServerProcessing processing = nextServerChannelSink.ProcessMessage(sinkStack,
                requestMsg, requestHeaders, requestStream,
                out responseMsg, out responseHeaders, out responseStream);

            if (processing == ServerProcessing.Complete)
            {
                ProcessResponse(responseMsg, responseHeaders, ref responseStream, state);
            }

            return processing;
        }

        #endregion

        #region IMessageSink implementation

        IMessageCtrl IMessageSink.AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            object state = null;
            Stream dummyStream = null;
            ProcessRequest(msg, null, ref dummyStream, ref state);
            ReplySink myReplySink = new ReplySink(replySink, this, state);

            return this.nextMessageSink.AsyncProcessMessage(msg, myReplySink);
        }

        IMessage IMessageSink.SyncProcessMessage(IMessage reqMsg) 
        {
            object state = null;
            Stream dummyStream = null;
            ProcessRequest(reqMsg, null, ref dummyStream, ref state);
            IMessage respMsg = this.nextMessageSink.SyncProcessMessage(reqMsg);
            dummyStream = null;
            ProcessResponse(respMsg, null, ref dummyStream, state);
            return respMsg;
        }

        IMessageSink IMessageSink.NextSink
        {
            get
            {
                return this.nextMessageSink;
            }
        }

        // the following class is needed in order to receive the reply
        // of the asyncronous message
        private class ReplySink : IMessageSink
        {
            IMessageSink nextSink;
            BaseCustomSink parentSink;
            object state;

            public ReplySink(IMessageSink nextSink, BaseCustomSink parentSink, object state)
            {
                this.nextSink = nextSink;
                this.parentSink = parentSink;
                this.state = state;
            }

            IMessageCtrl IMessageSink.AsyncProcessMessage(IMessage msg, IMessageSink replySink)
            {
                throw new Exception("ReplySink.AsyncProcessMessage should never be called!");
            }

            IMessage IMessageSink.SyncProcessMessage(IMessage reqMsg)
            {
                Stream dummyStream = null;
                this.parentSink.ProcessResponse(reqMsg, null, ref dummyStream, this.state);
                return this.nextSink.SyncProcessMessage(reqMsg);
            }

            IMessageSink IMessageSink.NextSink
            {
                get
                {
                    return this.nextSink;
                }
            }
        }

        #endregion

        #region Overridables

        protected virtual void ProcessRequest(IMessage message, ITransportHeaders headers, ref Stream stream, ref object state)
        {
        }
               
        protected virtual void ProcessResponse(IMessage message, ITransportHeaders headers, ref Stream stream, object state)
        {
        }

        #endregion
    }
}
