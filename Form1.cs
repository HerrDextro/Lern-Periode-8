using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForms_TTT
{
    public class IOHandler //manages relation between AI and UI
    {
        //nned function for input
        //need way to avoid X on already taken fields
        //need win/loss screen
        //need restart button
        //PLAYER MOVE IS 1 AND AI MOVE IS -1!!!!!!!!

        
        public static void InputGiven(string buttonName)
        {
            int[] inputArray = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            switch (buttonName)
            {
                case ("a1"):
                    inputArray[0] = 1;
                    break;
                case ("a2"):
                    inputArray[1] = 1;
                    break;
                case ("a3"):
                    inputArray[1] = 1;
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

        }
    }
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void a1_Click(object sender, EventArgs e)
        {
            IOHandler.InputGiven("a1");
            Console.WriteLine(inputArray);
        }

        private void a2_Click(object sender, EventArgs e)
        {

        }

        private void a3_Click(object sender, EventArgs e)
        {

        }

        private void b1_Click(object sender, EventArgs e)
        {

        }

        private void b2_Click(object sender, EventArgs e)
        {

        }

        private void b3_Click(object sender, EventArgs e)
        {

        }

        private void c1_Click(object sender, EventArgs e)
        {

        }

        private void c2_Click(object sender, EventArgs e)
        {

        }

        private void c3_Click(object sender, EventArgs e)
        {

        }
    }

    
}
