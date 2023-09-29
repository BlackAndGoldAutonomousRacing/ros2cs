// Copyright 2023 Alec Pannunzio
// Copyright 2019-2021 Robotec.ai
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using ROS2.Internal;

namespace ROS2
{
  /// <summary> Timer with a callback and a given time delay </summary>
  /// <description> Timers are created through INode interface (CreateTimer) </description>
  public class Timer: ITimer
  {


    public bool IsDisposed { get { return disposed; } }

    public float Delay { get { return delay; } }
    private bool disposed = false;

    private rcl_node_t nodeHandle;
    private readonly Action callback;

    private float delay; // the time duration for this timer
    private ROS2.Clock clock; // clock to keep track of rostime for the timer
    private RosTime prevCallTime; // the rostime where we last called the callback
    public object Mutex { get { return mutex; } }
    private object mutex = new object();

    /// <summary> Tries to get a message from rcl/rmw layers. Calls the callback if successful </summary>
    // TODO(adamdbrw) this should not be public - add an internal interface
    public void TakeMessage()
    {
        RosTime now = clock.Now;
        double timeSpan = (now.sec - prevCallTime.sec) + (now.nanosec - prevCallTime.nanosec)/1e9;
        if (timeSpan > delay) // FIXME should check if timerspan is done
        {
            callback();
            prevCallTime = now;
        }
      
    }

    /// <summary> Internal constructor for Timer. Use INode.CreateTimer to construct </summary>
    /// <see cref="INode.CreateTimer"/>
    internal Timer(float delay, Node node, Action cb)
    {
      this.delay = delay;
      callback = cb;

      clock = new ROS2.Clock();
      prevCallTime = clock.Now;
    }

    ~Timer()
    {
      DestroyTimer();
    }

    public void Dispose()
    {
      DestroyTimer();
    }

    /// <summary> "Destructor" supporting disposable model </summary>
    private void DestroyTimer()
    {
      lock (mutex)
      {
        if (!disposed)
        {
          // TODO dispose of anything that needs to be disposed. (I don't think there is anything)
          disposed = true;
          Ros2csLogger.GetInstance().LogInfo("Timer destroyed");
        }
      }
    }
  }
}
