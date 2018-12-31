using CSTPad.Model.Text;
using CSTPad.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace CSTPad.Model
{
    public class CsTextBehavior : Behavior<TextBox>
    {
        private TextBoxProcessorBase TabToIndent { get; } = new TabToIndentProcessor();
        private TextBoxProcessorBase ScriptIndent { get; } = new ScriptIndentProcessor();
        private IntellisencePopupProcessor Intellisence { get; } = new IntellisencePopupProcessor();
        private TextBoxProcessorBase History { get; } = new HistoryProcessor();
        
        public ICommand FocusCommand
        {
            get { return (ICommand)GetValue(FocusCommandProperty); }
            set { SetValue(FocusCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FocusCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusCommandProperty =
            DependencyProperty.Register("FocusCommand", typeof(ICommand), typeof(CsTextBehavior), new PropertyMetadata(null));
        
        public ObservableCollection<string> IntellisenceItems
        {
            get { return (ObservableCollection<string>)GetValue(IntellisenceItemsProperty); }
            set { SetValue(IntellisenceItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IntellisenceItems.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IntellisenceItemsProperty =
            DependencyProperty.Register("IntellisenceItems", typeof(ObservableCollection<string>), typeof(CsTextBehavior), new PropertyMetadata(null));
        
        protected override void OnAttached()
        {
            base.OnAttached();

            Intellisence.Registor(AssociatedObject);
            TabToIndent.Registor(AssociatedObject);
            ScriptIndent.Registor(AssociatedObject);
            History.Registor(AssociatedObject);

            AssociatedObject.KeyDown += (sender, e) =>
            {
                bool isUpdate = false;

                if (null == Intellisence.FocusCommand && null != FocusCommand)
                {
                    Intellisence.FocusCommand = FocusCommand;

                    isUpdate = true;
                }

                if (null == Intellisence.Items && null != IntellisenceItems)
                {
                    Intellisence.Items = IntellisenceItems;

                    isUpdate = true;
                }

                if (isUpdate)
                {
                    Intellisence.RaiseOnKeyDown(sender, e);
                }
            };
        }
    }
}
