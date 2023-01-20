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

        public void PressButton(int floor, Elevator elevator)
        {
            lock (elevator.Queue)
            {
                elevator.Queue.Add(floor);
            }
        }
    }
}
