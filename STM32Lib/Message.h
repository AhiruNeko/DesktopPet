#ifndef MESSAGE_H
#define MESSAGE_H
#include <string>
using namespace std;


class Message {
public:
    Message();
    void set(const char* key, float value);
    float get(const char* key);
    void addKey(const char* key);
    void initialized();

private:
    static constexpr int MAX_ITEMS = 16;
    float values[MAX_ITEMS];
    const char* keys[MAX_ITEMS];
    int count = 0;
    bool init = false;
};



#endif //MESSAGE_H
