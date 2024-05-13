using System;
using System.Collections.Generic;
using System.IO;

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
        return $"{DayOfWeek}: {Time} - {Subject} ({Classroom})";
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
        scheduleItems.Add(item);
    }

    public void RemoveScheduleItem(ScheduleItem item)
    {
        scheduleItems.Remove(item);
    }

    public List<ScheduleItem> GetSchedule()
    {
        return scheduleItems;
    }

    public List<ScheduleItem> GetScheduleForDay(DayOfWeek dayOfWeek)
    {
        List<ScheduleItem> daySchedule = new List<ScheduleItem>();
        foreach (var item in scheduleItems)
        {
            if (item.DayOfWeek == dayOfWeek)
            {
                daySchedule.Add(item);
            }
        }
        daySchedule.Sort((x, y) => x.Time.CompareTo(y.Time));
        return daySchedule;
    }

    public void PrintSchedule(List<ScheduleItem> schedule)
    {
        foreach (var item in schedule)
        {
            Console.WriteLine(item);
        }
    }

    public void SaveToFile(string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var item in scheduleItems)
            {
                writer.WriteLine($"{item.DayOfWeek},{item.Time},{item.Subject},{item.Classroom}");
            }
        }
    }

    public void LoadFromFile(string filePath)
    {
        scheduleItems.Clear();
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 4)
                {
                    DayOfWeek dayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), parts[0]);
                    TimeSpan time = TimeSpan.Parse(parts[1]);
                    string subject = parts[2];
                    string classroom = parts[3];
                    scheduleItems.Add(new ScheduleItem(dayOfWeek, time, subject, classroom));
                }
            }
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
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
        Console.WriteLine($"\nРасписание на {dayToPrint}:");
        schedule.PrintSchedule(mondaySchedule);
    }
}