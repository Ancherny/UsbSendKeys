using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

public class DirectInputRcTx : IRcTx
{
    private class ChannelsState : IChannelsState
    {
        // Channel state is standard RC TX microseconds range of (1000:2000)
        private readonly int[] _state;

        private static int AxisValueToMicroseconds(int axisValue)
        {
            return (int)(1.0f / uint.MaxValue * axisValue * 1000 + 1000);
        }

        private static readonly Func<JoystickState, int>[] _getState =
        {
            js => AxisValueToMicroseconds(js.X),
            js => AxisValueToMicroseconds(js.Y),
            js => AxisValueToMicroseconds(js.Z),
            js => AxisValueToMicroseconds(js.RotationX),
            js => AxisValueToMicroseconds(js.RotationY),
            js => AxisValueToMicroseconds(js.RotationZ),
            js => AxisValueToMicroseconds(js.Sliders[0]),
            js => AxisValueToMicroseconds(js.Sliders[1]),
        };

        public ChannelsState()
        {
            _state = new int[_getState.Length];
        }

        public bool Init(JoystickState state)
        {
            for (int channelId = 0; channelId < _getState.Length; channelId++)
            {
                _state[channelId] = _getState[channelId](state);
            }
            return true;
        }

        public bool[] GetActivated(Config.Key[] keys, IChannelsState lastState)
        {
            throw new NotImplementedException();
        }

    }

    private readonly Joystick _txJoystick;

    private DirectInputRcTx(Joystick txJoystick)
    {
        _txJoystick = txJoystick;
    }

    public static bool CreateByName(out IRcTx tx, string txName)
    {
        tx = null;
        DirectInput directInput = new DirectInput();
        IList<DeviceInstance> devices = directInput.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AllDevices);

        DeviceInstance txDevice = devices.FirstOrDefault(device => device.InstanceName == txName);
        if (txDevice == null)
        {
            Log.Error($"Cannot find tx device by name '{txName}'");
            return false;
        }

        Joystick txJoystick = new Joystick(directInput, txDevice.InstanceGuid);
        tx = new DirectInputRcTx(txJoystick);

        // Acquire the joystick
        try
        {
            txJoystick.Acquire();
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            Log.Error("Failed to acquire joystick device.");
            return false;
        }

        return true;
    }

    public bool GetChannelsState(out IChannelsState state)
    {
        ChannelsState channelsState = new ChannelsState();
        state = channelsState;
        return channelsState.Init(_txJoystick.GetCurrentState());
    }

    public void Dispose()
    {
        if (_txJoystick != null)
        {
            _txJoystick.Unacquire();
            _txJoystick.Dispose();
        }
    }
}
