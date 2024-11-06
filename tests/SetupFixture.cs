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

        sealed class TestContextStatusResponder : ProgressStatusResponder
        {
            public override void LoadProgressChanged(LanguageLoadProgressReport value)
            {
                if (value.IsComplete)
                    TestContext.Progress.WriteLine($"Loading Languages Complete");
                else
                    TestContext.Progress.WriteLine($"Loading {value.Spec} ...");
            }

            public override void StatusChanged(ILanguage language, ProgressChangedEventArgs args)
            {
                // e.g.
                // Initializing Python 3.9.10: 6% - Deploying runtime
                int progress = Convert.ToInt32(language.Status.Progress.Value * 100);
                if (progress < 100)
                    TestContext.Progress.WriteLine($"Initializing {language.Id.Name} {language.Id.Version}: {progress,3}% - {language.Status.Progress.Message}");
                else
                    TestContext.Progress.WriteLine($"Initializing {language.Id.Name} {language.Id.Version}: Complete");
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        static void LoadLanguages()
        {
            Registrar.StartScripting();

            RhinoCode.ReportProgressToConsole = true;
            RhinoCode.Languages.WaitStatusComplete(LanguageSpec.Any, new TestContextStatusResponder());
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
