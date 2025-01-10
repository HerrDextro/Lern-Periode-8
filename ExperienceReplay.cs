using System;
using System.Collections.Generic;
using System.Linq;

namespace TicTacToe_Neuronics
{
    public class ExperienceBuffer
    {
        private readonly int maxSize;
        private Queue<(double[] state, double[] target)> experiences;

        public ExperienceBuffer(int size = 1000)
        {
            maxSize = size;
            experiences = new Queue<(double[] state, double[] target)>();
        }

        public void Add(double[] state, double[] target)
        {
            if (experiences.Count >= maxSize)
            {
                experiences.Dequeue();
            }
            experiences.Enqueue((state.ToArray(), target.ToArray()));
        }

        public List<(double[] state, double[] target)> Sample(int batchSize)
        {
            var count = Math.Min(batchSize, experiences.Count);
            return experiences.OrderBy(x => Random.Shared.Next()).Take(count).ToList();
        }

        public int Count => experiences.Count;
    }
}