using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Template.Features.ClassFactory;
using Template.Features.LayerUI;
using Template.Features.UIViewFactory;
using UnityEngine;
using UnityEngine.UI;

namespace Template.Features.Popups
{
    public abstract class PopupFactoryBase : UIViewFactoryBase<BasePopup, PopupEntryBase>, IPopupFactory
    {
        protected PopupFactoryBase(IClassFactory classFactory, IUILayersService layersService)
            : base(classFactory, layersService)
        {
        }

        protected override UILayerType Layer => UILayerType.Popup;

        protected override int GetKey(PopupEntryBase entry)
        {
            return (int)entry.Type;
        }

        protected override bool IsPooled(PopupEntryBase entry)
        {
            return entry.Pooled;
        }

        protected override bool HasSource(PopupEntryBase entry)
        {
            return entry.HasSource;
        }

        protected override string Describe(PopupEntryBase entry)
        {
            return $"PopupType.{entry.Type}";
        }
    }
}