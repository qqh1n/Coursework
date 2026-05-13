#pragma once

#include <string>
#include <iomanip>
#include <fstream>
#include <sstream>
#include <chrono>
#include <mutex>

class LoggerCpp
{
public:
    static void log(const std::string& msg);
private:
    static std::string getTimestamp();
};
