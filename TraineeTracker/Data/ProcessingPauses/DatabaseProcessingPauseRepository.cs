using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.ProcessingPauses
{
    public class DatabaseProcessingPauseRepository : IProcessingPauseRepository
    {
        private readonly ApplicationDbContext _context;

        public DatabaseProcessingPauseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Exists(ProcessingPause processingPause)
        {
            return _context.ProcessingPauses.Any(p =>
            p.Id == processingPause.Id
            );
        }

        public void Create(ProcessingPause processingPause)
        {
            _context.ProcessingPauses.Add(processingPause);
            _context.SaveChanges();
        }

        public void Update(ProcessingPause processingPause)
        {
            _context.ProcessingPauses.Update(processingPause);
            _context.SaveChanges();
        }

        public void Delete(ProcessingPause processingPause)
        {
            _context.ProcessingPauses.Remove(processingPause);
            _context.SaveChanges();
        }

        public IEnumerable<ProcessingPause> GetAllPauses()
        {
            return _context.ProcessingPauses.AsNoTracking().ToList();
        }
    }
}
