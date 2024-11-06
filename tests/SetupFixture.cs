using System;
using System.Runtime.CompilerServices;

using NUnit.Framework;

using Rhino.Runtime.Code;
using Rhino.Runtime.Code.Languages;

using RhinoCodePlatform.Rhino3D;

namespace WorkflowToolsNUnitTests
{
    [SetUpFixture]
    public sealed class SetupFixture : Rhino.Testing.Fixtures.RhinoSetupFixture
    {
        public override void OneTimeSetup()
        {
            base.OneTimeSetup();
            LoadLanguages();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        static void LoadLanguages()
        {
            Registrar.StartScripting();

            RhinoCode.ReportProgressToConsole = true;
            RhinoCode.Languages.WaitStatusComplete(LanguageSpec.Any);
            foreach (ILanguage language in RhinoCode.Languages)
            {
                if (language.Status.IsErrored)
                {
                    throw new Exception($"Language init error | {RhinoCode.Logger.Text}");
                }
            }
        }
    }
}
