using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSTPad.Model.Text
{
    public class HistoryProcessor : TextBoxProcessorBase
    {
        private TextHistory History { get; } = new TextHistory();

        protected override void OnKeyDown(string text, char key, int caret, KeyEventArgs e)
        {
            bool isCtrlKeyDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (!isCtrlKeyDown)
            {
                History.Push(caret, text);
            }

            if (isCtrlKeyDown && e.Key == Key.Z)
            {
                var value = History.Pop();

                AssociatedObject.Text = value.Text;
                AssociatedObject.CaretIndex = value.Caret;

                e.Handled = true;
            }
            else if (isCtrlKeyDown && e.Key == Key.Y)
            {
                var value = History.Yank();

                AssociatedObject.Text = value.Text;
                AssociatedObject.CaretIndex = value.Caret;

                e.Handled = true;
            }
        }

        protected override void OnKeyUp(string text, char key, int caret, KeyEventArgs e)
        {
            bool isCtrlKeyDown = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);

            if (!isCtrlKeyDown)
            {
                History.Push(caret, text);
            }
        }
    }
}
