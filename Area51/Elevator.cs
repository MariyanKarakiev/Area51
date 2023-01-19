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
        ManualResetEvent leaveElevator;
        public int CurrentFloor { get; private set; }
        public List<int> Queue { get; set; } = new List<int>();
        public List<Agent> AgentsOnBoard { get; set; } = new List<Agent>();

        public Elevator(ManualResetEvent _leaveElevator)
        {
            leaveElevator = _leaveElevator;
        }

        // HttpClient implements IDisposable by mistake. Use it with static 
        //Mark Troegisen 
        public void Start(CancellationToken token)
        {
            var elevatorThr = new Thread(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    if (Queue.Count != 0)
                    {
                        Console.WriteLine($"Current floor {CurrentFloor}-{(FloorsEnum)CurrentFloor}");
                       
                        lock (Queue)
                        {
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
                                if (AgentsOnBoard.Count != 0)
                                {
                                    TryOpenDoor(CurrentFloor);
                                   
                                    lock (Queue)
                                    {
                                        Queue.RemoveAll(a => a == CurrentFloor);
                                    }

                                }
                                else
                                {
                                    Console.WriteLine($"Elevator is empty!");
                                   
                                    leaveElevator.Set();
                                    Thread.Sleep(1000);
                                    leaveElevator.Reset();
                                }
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
            });
            elevatorThr.Start();
        }


        public void TryOpenDoor(int floor)
        {
            var agentsLeaving = AgentsOnBoard.Where(a => a.SelectedFloor == CurrentFloor).ToList();

            if (agentsLeaving.Count != 0)
            {
                Console.WriteLine($"Agent/s {string.Join(", ", agentsLeaving.Select(a => a.Name))} selected this floor - they want to leave.");

                var agentsWithAccess = agentsLeaving.Where(a => a.FloorsCanAccess.Contains((FloorsEnum)floor)).ToList();
                var agentsWithoutAccess = agentsLeaving.Where(a => !a.FloorsCanAccess.Contains((FloorsEnum)floor)).ToList();

                if (agentsWithoutAccess.Count != 0)
                {
                    Console.WriteLine($"Agent/s {string.Join(", ", agentsWithoutAccess.Select(a => a.Name))} - Access denied!");
                }

                if (agentsWithAccess.Count != 0)
                {
                    Console.WriteLine($"Door opens for Agent/s {string.Join(", ", agentsWithAccess.Select(a => a.Name))}!");
                    lock (agentsWithAccess)
                    {

                        agentsWithAccess.ForEach(a => { a.IsLeavingElevator = true; a.CurrentFloor = floor; });
                    }
                    leaveElevator.Set();
                    leaveElevator.Reset();
                }
            }
        }
    }
}
