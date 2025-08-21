#include "Message.h"
#include <cstring>
#include <string>
#include <cstdlib>


int search(const char* keys[], int size, const char* key) {
    for (int i = 0; i < size; ++i) {
        if (strcmp(keys[i], key) == 0) {
            return i;
        }
    }
    return -1;
}

Message::Message() {
    for (int i = 0; i < MAX_ITEMS; ++i) {
        keys[i] = nullptr;
        values[i] = 0.0f;
    }
    count = 0;
    init = false;
}

void Message::initialized() {
    init = true;
}

void Message::add_key(const char* key) {
    if (!init) {
        if (count >= MAX_ITEMS || search(keys, count, key) != -1) {
            return;
        }
        keys[count] = key;
        count++;
    }
}

float Message::get(const char* key) {
    if (init) {
        int num = search(keys, count, key);
        if (num == -1) return 0.0f;
        return values[num];
    }
    return 0.0f;
}

void Message::set(const char* key, float value) {
    if (init) {
        int num = search(keys, count, key);
        if (num == -1) return;
        values[num] = value;
    }
}

char* itoa_simple_ll(char* p, long long val) {
    char buf[24];
    int i = 0;
    bool neg = val < 0;
    if (neg) val = -val;
    if (val == 0) buf[i++] = '0';
    while (val) { buf[i++] = '0' + (val % 10); val /= 10; }
    if (neg) buf[i++] = '-';
    for (int j = i - 1; j >= 0; j--) *p++ = buf[j];
    return p;
}

char* write_key(char* p, const char* key) {
    *p++ = '"';
    while (*key) *p++ = *key++;
    *p++ = '"';
    *p++ = ':';
    return p;
}

char* write_value(char* p, float val) {
    if (val < 0) { *p++ = '-'; val = -val; }

    long long int_part = (long long)val;
    long long frac_part = (long long)((val - int_part) * 1000000.0f + 0.5f);

    p = itoa_simple_ll(p, int_part);
    *p++ = '.';
    char buf[7];
    for (int i = 5; i >= 0; i--) {
        buf[i] = '0' + (frac_part % 10);
        frac_part /= 10;
    }
    for (int i = 0; i < 6; i++) *p++ = buf[i];

    return p;
}

char* Message::encode() const {
    if (!init) return nullptr;

    int bufSize = count * 48 + 2;
    char* buffer = (char*)malloc(bufSize);
    if (!buffer) return nullptr;

    char* p = buffer;
    *p++ = '{';

    for (int i = 0; i < count; i++) {
        p = write_key(p, keys[i]);
        p = write_value(p, values[i]);

        if (i < count - 1) {
            *p++ = ',';
            *p++ = ' ';
        }
    }

    *p++ = '}';
    *p = '\0';
    return buffer;
}

void Message::decode(const char* str_msg) {
    if (init) {
        if (!str_msg) return;
        const char* p = str_msg;
        while (*p && *p != '{') p++;
        if (*p == '{') p++;

        while (*p && *p != '}') {
            while (*p == ' ' || *p == '\t' || *p == '\n') p++;

            if (*p != '"') break;
            p++;

            const char* keyStart = p;
            while (*p && *p != '"') p++;
            if (*p != '"') break;
            int keyLen = p - keyStart;
            std::string key(keyStart, keyLen);
            p++;

            while (*p && (*p == ' ' || *p == ':')) p++;

            const char* valStart = p;
            while (*p && *p != ',' && *p != '}') p++;
            int valLen = p - valStart;
            std::string valStr(valStart, valLen);

            float val = std::strtof(valStr.c_str(), nullptr);

            for (int i = 0; i < count; i++) {
                if (key == keys[i]) {
                    values[i] = val;
                    break;
                }
            }
            if (*p == ',') p++;
        }
    }
}

HAL_StatusTypeDef Message::send(UART_HandleTypeDef *huart) {
    if (init) {
        char* data = encode();
        if (data == nullptr || huart == nullptr) return HAL_ERROR;
        size_t len = strlen(data);
        HAL_StatusTypeDef status = HAL_UART_Transmit(huart, (uint8_t*)data, len, HAL_MAX_DELAY);
        free(data);
        return status;
    }
    return HAL_ERROR;
}


