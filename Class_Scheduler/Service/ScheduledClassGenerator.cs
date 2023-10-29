using Class_Scheduler.Common.Models;
using Class_Scheduler.Common.Models.ClassScheduling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Class_Scheduler.Service
{
    public class ScheduledClassGenerator
    {
        private List<Room> rooms = new List<Room>();
        private List<int> roomsId = new List<int>();
        private List<ScheduledClass> scheduledClasses = new List<ScheduledClass>();
        private List<int> globalNASlot = new List<int>();
        private UserRestriction restriction;

        public ScheduledClassGenerator(List<Room> rooms, UserRestriction restriction)
        {
            this.restriction = restriction;
            this.rooms = rooms;
            this.globalNASlot.AddRange(restriction.GlobalNaSlots);

            InitializeRoomIds();
        }

        private void InitializeRoomIds()
        {
            for (int i = 0; i < rooms.Count; i++)
                roomsId.Add(rooms[i].Name);
        }

        public List<ScheduledClass> GenerateScheduledClass(List<CourseClass> courseClasses, ObservableCollection<Slot> slots__)
        {
            try
            {
                // Use Json to deep copy the slots
                string json = JsonConvert.SerializeObject(slots__);
                ObservableCollection<Slot> clonedSlots = JsonConvert.DeserializeObject<ObservableCollection<Slot>>(json);

                // Record the assignment of class for teachers, students and rooms
                Dictionary<int, List<ScheduledClass>> studentAssignments = new Dictionary<int, List<ScheduledClass>>();
                Dictionary<int, List<ScheduledClass>> teacherAssignments = new Dictionary<int, List<ScheduledClass>>();
                Dictionary<int, List<int>> roomsAvailableBySlot = new Dictionary<int, List<int>>();
                Random random = new Random();

                // Initialize the roomsAvailableBySlot
                for (int i = 1; i <= 45; i++)
                {
                    var roomList = new List<int>();
                    roomList.AddRange(roomsId);
                    roomsAvailableBySlot.Add(i, roomList);
                }

                // Get the maximum count that a single slot can has
                int totalCourseCnt = 0;
                foreach(var course in courseClasses)
                    totalCourseCnt += course.CountPerWeek;
                
                int avgCoursePerSlot = (int)Math.Ceiling((double)totalCourseCnt / (45 - restriction.GlobalNaSlots.Count));

                System.Diagnostics.Debug.WriteLine(avgCoursePerSlot);

                for (int j = 0; j < courseClasses.Count; j++)
                {
                    var curCourse = courseClasses[j];

                    HashSet<int> unavailableSlots = new HashSet<int>();
                    // Add the unavailable time slot for teacher
                    for (int w = 0; w < curCourse.Teacher.UnavailableTimeSlotId.Count; w++)
                        unavailableSlots.Add(curCourse.Teacher.UnavailableTimeSlotId[w]);
                    // Add the unavailable time slot for the entire grade
                    for (int w = 0; w < globalNASlot.Count; w++)
                        unavailableSlots.Add(globalNASlot[w]);

                    for (int i = 0; i < curCourse.CountPerWeek; i++)
                    {
                        var timeSlotId = 0;
                        while (true)
                        {
                            bool flag = true;
                            // Randomly select a time slot
                            int randIdx = random.Next(clonedSlots.Count);
                            var timeSlot = clonedSlots[randIdx];
                            timeSlotId = timeSlot.SlotId;

                            System.Diagnostics.Debug.WriteLine("Cease" + unavailableSlots.Count);

                            // Filter the unavailbale time slot, if the condition are not met, then regenerate the time slot
                            foreach (var item in unavailableSlots)
                            {
                                if (item == timeSlot.SlotId)
                                {
                                    flag = false;
                                    break;
                                }
                            }

                            if (!flag)
                                continue;

                            // Filter the time slot that has been taken by same teachers
                            if (teacherAssignments.ContainsKey(curCourse.Teacher.Id))
                            {
                                foreach (var scheduledClass in teacherAssignments[curCourse.Teacher.Id])
                                {
                                    if (timeSlot.SlotId == scheduledClass.SlotId)
                                    {
                                        // Regenerate the time slot
                                        flag = false;
                                        unavailableSlots.Add(timeSlot.SlotId);
                                        break;
                                    }
                                }
                            }

                            if (!flag)
                                continue;

                            //// Filter the time slot in which the students has multiple classes
                            //foreach (var std in curCourse.Students)
                            //{
                            //    if (studentAssignments.ContainsKey(std.Id))
                            //    {
                            //        foreach (var stdClass in studentAssignments[std.Id])
                            //        {
                            //            if (stdClass.SlotId == timeSlot.SlotId)
                            //            {
                            //                flag = false;
                            //                unavailableSlots.Add(timeSlot.SlotId);
                            //                break;
                            //            }
                            //        }
                            //    }

                            //    if (!flag)
                            //        break;
                            //}

                            //if (!flag)
                            //    continue;

                            // Filter the time slot of weekday which a teacher has over maximum number of courses
                            int maximumCnt = 6;
                            if (teacherAssignments.ContainsKey(curCourse.Teacher.Id))
                            {
                                // Weekday of current slot
                                int weekday = GetDayFromSlot(timeSlot.SlotId);
                                int courseCountPerDay = teacherAssignments[curCourse.Teacher.Id].Count(sClass => sClass.WeekDay == weekday);
                                if (courseCountPerDay >= maximumCnt)
                                {
                                    flag = false;
                                    unavailableSlots.Add(timeSlot.SlotId);
                                    // Put the slot id of a whole day into the NaSlot List
                                    var slotsInDay = GetSlotIdsByDay(weekday);
                                    //for (int z = 0; z < slotsInDay.Count; z++)
                                    //    unavailableSlots.Add(slotsInDay[z]);
                                }
                            }

                            if (!flag)
                                continue;

                            // Filter the time slot that has too much course
                            int scheduledCouresSlotCount = scheduledClasses.Count(sClass => sClass.SlotId == timeSlotId);
                            if (scheduledCouresSlotCount >= avgCoursePerSlot)
                            {
                                flag = false;
                                System.Diagnostics.Debug.WriteLine(scheduledCouresSlotCount);
                                // Record the slot (For every courses, this slot would be unavailable)
                                unavailableSlots.Add(timeSlot.SlotId);
                                globalNASlot.Add(timeSlot.SlotId);
                            }

                            if (!flag)
                                continue;

                            // Filter the time slot in which the classroom is unavailable
                            if (roomsAvailableBySlot[timeSlot.SlotId].Count <= 0)
                            {
                                flag = false;
                                unavailableSlots.Add(timeSlot.SlotId);
                                globalNASlot.Add(timeSlot.SlotId);
                            }

                            if (!flag)
                                continue;

                            

                            System.Diagnostics.Debug.WriteLine(unavailableSlots.Count);
                            break;
                        }

                        // Choose a classrooms and remove it from available slot
                        var availableClassrooms = roomsAvailableBySlot[timeSlotId];
                        int randRoomIdx = random.Next(availableClassrooms.Count);
                        var roomChosen = availableClassrooms[randRoomIdx];
                        availableClassrooms.RemoveAt(randRoomIdx);

                        // Create the schedule object
                        var scheduledResult = new ScheduledClass
                        {
                            Course = curCourse,
                            SlotId = timeSlotId,
                            RoomId = roomChosen,
                            WeekDay = GetDayFromSlot(timeSlotId)
                        };

                        scheduledClasses.Add(scheduledResult);

                        // Complement the data for the teacher's assignment
                        var Teacher = curCourse.Teacher;

                        if (!teacherAssignments.ContainsKey(Teacher.Id))
                        {
                            teacherAssignments.Add(Teacher.Id, new List<ScheduledClass>());
                        }
                        teacherAssignments[Teacher.Id].Add(scheduledResult);


                        // Complement the data for the student's assignment
                        foreach (var student in curCourse.Students)
                        {
                            if (!studentAssignments.ContainsKey(student.Id))
                            {
                                studentAssignments.Add(student.Id, new List<ScheduledClass>());
                            }
                            studentAssignments[student.Id].Add(scheduledResult);
                        }       
                    }
                    
                }

                System.Diagnostics.Debug.WriteLine(scheduledClasses.Count);
                return scheduledClasses;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static int GetDayFromSlot(int timeSlot)
        {
            if (timeSlot >= 1 && timeSlot <= 9) return 1;
            if (timeSlot >= 10 && timeSlot <= 18) return 2;
            if (timeSlot >= 19 && timeSlot <= 27) return 3;
            if (timeSlot >= 28 && timeSlot <= 36) return 4;
            if (timeSlot >= 37 && timeSlot <= 45) return 5;
            return 0;
        }

        private List<int> GetSlotIdsByDay(int day)
        {
            return Enumerable.Range((day - 1) * 9 + 1, 9).ToList();
        }
    }
}
