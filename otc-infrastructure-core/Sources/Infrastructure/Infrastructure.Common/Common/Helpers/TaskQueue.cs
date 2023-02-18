using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Common.Helpers
{
    public class TaskQueue
    {
        private readonly int _limit;
        private readonly LinkedList<Task> _tasks;

        public TaskQueue(int limit)
        {
            _limit = limit;
            _tasks = new LinkedList<Task>();
        }

        public ValueTask AddTaskAsync(Action func)
        {
            return AddTaskAsync(() => Task.Run(func));
        }

        public async ValueTask AddTaskAsync(Func<Task> func)
        {
            while (true)
            {
                if (_tasks.Count >= _limit)
                {
                    await WaitOneAsync();
                    continue;
                }

                lock (_tasks)
                {
                    if (_tasks.Count >= _limit)
                    {
                        continue;
                    }

                    _tasks.AddLast(func());
                    break;
                }
            }
        }

        public async ValueTask WaitAllAsync()
        {
            while (_tasks.Count > 0)
            {
                await WaitOneAsync();
            }
        }

        private async ValueTask WaitOneAsync()
        {
            if (_tasks.Count == 0)
            {
                return;
            }
            var tasksArray = _tasks.ToArray();

            if (tasksArray.Length > 0)
            {
                var task = await Task.WhenAny(tasksArray);
                _tasks.Remove(task);
            }
        }
    }
}