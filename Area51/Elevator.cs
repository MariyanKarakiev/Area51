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

        CancellationToken token;

        ManualResetEvent leaveElevator;
        CancellationTokenSource cts = new CancellationTokenSource();

        public List<int> Queue { get; set; } = new List<int>();
        public int currentFloor { get; private set; }


        public List<Agent> AgentsOnBoard { get; set; } = new List<Agent>();

        public Elevator(ManualResetEvent _leaveElevator)
        {
            token = cts.Token;
            leaveElevator = _leaveElevator;
        }
        // HttpClient implements IDisposable by mistake. Use it with static 
        //Mark Troegisen 
        public void Start()
        {
            var elevatorThr = new Thread(() =>
            {
                while (true)
                {
                    if (token.IsCancellationRequested)
                    {
                        break;
                    }

                    if (Queue.Count == 0)
                    {
                        continue;
                    }

                    Console.WriteLine($"Current floor {currentFloor}-{(FloorsEnum)currentFloor}");

                    lock (Queue)
                    {
                        var selectedFloor = Queue.First();

                        if (selectedFloor > currentFloor)
                        {
                            currentFloor++;
                        }

                        else if (selectedFloor < currentFloor)
                        {
                            currentFloor--;
                        }

                        else
                        {
                            TryOpenDoor(currentFloor);
                            lock (Queue)
                            {
                                Queue.Remove(currentFloor);
                            }

                            Thread.Sleep(500);         
                        }
                    }

                    Thread.Sleep(1000);
                }
            });
            elevatorThr.Start();
        }


        public void TryOpenDoor(int floor)
        {
            if (AgentsOnBoard.Count == 0)
            {
                Console.WriteLine($"Elevator is empty!");
                leaveElevator.Set();
                leaveElevator.Reset();
                return;
            }

            var agentsWithAccess = AgentsOnBoard.Where(a => a.FloorsCanAccess.Contains((FloorsEnum)floor) && a.SelectedFloor == currentFloor).ToList();

            if (agentsWithAccess.Count != 0)
            {
                Console.WriteLine($"Door opens for agent {string.Join(", ", agentsWithAccess.Select(a => a.Name))}!");
                agentsWithAccess.ForEach(a => { a.IsLeaving = true; a.CurrentFloor = floor; });
                leaveElevator.Set();
                leaveElevator.Reset();
              
            }

            else
            {
                Console.WriteLine($"Agent/s {string.Join(", ", AgentsOnBoard.Select(a => a.Name))} does not have access to this floor!");
            }
        }


    }
}
