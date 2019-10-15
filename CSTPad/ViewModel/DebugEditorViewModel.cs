using CsTxt;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Timers;
using System.Windows.Input;
using WpfKit.ViewModelKit;

namespace CSTPad.ViewModel
{
    public class DebugEditorViewModel : INotifyPropertyChanged
    {
        private static CSharpText CSharpTextInstance { get; } = new CSharpText(string.Empty);

        public event PropertyChangedEventHandler PropertyChanged;

        private ActionBlock<string> CSharpConvertionBlock { get; set; }

        public virtual string CsText { get; set; } = string.Empty;

        public virtual string CSharp { get; set; } = string.Empty;

        public virtual string FilePath { get; set; } = string.Empty;

        public virtual string ResultText { get; set; } = string.Empty;

        public virtual DateTime NowTime { get; set; } = DateTime.Now;

        public ICommand Initialize => new ActionCommand(context =>
        {
            CSharpConvertionBlock = new ActionBlock<string>(text =>
            {
                try
                {
                    CSharp = CSharpTextInstance.CompileToCSharpAsync().Result;
                    var result = CSharpTextInstance.RunAsync(text).Result;

                    ResultText = result;
                }
                catch (Exception e)
                {
                    ResultText = e.InnerException.Message + "\r\n" + e.InnerException.StackTrace;
                }
            });

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "NowTime")
                {
                    return;
                }

                if (nameof(CsText) == e.PropertyName)
                {
                    CSharpConvertionBlock.Post(CsText);
                }
            };

            Timer timer = new Timer(1000);
            timer.Elapsed += (sender, e) => NowTime = DateTime.Now;
            timer.Start();

            CSharp = ResultText = "<初期化中...>";
            CSharpConvertionBlock.Post(string.Empty);

            // 引数で渡されたファイルを開く
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2 && File.Exists(args[1]) && Path.GetExtension(args[1]).ToLower() == ".cst")
            {
                CsText = File.ReadAllText(args[1]);
                FilePath = args[1];
            }
        });

        public ICommand OpenCommand => new ActionCommand(parameter =>
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "CSText File(*.cst)|*.cst";

            if (ofd.ShowDialog() ?? false)
            {
                FilePath = ofd.FileName;

                CsText = File.ReadAllText(FilePath);
            }
        });

        public ICommand SaveCommand => new ActionCommand(parameter =>
        {
            if (string.IsNullOrWhiteSpace(FilePath))
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSText File(*.cst)|*.cst";
                if (sfd.ShowDialog() ?? false)
                {
                    FilePath = sfd.FileName;
                }
                else
                {
                    return;
                }
            }

            File.WriteAllText(FilePath, CsText);
        });
    }
}
