#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.11
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */


public enum AkMIDIEventTypes {
  NOTE_OFF = 0x80,
  NOTE_ON = 0x90,
  NOTE_AFTERTOUCH = 0xa0,
  CONTROLLER = 0xb0,
  PROGRAM_CHANGE = 0xc0,
  CHANNEL_AFTERTOUCH = 0xd0,
  PITCH_BEND = 0xe0,
  SYSEX = 0xf0,
  ESCAPE = 0xf7,
  META = 0xff
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.