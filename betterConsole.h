#include <iostream>
#include <string>
#include <cstring>

using namespace std;

#define _WIN32_WINNT 0x0500

#include <windows.h>
#include <conio.h>

#define gotoxy(x,y)     printf("\033[%d;%dH", (y), (x))

#define UNDERLINE "\x1B[4m"

#define BLACK    "\x1B[30m"
#define RED      "\x1B[31m"
#define GREEN    "\x1B[32m"
#define YELLOW   "\x1B[33m"
#define BLUE     "\x1B[34m"
#define MAGENTA  "\x1B[35m"
#define CYAN     "\x1B[36m"
#define WHITE    "\x1B[37m"

#define BGBLACK    "\x1B[40m"
#define BGRED      "\x1B[41m"
#define BGGREEN    "\x1B[42m"
#define BGYELLOW   "\x1B[43m"
#define BGBLUE     "\x1B[44m"
#define BGMAGENTA  "\x1B[45m"
#define BGCYAN     "\x1B[46m"
#define BGWHITE    "\x1B[47m"

#define BRBLACK    "\x1B[90m"
#define BRRED      "\x1B[91m"
#define BRGREEN    "\x1B[92m"
#define BRYELLOW   "\x1B[93m"
#define BRBLUE     "\x1B[94m"
#define BRMAGENTA  "\x1B[95m"
#define BRCYAN     "\x1B[96m"
#define BRWHITE    "\x1B[97m"

#define BRBGBLACK    "\x1B[100m"
#define BRBGRED      "\x1B[101m"
#define BRBGGREEN    "\x1B[102m"
#define BRBGYELLOW   "\x1B[103m"
#define BRBGBLUE     "\x1B[104m"
#define BRBGMAGENTA  "\x1B[105m"
#define BRBGCYAN     "\x1B[106m"
#define BRBGWHITE    "\x1B[107m"

#define RESET    "\x1B[37m"
#define BGRESET  "\x1B[90m"

//----------------------------------------------------------------------

namespace betterCon {
	int windowWidth = 120;
	int windowHeight = 30;
}

RTL_OSVERSIONINFOW GetRealOSVersion();
void strLengthCorrection(string, int*);

void consoleSetup(bool resizeEnabled, int width, int height) {

	betterCon::windowWidth = width;
	betterCon::windowHeight = height;

	HANDLE hstdin = GetStdHandle(STD_INPUT_HANDLE);
	DWORD mode;
	hstdin = GetStdHandle(STD_OUTPUT_HANDLE);

	GetConsoleMode(hstdin, &mode);
	mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
	SetConsoleMode(hstdin, ENABLE_ECHO_INPUT | ENABLE_PROCESSED_INPUT);

	HWND consoleWindow = GetConsoleWindow();
	RECT window;
	GetWindowRect(consoleWindow, &window);

	//if (!MoveWindow(consoleWindow, window.left, window.top, 993, 519, TRUE))

	_COORD coord = { height, width };
	_SMALL_RECT Rect = { 0, 0, width - 1, height - 1 };

	SetConsoleScreenBufferSize(hstdin, coord);
	SetConsoleWindowInfo(hstdin, TRUE, &Rect);

	if (!resizeEnabled)
		SetWindowLong(consoleWindow, GWL_STYLE, GetWindowLong(consoleWindow, GWL_STYLE) & ~WS_MAXIMIZEBOX & ~WS_SIZEBOX);

	time_t current_time = time(NULL);
	srand((unsigned)time(NULL));
}

void printNTimes(int n, char ch = ' ') {

	cout << string(n, ch);
}

void printCenter(string str, int width = betterCon::windowWidth) {

	if (width == 0) width = betterCon::windowWidth;

	int length = str.size();
	strLengthCorrection(str, &length);

	if (length > width)
		str.erase(width, length);

	int pad = (width - length) / 2;

	if (pad < 0) pad = 0;
	else if (length + pad > width)
		pad = width - length;

	printNTimes(pad);

	cout << str;
}

void printOffset(string str, int pad = 0, int width = betterCon::windowWidth) {

	if (width == 0) width = betterCon::windowWidth;

	if (str.size() > width)
		str.erase(width, str.size());

	int length;
	strLengthCorrection(str, &length);

	if (pad < 0) pad = 0;
	else if (length + pad > width)
		pad = length;

	printNTimes(pad);
	cout << str;
}

// This supports some escape sequences, more are to be added later
void strLengthCorrection(string str, int *length) {

	*length = str.size();
	string stringCut = str;
	stringCut.erase(2, stringCut.size());

	for (int i = 0; i < str.size(); i++)
	{
		stringCut.erase(0, 1);
		stringCut += str[i];
		if (stringCut == "\x1B[") {

			switch (str[i + 1]) {
			case '1':
				if (str[i + 2] != '0')
					break;
			case '3':
			case '4':
			case '9':
				*length -= 1;
				break;
			default:
				break;
			}
			*length -= 4;
		}
	}

	for (int i = str.size() - 1; i >= 0; i--)
	{
		switch (str[i]) {
		case '\n':
			*length -= 1;
			break;
		default:
			return;
		}
	}
}

void inputCheck(char* ch) {
	if (_kbhit) *ch = _getch();
}

void clrScreen() {

	gotoxy(0, 0);
	for (int i = 1; i < betterCon::windowHeight; i++)
		printNTimes(betterCon::windowWidth);
	gotoxy(0, 0);
}

void clrLn() {
	cout << "\r";
	printNTimes(betterCon::windowWidth);
	cout << "\r";
}

void prtStrWithoutColour(string str) { // untested but should work

	for (int i = 0; i < str.size(); i++)
	{
		if (str[i] == '\x1B') {
			while (str[i] != 'm')
				i++;
			i++;
			if (str[i] == '\n')
				break;
		}
		cout << str[i];
	}
}

// Code courtesy of Inspectable from StackOverflow
typedef LONG NTSTATUS, * PNTSTATUS;
#define STATUS_SUCCESS (0x00000000)

typedef NTSTATUS(WINAPI* RtlGetVersionPtr)(PRTL_OSVERSIONINFOW);

RTL_OSVERSIONINFOW GetRealOSVersion() {
	HMODULE hMod = ::GetModuleHandleW(L"ntdll.dll");
	if (hMod) {
		RtlGetVersionPtr fxPtr = (RtlGetVersionPtr)::GetProcAddress(hMod, "RtlGetVersion");
		if (fxPtr != nullptr) {
			RTL_OSVERSIONINFOW rovi = { 0 };
			rovi.dwOSVersionInfoSize = sizeof(rovi);
			if (STATUS_SUCCESS == fxPtr(&rovi))
				return rovi;
		}
	}
	RTL_OSVERSIONINFOW rovi = { 0 };
	return rovi;
}