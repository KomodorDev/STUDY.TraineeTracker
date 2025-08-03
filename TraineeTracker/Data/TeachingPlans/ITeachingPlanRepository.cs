using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TeachingPlans {
    
    // ------------------------------------------------------
    /// <summary>
    /// Provides CRUD operations for <see cref="TeachingPlan"/> entities using Entity Framework Core.
    /// </summary>
    /// <remarks>
    /// Code Ownership: Alexandros Blask
    /// </remarks>
    public interface ITeachingPlanRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="TeachingPlan"/> with the specified ID exists.
        /// </summary>
        /// <param name="id">The ID of the teaching plan to check.</param>
        /// <returns>True if the teaching plan exists; otherwise, false.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<bool> ExistsAsync(int id);

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a given <see cref="TeachingPlan"/> entity already exists based on name and last updated date.
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to check.</param>
        /// <returns>True if a matching teaching plan exists; otherwise, false.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<bool> ExistsAsync(TeachingPlan teachingPlan);

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="TeachingPlan"/> to the database.
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to create.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task CreateAsync(TeachingPlan teachingPlan);

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="TeachingPlan"/> in the database.
        /// </summary>
        /// <param name="teachingPlan">The updated teaching plan entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task UpdateAsync(TeachingPlan teachingPlan);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a specified <see cref="TeachingPlan"/> from the database.
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task DeleteAsync(TeachingPlan teachingPlan);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="TeachingPlan"/> by its ID.
        /// </summary>
        /// <param name="id">The ID of the teaching plan.</param>
        /// <returns>The matching teaching plan or null if not found.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<TeachingPlan?> GetTeachingPlanByIdAsync(int id);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="TeachingPlan"/> by ID, including its associated lessons and trainees.
        /// </summary>
        /// <param name="id">The ID of the teaching plan.</param>
        /// <returns>The teaching plan with related entities or null if not found.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<TeachingPlan?> GetTeachingPlanByIdWithLessonsAndTraineesAsync(int id);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="TeachingPlan"/> entities from the database.
        /// </summary>
        /// <returns>A list of all teaching plans.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync();

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="TeachingPlan"/> entities including their lessons and trainees.
        /// </summary>
        /// <returns>A list of teaching plans with related entities.</returns>
        /// <remarks>
        /// Code Ownership: Alexandros Blask
        /// </remarks>
        Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansWithLessonsAndTraineesAsync();
    }
}
