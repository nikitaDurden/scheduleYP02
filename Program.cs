using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

public class ScheduleItem
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan Time { get; set; }
    public string Subject { get; set; }
    public string Classroom { get; set; }

    public ScheduleItem(DayOfWeek dayOfWeek, TimeSpan time, string subject, string classroom)
    {
        DayOfWeek = dayOfWeek;
        Time = time;
        Subject = subject;
        Classroom = classroom;
    }

    public override string ToString()
    {
        // Переводим день недели на русский язык
        string dayOfWeekRussian = DayOfWeekToString(DayOfWeek);
        return $"{dayOfWeekRussian}: {Time} - {Subject} ({Classroom})";
    }

    private string DayOfWeekToString(DayOfWeek dayOfWeek)
    {
        // Используем CultureInfo для перевода дня недели
        return CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.GetDayName(dayOfWeek);
    }
}

public class Schedule
{
    private List<ScheduleItem> scheduleItems;

    public Schedule()
    {
        scheduleItems = new List<ScheduleItem>();
    }

    public void AddScheduleItem(ScheduleItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        scheduleItems.Add(item);
    }

    public void RemoveScheduleItem(ScheduleItem item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));
        scheduleItems.Remove(item);
    }

    public List<ScheduleItem> GetSchedule()
    {
        return scheduleItems;
    }

    public List<ScheduleItem> GetScheduleForDay(DayOfWeek dayOfWeek)
    {
        // Используем LINQ для фильтрации и сортировки расписания по времени
        return scheduleItems
            .Where(item => item.DayOfWeek == dayOfWeek)
            .OrderBy(item => item.Time)
            .ToList();
    }

    public void PrintSchedule(List<ScheduleItem> schedule)
    {
        if (schedule == null || schedule.Count == 0)
        {
            Console.WriteLine("Расписание пусто.");
            return;
        }

        foreach (var item in schedule)
        {
            Console.WriteLine(item);
        }
    }

    public void SaveToFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Некорректный путь к файлу.");

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var item in scheduleItems)
                {
                    writer.WriteLine($"{item.DayOfWeek},{item.Time},{item.Subject},{item.Classroom}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
        }
    }

    public void LoadFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentException("Некорректный путь к файлу.");

        scheduleItems.Clear();
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(',');
                    if (parts.Length == 4 &&
                        Enum.TryParse(parts[0], out DayOfWeek dayOfWeek) &&
                        TimeSpan.TryParse(parts[1], out TimeSpan time))
                    {
                        string subject = parts[2];
                        string classroom = parts[3];
                        scheduleItems.Add(new ScheduleItem(dayOfWeek, time, subject, classroom));
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Устанавливаем культуру для текущего потока на русскую
        Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

        Schedule schedule = new Schedule();

        // Для тестирования, добавим несколько записей
        //schedule.AddScheduleItem(new ScheduleItem(DayOfWeek.Monday, new TimeSpan(8, 0, 0), "Математика", "Ауд. 101"));
        //schedule.AddScheduleItem(new ScheduleItem(DayOfWeek.Wednesday, new TimeSpan(10, 0, 0), "История", "Ауд. 201"));
        //schedule.AddScheduleItem(new ScheduleItem(DayOfWeek.Friday, new TimeSpan(14, 0, 0), "Физика", "Ауд. 301"));

        // Выводим расписание на экран
        Console.WriteLine("Весь список:");
        schedule.PrintSchedule(schedule.GetSchedule());

        // Сохраняем расписание в файл
        schedule.SaveToFile("D:\\schedule.txt");

        // Очищаем текущее расписание
        schedule = new Schedule();

        // Загружаем расписание из файла
        schedule.LoadFromFile("D:\\schedule2.txt");

        // Выводим расписание на экран после загрузки из файла
        Console.WriteLine("\nРасписание после загрузки из файла:");
        schedule.PrintSchedule(schedule.GetSchedule());

        // Печатаем расписание на определенный день (например, понедельник)
        DayOfWeek dayToPrint = DayOfWeek.Monday;
        List<ScheduleItem> mondaySchedule = schedule.GetScheduleForDay(dayToPrint);
        Console.WriteLine($"\nРасписание на {CultureInfo.GetCultureInfo("ru-RU").DateTimeFormat.GetDayName(dayToPrint)}:");
        schedule.PrintSchedule(mondaySchedule);
    }
}
