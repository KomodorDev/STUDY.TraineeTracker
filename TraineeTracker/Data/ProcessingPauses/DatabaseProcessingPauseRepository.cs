using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses {
    public class DatabaseProcessingPauseRepository : IProcessingPauseRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseProcessingPauseRepository(ApplicationDbContext context) {
            _context = context;
        }

        public async Task CreateAsync(ProcessingPause processingPause) {
            await _context.ProcessingPauses.AddAsync(processingPause);
            await _context.SaveChangesAsync();
        }

        [Obsolete("Use OverlapsAsync instead.")]
        public async Task<bool> ExistsAsync(ProcessingPause processingPause) {
            return await _context.ProcessingPauses.AnyAsync(p =>
                p.TraineeId == processingPause.TraineeId &&
                p.StartDate == processingPause.StartDate &&
                p.EndDate == processingPause.EndDate
            );
        }

        public async Task<bool> OverlapsAsync(ProcessingPause processingPause) {
            return await _context.ProcessingPauses.AnyAsync(p =>
                p.TraineeId == processingPause.TraineeId &&
                ((p.StartDate == processingPause.StartDate && p.EndDate == processingPause.EndDate) ||  // Same
                (processingPause.StartDate < p.StartDate && p.StartDate < processingPause.EndDate) ||   // StartDate strictly inside new pause
                (processingPause.StartDate < p.EndDate && p.EndDate < processingPause.EndDate) ||       // EndDate strictly inside new Pause
                (p.StartDate == processingPause.StartDate && p.EndDate > processingPause.StartDate) ||  // Enddate lurks inside new Pause
                (p.EndDate == processingPause.EndDate && p.StartDate < processingPause.EndDate))        // Startdate lurks inside new Pause
            );
        }

        public async Task<ProcessingPause?> FindByIdAsync(int processingPauseId) {
            return await _context.ProcessingPauses.FirstOrDefaultAsync(p => p.ProcessingPauseId == processingPauseId);
        }

        public async Task UpdateAsync(ProcessingPause processingPause) {
            _context.ProcessingPauses.Update(processingPause);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ProcessingPause processingPause) {
            _context.ProcessingPauses.Remove(processingPause);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProcessingPause>> GetAllPausesAsync(string traineeId) {
            return await _context.ProcessingPauses.Where(p => p.TraineeId == traineeId).ToListAsync();
        }
    }
}
