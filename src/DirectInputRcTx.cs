using System.Collections.Generic;
using System.Linq;
using SharpDX.DirectInput;

public class DirectInputRcTx : IRcTx
{
    private readonly DeviceInstance _txDevice;

    private DirectInputRcTx(DeviceInstance txDevice)
    {
        _txDevice = txDevice;
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

        tx = new DirectInputRcTx(txDevice);
        return true;
    }
}
