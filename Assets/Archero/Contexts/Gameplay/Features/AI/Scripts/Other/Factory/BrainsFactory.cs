using System;
using System.Collections.Generic;
using System.Linq;
using Archero.Template.Scripts.Utils;
using Contexts.Scripts.Utilities.Reactive;
using Gameplay.Features.AI.Scripts;
using Gameplay.Features.EntityCore.Features.Attack;
using Gameplay.Features.EntityCore.Features.Energy;
using Gameplay.Features.MovementFeature;
using Gameplay.Features.Sensers;
using Persistance.Features.EntityCore;
using Template.Features.NewInputFeature;
using Template.Scripts.Utils;
using UnityEngine;

namespace Gameplay.Features.AI
{
    public class BrainsFactory : IBrainsFactory
    {
        private readonly IAiBrainContext _aiBrainContext;
        private readonly Dictionary<EntityType, IBrainBuilder> _builders;

        // Zenject умеет инжектить List<T>, а IEnumerable<T> нет
        public BrainsFactory(IAiBrainContext aiBrainContext, List<IBrainBuilder> builders)
        {
            _aiBrainContext = aiBrainContext;
            _builders = builders.ToDictionary(x => x.Type);
        }

        public StateMachineBrain Create(Entity entity, EntityType type)
        {
            if (!_builders.TryGetValue(type, out IBrainBuilder builder))
                throw new InvalidOperationException($"No brain builder for {type}");

            StateMachineBrain brain = new StateMachineBrain(builder.Build(entity));
            _aiBrainContext.SetFor(entity, brain);

            return brain;
        }
    }
}