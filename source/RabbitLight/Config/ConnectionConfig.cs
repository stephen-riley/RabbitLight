﻿using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;

namespace RabbitLight.Config
{
    public class ConnectionConfig : ConnectionFactory
    {
        /// <summary>
        /// Port where RabbitMQ management UI plugin is available
        /// </summary>
        public ushort PortApi { get; set; }
        /// <summary>
        /// Minimum number of parallel channels
        /// <br>https://www.rabbitmq.com/consumers.html#concurrency</br>
        /// </summary>
        public ushort MinChannels { get; set; } = 10;
        /// <summary>
        /// Maximum number of parallel channels
        /// <br>https://www.rabbitmq.com/consumers.html#concurrency</br>
        /// </summary>
        public ushort MaxChannels { get; set; } = 50;
        /// <summary>
        /// Number of messages required to scale a new channel (e.g. 1000 messages)
        /// </summary>
        public ushort? ScallingThreshold { get; set; }
        /// <summary>
        /// Number of messages that will be cached by each channel at once.
        /// <br>As RabbitMQ directs the channel's worker to the consumers with Round Robin,</br>
        /// <br>it should be a number smaller enough to prevent blocking other queues for a long time,</br>
        /// <br>and big enough to prevent multipe fetchs to the server (e.g. 10).</br>
        /// </summary>
        public ushort PrefetchCount { get; set; } = 10;
        /// <summary>
        /// Number of channels per connection (RabbitMQ's IConnection).
        /// <br>Should be equal to or smaller than the one configured on the server.</br>
        /// </summary>
        public ushort ChannelsPerConnection { get; set; } = 25;
        /// <summary>
        /// Delay for when Nacking a message for requeue
        /// </summary>
        public TimeSpan? RequeueDelay { get; set; }
        /// <summary>
        /// Interval regarding channel monitoring tasks (health check and scalling)
        /// </summary>
        public TimeSpan MonitoringInterval { get; set; } = TimeSpan.FromSeconds(60);

        private ConnectionConfig()
        {
        }

        public ConnectionConfig(ushort portApi, ushort? minChannels = null, ushort? maxChannels = null,
            ushort? scallingThreshold = null, ushort? prefetchCount = null, ushort? channelsPerConnection = null,
            TimeSpan? requeueDelay = null, TimeSpan? monitoringInterval = null)
        {
            PortApi = portApi;
            MinChannels = minChannels ?? MinChannels;
            MaxChannels = maxChannels ?? MaxChannels;
            ScallingThreshold = scallingThreshold;
            PrefetchCount = prefetchCount ?? PrefetchCount;
            ChannelsPerConnection = channelsPerConnection ?? ChannelsPerConnection;
            RequeueDelay = requeueDelay;
            MonitoringInterval = monitoringInterval ?? MonitoringInterval;

            Validate(this);
        }

        public static ConnectionConfig FromConfig(IConfiguration configuration)
        {
            var config = new ConnectionConfig();
            configuration.Bind(config);

            Validate(config);

            return config;
        }

        private static void Validate(ConnectionConfig config)
        {
            if (config.MinChannels < 1)
                throw new ArgumentException("Should be bigger than 0", nameof(config.MinChannels));

            if (config.MaxChannels < 1)
                throw new ArgumentException("Should be bigger than 0", nameof(config.MaxChannels));

            if (config.MinChannels > config.MaxChannels)
                throw new ArgumentException($"{nameof(config.MaxChannels)} should be bigger than {nameof(config.MinChannels)}", nameof(config.MaxChannels));

            if (config.ScallingThreshold < 1)
                throw new ArgumentException("Should be bigger than 0", nameof(config.ScallingThreshold));

            if (config.PrefetchCount < 0)
                throw new ArgumentException("Should be equal to or bigger than 0", nameof(config.PrefetchCount));

            if (config.ChannelsPerConnection < 1)
                throw new ArgumentException("Should be bigger than 0", nameof(config.ChannelsPerConnection));
        }
    }
}
