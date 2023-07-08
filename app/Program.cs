// See https://aka.ms/new-console-template for more information
using System;
using System.Collections.Generic;

namespace app
{
class Program
{
    static int[] denominations = { 10, 50, 100 };
    static Dictionary<int, int[]> combinations = new Dictionary<int, int[]>();

    static void Main()
    {
        int[] payouts = { 30, 50, 60, 80, 140, 230, 370, 610, 980 };

        foreach (int payout in payouts)
        {
            Console.WriteLine($"Possible combinations for {payout} EUR:");
            combinations.Clear();
            FindCombinations(payout, 0, new int[denominations.Length]);
            PrintCombinations();
            Console.WriteLine();
        }
    }

    static void FindCombinations(int remainingAmount, int index, int[] currentCount)
    {
        if (remainingAmount == 0)
        {
            int[] countCopy = new int[currentCount.Length];
            Array.Copy(currentCount, countCopy, currentCount.Length);
            combinations.Add(combinations.Count + 1, countCopy);
            return;
        }

        if (index >= denominations.Length || remainingAmount < 0)
            return;

        int denomination = denominations[index];
        int maxCount = remainingAmount / denomination;

        for (int count = maxCount; count >= 0; count--)
        {
            currentCount[index] = count;
            FindCombinations(remainingAmount - (count * denomination), index + 1, currentCount);
        }

        currentCount[index] = 0;
    }

    static void PrintCombinations()
    {
        foreach (var combination in combinations)
        {
            int[] count = combination.Value;
            Console.Write($"{combination.Key}: ");
            for (int i = 0; i < count.Length; i++)
            {
                if (count[i] > 0)
                {
                    Console.Write($"{denominations[i]} EUR x {count[i]}");
                    if (i < count.Length - 1)
                        Console.Write(", ");
                }
            }
            Console.WriteLine();
        }
    }
}

}
