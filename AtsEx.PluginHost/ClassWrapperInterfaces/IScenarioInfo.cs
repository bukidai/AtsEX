using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automatic9045.AtsEx.PluginHost.ClassWrappers
{
    public interface IScenarioInfo : IClassWrapper
    {
        string Path { get; }
        string FileName { get; }
        string DirectoryName { get; }
        string Title { get; }
        string ImagePath { get; }
        string Author { get; }
        string Comment { get; }
        IRandomFileList RouteFiles { get; }
        IRandomFileList VehicleFiles { get; }
        string RouteTitle { get; }
        string VehicleTitle { get; }
        List<ILoadError> ScenarioFileLoadErrors { get; }
    }
}
