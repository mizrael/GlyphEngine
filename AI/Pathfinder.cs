using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GlyphEngine.AI
{
    public class Path<TN> : IEnumerable<TN>
    {
        public TN LastStep { get; private set; }
        public Path<TN> PreviousSteps { get; private set; }
        public double TotalCost { get; private set; }

        private Path(TN lastStep, Path<TN> previousSteps, double totalCost)
        {
            LastStep = lastStep;
            PreviousSteps = previousSteps;
            TotalCost = totalCost;
        }
        public Path(TN start) : this(start, null, 0) { }
        public Path<TN> AddStep(TN step, double stepCost)
        {
            return new Path<TN>(step, this, TotalCost + stepCost);
        }

        public IEnumerator<TN> GetEnumerator()
        {
            for (Path<TN> p = this; p != null; p = p.PreviousSteps)
                yield return p.LastStep;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class Pathfinder
    {
        public static Path<TN> FindPath<TN>(TN start,
                                             TN destination,
                                             Func<TN, TN, double> distance,
                                             Func<TN, TN, double> estimate,
                                             Func<TN, IEnumerable<TN>> findNeighbours)
        {
            var closed = new HashSet<TN>();
            var queue = new PriorityQueue<double, Path<TN>>();
            queue.Enqueue(0, new Path<TN>(start));
            while (!queue.IsEmpty)
            {
                var path = queue.Dequeue();
                if (closed.Contains(path.LastStep))
                    continue;
                if (path.LastStep.Equals(destination))
                    return path;
                closed.Add(path.LastStep);

                var neighs = findNeighbours(path.LastStep);
                if (null != neighs && neighs.Any())
                {
                    foreach (TN n in neighs)
                    {
                        double d = distance(path.LastStep, n);
                        var newPath = path.AddStep(n, d);
                        queue.Enqueue(newPath.TotalCost + estimate(n, destination), newPath);
                    }
                }
            }
            return null;
        }
    }

}
