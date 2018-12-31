using CSTPad.Model.Intellisence;
using CSTPad.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CSTPad.Model.Text
{
    public class IntellisencePopupProcessor : TextBoxProcessorBase
    {
        public ICommand FocusCommand { get; set; }

        public ObservableCollection<string> Items { get; set; }

        private IEnumerable<string> GetSnipetKeys(string word)
        {
            if (!string.IsNullOrWhiteSpace(word))
            {
                word = string.Join(".*", Regex.Escape(word).ToCharArray());

                foreach (var snipet in Snipet.SnipetDictionary)
                {
                    if (Regex.IsMatch($"{snipet.Key}", $"^{word}"))
                    {
                        yield return snipet.Key;
                    }
                }
            }
        }

        protected override void OnKeyDown(string text, char key, int caret, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                string word = GetCaretWord(text, caret).ToLower();
                string snipetKey = GetSnipetKeys(word).FirstOrDefault();
                
                if (!string.IsNullOrWhiteSpace(snipetKey))
                {
                    Items.Clear();

                    string value = Snipet.SnipetDictionary[snipetKey];
                    int caretIndex = 0;
                    if (value.Contains("{caret}"))
                    {
                        caretIndex = value.IndexOf("{caret}");
                        value = value.Replace("{caret}", string.Empty);
                    }

                    AssociatedObject.Text =
                        AssociatedObject.Text.Remove(caret - word.Length, word.Length).Insert(caret - word.Length, value);
                    AssociatedObject.CaretIndex = caret - word.Length + caretIndex;

                    e.Handled = true;
                }
            }
        }

        protected override void OnKeyUp(string text, char key, int caret, KeyEventArgs e)
        {
            Items?.Clear();

            var word = GetCaretWord(text, caret).ToLower();
            var snipetKeys = GetSnipetKeys(word);

            foreach (var snipetKey in snipetKeys)
            {
                if (!string.IsNullOrWhiteSpace(snipetKey))
                {
                    Items.Add(snipetKey);
                }
            }
            
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                if (0 < (Items?.Count ?? 0))
                {
                    FocusCommand?.Execute(null);
                }
            }
        }
    }
}
