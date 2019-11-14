using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RenditionStudios.Library.NET.Observable
{
    public abstract class ViewModel : Model
    {
        protected View View { get; set; }

        public ViewModel()
        {
            View = new View(this);
        }
    }
}
