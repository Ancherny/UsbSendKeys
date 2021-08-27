# UsbSendKeys
Translates usb game controller state into keypresses

This is a Windows .Net 4.6 console application.

The original idea was to switch flight modes in [Freerider quadrocopter simulatior](https://store.steampowered.com/app/813530/FPV_Freerider_Recharged/) directly from the RC transmitter as in happens in real flight instead of pressing keys on the keyboard.

RC Transmitter is expected to be plugged in as a common usb game controller.

The UsbSendKeys is expected to be running in the background while playing the main application.

The configuration of keypresses is defined in json file config.json.
The provided example is configured for Freerider keys switching Acro mode (I key) and Self-Leveling (or Angle) mode (U key).
The RC transmitter used in this example is Radiomaster TX16S and it is expected to be configured having come switch mapped to the 6th channel with a pulse range from 1000µS to 1400µS for Acro mode and from 1600µS to 2000µS for Angle mode.
The this pulse mapping is used to be common with one used in [BetaFlight Configurator](https://github.com/betaflight/betaflight-configurator) - the main application for configuring quadrocopter flight controllers.