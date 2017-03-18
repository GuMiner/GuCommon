#include <sstream>
#include "logging\Logger.h"
#include "MidiReader.h"

MidiReader::MidiReader()
    : portMidiInputStream(nullptr)
{
    PmError error = Pm_Initialize();
    if (error != PmError::pmNoError)
    {
        Logger::LogError("Error initializing port MIDI: ", Pm_GetErrorText(error));
    }
}

int MidiReader::GetMidiDeviceCount()
{
    return Pm_CountDevices();
}

std::string MidiReader::GetMidiDeviceInfo(int id)
{
    std::stringstream deviceInfoString("");

    const PmDeviceInfo *midiDeviceInfo = Pm_GetDeviceInfo(id);
    if (midiDeviceInfo == nullptr)
    {
        deviceInfoString << "Error reading MIDI device with ID ";
        deviceInfoString << id;
    }
    else
    {
        deviceInfoString << "'" << midiDeviceInfo->name << "': " << midiDeviceInfo->input << " " << midiDeviceInfo->output << ".";
    }

    return deviceInfoString.str();
}

bool MidiReader::ConnectToDevice(int id)
{
    PmError errorMsg = Pm_OpenInput(&portMidiInputStream, id, NULL, BUFFER_SIZE, NULL, NULL);
    if (errorMsg != pmNoError)
    {
        Logger::LogError("Error opening device for input: ", Pm_GetErrorText(errorMsg));
        return false;
    }

    return true;
}

void MidiReader::UpdateMidiDevice()
{
    
}

bool MidiReader::ReadNextEvent(MidiEvent& midiEvent)
{
    
}

MidiReader::~MidiReader()
{
    if (portMidiInputStream != nullptr)
    {
        PmError error = Pm_Close(portMidiInputStream);
        if (error != PmError::pmNoError)
        {
            Logger::LogError("Error closing the input stream: ", Pm_GetErrorText(error));
        }
        
        portMidiInputStream = nullptr;
    }

    Pm_Terminate();
}