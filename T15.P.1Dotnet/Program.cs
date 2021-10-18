using System;
using System.Threading;
using System.Threading.Tasks;

using Accord.Math;

namespace T15.P._1Dotnet
{
    public static class Program
    {
        /// <summary>
        /// The application entry point. Please note that the Accord.Math NuGet Package was used for this solution
        /// </summary>
        static void Main(string[] args)
        {
            // solve problem
            Minimise();

            //display results
            foreach (var decision in X)
            {
                Console.Write($"{decision}, \t");
            }
            Console.WriteLine("\n");
            Console.WriteLine($"Is feasible {IsFeasiable(X)}, Min: {Min}");

            // compare min with the result from CPLEX
            var CPLEX = Objective(new int[] { 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1, 1 });
            Console.WriteLine($"Expected (From CPLEX): {CPLEX}");
        }


        /// <summary>
        /// Data_cities copy and pasted to avoid an over complicated implementation using the COM interface, given a CPLEX implementation has already been done
        /// </summary>
        private static readonly int[,] Cities =
        {
            { 0, 12, 4, 8, 11, 17, 19, 31, 23, 27, 30, 36, 14, 26, 18 },
            { 12, 0, 8, 4, 1, 5, 31, 19, 27, 23, 20, 24, 26, 14, 22 },
            { 4, 8, 0, 4, 7, 13, 23, 27, 19, 23, 26, 32, 18, 22, 14 },
            { 8, 4, 4, 0, 3, 9, 27, 23, 23, 19, 22, 28, 22, 18, 18 },
            { 11, 1, 7, 3, 0, 6, 30, 20, 26, 22, 19, 25, 25, 15, 21 },
            { 17, 5, 13, 9, 6, 0, 36, 24, 32, 28, 25, 19, 31, 19, 27 },
            { 19, 31, 23, 27, 30, 36, 0, 12, 4, 8, 11, 17, 5, 17, 9 },
            { 31, 19, 27, 23, 20, 24, 12, 0, 8, 4, 1, 5, 17, 5, 13 },
            { 23, 27, 19, 23, 26, 32, 4, 8, 0, 4, 7, 13, 9, 13, 5 },
            { 27, 23, 23, 19, 22, 28, 8, 4, 4, 0, 3, 9, 13, 9, 9 },
            { 30, 20, 26, 22, 19, 25, 11, 1, 7, 3, 0, 6, 16, 6, 12 },
            { 36, 24, 32, 28, 25, 19, 17, 5, 13, 9, 6, 0, 22, 10, 18 },
            { 14, 26, 18, 22, 25, 31, 5, 17, 9, 13, 16, 22, 0, 12, 4 },
            { 26, 14, 22, 18, 15, 19, 17, 5, 13, 9, 6, 10, 12, 0, 8 },
            { 18, 22, 14, 18, 21, 27, 9, 13, 5, 9, 12, 18, 4, 8, 0 }
        };

        /// <summary>
        /// number of cities
        /// </summary>
        private const int N = 15;
        /// <summary>
        /// number of planned fire stations
        /// </summary>
        private const int K = 6;
        /// <summary>
        /// an array to capture whether to choose a city (either 1 or 0)
        /// </summary>
        private static int[] X = new int[N];
        /// <summary>
        /// the final objective value for the optimal solution
        /// </summary>
        private static int Min = int.MaxValue;


        /// <summary>
        /// minmises the distances of the cites and fire stations
        /// </summary>
        private static void Minimise()
        {
            var m = new Mutex();
            int symbols = 2; // Binary variables: either 0 or 1
            int length = N;  // The number of variables; or number 
                             // of columns in the generated table.

            // for every possible set of choices
            Parallel.ForEach(Combinatorics.Sequences(symbols, length), (d, state) =>
            {
                // if the choices is feasible
                if (IsFeasiable(d))
                {
                    // calculate the objective value of the choices
                    int sum = Objective(d);
                    // mutex to elimate a data race for this parallel loop
                    m.WaitOne();
                    // if the objective value is better than the current best, then update the value
                    if (sum < Min)
                    {
                        Min = sum;
                        X = d;
                    }
                    m.ReleaseMutex();
                }
            });
        }


        /// <summary>
        /// a method for determining if the choices are feasible
        /// </summary>
        /// <param name="x">an array of choices of cities</param>
        /// <returns>return T/F depending on whether the choices are feasible</returns>
        private static bool IsFeasiable(int[] x)
        {
            byte count = 0;
            foreach (var decision in x)
                if (decision == 1)
                    count++;
            return count == K;
        }


        /// <summary>
        /// A method for obtaining the objective value
        /// </summary>
        /// <param name="x">an array of choices of cities</param>
        /// <returns>the objective value of the choices</returns>
        private static int Objective(int[] x)
        {
            int sum = 0;
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    sum += Cities[i, j] * x[i];
                }
            }
            return sum;
        }
    }
}
