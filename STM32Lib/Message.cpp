#include "Message.h"

#include <cstring>


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

void Message::addKey(const char* key) {
    if (!init) {
        if (count >= MAX_ITEMS || search(keys, count, key) != -1) {
            return;
        }
        count++;
        keys[count] = key;
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

char* Message::encode() const {
    int bufSize = count * 64 + 2;
    char* buffer = (char*)malloc(bufSize);
    char* p = buffer;

    *p++ = '{';

    for (int i = 0; i < count; i++) {
        p += sprintf(p, "\"%s\":", keys[i]);
        p += sprintf(p, "%.*g", 9, values[i]);

        if (i < count - 1) {
            *p++ = ',';
            *p++ = ' ';
        }
    }

    *p++ = '}';
    *p = '\0';

    return buffer;
}

void Message::decode(const char* strMsg) {
    if (!strMsg) return;
    const char* p = strMsg;
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


