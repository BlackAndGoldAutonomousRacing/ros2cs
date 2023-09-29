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

    private bool useRosTime = false;
    private float delay; // the time duration for this timer
    private ROS2.Clock clock; // clock to keep track of rostime for the timer
    private RosTime prevCallTime; // the rostime where we last called the callback
    private DateTime prevCallDateTime = DateTime.Now;
    public object Mutex { get { return mutex; } }
    private object mutex = new object();

    /// <summary> Tries to get a message from rcl/rmw layers. Calls the callback if successful </summary>
    // TODO(adamdbrw) this should not be public - add an internal interface
    public void TakeMessage()
    {
      if (useRosTime)
      {
        RosTime now = clock.Now;
        double timeDiff = (now.sec - prevCallTime.sec) + (now.nanosec - prevCallTime.nanosec)/1e9;

        if ((now.sec - prevCallTime.sec) + (now.nanosec - prevCallTime.nanosec)/1e9 > delay)
        {
            callback();

            if (timeDiff < 2*delay) {
              // if we are only a little late, increment the prevCallTime by delay to target the desired frequency
              prevCallTime.sec += (int) delay;
              prevCallTime.nanosec += (uint) ((delay-(int)delay)*1e9);
            }else{
              // if we are really late, reset the timer so we don't pile up callbacks
              prevCallTime = now;
            }
        }
      }
      else
      {
        DateTime now = DateTime.Now;
        if (now.Subtract(prevCallDateTime).TotalSeconds > delay) {
          callback();
          prevCallDateTime = now;
        }

      }
    }

    /// <summary> Internal constructor for Timer. Use INode.CreateTimer to construct </summary>
    /// <see cref="INode.CreateTimer"/>
    internal Timer(float delay, Node node, Action cb, bool useRosTime = true)
    {
      this.delay = Math.Abs(delay);
      this.callback = cb;
      this.useRosTime = useRosTime;
      this.clock = new ROS2.Clock();
      this.prevCallTime = clock.Now;
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
