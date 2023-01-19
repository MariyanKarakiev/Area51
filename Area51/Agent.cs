using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Area51.Enum;

namespace Area51
{
    public class Agent
    {
        private Elevator elevator;
        public string Name { get; private set; }
        public int CurrentFloor { get; set; }
        public int SelectedFloor { get; set; }
        public bool IsLeavingElevator { get; set; }
        public bool AccessDenied { get; set; }
        public SecurityLevelEnum SecurityLevel { get; private set; }
        public List<FloorsEnum> FloorsCanAccess { get; private set; }

        public Agent(string name, SecurityLevelEnum securityLevel, Elevator _elevator)
        {
            Name = name;
            CurrentFloor = 0;
            SecurityLevel = securityLevel;
            elevator = _elevator;

            switch ((int)SecurityLevel)
            {
                case 0:
                    {
                        FloorsCanAccess = new List<FloorsEnum>() { FloorsEnum.G };
                        break;
                    }
                case 1:
                    {
                        FloorsCanAccess = new List<FloorsEnum>() { FloorsEnum.G, FloorsEnum.S };
                        break;
                    }
                case 2:
                    {
                        FloorsCanAccess = new List<FloorsEnum>() { FloorsEnum.G, FloorsEnum.S, FloorsEnum.T1, FloorsEnum.T2 };
                        break;
                    }
            }
        }

        public void Start(CancellationToken token)
        {

        }
        public void SelectFloor(Button button)
        {
           SelectedFloor = button.PressRandomButton(elevator);
            Console.WriteLine($"Agent {this.Name} pressed {SelectedFloor} ");
        }

        public void CallElevator(Button button)
        {
            button.PressButton(CurrentFloor, elevator);
        }

        public void Leave()
        {
            lock (elevator.AgentsOnBoard)
            {
                elevator.AgentsOnBoard.Remove(this);
            }
            Console.WriteLine($"Agent {this.Name} leaves the elevator.");
        }

        public void GetIn(Button button)
        {
            elevator.AgentsOnBoard.Add(this);
            Console.WriteLine($"Agent {this.Name} is in the elevator.");

        }
    }
}
