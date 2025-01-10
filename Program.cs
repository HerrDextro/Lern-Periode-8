﻿using System;
using System.Data;
using System.Media;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Linq;
//using TTT_Neuronics_V1;

namespace TicTacToe_Neuronics
{
    class Program
    {
        static void Main()
        {
            Program program = new Program();
            NeuralNetwork neuralNetwork = new NeuralNetwork(9, 15, /*30, 15,*/ 9); //params here are layers and neuron count (9, 15, 10, 9) NOTE: if update hidden layers update training too
            Game game = new Game(neuralNetwork);
            program.PlayMusic();
            var trainer = new TicTacToeTrainer();
            trainer.Train();

            string introFilePath = "C:\\Users\\Neo\\source\\repos\\TicTacToe Neuronics\\TicTacToe Neuronics\\resources\\Intro.txt";
            string introText = File.ReadAllText(introFilePath);
            Console.WriteLine(introText);


            bool running = true;
            while (running)
            {
                Console.WriteLine("1. Play Game");
                Console.WriteLine("2. Train AI");
                Console.WriteLine("3. Save AI Profile");
                Console.WriteLine("4. Load AI Profile");
                Console.WriteLine("5. Exit");

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        game.PlayGame();
                        break;
                    case "2":
                        game.TrainOnStoredMoves();
                        Console.WriteLine("The AI is evolving :)");
                        break;
                    case "3":
                        Console.WriteLine("Enter profile name:");
                        string saveProfile = Console.ReadLine();
                        neuralNetwork.SaveWeights(saveProfile);
                        Console.WriteLine("Profile saved!");
                        break;
                    case "4":
                        Console.WriteLine("Enter profile name:");
                        string loadProfile = Console.ReadLine();
                        neuralNetwork.LoadWeights(loadProfile);
                        Console.WriteLine("Profile loaded!");
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
        }

        private void PlayMusic()
        {
            string audioPath = @"C:\Users\Neo\source\repos\TTT Neuronics V1\TTT Neuronics V1\resources\NeuronicsTheme2.wav"; //AI, no idea what or why
            SoundPlayer player = new SoundPlayer(audioPath);

            Thread musicThread = new Thread(() =>
            {
                player.PlayLooping();
                while (musicRunning)
                {
                    Thread.Sleep(100); // Small delay to prevent busy-waiting
                }
                player.Stop();
            });

            musicThread.IsBackground = true;
            musicThread.Start();
        }
        volatile bool musicRunning = true;


        //for debug only
        static public void ShowArray(double[] array) //DEBUGGING BEDUGGING GEBUNING BRUHHHH
        {
            for (int i = 0; i < array.Length; i++)
            {
                Console.Write(array[i] + " "); //space for spacing between tokens
            }
        }


    }
    public class Game
    {
        private double[] boardState;
        private NeuralNetwork network;
        private bool isPlayerTurn;
        private ExperienceBuffer experienceBuffer = new ExperienceBuffer(); // Add this line

        public Game(NeuralNetwork network)
        {
            this.network = network;
            boardState = new double[9];
            isPlayerTurn = true; // Player starts with X
        }

        public void PlayGame()
        {
            bool gameEnded = false;

            while (!gameEnded)
            {
                PrintBoard();

                if (isPlayerTurn)
                {
                    MakePlayerMove();
                }
                else
                {
                    MakeAIMove();
                }

                if (CheckWin(out string winner))
                {
                    PrintBoard();
                    Console.WriteLine($"{winner} wins!");
                    gameEnded = true;
                }
                else if (IsBoardFull())
                {
                    PrintBoard();
                    Console.WriteLine("It's a draw!");
                    gameEnded = true;
                }

                isPlayerTurn = !isPlayerTurn;
            }

            Console.WriteLine("Play again? (y/n)");
            if (Console.ReadLine().ToLower() == "y")
            {
                ResetGame();
                PlayGame();
            }
        }

        private void PrintBoard()
        {
            Console.Clear();
            Console.WriteLine("Current Board:");
            Console.WriteLine("-------------");
            for (int i = 0; i < 9; i += 3)
            {
                Console.Write("| ");
                for (int j = 0; j < 3; j++)
                {
                    char symbol = boardState[i + j] switch
                    {
                        1 => 'X',
                        -1 => 'O',
                        _ => (i + j + 1).ToString()[0]
                    };
                    Console.Write(symbol + " | ");
                }
                Console.WriteLine("\n-------------");
            }
        }

        private void MakePlayerMove()
        {
            bool validMove = false;
            while (!validMove)
            {
                Console.WriteLine("Enter position (1-9): ");
                if (int.TryParse(Console.ReadLine(), out int position) && position >= 1 && position <= 9)
                {
                    position--; // Convert to 0-based index
                    if (boardState[position] == 0)
                    {
                        boardState[position] = 1; // X for player
                        validMove = true;

                        // Store this move for training
                        SaveMoveForTraining(boardState.ToArray(), position);
                    }
                    else
                    {
                        Console.WriteLine("Position already taken!");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input! Please enter a number between 1 and 9.");
                }
            }
        }

        private void MakeAIMove()
        {
            // Get AI's move prediction
            network.FeedForward(boardState);
            var outputs = network.OutputLayer.Select(n => n.Output).ToArray();

            // Filter out invalid moves more aggressively
            for (int i = 0; i < outputs.Length; i++)
            {
                if (boardState[i] != 0)
                {
                    outputs[i] = double.MinValue;
                }
            }

            // Add validation
            int movePosition = Array.IndexOf(outputs, outputs.Max());
            if (boardState[movePosition] != 0)
            {
                // If somehow still got an invalid move, find first empty space
                movePosition = Array.IndexOf(boardState, 0.0);
                Console.WriteLine("AI attempted illegal move, falling back to first available space");
            }

            boardState[movePosition] = -1; // O for AI
        }

        private bool CheckWin(out string winner)
        {
            // Check rows, columns and diagonals
            int[][] lines = new int[][]
            {
            new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8}, // rows
            new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8}, // columns
            new int[] {0, 4, 8}, new int[] {2, 4, 6} // diagonals
            };

            foreach (var line in lines)
            {
                double sum = boardState[line[0]] + boardState[line[1]] + boardState[line[2]];
                if (sum == 3) { winner = "Player"; return true; }
                if (sum == -3) { winner = "AI"; return true; }
            }

            winner = null;
            return false;
        }

        private bool IsBoardFull()
        {
            return !boardState.Contains(0);
        }

        private void ResetGame()
        {
            boardState = new double[9];
            isPlayerTurn = true;
        }

        // Training-related methods
        private List<(double[] state, int move)> trainingData = new List<(double[] state, int move)>();

        private void SaveMoveForTraining(double[] state, int move)
        {
            // Create a copy of the state before the move was made
            double[] previousState = state.ToArray();
            previousState[move] = 0; // Reset the move position to empty

            double[] targetOutput = new double[9];
            targetOutput[move] = 1;

            // Store the state BEFORE the move was made
            experienceBuffer.Add(previousState, targetOutput);
            trainingData.Add((previousState.ToArray(), move));
        }


        public void TrainOnStoredMoves(int epochs = 100)
        {
            for (int epoch = 0; epoch < epochs; epoch++)
            {
                // Train on recent experiences
                var batch = experienceBuffer.Sample(Math.Min(10, experienceBuffer.Count));
                foreach (var (state, target) in batch)
                {
                    network.Train(state, target);
                }

                // Mix in some training on historical successful moves
                foreach (var (state, move) in trainingData.OrderBy(x => Random.Shared.Next()).Take(5))
                {
                    double[] targetOutput = new double[9];
                    targetOutput[move] = 1;
                    network.Train(state, targetOutput);
                }
            }
        }
    }
}
