#include <ctime>
#include <sstream>
#include "Logger.h"

Logger *Logger::LogStream;
bool Logger::multilineArrays = false;

void Logger::Setup(std::string logName)
{
    Logger::Setup(logName, false);
}

void Logger::Setup(std::string logName, bool mirrorToConsole)
{
    Logger::Setup(logName, mirrorToConsole, std::vector<std::string>());
}

void Logger::Setup(std::string logName, bool mirrorToConsole, std::vector<std::string> categoryList)
{
    Logger::Setup(logName, mirrorToConsole, categoryList, false);
}

void Logger::Setup(std::string logName, bool mirrorToConsole, std::vector<std::string> categoryList, bool multilineArrays)
{
    Logger::multilineArrays = multilineArrays;
    LogStream = new Logger(logName.c_str(), mirrorToConsole, categoryList);
    LogStream->logLevel = LogType::INFO;
    LogStream->release = false;
}

void Logger::Shutdown()
{
    delete LogStream;
}

// Creates and logs the startup text
Logger::Logger(const char* fileName, bool mirrorToConsole, std::vector<std::string> categoryList)
    : NoCategory("NoCategory"), mirrorToConsole(mirrorToConsole), categoryList(categoryList)
{
    logFile.open(fileName);

    if (logFile.is_open())
    {
        logFile << "Application Starting..." << std::endl;
    }
}

// Logs a message out the logger
void Logger::LogInternal(LogType logType, const char* message)
{
    // Only log at all if we're not in release mode or we're in release mode and this is an important error.
    if ((release && logType >= LogType::WARN) || !release)
    {
        writeLock.lock();
        std::stringstream logLines;
        logLines << LogTime() << GetLogType(logType) << message << std::endl;
        logFile << logLines.str();
        if (mirrorToConsole && logType >= this->logLevel)
        {
            // Only mirror to console if we've enabled it and this greater than the specified logging level.
            std::cout << logLines.str();
        }

        writeLock.unlock();
    }
}

// Logs the current time out to the log file.
std::string Logger::LogTime()
{
    time_t rawTime;
    struct tm* localTime;
    time(&rawTime);
    localTime = localtime(&rawTime);

    std::stringstream time;
    time << "[" << (localTime->tm_year + 1900) << "-" << (localTime->tm_mon + 1) << "-" << localTime->tm_mday
        << " " << localTime->tm_hour << ":" << localTime->tm_min << ":" << localTime->tm_sec << "] ";
    return time.str();
}

// Retrieve the log type given the enumeration.
const char* Logger::GetLogType(LogType logType)
{
    switch (logType)
    {
    case LogType::DEBUG:
        return "dbg: ";
    case LogType::INFO:
        return "inf: ";
    case LogType::WARN:
        return "wrn: ";
    case LogType::ERR:
        return "err: ";
    }

    return "bug: ";
}

Logger::~Logger()
{
    if (logFile.is_open())
    {
        logFile << std::endl;
        logFile.close();
    }
}
