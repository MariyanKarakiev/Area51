﻿using Area51;
using Area51.Enum;
using System.Dynamic;

var rand = new Random();

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken token = cts.Token;

var getInTheElevator = new ManualResetEvent(false);
var callElevator = new ManualResetEvent(false);
var semaphore = new Semaphore(0, 4);
CountdownEvent cde = new CountdownEvent(1);
Barrier bar = new Barrier(0);
var button = new Button();
var elevator = new Elevator(callElevator, button);

var agentsInBase = new List<Agent>();

int calledOnFloor = 2;


Parallel.For(0, 2, i =>
{
    agentsInBase.Add(new Agent("00" + i.ToString(), (SecurityLevelEnum)rand.Next(0, 3), elevator));
});


elevator.Start(token, getInTheElevator, bar);

foreach (var agent in agentsInBase)
{
    var agentThr = new Thread(() =>
    {
        for (int i = 0; i < 1; i++)
        {
            if (token.IsCancellationRequested)
            {
            }
            Thread.Sleep(rand.Next(500, 2600));
            //while (!agent.LeftElevator) { Thread.Sleep(0); }           

            callElevator.WaitOne();

            agent.CallElevator(button);

            //pass elevator to mthd
            while ( elevator.CurrentFloor != agent.CurrentFloor) { Thread.Sleep(10); }
            // cde.AddCount(1);
            bar.AddParticipant();
            getInTheElevator.WaitOne();
            agent.GetIn(button);
            agent.SelectFloor(button);
            bar.SignalAndWait();

            //cde.Signal();
        }
    });
    agentThr.Start();
}

//var k = Console.ReadKey();
//if (k.Key == ConsoleKey.E)
//{
//   // cts.Cancel();

//}



