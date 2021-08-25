using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SharpDX.DirectInput;

public class DirectInputRcTx : IRcTx
{
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

        foreach (DeviceObjectInstance doi in txJoystick.GetObjects(DeviceObjectTypeFlags.AbsoluteAxis))
        {
            Log.Info(doi.Name);
            Log.Info(doi.ObjectId.ToString());
        }

        return true;
        // Set BufferSize in order to use buffered data.
        txJoystick.Properties.BufferSize = 128;

        // Acquire the joystick
        txJoystick.Acquire();

        // Poll events from joystick
        while (true)
        {
            txJoystick.Poll();
            JoystickUpdate[] joystickUpdates = txJoystick.GetBufferedData();
            foreach (JoystickUpdate joystickUpdate in joystickUpdates)
            {
                Log.Info(joystickUpdate.ToString());
            }
            Thread.Sleep(1000);
        }
        return true;
    }
}
