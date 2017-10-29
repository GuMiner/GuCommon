using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace Common.Cnc
{
    /// <summary>
    /// An output device implementation for the Parallax Propeller-controlled mill.
    /// </summary>
    /// <remarks>
    /// This was the client-side code to run the Parallax Propeller's embedded software
    /// to control the mill. Unfortunately putting too much of the logic in the propeller
    /// made the software speed run too slowly, and putting all of the linear step logic here
    /// made the amount of commands needed to be sent also untenable for reasonable operation.
    /// </remarks>
    public class ParallaxMillController : IDisposable
    {
        // These constants are set in the Mill .spin command file.
        private const int BaudRate = 115200;
        private const char AckChar = 'A';
        private const char QueueChar = 'Q';
        private const char LineChar = 'L';
        private const char ShutdownChar = 'S';
        private const char NewLine = '\r';

        private SerialPort serialPort;

        private Thread communicationThread;
        private bool isAlive = true;
        private bool isPaused = false;

        private int linearDelay;

        // The free space length will be updated every 10 calls.
        // This also means that if the queue length is < 10, new commands won't be sent to the device.
        private const int QueueFreeSpaceCommandRate = 10;
        private int currentQueueCommandCalls = 0;

        private ConcurrentQueue<LinearMillCommand> millCommandQueue = new ConcurrentQueue<LinearMillCommand>();

        public ParallaxMillController(String portName)
        {
            CurrentCommand = 0;

            serialPort = new SerialPort(portName, BaudRate);
            serialPort.NewLine = NewLine.ToString(); // ASCII 13

            serialPort.Open();

            // Skip the first two credit lines and update the amount of free space there is.
            serialPort.ReadExisting();
            UpdateQueueFreeSpace(true);
            MaxQueueSpace = FreeQueueSpace;

            linearDelay = 2000;
            communicationThread = new Thread(PerformDeviceCommunication);
            communicationThread.Name = "Mill COM Thread";
            communicationThread.Start();
        }

        /// <summary>
        /// The amount of free space remaining on the mill queue.
        /// </summary>
        public int FreeQueueSpace { get; private set; }

        /// <summary>
        /// The amount of space the mill queue has in total.
        /// </summary>
        public int MaxQueueSpace { get; private set; }

        /// <summary>
        /// The number of command stored that haven't been sent to the mill yet.
        /// </summary>
        public int LocalQueueLength => millCommandQueue.Count;

        /// <summary>
        /// The last command ID that was sent to the mill.
        /// </summary>
        public int CurrentCommand { get; private set; }

        /// <summary>
        /// The time it took to send the last command to the mill, in milliseconds.
        /// </summary>
        public long CommandTime { get; private set; }

        /// <summary>
        /// The current delay per step.
        /// </summary>
        public int LinearDelay => linearDelay;

        public void Dispose()
        {
            if (communicationThread != null)
            {
                isAlive = false;
                communicationThread.Join();
                communicationThread = null;
            }

            serialPort?.Close();
            serialPort?.Dispose();

            serialPort = null;
        }

        /// <summary>
        /// Adds a linear motion operation to the operations queue.
        /// </summary>
        public void AddOperation(LinearMillCommand millCommand)
        {
            millCommandQueue.Enqueue(millCommand);
        }

        /// <summary>
        /// Removes any pending queue operations from the backlog.
        /// </summary>
        public void DiscardQueueBacklog()
        {
            millCommandQueue = new ConcurrentQueue<LinearMillCommand>();
        }

        public void Pause() => isPaused = true;
        public void Resume() => isPaused = false;

        /// <summary>
        /// Busy-loops to consume the queue elements added and run the mill.
        /// </summary>
        private void PerformDeviceCommunication()
        {
            while (isAlive)
            {
                if (!isPaused && FreeQueueSpace > QueueFreeSpaceCommandRate)
                {
                    // Send the mill command.
                    Stopwatch timer = Stopwatch.StartNew();

                    bool sentCommand = false;
                    LinearMillCommand millCommand;
                    if (millCommandQueue.TryDequeue(out millCommand))
                    {
                        CurrentCommand = millCommand.Id;
                        SendMotionCommand(millCommand);
                        sentCommand = true;
                    }

                    if (sentCommand)
                    {
                        // Update the queue length, if appropriate.
                        UpdateQueueFreeSpace();
                    }

                    timer.Stop();
                    CommandTime = sentCommand ? timer.ElapsedMilliseconds : CommandTime;
                }
                else if (!isPaused) // & free space limit is not reached
                {
                    UpdateQueueFreeSpace();
                    Thread.Sleep(100);
                }
                else if (isPaused)
                {
                    // Save a bit of power consumption by waiting when we're paused.
                    Thread.Sleep(100);
                }
            }

            SendShutdownCommand();
        }

        /// <summary>
        /// Tells the mill to shutdown its operation.
        /// </summary>
        private void SendShutdownCommand()
        {
            serialPort.Write(new[] { ShutdownChar }, 0, 1);
        }

        /// <summary>
        /// Sends the motion command to the mill.
        /// </summary>
        private void SendMotionCommand(LinearMillCommand millCommand)
        {
            if (millCommand.Speed != linearDelay)
            {
                // Update the mill engraving speed, if necessary.
                linearDelay = millCommand.Speed;
                serialPort.Write($"D{linearDelay}\r");
            }

            int iterations;
            int xDelay, xDelayFactor;
            int yDelay, yDelayFactor;
            int zDelay, zDelayFactor;
            int aDelay, aDelayFactor;

            DecodeMillCommand(millCommand, out iterations,
                out xDelay, out xDelayFactor,
                out yDelay, out yDelayFactor,
                out zDelay, out zDelayFactor,
                out aDelay, out aDelayFactor);

            int xDir = millCommand.XSteps > 0 ? 1 : 0;
            int yDir = millCommand.YSteps > 0 ? 1 : 0;
            int zDir = millCommand.ZSteps > 0 ? 1 : 0;
            int aDir = millCommand.ASteps > 0 ? 1 : 0;

            string lineCommand = $"L{iterations}\r";
            serialPort.Write(lineCommand);
            char response = (char)serialPort.ReadChar();
            if (response != 'X')
            {
                Debug.Fail("Expected confirmation that the X-axis is ready");
            }

            string xCommand = $"{xDir}\r{xDelay}\r{xDelayFactor}\r";
            serialPort.Write(xCommand);

            string yCommand = $"{yDir}\r{yDelay}\r{yDelayFactor}\r";
            serialPort.Write(yCommand);

            string zCommand = $"{zDir}\r{zDelay}\r{zDelayFactor}\r";
            serialPort.Write(zCommand);

            string aCommand = $"{aDir}\r{aDelay}\r{aDelayFactor}\r";
            serialPort.Write(aCommand);
            response = (char)serialPort.ReadChar();
            if (response != 'D')
            {
                Debug.Fail("Expected comfirmation that the line command was written.");
            }
        }

        /// <summary>
        /// Calls into the device to get the current free space amount.
        /// </summary>
        private void UpdateQueueFreeSpace(bool force = false)
        {
            currentQueueCommandCalls++;
            if (currentQueueCommandCalls != QueueFreeSpaceCommandRate && !force)
            {
                return;
            }

            serialPort.Write(new[] { QueueChar }, 0, 1);

            string response = serialPort.ReadLine();
            int queueSpace;
            if (!int.TryParse(response, out queueSpace))
            {
                Debug.Fail($"Expected the queue free space, received '{response}'.");
            }

            FreeQueueSpace = queueSpace;
            currentQueueCommandCalls = 0;
        }

        /// <summary>
        /// Decodes a mill command into the number of total iterations, delays, and delay factors to 
        ///  get a smooth linear motion.
        /// </summary>
        private void DecodeMillCommand(LinearMillCommand millCommand, out int iterations,
            out int xDelay, out int xDelayFactor,
            out int yDelay, out int yDelayFactor,
            out int zDelay, out int zDelayFactor,
            out int aDelay, out int aDelayFactor)
        {
            int xSteps = Math.Abs(millCommand.XSteps);
            int ySteps = Math.Abs(millCommand.YSteps);
            int zSteps = Math.Abs(millCommand.ZSteps);
            int aSteps = Math.Abs(millCommand.ASteps);

            // Minimum amount to avoid aliasing issues.
            iterations = 2 * Math.Max(xSteps, Math.Max(ySteps, Math.Max(zSteps, aSteps)));
            DecodeSteps(iterations, xSteps, out xDelay, out xDelayFactor);
            DecodeSteps(iterations, ySteps, out yDelay, out yDelayFactor);
            DecodeSteps(iterations, zSteps, out zDelay, out zDelayFactor);
            DecodeSteps(iterations, aSteps, out aDelay, out aDelayFactor);
        }

        /// <summary>
        /// Decodes the steps for an axis to provide smooth linear motion.
        /// </summary>
        private void DecodeSteps(int iterations, int steps, out int delay, out int delayFactor)
        {
            if (steps == iterations / 2)
            {
                // This is the constraining axis, so it steps every iteration.
                delay = 1;
                delayFactor = iterations + 20;
            }
            else if (steps == 0)
            {
                // Ensure that there's never a step for this axis.
                delay = iterations + 20;
                delayFactor = iterations + 20;
            }
            else
            {
                // First, choose a delay amount that gives a *lower* (or equal) number of steps specified.
                delay = (int)Math.Ceiling((double)iterations / (double)steps);

                int actualSteps = iterations / delay; // Effectively, Math.floor
                if (actualSteps == steps)
                {
                    // Equal to the number of steps specified, no additional factor needed.
                    delayFactor = iterations + 20;
                }
                else
                {
                    // We need a few more steps.
                    int stepDelta = steps - actualSteps;
                    Debug.Assert(stepDelta > 0);

                    delayFactor = (int)Math.Ceiling((double)iterations / (double)stepDelta);
                }
            }
        }
    }
}