using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jello.Physics
{
    interface IAnchor
    {
        Vector3 AnchorPosition { get; }
        Vector3 AnchorVelocity { get; }
    }
}
