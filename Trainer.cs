using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class TicTacToeTrainer
{
    private NeuralNetwork network;
    private const int BOARD_SIZE = 9;
    private const int TRAINING_EPISODES = 10000000;
    private const float LEARNING_RATE = 0.003f; //default 0.001f
    private const float EPSILON = 0.3f;  // For epsilon-greedy exploration
    private Random random = new Random();

    public TicTacToeTrainer()
    {
        // Updated neural network architecture
        network = new NeuralNetwork(new int[] { BOARD_SIZE, 15, 30, 15, BOARD_SIZE }); //maybe change to smaller
    }

    private class GameState
    {
        public int[] Board { get; set; }
        public List<int> Actions { get; set; }
        public float Value { get; set; }

        public GameState()
        {
            Board = new int[BOARD_SIZE];
            Actions = new List<int>();
            Value = 0;
        }
    }

    public void Train()
    {
        for (int episode = 0; episode < TRAINING_EPISODES; episode++)
        {
            var gameHistory = PlayGame();
            UpdateNetwork(gameHistory);
            Thread.Sleep(0);

            if (episode % 1000 == 0)
            {
                Console.WriteLine($"Episode {episode}: Training in progress...");
            }
        }
    }

    private List<GameState> PlayGame()
    {
        var history = new List<GameState>();
        var board = new int[BOARD_SIZE];
        int currentPlayer = 1;

        while (!IsGameOver(board))
        {
            var state = new GameState
            {
                Board = board.ToArray(),
                Actions = GetValidMoves(board)
            };

            // Choose action using epsilon-greedy policy
            int action;
            if (random.NextDouble() < EPSILON)
            {
                action = state.Actions[random.Next(state.Actions.Count)];
            }
            else
            {
                var prediction = network.Forward(NormalizeBoard(board));
                action = GetBestValidMove(prediction, state.Actions);
            }

            // Make move
            board[action] = currentPlayer;
            state.Value = EvaluatePosition(board, currentPlayer);
            history.Add(state);

            currentPlayer *= -1;  // Switch players (1 to -1 or -1 to 1)
        }

        return history;
    }

    private void UpdateNetwork(List<GameState> history)
    {
        foreach (var state in history)
        {
            var input = NormalizeBoard(state.Board);
            var target = new float[BOARD_SIZE];

            // Set target values based on game outcome
            foreach (var action in state.Actions)
            {
                target[action] = state.Value;
            }

            network.Backward(input, target, LEARNING_RATE);
        }
    }

    private float[] NormalizeBoard(int[] board)
    {
        // Convert board state to neural network input (-1, 0, 1 to float values)
        return board.Select(x => (float)x).ToArray();
    }

    private bool IsGameOver(int[] board)
    {
        // Check for win
        int[][] winLines = {
            new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8}, // Rows
            new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8}, // Columns
            new[] {0,4,8}, new[] {2,4,6}                  // Diagonals
        };

        foreach (var line in winLines)
        {
            if (board[line[0]] != 0 &&
                board[line[0]] == board[line[1]] &&
                board[line[1]] == board[line[2]])
                return true;
        }

        // Check for draw
        return !board.Contains(0);
    }

    private List<int> GetValidMoves(int[] board)
    {
        return board.Select((value, index) => new { value, index })
                   .Where(x => x.value == 0)
                   .Select(x => x.index)
                   .ToList();
    }

    private int GetBestValidMove(float[] predictions, List<int> validMoves)
    {
        return validMoves.OrderByDescending(move => predictions[move]).First();
    }

    private float EvaluatePosition(int[] board, int player)
    {
        if (IsWinningPosition(board, player)) return 1.0f;
        if (IsWinningPosition(board, -player)) return -1.0f;
        if (!board.Contains(0)) return 0.0f;  // Draw
        return 0.0f;
    }

    private bool IsWinningPosition(int[] board, int player)
    {
        int[][] winLines = {
            new[] {0,1,2}, new[] {3,4,5}, new[] {6,7,8},
            new[] {0,3,6}, new[] {1,4,7}, new[] {2,5,8},
            new[] {0,4,8}, new[] {2,4,6}
        };

        return winLines.Any(line =>
            board[line[0]] == player &&
            board[line[1]] == player &&
            board[line[2]] == player);
    }
}

// Simple neural network implementation
public class NeuralNetwork
{
    private readonly int[] layers;
    private float[][] neurons;
    private float[][][] weights;
    private Random random = new Random();

    public NeuralNetwork(int[] layers)
    {
        this.layers = layers;
        InitializeNetwork();
    }

    private void InitializeNetwork()
    {
        neurons = new float[layers.Length][];
        weights = new float[layers.Length - 1][][];

        for (int i = 0; i < layers.Length; i++)
        {
            neurons[i] = new float[layers[i]];

            if (i > 0)
            {
                weights[i - 1] = new float[layers[i - 1]][];
                for (int j = 0; j < layers[i - 1]; j++)
                {
                    weights[i - 1][j] = new float[layers[i]];
                    for (int k = 0; k < layers[i]; k++)
                    {
                        // Xavier/Glorot initialization for better training in deep networks
                        double limit = Math.Sqrt(6.0 / (layers[i - 1] + layers[i]));
                        weights[i - 1][j][k] = (float)((random.NextDouble() * 2 * limit) - limit);
                    }
                }
            }
        }
    }

    public float[] Forward(float[] inputs)
    {
        // Set input layer
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // Forward propagation through all layers
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                float sum = 0;
                for (int k = 0; k < layers[i - 1]; k++)
                {
                    sum += neurons[i - 1][k] * weights[i - 1][k][j];
                }
                // ReLU activation for hidden layers, tanh for output layer
                neurons[i][j] = i == layers.Length - 1 ?
                    (float)Math.Tanh(sum) :
                    Math.Max(0, sum);  // ReLU activation
            }
        }

        return neurons[layers.Length - 1];
    }

    public void Backward(float[] inputs, float[] targets, float learningRate)
    {
        // Forward pass
        var outputs = Forward(inputs);

        // Calculate gradients for each layer
        float[][] deltas = new float[layers.Length][];

        // Initialize deltas arrays
        for (int i = 0; i < layers.Length; i++)
        {
            deltas[i] = new float[layers[i]];
        }

        // Calculate output layer deltas
        for (int i = 0; i < outputs.Length; i++)
        {
            float error = targets[i] - outputs[i];
            deltas[layers.Length - 1][i] = error * (1 - outputs[i] * outputs[i]); // tanh derivative
        }

        // Backpropagate deltas
        for (int i = layers.Length - 2; i >= 0; i--)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                float sum = 0;
                for (int k = 0; k < layers[i + 1]; k++)
                {
                    sum += weights[i][j][k] * deltas[i + 1][k];
                }
                // ReLU derivative
                deltas[i][j] = neurons[i][j] > 0 ? sum : 0;
            }
        }

        // Update weights
        for (int i = 0; i < layers.Length - 1; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                for (int k = 0; k < layers[i + 1]; k++)
                {
                    weights[i][j][k] += learningRate * neurons[i][j] * deltas[i + 1][k];
                }
            }
        }
    }
}