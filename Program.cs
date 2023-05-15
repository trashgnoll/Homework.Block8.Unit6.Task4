using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;

namespace FinalTask
{
    internal class Program
    {

        public static string InputString(string prompt, bool allowEmptyInput = false)
        {
            Console.Write(prompt);
            string? result;
            if (allowEmptyInput)
                result = Console.ReadLine();
            else
                while ((result = Console.ReadLine()) is null || result == string.Empty)
                    Console.Write(prompt);
            return (result is not null ? result : string.Empty);
        }
        public static void ClearDir(string path)
        {
            DirectoryInfo folder = new(path);
            foreach (FileInfo fi in folder.GetFiles())
                File.Delete(fi.FullName);
            foreach (DirectoryInfo di in folder.GetDirectories())
                ClearDir(di.FullName);
        }
        public static void CheckDirectoryAndDeleteIfNeededAndThenCreateEmpty(string path)
        {
            DirectoryInfo di = new(path);
            if (di.Exists)
                ClearDir(di.FullName);
            Directory.CreateDirectory(path);
        }
        static void Main(string[] args)
        {
            FileInfo fi = new("Students.dat");
            if (!fi.Exists)
            {
                string path = (args.Length == 0 ?
                    InputString("Enter path to \"Students.dat\" file:") : args[0]);
                fi = new(path);
                if (!fi.Exists)
                {
                    Console.WriteLine("File not found.");
                    return;
                }
            }
            Student[] students;
            BinaryFormatter formatter = new BinaryFormatter();
            using (var fs = new FileStream(fi.FullName, FileMode.Open))
                students = (Student[])formatter.Deserialize(fs);
            Dictionary<string, List<Student>> groups = new();
            foreach(Student student in students)
            {
                if (!groups.ContainsKey(student.Group))
                    groups.Add(student.Group, new());
                groups[student.Group].Add(student);
            }
            string dirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                                           "Students");
            try
            {
                CheckDirectoryAndDeleteIfNeededAndThenCreateEmpty(dirPath);
            }
            catch( Exception e )
            {
                Console.WriteLine("Can't create or clear directory. Error: " + e.Message);
            }

            if (Directory.Exists(dirPath))
                Directory.Delete(dirPath);
            Directory.CreateDirectory(dirPath);
            foreach(List<Student> groupList in groups.Values.ToArray())
            {
                Student[] group = groupList.ToArray();
                foreach (Student student in group)
                {
                    using (StreamWriter sw = File.CreateText(Path.Combine(dirPath, student.Group + ".txt")))
                        sw.WriteLine(student.Name + ", " + student.DateOfBirth.ToString());
                }
            }
            Console.WriteLine($"Done, {0} students parsed ", students.Length.ToString());
        }
    }
}
