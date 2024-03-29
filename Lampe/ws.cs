﻿using Newtonsoft.Json.Linq;
using Phoenix;
using System;

namespace Lampe
{
    public class ws
    {
        internal ws()
        {
            //let socket = new Socket("/ws", {params: {userToken: "123"}})
            //socket.connect()
            var p = new JObject();
            p["user"] = "s";
            var options = new SocketOptions()
            {
                LogCallback = Logger,
                Params = p,
            };
            var socket = new Socket("ws://10.48.211.25:4000/socket", options);
            socket.Connect();

            //let channel = socket.channel("rooms:123", { token: roomToken})
            //channel.on("new_msg", msg => console.log("Got message", msg) )
            var data = new JObject();

            var channel = socket.Channel("rooms:lobby", data);
            channel.On("new_msg", (jo, x) => Console.WriteLine($"new_msg { jo.ToString() }"));

            //channel.join()
            //  .receive("ok", ({messages}) => console.log("catching up", messages) )
            //  .receive("error", ({reason}) => console.log("failed join", reason) )
            //  .receive("timeout", () => console.log("Networking issue. Still waiting...") )
            channel.Join()
              .Receive("ok", (jo) => Console.WriteLine("ok"))
              .Receive("error", (jo) => Console.WriteLine("error"))
              .Receive("timeout", (jo) => Console.WriteLine("timeout"))
              ;

            Channel = channel;

        }

        internal Channel Channel;

        internal void ReadLoop(Channel channel, String msg)
        {
            Console.WriteLine("trying to send");
            var data = new JObject();
            data["body"] = msg;
            channel.Push("new_msg", data).Receive("ok", (jo) => Console.WriteLine($"PUSH new_msg ok { jo.ToString() }"));
         
        }
        private static void Logger(string kind, string msg, JObject data = null)
        {
            Console.WriteLine($"{kind} - {msg}");
        }

    }
}
