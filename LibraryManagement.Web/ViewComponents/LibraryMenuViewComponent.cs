using LibraryManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.Web.ViewComponents
{
    /// <summary>
    /// ViewComponent to generate the list of libraries dynamically
    /// for the Loans dropdown menu in the navbar.
    /// </summary>
    public class LibraryMenuViewComponent : ViewComponent
    {
        private readonly ILibraryService _libraryService;

        /// <summary>
        /// Constructor injecting the ILibraryService.
        /// </summary>
        /// <param name="libraryService">Service to fetch libraries</param>
        public LibraryMenuViewComponent(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        /// <summary>
        /// Invokes the ViewComponent asynchronously and returns the view with libraries.
        /// </summary>
        /// <returns>IViewComponentResult with list of libraries</returns>
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Fetch all libraries from the service
            var libraries = await _libraryService.GetAllAsync();

            // Return the view with the model
            return View(libraries);
        }
    }
}
