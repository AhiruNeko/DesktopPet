#ifndef MESSAGE_H
#define MESSAGE_H
extern "C" {
#include "usart.h"
}
using namespace std;


class Message {
public:
    Message();
    void set(const char* key, float value);
    float get(const char* key);
    void add_key(const char* key);
    void initialized();
    char* encode() const;
    void decode(const char* str_msg);
    HAL_StatusTypeDef send(UART_HandleTypeDef *huart);

private:
    static constexpr int MAX_ITEMS = 16;
    float values[MAX_ITEMS];
    const char* keys[MAX_ITEMS];
    int count = 0;
    bool init = false;
};

#endif //MESSAGE_H
