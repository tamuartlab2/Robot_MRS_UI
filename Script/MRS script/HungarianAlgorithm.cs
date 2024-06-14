using System;
using UnityEngine;

public class HungarianAlgorithm
{
    private readonly double[,] costMatrix;
    private readonly int rows, cols, dim;
    private readonly double[] labelByWorker, labelByJob;
    private readonly int[] minSlackWorkerByJob;
    private readonly double[] minSlackValueByJob;
    private readonly int[] matchJobByWorker, matchWorkerByJob;
    private readonly int[] parentWorkerByCommittedJob;
    private readonly bool[] committedWorkers;

    public HungarianAlgorithm(double[,] costMatrix)
    {
        int Rows = costMatrix.GetLength(0);
        int Cols = costMatrix.GetLength(1);
        if (Rows != Cols)
        {
            costMatrix = MakeSquareMatrix(costMatrix);
        }
        this.costMatrix = costMatrix;
        rows = costMatrix.GetLength(0);
        cols = costMatrix.GetLength(1);
        dim = Math.Max(rows, cols);
        labelByWorker = new double[dim];
        labelByJob = new double[dim];
        minSlackWorkerByJob = new int[dim];
        minSlackValueByJob = new double[dim];
        committedWorkers = new bool[dim];
        parentWorkerByCommittedJob = new int[dim];
        matchJobByWorker = new int[dim];
        matchWorkerByJob = new int[dim];

        // Initialize arrays
        for (int i = 0; i < dim; i++)
        {
            matchJobByWorker[i] = -1;
            matchWorkerByJob[i] = -1;
            parentWorkerByCommittedJob[i] = -1;
        }
    }

    public int[] Run()
    {
        Reduce();
        ComputeInitialFeasibleSolution();
        GreedyMatch();

        int w = FetchUnmatchedWorker();
        while (w < dim)
        {
            InitializePhase(w);
            ExecutePhase();
            w = FetchUnmatchedWorker();
        }

        int[] result = new int[rows];
        for (int i = 0; i < rows; i++)
        {
            if (matchJobByWorker[i] < cols)
            {
                result[i] = matchJobByWorker[i];
            }
            else
            {
                result[i] = -1;
            }
        }
        return result;
    }

    private void Reduce()
    {
        for (int w = 0; w < dim; w++)
        {
            double rowMin = double.MaxValue;
            for (int j = 0; j < dim; j++)
            {
                if (costMatrix[w, j] < rowMin)
                {
                    rowMin = costMatrix[w, j];
                }
            }
            for (int j = 0; j < dim; j++)
            {
                costMatrix[w, j] -= rowMin;
            }
        }

        double[] colMin = new double[dim];
        for (int j = 0; j < dim; j++)
        {
            colMin[j] = double.MaxValue;
        }
        for (int w = 0; w < dim; w++)
        {
            for (int j = 0; j < dim; j++)
            {
                if (costMatrix[w, j] < colMin[j])
                {
                    colMin[j] = costMatrix[w, j];
                }
            }
        }
        for (int w = 0; w < dim; w++)
        {
            for (int j = 0; j < dim; j++)
            {
                costMatrix[w, j] -= colMin[j];
            }
        }
    }

    private void ComputeInitialFeasibleSolution()
    {
        for (int j = 0; j < dim; j++)
        {
            labelByJob[j] = double.MaxValue;
        }
        for (int w = 0; w < dim; w++)
        {
            for (int j = 0; j < dim; j++)
            {
                if (costMatrix[w, j] < labelByJob[j])
                {
                    labelByJob[j] = costMatrix[w, j];
                }
            }
        }
    }

    private void GreedyMatch()
    {
        for (int w = 0; w < dim; w++)
        {
            for (int j = 0; j < dim; j++)
            {
                if (matchJobByWorker[w] == -1 && matchWorkerByJob[j] == -1 &&
                    costMatrix[w, j] - labelByWorker[w] - labelByJob[j] == 0)
                {
                    match(w, j);
                }
            }
        }
    }

    private void InitializePhase(int w)
    {
        for (int i = 0; i < committedWorkers.Length; i++)
        {
            committedWorkers[i] = false;
        }
        for (int i = 0; i < parentWorkerByCommittedJob.Length; i++)
        {
            parentWorkerByCommittedJob[i] = -1;
        }
        committedWorkers[w] = true;

        for (int j = 0; j < dim; j++)
        {
            minSlackValueByJob[j] = costMatrix[w, j] - labelByWorker[w] - labelByJob[j];
            minSlackWorkerByJob[j] = w;
        }
    }

    private void ExecutePhase()
    {
        while (true)
        {
            int minSlackWorker = -1, minSlackJob = -1;
            double minSlackValue = double.MaxValue;
            for (int j = 0; j < dim; j++)
            {
                if (parentWorkerByCommittedJob[j] == -1)
                {
                    if (minSlackValueByJob[j] < minSlackValue)
                    {
                        minSlackValue = minSlackValueByJob[j];
                        minSlackWorker = minSlackWorkerByJob[j];
                        minSlackJob = j;
                    }
                }
            }

            if (minSlackValue > 0)
            {
                UpdateLabeling(minSlackValue);
            }

            parentWorkerByCommittedJob[minSlackJob] = minSlackWorker;
            if (matchWorkerByJob[minSlackJob] == -1)
            {
                int committedJob = minSlackJob;
                int parentWorker = parentWorkerByCommittedJob[committedJob];
                while (true)
                {
                    int temp = matchJobByWorker[parentWorker];
                    match(parentWorker, committedJob);
                    committedJob = temp;
                    if (committedJob == -1)
                    {
                        break;
                    }
                    parentWorker = parentWorkerByCommittedJob[committedJob];
                }
                return;
            }
            else
            {
                int worker = matchWorkerByJob[minSlackJob];
                committedWorkers[worker] = true;
                for (int j = 0; j < dim; j++)
                {
                    if (parentWorkerByCommittedJob[j] == -1)
                    {
                        double slack = costMatrix[worker, j] - labelByWorker[worker] - labelByJob[j];
                        if (minSlackValueByJob[j] > slack)
                        {
                            minSlackValueByJob[j] = slack;
                            minSlackWorkerByJob[j] = worker;
                        }
                    }
                }
            }
        }
    }

    private void UpdateLabeling(double slack)
    {
        for (int w = 0; w < dim; w++)
        {
            if (committedWorkers[w])
            {
                labelByWorker[w] += slack;
            }
        }
        for (int j = 0; j < dim; j++)
        {
            if (parentWorkerByCommittedJob[j] != -1)
            {
                labelByJob[j] -= slack;
            }
            else
            {
                minSlackValueByJob[j] -= slack;
            }
        }
    }

    private int FetchUnmatchedWorker()
    {
        for (int w = 0; w < dim; w++)
        {
            if (matchJobByWorker[w] == -1)
            {
                return w;
            }
        }
        return dim;
    }

    private void match(int w, int j)
    {
        matchJobByWorker[w] = j;
        matchWorkerByJob[j] = w;
    }

    public double[,] MakeSquareMatrix(double[,] originalMatrix)
    {
        int originalRows = originalMatrix.GetLength(0);
        int originalCols = originalMatrix.GetLength(1);

        int squareSize = Mathf.Max(originalRows, originalCols);

        double[,] squareMatrix = new double[squareSize, squareSize];

        for (int i = 0; i < originalRows; i++)
        {
            for (int j = 0; j < originalCols; j++)
            {
                squareMatrix[i, j] = originalMatrix[i, j];
            }
        }

        // Fill additional columns with zeros
        for (int i = originalCols; i < squareSize; i++)
        {
            for (int j = 0; j < squareSize; j++)
            {
                squareMatrix[j, i] = 0.0;
            }
        }

        return squareMatrix;
    }
}
