using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.TeachingPlans {
    
    // ------------------------------------------------------
    /// <summary>
    /// Provides CRUD operations for <see cref="TeachingPlan"/> entities using Entity Framework Core.
    /// Code Ownership: Alexandros Blask
    /// </summary>
    public interface ITeachingPlanRepository {

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a <see cref="TeachingPlan"/> with the specified ID exists.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="id">The ID of the teaching plan to check.</param>
        /// <returns>True if the teaching plan exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(int id);

        // ------------------------------------------------------
        /// <summary>
        /// Checks whether a given <see cref="TeachingPlan"/> entity already exists based on name and last updated date.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to check.</param>
        /// <returns>True if a matching teaching plan exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(TeachingPlan teachingPlan);

        // ------------------------------------------------------
        /// <summary>
        /// Adds a new <see cref="TeachingPlan"/> to the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to create.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        Task CreateAsync(TeachingPlan teachingPlan);

        // ------------------------------------------------------
        /// <summary>
        /// Updates an existing <see cref="TeachingPlan"/> in the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="teachingPlan">The updated teaching plan entity.</param>
        /// <returns>A Task representing the asynchronous update operation.</returns>
        Task UpdateAsync(TeachingPlan teachingPlan);

        // ------------------------------------------------------
        /// <summary>
        /// Deletes a specified <see cref="TeachingPlan"/> from the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="teachingPlan">The teaching plan to delete.</param>
        /// <returns>A Task representing the asynchronous delete operation.</returns>
        Task DeleteAsync(TeachingPlan teachingPlan);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="TeachingPlan"/> by its ID.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="id">The ID of the teaching plan.</param>
        /// <returns>The matching teaching plan or null if not found.</returns>
        Task<TeachingPlan?> GetTeachingPlanByIdAsync(int id);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves a <see cref="TeachingPlan"/> by ID, including its associated lessons and trainees.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <param name="id">The ID of the teaching plan.</param>
        /// <returns>The teaching plan with related entities or null if not found.</returns>
        Task<TeachingPlan?> GetTeachingPlanByIdWithLessonsAndTraineesAsync(int id);

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="TeachingPlan"/> entities from the database.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <returns>A list of all teaching plans.</returns>
        Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansAsync();

        // ------------------------------------------------------
        /// <summary>
        /// Retrieves all <see cref="TeachingPlan"/> entities including their lessons and trainees.
        /// Code Ownership: Alexandros Blask
        /// </summary>
        /// <returns>A list of teaching plans with related entities.</returns>
        Task<IEnumerable<TeachingPlan>> GetAllTeachingPlansWithLessonsAndTraineesAsync();
    }
}
