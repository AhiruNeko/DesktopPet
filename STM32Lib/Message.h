#ifndef MESSAGE_H
#define MESSAGE_H
#include <any>
#include <string>
#include <unordered_map>
using namespace std;


class Message {
public:
    void put(const string& key, const any& value);

    any get(const string& key);

    bool contains(const string& key);

    string encode();

    void decode(const string& strMsg);

private:
    unordered_map<string, any> msg;
};



#endif //MESSAGE_H
