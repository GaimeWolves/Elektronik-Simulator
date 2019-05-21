using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OESim.Circuit.Logic.Components
{
    public interface IEComponent
    {
        T GetAttribute<T>(string name);

        void SetAttribute<T>(string name, T value);

        UIElement CreateVisual();
    }
}
