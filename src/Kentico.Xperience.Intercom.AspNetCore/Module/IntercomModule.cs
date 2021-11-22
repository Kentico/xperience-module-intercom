using CMS;
using CMS.DataEngine;

using Kentico.Xperience.Intercom;

[assembly: AssemblyDiscoverable]

[assembly: RegisterModule(typeof(IntercomModule))]

namespace Kentico.Xperience.Intercom
{
    /// <summary>
    /// Represents the Intercom chat integration module.
    /// </summary>
    public class IntercomModule : Module
    {
        public const string NAME = "Kentico.Xperience.Intercom";


        /// <summary>
        /// Initializes a new instance of the <see cref="IntercomModule"/> class.
        /// </summary>
        public IntercomModule() : base(NAME)
        {
        }
    }
}