using Area51;
using Area51.Enum;
using System.Dynamic;

var rand = new Random();
CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken token = cts.Token;
var leaveElevator = new ManualResetEvent(false);
var elevator = new Elevator(leaveElevator);
var semaphore = new Semaphore(0, 4);
var button = new Button();
var agentsInBase = new List<Agent>();

int calledOnFloor = 2;


Parallel.For(0, 4, i =>
{
    agentsInBase.Add(new Agent("00" + i.ToString(), (SecurityLevelEnum)rand.Next(0, 3), elevator));
});


elevator.Start(token);

foreach (var agent in agentsInBase)
{
    var agentThr = new Thread(() =>
    {
        while (!token.IsCancellationRequested) { 
        agent.CallElevator(button);

        if (elevator.CurrentFloor == agent.CurrentFloor)
        {
            leaveElevator.WaitOne();
            agent.GetIn(button);
            agent.SelectFloor(button);
        }

        while (elevator.AgentsOnBoard.Contains(agent))
        {
            leaveElevator.WaitOne();
            if (agent.IsLeavingElevator)
            {
                agent.Leave();
            }
        }
        }
    });
    agentThr.Start();
}


Thread.Sleep(10000);
cts.Cancel();
Console.ReadKey();



