using System.Collections.Generic;
using System.Threading.Tasks;
using TraineeTracker.Models.Domain;

namespace TraineeTracker.Data.Lessons
{
    public interface ILessonRepository
    {
        bool Exists(int id);

        bool Exists(Lesson lesson);

        void Create(Lesson lesson);

        void Update(Lesson lesson);

        void Delete(Lesson lesson);

        Lesson GetLessonById(int id);

        IEnumerable<Lesson> GetAllLessons();
    }
}

