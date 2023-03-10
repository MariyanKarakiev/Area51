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
        private Random rand = new Random();
        private Elevator elevator;

        public Agent(string name, SecurityLevelEnum securityLevel, Elevator _elevator)
        {
            Name = name;
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
        public string Name { get; private set; }
        public int CurrentFloor { get; set; }
        public int SelectedFloor { get; set; }
        public bool LeftElevator { get; set; } = true;

        public SecurityLevelEnum SecurityLevel { get; private set; }
        public List<FloorsEnum> FloorsCanAccess { get; private set; }


        public void SelectFloor(Button button)
        {
            var randomFloor = rand.Next(0, 4);

            while (randomFloor == CurrentFloor)
            {
                randomFloor = rand.Next(0, 4);
            }

            button.PressButton(randomFloor, elevator);
            SelectedFloor = randomFloor;
            Console.WriteLine($"Agent {this.Name} pressed {SelectedFloor}-{(FloorsEnum)CurrentFloor} button in the elevator.");
        }

        public void SelectFloor(int floor, Button button)
        {
            button.PressButton(0, elevator);
            SelectedFloor = 0;
            Console.WriteLine($"Agent {this.Name} pressed {SelectedFloor}-{(FloorsEnum)CurrentFloor} button in the elevator.");
        }

        public void CallElevator(Button button)
        {
            button.PressButton(CurrentFloor, elevator);
            Console.WriteLine($"Agent {this.Name} pressed button on {CurrentFloor}-{(FloorsEnum)CurrentFloor} and called the elevator");
        }

        public void Leave(int floor)
        {
            lock (elevator.AgentsOnBoard)
            {
                elevator.AgentsOnBoard.Remove(this);
            }

            CurrentFloor = floor;
            LeftElevator = true;

            Console.WriteLine($"Agent {this.Name} leaves the elevator on floor {CurrentFloor}-{(FloorsEnum)CurrentFloor}.");
        }

        public void GetIn(Button button)
        {
            elevator.AgentsOnBoard.Add(this);
            LeftElevator = false;
            Console.WriteLine($"Agent {this.Name} gets in the elevator on floor {CurrentFloor}-{(FloorsEnum)CurrentFloor}.");
        }
    }
}
