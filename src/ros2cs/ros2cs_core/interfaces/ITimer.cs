//  Copyright 2023 Alec Pannunzio <apun189@gmail.com>
//  Copyright 2019-2021 Robotec.ai
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at

//      http://www.apache.org/licenses/LICENSE-2.0

//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License. 


using System;

namespace ROS2
{
  /// <summary> Non-generic base interface for all timers </summary>
  /// <description> Use Ros2cs.CreateTimer to construct </description>
  public interface ITimerBase : IExtendedDisposable
  {
    // TODO(adamdbrw) this should not be public - add an internal interface
    void TakeMessage();
    void Dispose();

    /// <summary> topic delay which was used when calling Ros2cs.CreateTimer </summary>
    float Delay {get;}

    

    /// <summary> subscription mutex for internal use </summary>
    object Mutex { get; }
  }

  /// <summary> Generic base interface for all subscriptions </summary>
  public interface ITimer: ITimerBase {}
}
