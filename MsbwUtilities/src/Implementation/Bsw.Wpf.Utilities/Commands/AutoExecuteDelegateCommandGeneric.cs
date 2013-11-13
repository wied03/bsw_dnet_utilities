// Copyright 2013 BSW Technology Consulting, released under the BSD license - see LICENSING.txt at the top of this repository for details

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace Bsw.Wpf.Utilities.Commands
{
    public class AutoExecuteDelegateCommandGeneric<TParamType> : ICommand
    {
        private readonly DelegateCommand<TParamType> _wrapped;

        public AutoExecuteDelegateCommandGeneric(Action<TParamType> executeMethod)
            : this(executeMethod,
                   p => true)
        {
        }

        public AutoExecuteDelegateCommandGeneric(Action<TParamType> executeMethod,
                                                 Func<TParamType, bool> canExecuteMethod)
        {
            _wrapped = new DelegateCommand<TParamType>(executeMethod,
                                                       canExecuteMethod);
        }

        public bool CanExecute(TParamType parameter)
        {
            return _wrapped.CanExecute(parameter);
        }

        public void Execute(TParamType parameter)
        {
            _wrapped.Execute(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return CanExecute((TParamType) parameter);
        }

        public void Execute(object parameter)
        {
            Execute((TParamType) parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                _wrapped.CanExecuteChanged += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
                _wrapped.CanExecuteChanged -= value;
            }
        }
    }
}