using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Drive.v2;

namespace GDriveClientLib.Interfaces
{
    public interface IGoogleDriveService
    {
        FilesResource Files { get; }
        
        ChildrenResource Children { get; }
    }
}