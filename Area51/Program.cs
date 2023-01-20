using Area51;
using Area51.Enum;
using System.Dynamic;

var rand = new Random();

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken token = cts.Token;

var getInTheElevator = new ManualResetEvent(false);
var callElevator = new ManualResetEvent(false);

Barrier barrier = new Barrier(0);

var button = new Button();
var elevator = new Elevator(callElevator, button);
var agentsInBase = new List<Agent>();

Parallel.For(0, 2, i =>
{
    agentsInBase.Add(new Agent("00" + i.ToString(), (SecurityLevelEnum)rand.Next(0, 3), elevator));
});


elevator.Start(token, getInTheElevator, barrier);

foreach (var agent in agentsInBase)
{
    var agentThr = new Thread(() =>
    {
        while (true)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            while (!agent.LeftElevator) { Thread.Sleep(0); }

            callElevator.WaitOne();

            agent.CallElevator(button);


            while (elevator.CurrentFloor != agent.CurrentFloor) { Thread.Sleep(0); }

            barrier.AddParticipant();
            getInTheElevator.WaitOne();

            agent.GetIn(button);
            agent.SelectFloor(button);

            barrier.SignalAndWait();
            Thread.Sleep(rand.Next(50, 400));
        }

        if (agent.LeftElevator)
        {

            callElevator.WaitOne();

            agent.CallElevator(button);

            while (elevator.CurrentFloor != agent.CurrentFloor) { Thread.Sleep(0); }

            barrier.AddParticipant();
            getInTheElevator.WaitOne();

            agent.GetIn(button);
            barrier.SignalAndWait();
        }
        agent.SelectFloor(0, button);
    });
    agentThr.Start();
}


Thread.Sleep(rand.Next(10000, 30000));
cts.Cancel();
Console.WriteLine($"-----------Agents are going home.-----------");



