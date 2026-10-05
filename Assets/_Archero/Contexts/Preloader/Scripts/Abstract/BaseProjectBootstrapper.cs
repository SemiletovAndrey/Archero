using System;
using Template.Features.ProjectInit;
using Zenject;

namespace DomainOfBattles.Contexts.Preloader
{
    public class BaseProjectBootstrapper : BaseBootstrapper
    {
        private ProjectInstaller _projectInstaller;

        [Inject]
        private void Construct(ProjectInstaller projectInstaller)
        {
            _projectInstaller = projectInstaller;
        }

        private void Awake()
        {
            StartInitialization();
        }

        protected override BaseInstaller GetInstaller()
        {
            return _projectInstaller;
        }
    }
}