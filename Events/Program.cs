namespace Events
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press any key to start the course");

            Console.ReadKey();

            var course = new Course();

            course.CourseStarted += (sender, e) =>
            {
                Console.WriteLine("Course started");
            };
            course.CourseFinished += (sender, e) =>
            {
                Console.WriteLine("Course finished");
                Console.WriteLine(e.contents);
            };

            course.Start();
        }
    }

    public class Course
    {
        public record CourseFinishedEventArgs(string contents);

        public event EventHandler CourseStarted;

        public event EventHandler<CourseFinishedEventArgs> CourseFinished;

        public void Start()
        {
            // Wenn wir ein Event aufrufen uebergeben wir in der Regel den "sender"
            // welches der this Kontext ist und dann die EventArgs.
            CourseStarted?.Invoke(this, EventArgs.Empty);

            // 2 Sekunden warten
            Thread.Sleep(2000);

            var args = new CourseFinishedEventArgs("Hello World");
            CourseFinished?.Invoke(this, args);
        }
    }
}
