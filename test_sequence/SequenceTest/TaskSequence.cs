using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SequenceTest
{
	class TaskSequence : IDisposable
	{
		private readonly AutoResetEvent _signal = new AutoResetEvent(false);
		private readonly Queue<Action> _taskQueue = new Queue<Action>();
		private readonly Thread _worker;

		public TaskSequence()
		{
			_worker = new Thread(Execute);
			_worker.Start();
		}

		public void Dispose()
		{
			_taskQueue.Enqueue(null);
			_signal.Set();
			_worker.Join();
			_signal.Close();
		}

		public void Enqueue(Action task)
		{
			if (task == null)
				throw new ArgumentNullException(nameof(task));

			lock (_taskQueue)
			{
				_taskQueue.Enqueue(task);
				_signal.Set();
			}
		}

		private void Execute()
		{
			while (true)
			{
				while (true)
				{
					Action task = null;
					lock (_taskQueue)
					{
						if (_taskQueue.Any())
							task = _taskQueue.Dequeue();
						else
							break;
					}

					if (task == null)
						return;

					try
					{
						task();
					}
					catch (Exception e)
					{
						Console.WriteLine(e.Message);
					}
				}

				_signal.WaitOne();
			}
		}
	}
}