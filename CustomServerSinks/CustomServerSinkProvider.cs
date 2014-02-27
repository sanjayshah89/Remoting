using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Collections;
using System.Reflection;
using Common;

namespace CustomServerSinks
{
    public class CustomServerSinkProvider : IServerChannelSinkProvider
    {
        public IServerChannelSinkProvider Next { get; set; }
        private SinkProviderData data;
        private Type customSinkType;

        public CustomServerSinkProvider(IDictionary properties, ICollection providerData)
        {
            string customSinkType = (string)properties["customSinkType"];
            if (customSinkType == null)
            {
                throw new CustomSinkException("no customSinkType property in the <provider> element.");
            }
            this.customSinkType = Type.GetType(customSinkType);
            if (this.customSinkType == null)
            {
                throw new CustomSinkException(
                    string.Format("Could not load type {0}", customSinkType));
            }

            // make sure the custom sink type inherits BaseCustomSink
            if (!this.customSinkType.IsSubclassOf(typeof(BaseCustomSink)))
            {
                throw new CustomSinkException("Custom sink type does not inherit from BaseCustomSink");
            }

            // see if there is a <customData> element in the provider data
            // and save it for passing it to the custom sink's constructor
            foreach (SinkProviderData data in providerData)
            {
                if (data.Name == "customData")
                {
                    this.data = data;
                    break;
                }
            }

        }


        /// <summary>
        /// Creates a Sink . Implementation of IServerChannelSink method
        /// Gets invoked by .Net Framework
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            BaseCustomSink customSinkObject = null;
            bool keepCreatingInstance = false;


            // try to instantiate the custom sink
            // with a constructor that takes ServerSinkCreationData (or SinkCreationData)
            object[] par = { new ServerSinkData(this.data, channel) };
            try
            {
                customSinkObject = (BaseCustomSink)Activator.CreateInstance(this.customSinkType, par);
            }
            catch (MissingMethodException)
            {
                // do nothing - try to create using a parameterless constructor
                keepCreatingInstance = true;
            }
            catch (Exception e)
            {
                ExamineConstructionException(e);
            }

            // if there is no constructor that takes ServerSinkCreationData or SinkCreationData,
            // look for a parameterless constructor
            if (keepCreatingInstance)
            {
                try
                {
                    customSinkObject = (BaseCustomSink)Activator.CreateInstance(this.customSinkType);
                }
                catch (Exception e)
                {
                    ExamineConstructionException(e);
                }
            }

            // create next sink in the chain
            IServerChannelSink next = this.Next.CreateSink(channel);

            if (customSinkObject != null)
            {
                customSinkObject.SetNextSink(next);
                return customSinkObject;
            }
            else
            {
                return next;
            }
        }

        public void GetChannelData(IChannelDataStore channelData)
        {
        }

        private void ExamineConstructionException(Exception e)
        {
            if (!(e.InnerException is ExcludeMeException))
            {
                if (e is TargetInvocationException)
                {
                    throw new CustomSinkException(
                        string.Format("Could not create instance of {0}. {1} was thrown during construction. Message: {2}",
                        this.customSinkType.ToString(), e.InnerException.GetType().ToString(),
                        e.InnerException.Message), e);
                }
                else
                {
                    throw new CustomSinkException(
                        string.Format("Could not create instance of {0}.",
                        this.customSinkType.ToString()), e);
                }
            }

        }

    }
}
