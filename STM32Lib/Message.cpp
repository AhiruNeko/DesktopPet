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
    int num = search(keys, count, key);
    if (num == -1) return 0.0f;
    return values[num];
}

void Message::set(const char* key, float value) {
    int num = search(keys, count, key);
    if (num == -1) return;
    values[num] = value;
}



