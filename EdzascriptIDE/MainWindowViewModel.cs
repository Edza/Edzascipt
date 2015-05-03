using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace EdzascriptIDE
{
    class MainWindowViewModel : BindableBase
    {
        private readonly MainWindowModel _model = new MainWindowModel();

        private string _input = "def x 3" + Environment.NewLine + "out x";
        public string Input
        {
            get { return this._input; }
            set { SetProperty(ref this._input, value); }
        }

        private string _output = "";
        public string Output
        {
            get { return this._output; }
            set { SetProperty(ref this._output, value); }
        }

        public DelegateCommand _compileCommand;
        public DelegateCommand CompileCommand
        {
            get
            {
                return this._compileCommand;
            }
        }

        public DelegateCommand _runCommand;
        public DelegateCommand RunCommand
        {
            get
            {
                return this._runCommand;
            }
        }

        public MainWindowViewModel()
        {
            this._compileCommand = new DelegateCommand(Compile_Executed);
            this._runCommand = new DelegateCommand(Run_Executed);
        }

        private void Compile_Executed()
        {
            this.Output = this._model.Parse(this.Input);
        }

        private void Run_Executed()
        {
            this._model.BuildAndRun(this.Output);
        }
    }
}
