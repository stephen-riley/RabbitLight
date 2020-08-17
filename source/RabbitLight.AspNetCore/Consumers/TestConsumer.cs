﻿using Microsoft.Extensions.Logging;
using RabbitLight.AspNetCore.Models;
using RabbitLight.Attributes;
using RabbitLight.Interfaces;
using RabbitLight.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RabbitLight.AspNetCore.Consumers
{
    [Exchange("test-exchange1")]
    [Exchange("test-exchange2")]
    public class TestConsumer : IConsumer
    {
        private readonly ILogger<TestConsumer> _logger;

        private static int Count = 0;
        private static DateTime Start = DateTime.Now;

        public TestConsumer(ILogger<TestConsumer> logger)
        {
            _logger = logger;
        }

        [Queue("test-queue1", "test1")]
        [Queue("test-queue2", "test2")]
        public async Task Test(BusContext<Test> context)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Count++;

            // Start
            var msg = context.MessageFromJson();
            await Task.Delay(500);
            // Finish

            stopWatch.Stop();
            var duration = DateTime.Now - Start;
            _logger.LogInformation("\r\n----------"
                + "CONSUMING"
                + "----------"
                + $"{context.EventArgs.Exchange}:{context.EventArgs.RoutingKey} -> {msg.Text}"
                + $"\r\nElapsed: {stopWatch.Elapsed}"
                + $"\r\nCount: {Count}"
                + $"\r\nDuration: {duration}"
                + $"\r\nAvg: {duration / Count}"
                + $"\r\nMsg/s: {TimeSpan.FromSeconds(1) / (duration / Count)}\r\n");
        }

        [Queue("reset", "reset")]
        public void Reset(BusContext<Test> context)
        {
            _logger.LogInformation("Reseting Consumer...");
            Count = 0;
            Start = DateTime.Now;
        }
    }
}
