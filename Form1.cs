using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinForms_TTT
{
    
    public partial class Form1 : Form
    {
        public static int index; //for IsLegalMove

        public Form1()
        {
            InitializeComponent();

            // open console window for debugging purposes
            AllocConsole();
            Console.WriteLine("Debugging console initialized!");
        }

        // for opening console
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();
        // Close the console when the application closes

        private void YourForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FreeConsole();
        }

        private void a1_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("a1");
;            a1.Text = "X";
            index = 0;
        }

        private void a2_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("a2");
            a2.Text = "X";
            index = 1;
        }

        private void a3_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("a3");
            a3.Text = "X";
            index = 2;
        }

        private void b1_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("b1");
            b1.Text = "X";
            index = 3;
        }

        private void b2_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("b2");
            b2.Text = "X";
            index = 4;
        }

        private void b3_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("b3");
            b3.Text = "X";
            index = 5;
        }

        private void c1_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("c1");
            c1.Text = "X";
            index = 6;
        }

        private void c2_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("c2");
            c2.Text = "X";
            index = 7;
        }

        private void c3_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("c3");
            c3.Text = "X";
            index = 8;
        }
    }

    public class IOHandler //manages relation between AI and UI, and keeps moves legal
    {
        //nned function for input
        //need way to avoid X on already taken fields
        //need win/loss screen
        //need restart button
        //PLAYER MOVE IS 1 AND AI MOVE IS -1!!!!!!!!

        //array here static so it can be updated with fields instead of overwritten by fields
        public static int[] inputArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        GameHandler gameHandler = new GameHandler();



        public static void InputGiven(string buttonName)
        {
            
            //GameHandler gameHandler = new GameHandler();
            

            switch (buttonName)
            {
                case ("a1"):
                    inputArray[0] = 1;
                    break;
                case ("a2"):
                    inputArray[1] = 1;
                    break;
                case ("a3"):
                    inputArray[2] = 1;
                    break;
                case ("b1"):
                    inputArray[3] = 1;
                    break;
                case ("b2"):
                    inputArray[4] = 1;
                    break;
                case ("b3"):
                    inputArray[5] = 1;
                    break;
                case ("c1"):
                    inputArray[6] = 1;
                    break;
                case ("c2"):
                    inputArray[7] = 1;
                    break;
                case ("c3"):
                    inputArray[8] = 1;
                    break;
            }
            Console.WriteLine(string.Join(" ", inputArray));
        }

        
    }

}
