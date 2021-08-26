using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

public class DirectInputRcTx : IRcTx
{
    private readonly Joystick _txJoystick;
    private readonly int[] _channelsState = new int[8];

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

    private static int AxisValueToMicroseconds(int axisValue)
    {
        return (int)(1.0f / uint.MaxValue * axisValue * 1000 + 1000);
    }

    public bool GetChannelsState(out int[] channelsState)
    {
        JoystickState state = _txJoystick.GetCurrentState();
        _channelsState[0] = AxisValueToMicroseconds(state.X);
        _channelsState[1] = AxisValueToMicroseconds(state.Y);
        _channelsState[2] = AxisValueToMicroseconds(state.Z);
        _channelsState[3] = AxisValueToMicroseconds(state.RotationX);
        _channelsState[4] = AxisValueToMicroseconds(state.RotationY);
        _channelsState[5] = AxisValueToMicroseconds(state.RotationZ);
        _channelsState[6] = AxisValueToMicroseconds(state.Sliders[0]);
        _channelsState[7] = AxisValueToMicroseconds(state.Sliders[1]);
        channelsState = _channelsState;
        return true;
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
