using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

namespace Class_Scheduler.Common.Models.ClassScheduling
{
    public class ScheduledClass : BindableBase
    {
        private CourseClass course;
        public CourseClass Course
        {
            get { return course; }
            set { course = value; RaisePropertyChanged(); }
        }
        public int RoomId { get; set; }
        public int WeekDay { get; set; }
        public int SlotId { get; set; }

        public ScheduledClass() { }

        public ScheduledClass(CourseClass course, int weekDay, int slot, int roomId)
        {
            Course = course;
            WeekDay = weekDay;
            SlotId = slot;
            RoomId = roomId;
        }

        //public void Random_Init(List<Room> roomIds)
        //{
        //    Random random = new Random();
        //    RoomId = roomIds.ElementAt(random.Next(0, roomIds.Count - 1)).Id;
        //    WeekDay = random.Next(1, 5);
        //    Slot = random.Next(1, 9);
        //}

        //public ScheduledClass DeepCopy()
        //{
        //    object retval;
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        DataContractSerializer ser = new DataContractSerializer(typeof(ScheduledClass));
        //        ser.WriteObject(ms, this);
        //        ms.Seek(0, SeekOrigin.Begin);
        //        retval = ser.ReadObject(ms);
        //        ms.Close();
        //    }

        //    return (ScheduledClass)retval;
        //}
        //// 在 ClassEvolution 类中添加一个新的辅助方法，用于将 List<ScheduledClass> 转换为整数数组
        //public int[] EncodePopulation(List<List<ScheduledClass>> population, List<Room> rooms)
        //{
        //    int n = population[0].Count;
        //    // 每个个体中在编码数组中占据的总元素数量
        //    int individualSize = n * totalSlotsPerDay * rooms.Count;
        //    int[] encodedPopulation = new int[population.Count * individualSize];

        //    for (int i = 0; i < population.Count; i++)
        //    {
        //        int index = i * individualSize;
        //        foreach (var scheduledClass in population[i])
        //        {
        //            int courseId = scheduledClass.Course.Id;
        //            int slot = scheduledClass.Slot - 1; // 时段从1开始，需要减1转换为0开始
        //            int weekDay = scheduledClass.WeekDay - 1; // 星期从1开始，需要减1转换为0开始
        //            int roomId = scheduledClass.RoomId - 1; // 教室从1开始，需要减1转换为0开始

        //            /*
        //                i: 这是个体的索引，表示您种群中的第几个个体（课程表）。
        //                n: 这是每个个体中的课程数量，即一门课程需要占据多少个数组元素。您在解码和编码课程时使用了这个值。
        //                totalSlotsPerDay: 这是一天中的时间段数量，它表示一天被划分成多少个时间段。
        //                rooms.Count: 这是教室的数量，它表示有多少个教室可供使用。
        //                weekDay: 这是某门课程的安排在一周的哪一天（星期几）。                       
        //                slot: 这是某门课程的安排在一天中的哪个时间段。                        
        //                roomId: 这是某门课程的安排在哪个教室
        //             */
        //            // int index = i * (n * totalSlotsPerDay * rooms.Count) + weekDay * (n * totalSlotsPerDay) + slot * n + roomId;
        //            int classIndex = weekDay * n * totalSlotsPerDay + slot * n + roomId;
        //            encodedPopulation[index + classIndex] = 1; // 设置对应时间段和教室的标志位为1，表示已安排课程
        //        }
        //    }

        //    return encodedPopulation;
        //}

        //// 在 ClassEvolution 类中添加一个新的辅助方法，用于将整数数组转换为 List<ScheduledClass>
        ///*
        //    fix decode计算公式
        // */
        //public List<List<ScheduledClass>> DecodePopulation(int[] encodedPopulation, List<ScheduledClass> schedules, List<Room> rooms)
        //{
        //    int n = 6; // Each individual chooses 6 courses
        //    int individualSize = n * totalSlotsPerDay * rooms.Count;
        //    int individualsCount = encodedPopulation.Length / individualSize;

        //    List<List<ScheduledClass>> population = new List<List<ScheduledClass>>();

        //    for (int i = 0; i < individualsCount; i++)
        //    {
        //        List<ScheduledClass> individual = new List<ScheduledClass>();

        //        int index = i * individualSize;
        //        for (int j = 0; j < individualSize; j++)
        //        {
        //            if (encodedPopulation[index + j] == 1)
        //            {
        //                int weekDay = (j / n / totalSlotsPerDay) + 1;
        //                int slot = ((j / n) % totalSlotsPerDay) + 1;
        //                int roomId = (j % n) + 1;

        //                int courseId = (i * n) + roomId - 1; // Adjust course index
        //                if (courseId < schedules.Count)
        //                {
        //                    CourseClass course = schedules[courseId].Course;
        //                    ScheduledClass scheduledClass = new ScheduledClass(course, weekDay, slot, roomId);
        //                    individual.Add(scheduledClass);
        //                }
        //            }
        //        }
        //        population.Add(individual);
        //    }

        //    return population;
        //}

    }
}