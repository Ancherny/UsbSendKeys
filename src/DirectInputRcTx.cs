using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

public class DirectInputRcTx : IRcTx
{
    private class ChannelsState : IChannelsState
    {
        // Channel values are of standard RC TX microseconds range of (1000:2000)
        private readonly int[] _channelValues;

        private static int AxisValueToMicroseconds(int axisValue)
        {
            return (int)(1.0f / uint.MaxValue * axisValue * 1000 + 1000);
        }

        private static readonly Func<JoystickState, int>[] _getChannelValues =
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
            _channelValues = new int[_getChannelValues.Length];
        }

        public bool Init(JoystickState state)
        {
            for (int channelId = 0; channelId < _getChannelValues.Length; channelId++)
            {
                _channelValues[channelId] = _getChannelValues[channelId](state);
            }
            return true;
        }

        public bool GetActivated(out bool[] activated, Config.Key[] keys, IChannelsState lastChannelsState)
        {
            bool isSuccess = false;
            do
            {
                activated = new bool[_getChannelValues.Length];
                ChannelsState lastState = (ChannelsState)lastChannelsState;
                if (lastState == null || lastState._channelValues.Length != _getChannelValues.Length)
                {
                    Log.Error($"Bad last channels state.");
                    break;
                }

                bool wasError = false;
                foreach (Config.Key key in keys)
                {
                    if (key.ChannelId < 0 || key.ChannelId >= _getChannelValues.Length)
                    {
                        Log.Error("Channel id is out of range.");
                        wasError = true;
                        break;
                    }
                    int lastValue = lastState._channelValues[key.ChannelId];
                    bool wasActive = lastValue > key.From && lastValue < key.To;

                    int currentValue = _channelValues[key.ChannelId];
                    bool isActive = currentValue > key.From && currentValue < key.To;

                    bool isActivated = wasActive ^ isActive && isActive;
                    if (isActivated)
                    {
                        Log.Info($"Key '{key.Name}' is activated");
                    }
                    activated[key.ChannelId] = isActivated;
                }
                if (wasError)
                {
                    break;
                }

                isSuccess = true;

            } while (false);

            if (!isSuccess)
            {
                Log.Error("Failed to get activated channels state.");
            }
            return isSuccess;
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
