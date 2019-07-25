using MicroRabbit.Domain.Core.Commands;
using System;
using System.Linq;
using System.Reflection;

namespace MicroRabbit.Infra
{
    public  class ExtentionHelper
    {

        public static Assembly[] GetAllCommandsAssemblies()
        {
            var test = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => typeof(Command).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                 .Select(x => x.Assembly).ToArray();
            return test;
        }
    }
}
