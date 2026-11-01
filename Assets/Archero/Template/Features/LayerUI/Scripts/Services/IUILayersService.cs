namespace Template.Features.LayerUI
{
    public interface IUILayersService
    {
        /// <summary>
        /// Выдает Sort Order выше всех сейчас занятых в этом слое. Каждый выданный порядок нужно вернуть
        /// через ReleaseSortingOrder, когда UI-элемент закрыт.
        /// </summary>
        public int GetNextSortingOrder(UILayerType type);

        /// <summary>Возвращает порядок, выданный GetNextSortingOrder.</summary>
        public void ReleaseSortingOrder(UILayerType type, int order);

    }
}