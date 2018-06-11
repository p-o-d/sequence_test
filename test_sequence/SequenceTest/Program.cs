using System;
using System.Threading;

namespace SequenceTest
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var sequence = new TaskSequence())
			{
				for (int i = 1; i < 4; i++)
				{
					var index = i;
					sequence.Enqueue(() =>
					{
						Thread.Sleep(index * 1000);
						Console.WriteLine($"Task #{index} finished.");
					});
					Console.WriteLine($"Task #{index} added.");
				}

				Thread.Sleep(1200);

				sequence.Enqueue(() =>
				{
					Thread.Sleep(1000);
					throw new Exception("Task #4 failed.");
				});
				Console.WriteLine("Task #4 added.");

				sequence.Enqueue(() =>
				{
					Thread.Sleep(500);
					Console.WriteLine("Task #5 finished.");
				});
				Console.WriteLine("Task #5 added.");
			}

			Console.WriteLine("All tasks finished!");
			Console.ReadLine();
		}
	}
}
