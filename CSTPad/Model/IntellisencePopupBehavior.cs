using CSTPad.Model.Intellisence;
using CSTPad.Model.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using WpfKit.ViewModelKit;

namespace CSTPad.Model
{
    public class IntellisencePopupBehavior : Behavior<Popup>
    {
        public ICommand FocusCommand => new ActionCommand(_ =>
        {
            var grid = AssociatedObject.Child as Grid;
            var listBox = grid.Children[0] as ListBox;

            AssociatedObject.PlacementTarget = TextBox;
            AssociatedObject.PlacementRectangle =
                TextBox.GetRectFromCharacterIndex(TextBox.CaretIndex);

            listBox.Focus();
            if (-1 == listBox.SelectedIndex && 0 < listBox.Items.Count)
            {
                listBox.SelectedIndex = 0;
                listBox.ScrollIntoView(listBox.Items[0]);
                var listBoxItem = (ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem);
                listBoxItem.Focus();
            }
        });
        
        public TextBox TextBox
        {
            get { return (TextBox)GetValue(TextBoxProperty); }
            set { SetValue(TextBoxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TextBox.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.Register("TextBox", typeof(TextBox), typeof(IntellisencePopupBehavior), new PropertyMetadata(null));
        
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Opened += (sender, e) =>
            {
                AssociatedObject.PlacementTarget = TextBox;
                AssociatedObject.PlacementRectangle =
                    TextBox.GetRectFromCharacterIndex(TextBox.CaretIndex);
            };

            AssociatedObject.KeyDown += (sender, e) =>
            {
                var grid = AssociatedObject.Child as Grid;
                var listBox = grid.Children[0] as ListBox;

                if (e.Key == Key.Enter)
                {
                    string key = listBox.SelectedValue.ToString();
                    if (Snipet.SnipetDictionary.ContainsKey(key))
                    {
                        string value = Snipet.SnipetDictionary[key];
                        string word = TextBoxProcessorBase.GetCaretWord(TextBox.Text, TextBox.CaretIndex);

                        int caretIndex = 0;
                        if (value.Contains("{caret}"))
                        {
                            caretIndex = value.IndexOf("{caret}");
                            value = value.Replace("{caret}", string.Empty);
                        }

                        int caret = TextBox.CaretIndex;
                        TextBox.Text = TextBox.Text.Remove(caret - word.Length, word.Length).Insert(caret - word.Length, value);
                        if (0 == caretIndex)
                        {
                            TextBox.CaretIndex = caret - word.Length + value.Length;
                        }
                        else
                        {
                            TextBox.CaretIndex = caret - word.Length + caretIndex;
                        }
                    }
                    else
                    {
                        string text = TextBox.Text;
                        int caret = TextBox.CaretIndex;
                        string word = TextBoxProcessorBase.GetCaretWord(TextBox.Text, TextBox.CaretIndex);

                        TextBox.Text = text.Substring(0, caret - word.Length) + key + text.Substring(caret);
                        TextBox.CaretIndex = caret - word.Length + key.Length;
                    }

                    AssociatedObject.IsOpen = false;
                    TextBox.Focus();
                }
                else if (e.Key == Key.Up || e.Key == Key.Down)
                {

                }
                else
                {
                    AssociatedObject.IsOpen = false;
                    TextBox.Focus();
                }
            };
        }
    }
}
