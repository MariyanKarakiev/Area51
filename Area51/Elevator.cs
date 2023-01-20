using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Area51.Enum;

namespace Area51
{
    public class Elevator
    {
        private object obj = new object();
        private Button button;
        ManualResetEvent callElevator;
        public int CurrentFloor { get; private set; }
        public List<int> Queue { get; set; } = new List<int>();
        public List<Agent> AgentsOnBoard { get; set; } = new List<Agent>();

        public Elevator(ManualResetEvent _leaveElevator, Button _button)
        {
            callElevator = _leaveElevator;
            button = _button;
        }

        // HttpClient implements IDisposable by mistake. Use it with static 
        //Mark Troegisen 
        public void Start(CancellationToken token, ManualResetEvent getInTheElevator, Barrier countdownEvent)
        {
            var elevatorThr = new Thread(() =>
            {
                while (true)
                {
                    callElevator.Set();
                    callElevator.Reset();

                    if (token.IsCancellationRequested && Queue.Count == 00 && AgentsOnBoard.Count == 0)
                    {
                        Console.WriteLine("Elevator stoped!");
                        break;
                    }

                    lock (Queue)
                    {
                        // var uniqueElementsQueue = Queue.Distinct().ToList();
                        if (Queue.Count == 0)
                        {
                            continue;
                        }
                    }
                    Console.WriteLine($"Current floor {CurrentFloor}-{(FloorsEnum)CurrentFloor}");

                    var selectedFloor = Queue.First();

                    if (selectedFloor > CurrentFloor)
                    {
                        CurrentFloor++;
                    }

                    else if (selectedFloor < CurrentFloor)
                    {
                        CurrentFloor--;
                    }
                    else
                    {
                        Queue.RemoveAll(c => c == CurrentFloor);
                        OpenDoor(CurrentFloor, getInTheElevator, countdownEvent);
                    }
                    Thread.Sleep(1000);
                }
            });
            elevatorThr.Start();
        }


        public void OpenDoor(int floor, ManualResetEvent getInTheElevator, Barrier countdownEvent)
        {
            Console.WriteLine($"Door opens!");
            getInTheElevator.Set();
            getInTheElevator.Reset();


            if (AgentsOnBoard.Count != 0)
            {
                lock (AgentsOnBoard)
                {
                    var agentsLeaving = AgentsOnBoard.Where(a => a.SelectedFloor == CurrentFloor).ToList();
                    if (agentsLeaving.Count != 0)
                    {
                        Console.WriteLine($"Agent/s {string.Join(", ", agentsLeaving.Select(a => a.Name))} selected this floor - trying to leave.");

                        var agentsWithAccess = agentsLeaving.Where(a => a.FloorsCanAccess.Contains((FloorsEnum)floor)).ToList();
                        var agentsWithoutAccess = agentsLeaving.Where(a => !a.FloorsCanAccess.Contains((FloorsEnum)floor)).ToList();

                        if (agentsWithAccess.Count != 0)
                        {
                            Console.WriteLine($"Door opens for Agent/s {string.Join(", ", agentsWithAccess.Select(a => a.Name))}!");

                            lock (agentsWithAccess)
                            {
                                agentsWithAccess.ForEach(a =>
                                {
                                    lock (obj)
                                    {
                                        a.Leave(CurrentFloor);
                                    }
                                });
                            }
                        }

                        if (agentsWithoutAccess.Count != 0)
                        {
                            Console.WriteLine($"Agent/s {string.Join(", ", agentsWithoutAccess.Select(a => a.Name))} - Access denied!");

                            lock (agentsWithoutAccess)
                            {
                                agentsWithoutAccess.ForEach(a =>
                                {
                                    a.CurrentFloor = CurrentFloor;
                                    a.SelectFloor(button);
                                });
                            }
                        }
                    }
                }
            }

            else
            {
                Console.WriteLine($"Elevator is empty!");
            }

            countdownEvent.AddParticipant();
            countdownEvent.SignalAndWait(1000);

            Console.WriteLine($"Door closes!");
        }
    }
}
