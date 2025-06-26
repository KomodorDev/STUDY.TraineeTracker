using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses {
    public class DatabaseProcessingPauseRepository : IProcessingPauseRepository {
        private readonly ApplicationDbContext _context;

        public DatabaseProcessingPauseRepository(ApplicationDbContext context) {
            _context = context;
        }

        public bool Exists(int processingPauseId) {
            return _context.ProcessingPauses.Any(p => p.Id == processingPauseId);
        }

        public bool Exists(ProcessingPause processingPause) {
            return _context.ProcessingPauses.Any(p => p.Id == processingPause.Id);
        }

        public void Create(ProcessingPause processingPause) {
            _context.ProcessingPauses.Add(processingPause);
            _context.SaveChanges();
        }

        public void Update(ProcessingPause processingPause) {
            _context.ProcessingPauses.Update(processingPause);
            _context.SaveChanges();
        }

        public void Delete(ProcessingPause processingPause) {
            _context.ProcessingPauses.Remove(processingPause);
            _context.SaveChanges();
        }

        public IEnumerable<ProcessingPause> GetAllPauses(string traineeId) {
            return _context.ProcessingPauses.Where(p => p.TraineeId.ToString() == traineeId).ToList();
        }
    }
}
