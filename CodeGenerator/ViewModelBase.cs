﻿using System;

namespace CodeGenerator
{
    public class ViewModelBase : ObservableObject
    {
        public RelayCommand LoadedCommand => new RelayCommand(Loaded);
        public virtual void Loaded() => throw new NotImplementedException("not override Loaded method");       
    }
}
