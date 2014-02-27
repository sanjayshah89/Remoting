using System;
using System.Runtime.Remoting.Channels;
using System.Collections;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using Common;

namespace CustomClientSinks
{
    public class CustomClientSinkProvider : IClientChannelSinkProvider
    {
        private SinkProviderData data;
        private Type customSinkType;

        private IClientChannelSinkProvider nextProvider;

        public IClientChannelSinkProvider Next
        {
            get { return this.nextProvider; }
            set { this.nextProvider = value; }
        }

        public CustomClientSinkProvider(IDictionary properties, ICollection providerData)
        {

            // get custom sink type from configuration file
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


        public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData)
        {
            BaseCustomSink customSinkObject = null;
            bool keepCreatingInstance = false;

            IClientChannelSink next = this.nextProvider.CreateSink(channel, url, remoteChannelData);

            object[] par = { new ClientSinkData(this.data, channel, url, remoteChannelData) };

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
