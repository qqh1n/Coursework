#include "pch.h"
#include "LoggerCppHeader.h"

static std::mutex g_logMutex;

std::string LoggerCpp::getTimestamp()
{
    auto now = std::chrono::system_clock::now();
    auto tp = std::chrono::system_clock::to_time_t(now);
    std::stringstream ss;
    tm timeInfo;
    localtime_s(&timeInfo, &tp);
    ss << std::put_time(&timeInfo, "%Y-%m-%d %H:%M:%S") << "."
        << (now.time_since_epoch().count() % 1000000);
    return ss.str();
}

void LoggerCpp::log(const std::string& msg)
{
    std::lock_guard<std::mutex> lock(g_logMutex);

    std::ofstream log("adapter_cpp.log", std::ios::app);
    if (log)
    {
        log << getTimestamp() << " [CPP] " << msg << std::endl;
    }
}
