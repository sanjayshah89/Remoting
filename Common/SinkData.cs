using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
     
    #region ServerSinkData
    public class ServerSinkData
    {

        public readonly IChannelReceiver Channel;
        public readonly SinkProviderData ConfigurationData;

        public ServerSinkData(SinkProviderData configurationData, IChannelReceiver channel)
        {
            this.Channel = channel;
            this.ConfigurationData = configurationData;
        }
    }
    #endregion

    #region ClientSinkData
    public class ClientSinkData
    {
        public readonly IChannelSender Channel;
        public readonly string Url;
        public readonly object RemoteChannelData;
        public readonly SinkProviderData ConfigurationData;

        public ClientSinkData(SinkProviderData configurationData, IChannelSender channel, string url, object remoteChannelData)
        {
            this.Channel = channel;
            this.Url = url;
            this.RemoteChannelData = remoteChannelData;
            this.ConfigurationData = configurationData;
        }

    }
    #endregion
    
}