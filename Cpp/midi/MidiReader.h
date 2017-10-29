#pragma once

#include <queue>
#include <string>
#include <pm_common/portmidi.h>

struct MidiEvent
{
    unsigned char status;
    unsigned char lowByte;
    unsigned char highByte;
};

// Reads MIDI input
// For more information, see http://portmedia.sourceforge.net/portmidi/
class MidiReader
{
    static const int MAX_QUEUE_SIZE = 1024;
    static const int BUFFER_SIZE = 1024;

    PortMidiStream *portMidiInputStream;
    std::queue<MidiEvent> midiEvents;
    
public:
    MidiReader();

    int GetMidiDeviceCount();
    std::string GetMidiDeviceInfo(int id);
    bool ConnectToDevice(int id);

    void UpdateMidiDevice();
    bool ReadNextEvent(MidiEvent& midiEvent);

    ~MidiReader();
};