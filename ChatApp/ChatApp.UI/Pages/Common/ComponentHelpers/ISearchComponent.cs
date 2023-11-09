namespace ChatApp.UI.Pages.Common.ComponentHelpers
{
    public interface ISearchComponent
    {
        protected string? Input { get; set; }
        Task OnSearchChangeAsync(string? value);
    }
}
