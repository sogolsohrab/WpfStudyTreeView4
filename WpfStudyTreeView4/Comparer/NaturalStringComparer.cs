﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfStudyTreeView4.Comparer
{
    public class NaturalStringComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int indexX = 0;
            int indexY = 0;
            while (true)
            {
                // Handle the case when one string has ended.
                if (indexX == x.Length)
                {
                    return indexY == y.Length ? 0 : -1;
                }
                if (indexY == y.Length)
                {
                    return 1;
                }

                char charX = x[indexX];
                char charY = y[indexY];
                if (char.IsDigit(charX) && char.IsDigit(charY))
                {
                    // Skip leading zeroes in numbers.
                    while (indexX < x.Length && x[indexX] == '0')
                    {
                        indexX++;
                    }
                    while (indexY < y.Length && y[indexY] == '0')
                    {
                        indexY++;
                    }

                    // Find the end of numbers
                    int endNumberX = indexX;
                    int endNumberY = indexY;
                    while (endNumberX < x.Length && char.IsDigit(x[endNumberX]))
                    {
                        endNumberX++;
                    }
                    while (endNumberY < y.Length && char.IsDigit(y[endNumberY]))
                    {
                        endNumberY++;
                    }

                    int digitsLengthX = endNumberX - indexX;
                    int digitsLengthY = endNumberY - indexY;

                    // If the lengths are different, then the longer number is bigger
                    if (digitsLengthX != digitsLengthY)
                    {
                        return digitsLengthX - digitsLengthY;
                    }
                    // Compare numbers digit by digit
                    while (indexX < endNumberX)
                    {
                        if (x[indexX] != y[indexY])
                            return x[indexX] - y[indexY];
                        indexX++;
                        indexY++;
                    }
                }
                else
                {
                    // Plain characters comparison
                    int compareResult = char.ToUpperInvariant(charX).CompareTo(char.ToUpperInvariant(charY));
                    if (compareResult != 0)
                    {
                        return compareResult;
                    }
                    indexX++;
                    indexY++;
                }
            }
        }
    }
}
