using Area51;
using Area51.Enum;
using System.Dynamic;


// The elevator can carry a lot of agents. A limit can be set with Semaphore class, but I skipped this part.
// Every agent is on a single thread. 
// Agents can get in or out of the elevator only when the door is open.
// Elevator has simulation of opening and closing the doors. I added a security pass which lets agents out of the elevator.
// Agents go up and down for a period of time. After that a message shows and whereever an agent is calls the elevator.
// For every floor it is called the elevator goes to the ground level (0) so the agents can leave.

// There might be some bugs in calling or leaving the elevator, but rarely.

var rand = new Random();

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken token = cts.Token;

var getInTheElevator = new ManualResetEvent(false);
var callElevator = new ManualResetEvent(false);

Barrier barrier = new Barrier(0);

var button = new Button();
var elevator = new Elevator(callElevator, button, barrier, callElevator, getInTheElevator);
var agentsInBase = new List<Agent>();

var numberOfAgents = 10;


Parallel.For(0, numberOfAgents, i =>
{
    agentsInBase.Add(new Agent("00" + i.ToString(), (SecurityLevelEnum)rand.Next(0, 3), elevator));
});

elevator.Start(token);

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


Thread.Sleep(rand.Next(10000, 35000));
Console.WriteLine($"-----------Agents are going home.-----------");
cts.Cancel();



