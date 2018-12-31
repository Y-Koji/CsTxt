using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSTPad.Model
{
    public class TextHistory
    {
        private int Position { get; set; } = -1;
        private List<(int Caret, string Text)> History { get; } = new List<(int Caret, string Text)>();

        public void Push(int caret, string text)
        {
            if (-1 == Position)
            {
                History.Add((caret, text));
                Position = 0;
            }
            else
            {
                if (History[Position].Caret == caret && History[Position].Text == text)
                {
                    return;
                }
                else
                {
                    Position++;

                    if (History.Count <= Position)
                    {
                        History.Add((caret, text));
                    }
                    else
                    {
                        History[Position] = (caret, text);
                    }
                }
            }
        }

        public (int Caret, string Text) Pop()
        {
            if (0 == Position)
            {
                return (0, string.Empty);
            }
            else
            {
                Position--;

                return History[Position];
            }
        }

        public (int Caret, string Text) Yank()
        {
            if (Position < History.Count - 1)
            {
                Position++;
            }

            return History[Position];
        }

        public int Count => Position;
    }
}
