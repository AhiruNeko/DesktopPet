#ifndef MESSAGE_C_H
#define MESSAGE_C_H

#include "stm32f1xx_hal.h"

#ifdef __cplusplus
extern "C" {
#endif

    typedef void* MessageHandle;

    MessageHandle Message_create();
    void Message_destroy(MessageHandle msg);

    void Message_add_key(MessageHandle msg, const char* key);
    void Message_initialized(MessageHandle msg);

    void Message_set(MessageHandle msg, const char* key, float value);
    float Message_get(MessageHandle msg, const char* key);

    char* Message_encode(MessageHandle msg);
    void Message_decode(MessageHandle msg, const char* str_msg);

    HAL_StatusTypeDef Message_send(MessageHandle msg, UART_HandleTypeDef *huart);

#ifdef __cplusplus
}
#endif

#endif // MESSAGE_C_H
