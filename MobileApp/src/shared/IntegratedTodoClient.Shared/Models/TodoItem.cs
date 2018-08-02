using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace IntegratedTodoClient.Models
{
    public class TodoItem : INotifyPropertyChanged
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Notes { get; set; }

        public bool Done { get; set; }

        public string UserName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}