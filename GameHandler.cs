using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms_TTT
{
    internal class GameHandler
    {
        public static int[] boardState = new int[9];

        public static void GameHandlerMethod()
        {

            IsLegalMove(Form1.index, IOHandler.inputArray);

            if(IsWin(IOHandler.inputArray))
            {
                Console.WriteLine("win");
            }
        }
        static public bool IsLegalMove(int index, int[] inputArray)
        {
            if (inputArray[index] != 0)
            {
                return false;
            }
            else
            {
                boardState = inputArray;
                return true;
            }
            
        }

        static public bool IsWin(int[] boardState)
        {
            //first horizontal win
            //then verical win
            //Then diagonal wins
            int hStart = 0;
            int vStart = 0;
            int v = 1; //implement dynamic variable here later for only only check x win

            for (int hRow = 0; hRow < 3; hRow++)
            {
                if (boardState[hStart] == v && boardState[hStart + 1] == v && boardState[hStart + 2] == v) //horizontal wins
                {
                    return true;
                }
                hStart += 3;   
            }
            for (int vRow = 0; vRow < 3; vRow++)
            {
                if (boardState[vStart] == v && boardState[vStart + 3] == v && boardState[vStart + 6] == v) //vertical wins
                {
                    return true;
                }
                vStart++;
            }
            if (boardState[0] == v && boardState[4] == v && boardState[8] == v || boardState[2] == v && boardState[4] == v && boardState[6] == v) //diagonal wins
            {
                return true;
            }

            return false;
        }
        
        
    }
}
