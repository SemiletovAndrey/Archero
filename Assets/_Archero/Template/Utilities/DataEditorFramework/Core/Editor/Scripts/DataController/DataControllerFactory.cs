#if  UNITY_EDITOR
using System;

namespace DataEditorFramework.Core.Editor.Scripts.DataController
{
    public static class DataControllerFactory
    {
        public static IDataController Create(Type type)
        {
            var controllerType = typeof(AssetDataController);
            return (IDataController)Activator.CreateInstance(controllerType, new object[] { type });
        }
    }
}
#endif