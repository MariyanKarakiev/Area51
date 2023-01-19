using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Area51
{
    public class Button
    {
        private Random rnd;

        public Button()
        {
            rnd = new Random();
        }

        public bool IsPressed { get; private set; }
        public int ButtonPressed { get; private set; }


        public void PressButton(int floor, Elevator elevator)
        {
            lock (elevator.Queue)
            {
                elevator.Queue.Add(floor);
            }
        }

        public int PressRandomButton(Elevator elevator)
        {
            var floor = rnd.Next(1, 4);

            lock (elevator.Queue)
            {
                elevator.Queue.Add(floor);
            }
            return floor;
        }
        public void CallElevator(int floor, Elevator elevator)
        {
            lock (elevator.Queue)
            {
                elevator.Queue.Add(floor);
            }
        }
    }
}
