using System.Collections;

namespace DataEditorFramework.Core.Editor.Scripts.DataController
{
    public interface IDataController
    {
        public IList Items { get; }

        public void AddNew(string preferredDirectory = null);
        public void Remove(object item);

        public void ImportJson();
        public void ExportJson();

        public void MarkDirty(object item);
        public void SaveChanges();
    }
}
