using System;
using System.Collections.Generic;
using System.Text;
using Rocket.Surgery.Airframe.Forms;
using Splat;
using Xamarin.Forms;

namespace Rocket.Surgery.Airframe.Benchmarks
{
    public class TestApplication : ApplicationBase
    {
        public TestApplication()
        {
        }

        public TestApplication(Action<IMutableDependencyResolver> action)
            : base(action)
        {
        }
    }
}
