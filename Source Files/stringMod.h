#include <iostream>
#include <cstring>
#include <string>

using std::cout; std::string;


void removeSpecialChars(string& str) {
	if (str.size() == 0) return;

	str.erase(std::remove_if(str.begin(), str.end(),
		[](char c) { return !std::isspace(c) && !std::isalpha(c) && !std::isdigit(c); }),
		str.end());
}

void removeNonDigits(string& str) {
	if (str.size() == 0) return;

	str.erase(std::remove_if(str.begin(), str.end(),
		[](char c) { return !std::isdigit(c); }),
		str.end());
}

void removeWhitespace(string& str) {
	if (str.size() == 0) return;

	while (str[0] == ' ' || str[0] == '\t')
		str.erase(0, 1);
}

void splitStringAt(string& str1, string& str2, char ch) {
	str2 = str1;
	int erasedChars = 0;
	while (str2[0] != ch) {
		erasedChars++;
		str2.erase(0, 1);
		if (str2.size() == 0)
			return;
	}
	str2.erase(0, 1);
	str1.erase(erasedChars, str1.size());
}

void numToStrSeparated(string& str1, size_t num) {
	string str2 = to_string(num);

	for (int i = 0; i < str2.size(); i++) {
		str1 += str2[i];
		if ((str2.size() - i - 1) % 3 == 0)
			str1 += ' ';
	}
}

vector<string> splitString(string str, int maxWidth) {

	int stringletCount = 0, erasedChars;
	vector<string> stringlets;

	// Splits input string into smaller strings capped at 'maxWidth'
	while (str.size() > maxWidth) {
		stringlets.push_back(str);
		stringlets[stringletCount].erase(maxWidth, stringlets[stringletCount].size());

		erasedChars = maxWidth;
		while (true) {
			if (stringlets[stringletCount][stringlets[stringletCount].size() - 1] == ' ')
				break;
			stringlets[stringletCount].erase(stringlets[stringletCount].size() - 1, stringlets[stringletCount].size());
			erasedChars--;
			if (erasedChars == 0) {
				erasedChars = maxWidth;
				stringlets[stringletCount] = str;
				stringlets[stringletCount].erase(maxWidth, stringlets[stringletCount].size());
				break;
			}
		}
		str.erase(0, erasedChars);
		stringletCount++;
	}
	stringlets.push_back(str);
	stringletCount++;

	return stringlets;
}

string numToStrWithPadding(string &str, size_t num, int maxPad, char ch = '0') {
	string clipboard = to_string(num);
	str += (string(maxPad - clipboard.size(), ch)) + clipboard;
	return str;
}