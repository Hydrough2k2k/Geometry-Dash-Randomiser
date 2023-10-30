#include <iostream>
#include <cstring>
#include <string>

using std::cout; std::string;

void removeSpecialChars(string &str) {
	if (str.size() == 0) return;

	str.erase(std::remove_if(str.begin(), str.end(),
		[](char c) { return !std::isspace(c) && !std::isalpha(c) && !std::isdigit(c); }),
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