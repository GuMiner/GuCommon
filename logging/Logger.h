#pragma once
#include <fstream>
#include <iostream>
#include <mutex>
#include <string>
#include <sstream>
#include <vector>
#include <glm\vec2.hpp>
#include <glm\vec3.hpp>
#include <glm\mat4x4.hpp>

// A simple class for logging program events out to a file.
class Logger
{
    static bool multilineArrays;

    std::string NoCategory;
    bool mirrorToConsole;
    std::vector<std::string> categoryList;

public:
    enum LogType { DEBUG = 0, INFO = 1, WARN = 2, ERR = 3 };
    static Logger *LogStream;

    // Defines the log level for console logs.
    LogType logLevel;

    // If true, the log level is stuck to WARN and includes files logs. 'logLevel' is ignored.
    bool release;

    // Creates and logs the startup text
    Logger(const char* fileName, bool mirrorToConsole, std::vector<std::string> categoryList);

    // Various convenient logging methods.
    template<typename T>
    static void LogDebug(T message)
    {
        std::stringstream stringifiedMessage;
        stringifiedMessage << message;
        LogStream->LogInternal(LogType::DEBUG, stringifiedMessage.str().c_str());
    }

    template<typename T, typename... Remainder>
    static void LogDebug(T message, Remainder... remainder)
    {
        std::string fullMessage = FormFullLogMessage(message, remainder...);
        LogStream->LogInternal(LogType::DEBUG, fullMessage.c_str());
    }
    
    template<typename T>
    static void Log(T message)
    {
        std::stringstream stringifiedMessage;
        stringifiedMessage << message;
        LogStream->LogInternal(LogType::INFO, stringifiedMessage.str().c_str());
    }

    template<typename T, typename... Remainder>
    static void Log(T message, Remainder... remainder)
    {
        std::string fullMessage = FormFullLogMessage(message, remainder...);
        LogStream->LogInternal(LogType::INFO, fullMessage.c_str());
    }

    template<typename T>
    static void LogWarn(T message)
    {
        std::stringstream stringifiedMessage;
        stringifiedMessage << message;
        LogStream->LogInternal(LogType::WARN, stringifiedMessage.str().c_str());
    }

    template<typename T, typename... Remainder>
    static void LogWarn(T message, Remainder... remainder)
    {
        std::string fullMessage = FormFullLogMessage(message, remainder...);
        LogStream->LogInternal(LogType::WARN, fullMessage.c_str());
    }

    template<typename T>
    static void LogError(T message)
    {
        std::stringstream stringifiedMessage;
        stringifiedMessage << message;
        LogStream->LogInternal(LogType::ERR, stringifiedMessage.str().c_str());
    }

    template<typename T, typename... Remainder>
    static void LogError(T message, Remainder... remainder)
    {
        std::string fullMessage = FormFullLogMessage(message, remainder...);
        LogStream->LogInternal(LogType::ERR, fullMessage.c_str());
    }

    // Control methods for the static Logging instance.
    static void Setup(std::string logName);
    static void Setup(std::string logName, bool mirrorToConsole);
    static void Setup(std::string logName, bool mirrorToConsole, std::vector<std::string> categoryList);
    static void Setup(std::string logName, bool mirrorToConsole, std::vector<std::string> categoryList, bool dontMultilineArrays);
    static void Shutdown();

    // Destructs the logger
    ~Logger();

private:
    // Ensure this is non-copyable my making the copy and assignment operators private.
    Logger(const Logger&);
    Logger& operator=(const Logger&);

    std::ofstream logFile;
    std::mutex writeLock;

    // Recursion finale.
    template<typename T>
    static std::string FormFullLogMessage(T message)
    {
        std::stringstream streamData;
        streamData << message;
        return streamData.str();
    }

    // Customizations to support compliated types
    template<>
    static std::string FormFullLogMessage<glm::vec3>(glm::vec3 message)
    {
        std::stringstream streamData;
        streamData << "[" << message.x << ", " << message.y << ", " << message.z << "]";
        return streamData.str();
    }

    template<>
    static std::string FormFullLogMessage<glm::vec2>(glm::vec2 message)
    {
        std::stringstream streamData;
        streamData << "[" << message.x << ", " << message.y << "]";
        return streamData.str();
    }

    template<>
    static std::string FormFullLogMessage<glm::mat4>(glm::mat4 message)
    {
        std::string separatorChar = multilineArrays ? "\n" : ",";
        std::stringstream streamData;
        streamData << "[[" << message[0][0] << ", " << message[0][1] << ", " << message[0][2] << ", " << message[0][3] << "]" << separatorChar 
                   << "[" << message[1][0] << ", " << message[1][1] << ", " << message[1][2] << ", " << message[1][3] << "]" << separatorChar
                   << "[" << message[2][0] << ", " << message[2][1] << ", " << message[2][2] << ", " << message[2][3] << "]" << separatorChar
                   << "[" << message[3][0] << ", " << message[3][1] << ", " << message[3][2] << ", " << message[3][3] << "]]";
        return streamData.str();
    }

     // Uses variadic templates to form a full log message.
    template<typename T, typename... Remainder>
    static std::string FormFullLogMessage(T message, Remainder... remainder)
    {
        std::stringstream streamData;
        streamData << Logger::FormFullLogMessage(message) << Logger::FormFullLogMessage(remainder...);
        return streamData.str();
    }

    // Logs a message out the logger
    void LogInternal(LogType logType, const char* message);

    // Logs the current time out to the log file.
    std::string LogTime();

    // Retrieve the log type given the enumeration.
    const char* GetLogType(LogType logType);
};
