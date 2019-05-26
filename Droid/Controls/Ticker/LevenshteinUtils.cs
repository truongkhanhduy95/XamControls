using System;
using System.Collections.Generic;

namespace XamControls.Droid.Controls
{
    public class LevenshteinUtils
    {
        public const int ACTION_SAME = 0;
        public const int ACTION_INSERT = 1;
        public const int ACTION_DELETE = 2;

        public static int[] computeColumnActions(char[] source, char[] target,
            List<char> supportedCharacters)
        {
            int sourceIndex = 0;
            int targetIndex = 0;

            List<int> columnActions = new List<int>();
            while (true)
            {
                // Check for terminating conditions
                bool reachedEndOfSource = sourceIndex == source.Length;
                bool reachedEndOfTarget = targetIndex == target.Length;
                if (reachedEndOfSource && reachedEndOfTarget)
                {
                    break;
                }
                else if (reachedEndOfSource)
                {
                    fillWithActions(columnActions, target.Length - targetIndex, ACTION_INSERT);
                    break;
                }
                else if (reachedEndOfTarget)
                {
                    fillWithActions(columnActions, source.Length - sourceIndex, ACTION_DELETE);
                    break;
                }

                bool containsSourceChar = supportedCharacters.Contains(source[sourceIndex]);
                bool containsTargetChar = supportedCharacters.Contains(target[targetIndex]);

                if (containsSourceChar && containsTargetChar)
                {
                    // We reached a segment that we can perform animations on
                    int sourceEndIndex =
                            findNextUnsupportedChar(source, sourceIndex + 1, supportedCharacters);
                        int targetEndIndex =
                            findNextUnsupportedChar(target, targetIndex + 1, supportedCharacters);

                    appendColumnActionsForSegment(
                            columnActions,
                            source,
                            target,
                            sourceIndex,
                            sourceEndIndex,
                            targetIndex,
                            targetEndIndex
                    );
                    sourceIndex = sourceEndIndex;
                    targetIndex = targetEndIndex;
                }
                else if (containsSourceChar)
                {
                    // We are animating in a target character that isn't supported
                    columnActions.Add(ACTION_INSERT);
                    targetIndex++;
                }
                else if (containsTargetChar)
                {
                    // We are animating out a source character that isn't supported
                    columnActions.Add(ACTION_DELETE);
                    sourceIndex++;
                }
                else
                {
                    // Both characters are not supported, perform default animation to replace
                    columnActions.Add(ACTION_SAME);
                    sourceIndex++;
                    targetIndex++;
                }
            }

            // Concat all of the actions into one array
            int[] result = new int[columnActions.Count];
            for (int i = 0; i < columnActions.Count; i++)
            {
                result[i] = columnActions[i];
            }
            return result;
        }

        private static int findNextUnsupportedChar(char[] chars, int startIndex,
                List<char> supportedCharacters)
        {
            for (int i = startIndex; i < chars.Length; i++)
            {
                if (!supportedCharacters.Contains(chars[i]))
                {
                    return i;
                }
            }
            return chars.Length;
        }

        private static void fillWithActions(List<int> actions, int num, int action)
        {
            for (int i = 0; i < num; i++)
            {
                actions.Add(action);
            }
        }

        private static void appendColumnActionsForSegment(
                List<int> columnActions,
                char[] source,
                char[] target,
                int sourceStart,
                int sourceEnd,
                int targetStart,
                int targetEnd
        )
        {
        int sourceLength = sourceEnd - sourceStart;
             int targetLength = targetEnd - targetStart;
             int resultLength = Math.Max(sourceLength, targetLength);

            if (sourceLength == targetLength)
            {
                // No modifications needed if the length of the strings are the same
                fillWithActions(columnActions, resultLength, ACTION_SAME);
                return;
            }

             int numRows = sourceLength + 1;
             int numCols = targetLength + 1;

            // Compute the Levenshtein matrix
             int[,] matrix = new int[numRows,numCols];

            for (int i = 0; i < numRows; i++)
            {
                matrix[i,0] = i;
            }
            for (int j = 0; j < numCols; j++)
            {
                matrix[0,j] = j;
            }

            int cost;
            for (int i_row = 1; i_row < numRows; i_row++)
            {
                for (int j_col = 1; j_col < numCols; j_col++)
                {
                    cost = source[i_row - 1 + sourceStart] == target[j_col - 1 + targetStart] ? 0 : 1;

                    matrix[i_row,j_col] = min(
                            matrix[i_row - 1, j_col] + 1,
                            matrix[i_row, j_col - 1] + 1,
                            matrix[i_row - 1, j_col - 1] + cost);
                }
            }

            // Reverse trace the matrix to compute the necessary actions
            List<int> resultList = new List<int>(resultLength * 2);
            int row = numRows - 1;
            int col = numCols - 1;
            while (row > 0 || col > 0)
            {
                if (row == 0)
                {
                    // At the top row, can only move left, meaning insert column
                    resultList.Add(ACTION_INSERT);
                    col--;
                }
                else if (col == 0)
                {
                    // At the left column, can only move up, meaning delete column
                    resultList.Add(ACTION_DELETE);
                    row--;
                }
                else
                {
                     int insert = matrix[row, col - 1];
                     int delete = matrix[row - 1, col];
                     int replace = matrix[row - 1, col - 1];

                    if (insert < delete && insert < replace)
                    {
                        resultList.Add(ACTION_INSERT);
                        col--;
                    }
                    else if (delete < replace)
                    {
                        resultList.Add(ACTION_DELETE);
                        row--;
                    }
                    else
                    {
                        resultList.Add(ACTION_SAME);
                        row--;
                        col--;
                    }
                }
            }

            // Reverse the actions to get the correct ordering
             int resultSize = resultList.Count;
            for (int i = resultSize - 1; i >= 0; i--)
            {
                columnActions.Add(resultList[i]);
            }
        }

        private static int min(int first, int second, int third)
        {
            return Math.Min(first, Math.Min(second, third));
        }
    }
}
