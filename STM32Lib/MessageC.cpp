#include "MessageC.h"
#include "Message.h"

MessageHandle Message_create() {
    return new Message();
}

void Message_destroy(MessageHandle msg) {
    delete static_cast<Message*>(msg);
}

void Message_add_key(MessageHandle msg, const char* key) {
    static_cast<Message*>(msg)->add_key(key);
}

void Message_initialized(MessageHandle msg) {
    static_cast<Message*>(msg)->initialized();
}

void Message_set(MessageHandle msg, const char* key, float value) {
    static_cast<Message*>(msg)->set(key, value);
}

float Message_get(MessageHandle msg, const char* key) {
    return static_cast<Message*>(msg)->get(key);
}

char* Message_encode(MessageHandle msg) {
    return static_cast<Message*>(msg)->encode();
}

void Message_decode(MessageHandle msg, const char* str_msg) {
    static_cast<Message*>(msg)->decode(str_msg);
}

HAL_StatusTypeDef Message_send(MessageHandle msg, UART_HandleTypeDef *huart) {
    return static_cast<Message*>(msg)->send(huart);
}
