using System.Collections.Generic;
using SharpDX.DirectInput;

public class DirectInputRcController : IRcController
{
    public static bool CreateByName(out IRcController controller, string name)
    {
        DirectInput directInput = new DirectInput();
        IList<DeviceInstance> devices = directInput.GetDevices(DeviceClass.All, DeviceEnumerationFlags.AllDevices);
        Log.Info("Huy!");
        controller = new DirectInputRcController();
        return true;
    }
}
