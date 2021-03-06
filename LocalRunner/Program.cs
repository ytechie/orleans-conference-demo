﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using GrainInterfaces;
using log4net;

namespace LocalRunner
{
    /// <summary>
    /// Orleans test silo host
    /// </summary>
    public class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static void Main(string[] args)
        {
            Task.Run(() => Run()).Wait();

            Console.WriteLine("Orleans Silo is running.\nPress Enter to terminate...");
            Console.ReadLine();

            //hostDomain.DoCallBack(ShutdownSilo);
        }

        private static async void Run()
        {
            // The Orleans silo environment is initialized in its own app domain in order to more
            // closely emulate the distributed situation, when the client and the server cannot
            // pass data via shared memory.
            AppDomain hostDomain = AppDomain.CreateDomain("OrleansHost", null, new AppDomainSetup
            {
                AppDomainInitializer = InitSilo,
                AppDomainInitializerArguments = new string[0],
            });

            Log4stuff.Appender.Log4stuffAppender.AutoConfigureLogging("jy-orleans");

            Orleans.OrleansClient.Initialize("DevTestClientConfiguration.xml");
            Log.Info("Orleans Silo is running.");

            await IntroducingAGrain();

            await CheckInDemo();
        }

        private async static Task IntroducingAGrain()
        {
            //Easy to work with grains!
            var g1 = Grain1Factory.GetGrain(1);
            var reply = await g1.SayHello();
        }

        private async static Task CheckInDemo()
        {
            var setup = new List<Task>();

            var location1 = LocationFactory.GetGrain(1);
            setup.Add(location1.SetName("Big Data"));

            var location2 = LocationFactory.GetGrain(2);
            setup.Add(location2.SetName("Orleans"));


            var attendee1 = AttendeeFactory.GetGrain(1);

            var name = await attendee1.GetName();
            var location = await attendee1.GetCurrentLocation();
            await attendee1.CheckIn(location1);
            await attendee1.CheckIn(location2);

            location = await attendee1.GetCurrentLocation();
            var eq = location == location2;

            setup.Add(attendee1.SetName("Jason Young"));

            var attendee2 = AttendeeFactory.GetGrain(2);
            setup.Add(attendee2.SetName("Tony Guidici"));

            Task.WaitAll(setup.ToArray());

            await attendee1.CheckIn(location2);
            await attendee2.CheckIn(location1);

            var whereAreYou = await attendee1.GetCurrentLocation();
            Log.InfoFormat("I'm in session '{0}'", await whereAreYou.GetName());
        }

        static void InitSilo(string[] args)
        {
            Log4stuff.Appender.Log4stuffAppender.AutoConfigureLogging("jy-orleans-silo");
            Process.Start("http://log4stuff.com/app/jy-orleans-silo");

            hostWrapper = new OrleansHostWrapper(args);

            if (!hostWrapper.Run())
            {
                Console.Error.WriteLine("Failed to initialize Orleans silo");
            }
        }

        static void ShutdownSilo()
        {
            if (hostWrapper != null)
            {
                hostWrapper.Dispose();
                GC.SuppressFinalize(hostWrapper);
            }
        }

        private static OrleansHostWrapper hostWrapper;
    }
}
