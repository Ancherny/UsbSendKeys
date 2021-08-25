using System.Collections.Generic;
using SharpDX.DirectInput;

public class DirectInputRcTx : IRcTx
{
    public static bool CreateByName(out IRcTx tx, string txName)
    {
        DirectInput directInput = new DirectInput();
        IList<DeviceInstance> devices = directInput.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AllDevices);
        Log.Info("Huy!");
        tx = new DirectInputRcTx();
        return true;
    }
}
