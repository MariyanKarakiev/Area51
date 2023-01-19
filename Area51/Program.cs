using Area51;
using Area51.Enum;
using System.Dynamic;

var rand = new Random();

CancellationTokenSource cts = new CancellationTokenSource();
CancellationToken token = cts.Token;

var getInTheElevator = new ManualResetEvent(false);
var callElevator = new ManualResetEvent(false);
var semaphore = new Semaphore(0, 4);

var button = new Button();
var elevator = new Elevator(callElevator, button);

var agentsInBase = new List<Agent>();

int calledOnFloor = 2;


Parallel.For(0, 2, i =>
{
    agentsInBase.Add(new Agent("00" + i.ToString(), (SecurityLevelEnum)rand.Next(0, 3), elevator));
});


elevator.Start(token, getInTheElevator);

foreach (var agent in agentsInBase)
{
    var agentThr = new Thread(() =>
    {
        for (int i = 0; i < 2; i++)
        {
            if (token.IsCancellationRequested)
            {
            }

            while (!agent.LeftElevator) { Thread.Sleep(0); }

         
            Thread.Sleep(rand.Next(500, 2000));
            agent.CallElevator(button);

            callElevator.WaitOne();
            while (elevator.CurrentFloor != agent.CurrentFloor) { Thread.Sleep(0); }
            callElevator.WaitOne();
            //pass elevator to mthd
            agent.GetIn(button);
            agent.SelectFloor(button);

        }
    });
    agentThr.Start();
}

//var k = Console.ReadKey();
//if (k.Key == ConsoleKey.E)
//{
//   // cts.Cancel();

//}



