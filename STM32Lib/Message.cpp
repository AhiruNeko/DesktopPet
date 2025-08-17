#include "Message.h"
#include <string>
#include <sstream>
#include <typeindex>

#include "stm32f1xx_hal_gpio.h"


void Message::put(const string& key, const any& value) {
    msg[key] = value;
}

any Message::get(const string& key) {
    return msg[key];
}

bool Message::contains(const string& key) {
    return msg.find(key) != msg.end();
}

string Message::encode() const {
    string strMsg;
    bool first = true;
    for (const auto& it : msg) {
        if (!first) strMsg += ", ";
        first = false;

        const string& key = it.first;
        const any& value = it.second;
        string strValue;

        type_index t(value.type());
        if (t == typeid(int)) strValue = to_string(any_cast<int>(value));
        else if (t == typeid(float)) strValue = to_string(any_cast<float>(value));
        else if (t == typeid(double)) strValue = to_string(any_cast<double>(value));
        else if (t == typeid(long)) strValue = to_string(any_cast<long>(value));
        else if (t == typeid(long long)) strValue = to_string(any_cast<long long>(value));
        else if (t == typeid(short)) strValue = to_string(any_cast<short>(value));
        else if (t == typeid(char)) strValue = string(1, any_cast<char>(value));
        else if (t == typeid(bool)) strValue = any_cast<bool>(value) ? "true" : "false";
        else if (t == typeid(string)) strValue = any_cast<string>(value);
        else if (t == typeid(GPIO_PinState)) {
            auto s = any_cast<GPIO_PinState>(value);
            strValue = (s == GPIO_PIN_SET) ? "GPIO_PIN_SET" : "GPIO_PIN_RESET";
        } else strValue = "unknown";

        strMsg += key;
        strMsg += ": ";
        strMsg += strValue;
    }
    return "{" + strMsg + "}";
}

string trim(const string& s) {
    size_t start = s.find_first_not_of(" \t\n\r");
    size_t end = s.find_last_not_of(" \t\n\r");
    return (start == string::npos) ? "" : s.substr(start, end - start + 1);
}

unordered_map<string, string> parseToMap(const string& str) {
    unordered_map<string, string> result;
    if (str.size() < 2 || str.front() != '{' || str.back() != '}') return result;

    string body = str.substr(1, str.size() - 2);
    stringstream ss(body);
    string item;

    while (getline(ss, item, ',')) {
        auto pos = item.find(':');
        if (pos == string::npos) continue;

        string key = trim(item.substr(0, pos));
        string value = trim(item.substr(pos + 1));
        result[key] = value;
    }
    return result;
}

void Message::decode(const string& str) {
    auto kvMap = parseToMap(str);

    for (auto& [key, oldValueAny] : msg) {
        if (kvMap.find(key) == kvMap.end()) continue;

        const string& valueStr = kvMap[key];
        type_index t(oldValueAny.type());

        if (t == typeid(int)) oldValueAny = stoi(valueStr);
        else if (t == typeid(short)) oldValueAny = static_cast<short>(stoi(valueStr));
        else if (t == typeid(long)) oldValueAny = stol(valueStr);
        else if (t == typeid(long long)) oldValueAny = stoll(valueStr);
        else if (t == typeid(float)) oldValueAny = stof(valueStr);
        else if (t == typeid(double)) oldValueAny = stod(valueStr);
        else if (t == typeid(long double)) oldValueAny = stold(valueStr);
        else if (t == typeid(char)) {
            if (!valueStr.empty()) oldValueAny = valueStr[0];
        }
        else if (t == typeid(bool)) oldValueAny = (valueStr == "true");
        else if (t == typeid(string)) oldValueAny = valueStr;
        else if (t == typeid(GPIO_PinState)) {
            oldValueAny = (valueStr == "GPIO_PIN_SET") ? GPIO_PIN_SET : GPIO_PIN_RESET;
        }
    }
}
