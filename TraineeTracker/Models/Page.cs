namespace TraineeTracker.Models {

    /// <summary>
    /// Represents a paginated list of items of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>Code Ownership: Simon Hinterreiter (hintsimo)</remarks>
    public class Page<T> {

        /// <summary>
        /// Gets or sets the read-only list of items on the current page.
        /// </summary>
        public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();

        /// <summary>
        /// Gets or sets the current page number (1-based).
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of items across all pages.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets the total number of pages based on <see cref="TotalItems"/> and <see cref="PageSize"/>.
        /// </summary>
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPrevious => PageNumber > 1;

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// <summary>
        public bool HasNext => PageNumber < TotalPages;
    }
}
