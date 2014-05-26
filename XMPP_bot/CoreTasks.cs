using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using XmppBot.Common;

using XMPP_bot.Tasks;

namespace XmppBot
{
    public static class CoreTasks
    {
        private static NickNameProvider NickNameProvider;

        private static List<SimpleTaskBase> Tasks;


        static CoreTasks()
        {
            var fileWrapper = new BasicFileWrapper(Path.Combine(Environment.CurrentDirectory, "Data"), "NickNames.xml");
            NickNameProvider = new NickNameProvider(fileWrapper);

            Tasks = new List<SimpleTaskBase>();

            Tasks.Add(new GetNicknameTask(NickNameProvider));

            Tasks.Add(new SetNicknameTask(NickNameProvider));

            Tasks.Add(new SmackTask());

            Tasks.Add(new HighFiveTask());

            Tasks.Add(new HugTask());

            Tasks.Add(new GiveTask());

            Tasks.Add(new GetStorageLocationTask());

            Tasks.Add(new CloseTask());
        }

        public static bool ContainsCommand(ParsedLine taskInfo)
        {
            return Tasks.Any(t => t.IsMatch(taskInfo));
        }

        public static string ExecuteCommand(ParsedLine taskInfo)
        {
            var task = Tasks.FirstOrDefault(t => t.IsMatch(taskInfo));

            if (task == null) {
                return "Command not found.";
            }

            return task.Execute(taskInfo);
        }

        public static string GetNickName(string userFullName)
        {
            return NickNameProvider.GetName(userFullName);
        }

        public static string GetListOfTasks()
        {
            return string.Join("\n", Tasks.Select(t => t.FullName).ToArray());
        }

        public static SimpleTaskBase GetTask(string taskName)
        {
            return Tasks.FirstOrDefault(t => t.FullName == taskName);
        }
    }
}
