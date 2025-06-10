using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jurnal4
{
   class FanLaptop
    {
        private enum State {QUIED, BALANCED, PERFORMANCE, TURBO};
        private State currentState;
        public FanLaptop()
        {
            this.currentState = State.QUIED;
        }


        public void onTurboShorcut()
        {
            if(currentState == State.QUIED)
            {
                this.currentState = State.TURBO;
                Console.WriteLine("Fan Quiet berubah menjadi Turbo");
            }else
            {
                Console.WriteLine("Shorcut hanya bisa di gunakan dari QUIED mode");
            }
        }

        public void offTurboShorcut()
        {
            if (currentState == State.TURBO)
            {
                this.currentState = State.QUIED;
                Console.WriteLine("Fan Turbo berubah menjadi Quied");
            }
            else
            {
                Console.WriteLine("Shorcut hanya bisa di gunakan dari QUIED mode");
            }
        }

        public void modeUp()
        {
            if (currentState == State.QUIED)
            {
                this.currentState = State.BALANCED;
                Console.WriteLine("Fan Quiet berubah menjadi Balanced");
            }
            else if (currentState == State.BALANCED) 
            { 
                this.currentState= State.PERFORMANCE;
                Console.WriteLine("Fan Balance berubah menjadi Performance");
            }else if(currentState == State.PERFORMANCE)
            {
                this.currentState = State.TURBO;
                Console.WriteLine("Fan Performance berubah menjadi Turbo");
            }else if(currentState == State.TURBO)
            {
                Console.WriteLine("Fan Performance sudah mentok");
            }
        }



        public void modeDown()
        {
            if (currentState == State.TURBO)
            {
                this.currentState = State.PERFORMANCE;
                Console.WriteLine("Fan Turbo berubah menjadi Performance");
            }
            else if (currentState == State.PERFORMANCE)
            {
                this.currentState = State.BALANCED;
                Console.WriteLine("Fan Performance berubah menjadi Balanced");
            }
            else if (currentState == State.BALANCED)
            {
                this.currentState = State.QUIED;
                Console.WriteLine("Fan Balanced berubah menjadi Quied");
            }else if(currentState == State.QUIED)
            {
                Console.WriteLine("Fan Quied sudah mentok");
            }
        }
    }
}
