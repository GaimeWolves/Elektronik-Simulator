using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OESim.Circuit.Logic.Components
{
    public interface IEComponent
    {
        T GetAttribute<T>(string name);

        void SetAttribute<T>(string name, T value);
    }
}
