using ChatApp.Domain.Enums;

namespace ChatApp.UI.Pages.Common.ComponentHelpers
{
    public interface ISortComponent<TEnum>
        where TEnum : Enum
    {
        protected bool Asc { get; set; }
        protected bool Sorting { get; set; }
        protected TEnum SortFieldValue { get; set; }
        protected IEnumerable<TEnum> SortingFieldsDropDown { get; set; }
        Task OnSortChangeAsync();
    }
}
