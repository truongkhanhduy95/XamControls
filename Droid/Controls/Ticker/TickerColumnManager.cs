using System;
using System.Collections.Generic;
using Android.Graphics;

namespace XamControls.Droid.Controls
{
    public class TickerColumnManager
    {
        List<TickerColumn> tickerColumns = new List<TickerColumn>();
        private TickerDrawMetrics metrics;

        private TickerCharacterList[] characterLists;
        private List<char> supportedCharacters;

        public TickerColumnManager(TickerDrawMetrics metrics)
        {
            this.metrics = metrics;
        }

        public void setCharacterLists(char[] chars)
        {
            this.characterLists = new TickerCharacterList[chars.Length];
            for (int i = 0; i < characterLists.Length; i++)
            {
                this.characterLists[i] = new TickerCharacterList(chars[i].ToString());
            }

            this.supportedCharacters = new List<char>();
            for (int i = 0; i < characterLists.Length; i++)
            {
                this.supportedCharacters.AddRange(this.characterLists[i].getSupportedCharacters());
            }
        }

        public TickerCharacterList[] getCharacterLists()
        {
            return characterLists;
        }

        /**
         * Tell the column manager the new target text that it should display.
         */
        public void setText(char[] text)
        {
            if (characterLists == null)
            {
                throw new Exception("Need to call #setCharacterLists first.");
            }

            // First remove any zero-width columns
            for (int i = 0; i < tickerColumns.Count;)
            {
                TickerColumn tickerColumn = tickerColumns[i];
                if (tickerColumn.getCurrentWidth() > 0)
                {
                    i++;
                }
                else
                {
                    tickerColumns.Remove(tickerColumn);
                }
            }

            // Use Levenshtein distance algorithm to figure out how to manipulate the columns
            int[] actions = LevenshteinUtils.computeColumnActions(
                    getCurrentText(), text, supportedCharacters
            );
            int columnIndex = 0;
            int textIndex = 0;
            for (int i = 0; i < actions.Length; i++)
            {
                switch (actions[i])
                {
                    case LevenshteinUtils.ACTION_INSERT:
                        tickerColumns.Insert(columnIndex,
                                new TickerColumn(characterLists, metrics)); break;
                    // Intentional fallthrough
                    case LevenshteinUtils.ACTION_SAME:
                        tickerColumns[columnIndex].setTargetChar(text[textIndex]);
                        columnIndex++;
                        textIndex++;
                        break;
                    case LevenshteinUtils.ACTION_DELETE:
                        tickerColumns[columnIndex].setTargetChar(TickerUtils.EMPTY_CHAR);
                        columnIndex++;
                        break;
                    default:
                        throw new Exception("Unknown action: " + actions[i]);
                }
            }
        }

        public void onAnimationEnd()
        {
            for (int i = 0, size = tickerColumns.Count; i < size; i++)
            {
                TickerColumn column = tickerColumns[i];
                column.onAnimationEnd();
            }
        }

        public void setAnimationProgress(float animationProgress)
        {
            for (int i = 0, size = tickerColumns.Count; i < size; i++)
            {
                TickerColumn column = tickerColumns[i];
                column.setAnimationProgress(animationProgress);
            }
        }

        public float getMinimumRequiredWidth()
        {
            float width = 0f;
            for (int i = 0, size = tickerColumns.Count ; i < size; i++)
            {
                width += tickerColumns[i].getMinimumRequiredWidth();
            }
            return width;
        }

        public float getCurrentWidth()
        {
            float width = 0f;
            for (int i = 0, size = tickerColumns.Count; i < size; i++)
            {
                width += tickerColumns[i].getCurrentWidth();
            }
            return width;
        }

        char[] getCurrentText()
        {
            int size = tickerColumns.Count;
            char[] currentText = new char[size];
            for (int i = 0; i < size; i++)
            {
                currentText[i] = tickerColumns[i].getCurrentChar();
            }
            return currentText;
        }

        /**
         * This method will draw onto the canvas the appropriate UI state of each column dictated
         * by {@param animationProgress}. As a side effect, this method will also translate the canvas
         * accordingly for the draw procedures.
         */
        public void draw(Canvas canvas, Paint textPaint)
        {
            for (int i = 0, size = tickerColumns.Count; i < size; i++)
            {
                TickerColumn column = tickerColumns[i];
                column.draw(canvas, textPaint);
                canvas.Translate(column.getCurrentWidth(), 0f);
            }
        }
    }
}
