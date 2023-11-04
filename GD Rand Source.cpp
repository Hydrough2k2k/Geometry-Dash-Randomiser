#include <iostream>
#include <sstream>
#include <fstream>
#include <cstring>
#include <string>
#include <vector>
#include <ctime>
#include <direct.h>
#include <windows.h>
#include "betterConsole.h"
#include "stringMod.h"

const string appVersion = "1.0.8";

using std::cout; std::string;

//----------------------------------------------------------------------

const string configText[] = {
	"Text Randomisation",
	"Menu Texture Randomisation",
	"Icon Texture Randomisation",
	"Block Texture Randomisation",
	"Other settings"
};

string textRandSettings[] = {
	"Enabled",
	"Randomisation Mode",
	"Duplicate Characters"
};

string menuRandSettings[] = {
	"Enabled",
	"Duplicate Buttons"
};

string iconRandSettings[] = {
	"Enabled",
	"Duplicate Icons"
};

string blockRandSettings[] = {
	"Enabled",
	"Duplicate Blocks"
};

string miscSettings[] = {
	"",
	"Seed",
	"Randomised Qualities",
	"Delete Old Files"
};

//----------------------------------------------------------------------

#define primaryMenuElementCount 6
#define secondaryMenuMaxElementCount 9
#define totalPossibleInputs 255

//----------------------------------------------------------------------

typedef enum readValue { Invalid, True, False, Default };
typedef enum menuElementType { Nothing, OnOffSwitch, Switch, Button, Textbox, NumberBox, Slider };

typedef enum inputType { Undefined, Up, Down, Left, Right, Tab, Enter, Esc };
inputType userInputRefTable[totalPossibleInputs];

//----------------------------------------------------------------------

struct Vector2 {
	int X;
	int Y;
};

struct position {
	int primaryColumn;
	int secondaryColumn;
};

struct menuData {
	int width;
	int height;
	int textOffset;
	Vector2 location;
};

struct menuElementData {
	menuElementType type = Nothing;
	bool isEnabled = false;
	Vector2 location = { 0, 0 };
	size_t data = 0;
	size_t dataLimit = UINT_MAX;
	int switchState = 0;
	int minSwitchState = 0;
	int maxSwitchState = 0;
	bool dynamicDescription = false;
};

menuData leftMenu = { 53, 20, 9, {0, 11} };
menuData rightMenu = { 66, 20, 8, {55, 11} };
menuData bottomMenu = { 120, 20, 0, {0, 11} };

menuElementData menuElement[primaryMenuElementCount][secondaryMenuMaxElementCount + 1];
vector<string> description[primaryMenuElementCount][secondaryMenuMaxElementCount + 1];
// The amount of settings in each submenu
int secondaryMenuElementCount[primaryMenuElementCount] = { 2, 1, 1, 1, 3, 0 };

position cursor = { 0, 0 };
Vector2 printCursor = { leftMenu.location.X, leftMenu.location.Y + 1 };
int cursorPrintOffset = 3;
int activeColumn = 0;

int windowWidth = 120, windowHeight = 30;

string inputFileName = "";
string outputFileName = "";

const string descFileName = "Data\\descriptions.json";
const string configFileName = "Data\\config.json";
const string missingFilesFileName = "Missing Files.txt";
const string recentSeedsFileName = "Recent Seeds.txt";

unsigned int seed = 0;

//----------------------------------------------------------------------

const string plistExtensions[] = {
	".plist",
	"-hd.plist",
	"-uhd.plist"
};

const string fntExtensions[] = {
	".fnt",
	"-hd.fnt",
	"-uhd.fnt"
};

const string miscStrings[] = {
	"Original files\\",
	"Randomised files\\"
};

const string gameSheetNames[] = {
	"GJ_GameSheet03",
	"GJ_GameSheet04",
	"GJ_GameSheet02",
	"GJ_GameSheet",
	"GJ_GameSheetGlow",
	"FireSheet_01"
};

const string textFileNames[] = {
	"bigFont",
	"gjFont",
	"goldFont",
	"chatFont"
};

//----------------------------------------------------------------------

string MainSectionPrint[] = {
	BRCYAN "Geometry Dash Randomiser " RESET "- by " BRCYAN "Hydrough" RESET " v" + appVersion + "\n\n",
	"This allows you to randomise many things in " CYAN "Geometry Dash" RESET ", textures, sounds, whatever you like\n",
	"Some changes are weird, others might make the game " YELLOW "unstable" RESET ". Crashes could happen occasionally\n\n",
	"You can " GREEN "report bugs " RESET "or crashes on " GREEN "GitHub/Hydrough2k2k " RESET "or on Discord: " GREEN "hydrough_7165" RESET "\n",
	"Other socials: " RED "Youtube" RESET ":" CYAN " https://tinyurl.com/Hydrough" RESET ", Discord Server: " CYAN "https://discord.gg/GNQkgRN" RESET "\n\n"
};

string mainSettings[] = {
	"Text radomisation:",
	"Menu Texture Randomisation:",
	"Icon Texture Randomisation:",
	"Block Texture Randomisation:",
	"Other Settings",
	"Randomise!"
};

string secondarySettings[primaryMenuElementCount][secondaryMenuMaxElementCount] = {
	{
		CYAN "Text Randomisation Settings" RESET,
		"Randomisation Mode:",
		"Duplicate Characters:"
	},
	{
		CYAN "Menu Texture Randomisation Settings" RESET,
		"Duplicate Buttons:" RESET
	},
	{
		CYAN "Icon Texture Randomisation Settings" RESET,
		"Duplicate Icons: " RESET
	},
	{
		CYAN "Block Texture Randomisation Settings" RESET,
		"Duplicate Blocks: " RESET
	},
	{
		CYAN "Other Settings Menu" RESET,
		"Seed:",
		"Texture Qualities:",
		"Delete Old Files:"
	},
	{
		GREEN "Press Enter to start randomising" RESET,
		"Depending on your settings, randomising could take a while",
		"Just sit back, relax and watch the progressbars move!"
	}
};

string missingFilesPrint[] = {
	" files are missing from the \"" CYAN "Original files" RESET "\" folder",
	"You can get them from " GREEN "Geometry Dash" RESET "\\" GREEN "Resources" RESET,
	"",
	"You can find the missing file names in the \"" CYAN "Missing Files.txt" RESET "\" file",
	"Put those into the \"" CYAN "Original Files" RESET "\" folder to allow this app to randomise them"
};

string randomisationComplete[] = {
	"Files were successfully Randomised" RESET,
	"",
	"You can copy the randomised files from the \"" CYAN "Randomised files" RESET "\" folder in the exe's folder",
	"Then go to " GREEN "Steam" RESET ", right click on " GREEN "Geometry Dash" RESET " >> " GREEN "Manage" RESET " >> " GREEN "Browse local files" RESET,
	"Open the " CYAN "Resources" RESET " folder and paste the files in there, then click \"" YELLOW "Replace all\"" RESET,
	"",
	"If you want to revert the game files to the original ones copy the files from \"" CYAN "Original files" RESET "\"" ,
	"",
	"Anyways, enjoy the chaos :) - Press " GREEN "Enter" RESET " to start randomising again"
};

string qualitySwitchPrint[] = {
	YELLOW " ",
	"High only" RESET,
	"Medium only" RESET,
	"Medium and High" RESET,
	"Low only" RESET,
	"Low and High" RESET,
	"Low and Medium" RESET,
	"Low, Medium and High" RESET
};

string textRandSwitchPrint[] = {
	YELLOW " ",
	"Standard" RESET,
	"Broken Text" RESET
};

string cancelledStr = YELLOW "Randomisation cancelled" RESET;
string enabledStr = GREEN " Enabled" RESET;
string disabledStr = RED " Disabled" RESET;

//----------------------------------------------------------------------

void mainMenu();

void headPrint();
void mainSettingsPrint();
void secondarySettingsPrint();

int UIControl();
void moveCursorUp();
void moveCursorDown();
void switchColumn();
void changeSettings();
bool numBox(char);
void numBoxLimitCheck(bool);
void switchStatePrint(int);
void printDescription();

void randomiseStuff();
int fontRansomisation(fstream*, int*, bool);
void fontFileNameFill(int, int);
int randomiseFont();
int brokenText();

int textureRandomisation(fstream*, int*, bool, bool, bool);
void textureFileNameFill(int, int);
int readTextureData(bool);

int readConfigFile();
void readRandConfig(fstream*, int, string* settingArray, unsigned int);
readValue readTrueOrFalse(string);
int writeConfigFile();

void readDescriptions();
void makeDirectories();
void updateRecentSeedsFile();
void deleteOldFiles();

void configureInputLogic();
void configureValues();
void setUpDefaultValues();

void clrLeftSide();
void clrRightSide();
void clrBottomHalf();
void drawRightScrollBar();
void drawLeftScrollBar();

void strRandom();
int newRandChar();
void randomiseString(string&, int);

//----------------------------------------------------------------------

void main(void) {

	consoleSetup(true, 120, 30);

	readDescriptions();
	makeDirectories();
	configureInputLogic();
	configureValues();
	setUpDefaultValues();

	if (readConfigFile() == 0 && rand() % 25 == 0) {
		strRandom();
		for (int i = 0; i < 10; i++) {
			if (rand() % 4 == 0)
				strRandom();
			else break;
		}
	}

	mainMenu();
}

void mainMenu() {

	headPrint();

	int retval = UIControl();

	clrBottomHalf();
	gotoxy(bottomMenu.location.X, bottomMenu.location.Y - 1);
	printNTimes(windowWidth, 205);
	if (retval != 0) {
		gotoxy(bottomMenu.location.X, bottomMenu.location.Y + 1);
		printCenter(cancelledStr);
		_getch();
		return;
	}

	if (menuElement[primaryMenuElementCount - 2][3].isEnabled)
		deleteOldFiles();

	randomiseStuff();
	updateRecentSeedsFile();
	char ch = _getch();
	if (userInputRefTable[ch] == Enter)
		mainMenu();
}

//----------------------------------------------------------------------

void headPrint() {

	gotoxy(0, 2);
	for (int i = 0; i < sizeof(MainSectionPrint) / sizeof(MainSectionPrint[0]); i++)
		printCenter(MainSectionPrint[i]);

	gotoxy(bottomMenu.location.X, bottomMenu.location.Y - 1);
	printNTimes(leftMenu.width, 205);
	printNTimes(1, 203);
	printNTimes(rightMenu.width, 205);

	for (int i = windowHeight - bottomMenu.height + 1; i < windowHeight + 1; i++)
	{
		gotoxy(leftMenu.width + 1, i);
		cout << char(186);
	}
}

void mainSettingsPrint() {

	for (int i = 0; i < primaryMenuElementCount; i++)
	{
		gotoxy(menuElement[i][0].location.X, menuElement[i][0].location.Y);
		cout << mainSettings[i];
		if (i == primaryMenuElementCount - 1)
			break;
		if (menuElement[i][0].type == OnOffSwitch) {
			menuElement[i][0].isEnabled ? cout << enabledStr : cout << disabledStr;
		}
	}
}

void secondarySettingsPrint() {

	string printStr;
	gotoxy(rightMenu.location.X, rightMenu.location.Y + 1);
	printCenter(secondarySettings[cursor.primaryColumn][0], rightMenu.width);

	for (int i = 1; i < sizeof(secondarySettings[cursor.primaryColumn]) / sizeof(secondarySettings[cursor.primaryColumn][0].size()); i++)
	{
		if (secondarySettings[cursor.primaryColumn][i] == "")
			break;

		if (cursor.primaryColumn != primaryMenuElementCount - 1) {
			gotoxy(rightMenu.location.X + rightMenu.textOffset, rightMenu.location.Y + 2 + (i * 2));
			cout << secondarySettings[cursor.primaryColumn][i];

			if (menuElement[cursor.primaryColumn][i].type == OnOffSwitch)
				menuElement[cursor.primaryColumn][i].isEnabled ? cout << enabledStr : cout << disabledStr;

			else if (menuElement[cursor.primaryColumn][i].type == NumberBox) {
				numToStrSeparated(printStr, menuElement[cursor.primaryColumn][i].data);
				printNTimes(14 - printStr.size());
				cout << BRBLUE " " << printStr << RESET;
			}
			else if (menuElement[cursor.primaryColumn][i].type == Switch)
				switchStatePrint(i);
		}
		else {
			gotoxy(rightMenu.location.X, rightMenu.location.Y + 2 + (i * 2));
			printCenter(secondarySettings[cursor.primaryColumn][i], rightMenu.width);
		}
	}
}

int UIControl() {

	inputType userInput;

	while (true) {
		
		clrLeftSide();
		mainSettingsPrint();

		clrRightSide();
		secondarySettingsPrint();
		printDescription();

		if (activeColumn == 0) {
			gotoxy(menuElement[cursor.primaryColumn][0].location.X - cursorPrintOffset, menuElement[cursor.primaryColumn][0].location.Y);
			cout << ">>";
		}
		else {
			gotoxy(menuElement[cursor.primaryColumn][cursor.secondaryColumn].location.X - cursorPrintOffset, menuElement[cursor.primaryColumn][cursor.secondaryColumn].location.Y);
			cout << ">>";
		}

		do {
			char ch = _getch();
			userInput = userInputRefTable[ch];
			if (userInput != Undefined)
				break;
			if (menuElement[cursor.primaryColumn][cursor.secondaryColumn].type == NumberBox) {
				if (numBox(ch)) {
					writeConfigFile();
					break;
				}
			}
		} while (true);

		switch (userInput)
		{
		case Up:
			moveCursorUp();
			break;
		case Down:
			moveCursorDown();
			break;
		case Right:
		case Left:
		case Tab:
			switchColumn();
			break;
		case Enter:
			if (cursor.primaryColumn == primaryMenuElementCount - 1)
				return 0;
			changeSettings();
			break;
		case Esc:
			return -1;
		default:
			break;
		}
	}
}

void moveCursorUp() {
	if (activeColumn == 0) {
		cursor.primaryColumn--;
		if (cursor.primaryColumn < 0)
			cursor.primaryColumn = primaryMenuElementCount - 1;
	}
	else {
		cursor.secondaryColumn--;
		if (cursor.secondaryColumn <= 0)
			cursor.secondaryColumn = secondaryMenuElementCount[cursor.primaryColumn];
	}
}

void moveCursorDown() {
	if (activeColumn == 0) {
		cursor.primaryColumn++;
		if (cursor.primaryColumn > primaryMenuElementCount - 1)
			cursor.primaryColumn = 0;
	}
	else {
		cursor.secondaryColumn++;
		if (cursor.secondaryColumn > secondaryMenuElementCount[cursor.primaryColumn])
			cursor.secondaryColumn = 1;
	}
}

void switchColumn() {
	if (secondaryMenuElementCount[cursor.primaryColumn] == 0)
		return;

	if (activeColumn == 0) {
		activeColumn = 1;
		cursor.secondaryColumn = 1;
	}
	else {
		activeColumn = 0;
		cursor.secondaryColumn = 0;
	}
}

void changeSettings() {
	if (menuElement[cursor.primaryColumn][cursor.secondaryColumn].type == OnOffSwitch)
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].isEnabled = !menuElement[cursor.primaryColumn][cursor.secondaryColumn].isEnabled;

	else if (menuElement[cursor.primaryColumn][cursor.secondaryColumn].type == Switch) {
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].switchState++;

		if (menuElement[cursor.primaryColumn][cursor.secondaryColumn].switchState > menuElement[cursor.primaryColumn][cursor.secondaryColumn].maxSwitchState)
			menuElement[cursor.primaryColumn][cursor.secondaryColumn].switchState = menuElement[cursor.primaryColumn][cursor.secondaryColumn].minSwitchState;
	}
	writeConfigFile();
}

bool numBox(char ch) {
	switch (ch)
	{
	case 8:
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].data /= 10;
		return true;
	case 43:
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].data += 1;
		numBoxLimitCheck(true);
		return true;
	case 45:
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].data -= 1;
		numBoxLimitCheck(false);
		return true;
	case -32:
		if (_getch() == 83) { // Esc
			menuElement[cursor.primaryColumn][cursor.secondaryColumn].data = 0;
			return true;
		}
	default:
		break;
	}
	if (ch >= 48 && ch < 58) {
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].data *= 10;
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].data += (ch - 48);
		numBoxLimitCheck(false);
		return true;
	}
	return false;
}

void numBoxLimitCheck(bool overflow) {
	if (menuElement[cursor.primaryColumn][cursor.secondaryColumn].data > menuElement[cursor.primaryColumn][cursor.secondaryColumn].dataLimit) {
		if (overflow) menuElement[cursor.primaryColumn][cursor.secondaryColumn].data = 0;
		else menuElement[cursor.primaryColumn][cursor.secondaryColumn].data = menuElement[cursor.primaryColumn][cursor.secondaryColumn].dataLimit;
	}
}

void switchStatePrint(int secondaryColumn) {

	switch (cursor.primaryColumn, secondaryColumn)
	{
	case (primaryMenuElementCount - 2, 2):
		cout << qualitySwitchPrint[0] << qualitySwitchPrint[menuElement[primaryMenuElementCount - 2][2].switchState];
		break;
	case (0, 1):
		cout << textRandSwitchPrint[0] << textRandSwitchPrint[menuElement[0][1].switchState];
		break;
	default:
		break;
	}
}

void printDescription() {

	int descriptionIndex = 0;
	int vectorSize = description[cursor.primaryColumn][cursor.secondaryColumn].size();
	bool dynamicDescription = menuElement[cursor.primaryColumn][cursor.secondaryColumn].dynamicDescription;

	if (dynamicDescription)
		descriptionIndex = menuElement[cursor.primaryColumn][cursor.secondaryColumn].switchState - 1;

	if (vectorSize - 1 < descriptionIndex)
		return;

	string stringCopy = description[cursor.primaryColumn][cursor.secondaryColumn][descriptionIndex];
	if (stringCopy.size() == 0)
		return;

	int areaWidth = 64;
	vector<string> stringlets = splitString(stringCopy, areaWidth);

	for (int i = 0; i < stringlets.size(); i++)
	{
		gotoxy(rightMenu.location.X + 1, rightMenu.location.Y + rightMenu.height - 1 - (int)stringlets.size() + i);
		printCenter(stringlets[i], areaWidth);
	}
	vector<string>().swap(stringlets);
}

//----------------------------------------------------------------------

void randomiseStuff() {

	fstream missingFilesFile;
	missingFilesFile.open(missingFilesFileName, ios::out);

	if (menuElement[primaryMenuElementCount - 2][1].data == 0) {
		for (int i = 0; i < 9; i++)
			seed = (seed * 10) + rand() % 10;
	}
	else
		seed = menuElement[primaryMenuElementCount - 2][1].data;

	srand(seed);

	int missingFiles = 0, successfullyRandomised = 0;

	// Starts randomising if the settings are enabled
	missingFiles += (fontRansomisation(&missingFilesFile, &successfullyRandomised, menuElement[0][0].isEnabled));
	missingFiles += (textureRandomisation(&missingFilesFile, &successfullyRandomised, menuElement[1][0].isEnabled, menuElement[2][0].isEnabled, menuElement[3][0].isEnabled));

	clrBottomHalf();

	if (missingFiles != 0) {
		string buf = to_string(missingFiles) + missingFilesPrint[0];
		gotoxy(bottomMenu.location.X, bottomMenu.location.Y + 1);
		printCenter(buf);

		for (int i = 1; i < sizeof(missingFilesPrint) / sizeof(missingFilesPrint[0]); i++)
		{
			gotoxy(bottomMenu.location.X, bottomMenu.location.Y + 1 + i);
			if (missingFilesPrint[i].size() != 0)
				printCenter(missingFilesPrint[i]);
		}
	}

	string printString = GREEN + to_string(successfullyRandomised) + " ";

	for (int i = 0; i < sizeof(randomisationComplete) / sizeof(randomisationComplete[0]); i++)
	{
		printString += randomisationComplete[i];
		if (i == 0)
			printString += CYAN " (Seed: " + to_string(seed) + ")" RESET;
		gotoxy(bottomMenu.location.X, bottomMenu.location.Y + bottomMenu.height - 2 - (int)(sizeof(randomisationComplete) / sizeof(randomisationComplete[0])) + i);
		if (printString.size() != 0)
			printCenter(printString, bottomMenu.width);
		printString = "";
	}

	missingFilesFile.close();
	if (missingFiles == 0)
		std::remove("MissingFiles.txt");
}

//----------------------------------------------------------------------

int fontRansomisation(fstream* missingFiles, int* successful, bool textRand) {

	if (!textRand)
		return 0;

	bool lowQuality = (menuElement[primaryMenuElementCount - 2][2].switchState >> 2) % 2;
	bool mediumQuality = (menuElement[primaryMenuElementCount - 2][2].switchState >> 1) % 2;
	bool highQuality = (menuElement[primaryMenuElementCount - 2][2].switchState >> 0) % 2;

	int totalTextFileTypes = 14, textureQualities = 3;
	int totalTextFiles = totalTextFileTypes * lowQuality + totalTextFileTypes * mediumQuality + totalTextFileTypes * highQuality;
	int randomisedTextFiles = 0, missingTexturesFiles = 0;

	int retval;

	for (int quality = 0; quality < textureQualities; quality++)
	{
		if (!lowQuality && quality == 0) quality++;
		if (!mediumQuality && quality == 1) quality++;
		if (!highQuality && quality == 2) break;

		for (int textureID = 0; textureID < totalTextFileTypes; textureID++)
		{
			gotoxy(printCursor.X, printCursor.Y);
			cout << " - - Text Randomisation Progress: " << randomisedTextFiles << " / " << totalTextFiles << " files complete";

			fontFileNameFill(textureID, quality);

			if (menuElement[0][1].switchState == 1)
				retval = randomiseFont();
			else retval = brokenText();

			if (retval != 0) {
				*missingFiles << inputFileName + "\n";
				missingTexturesFiles++;
			}
			else {
				*successful += 1;
				randomisedTextFiles++;
			}
		}
	}

	gotoxy(printCursor.X, printCursor.Y);
	printNTimes(120, ' ');
	gotoxy(printCursor.X, printCursor.Y);
	cout << " - - Text Randomisation complete!";
	printCursor.Y += 2;

	return missingTexturesFiles;
}

void fontFileNameFill(int fontID, int fileExtension) {

	string buf;
	if (fontID == 0)
		buf = textFileNames[0];
	else if (fontID < 12) {
		buf = textFileNames[1];
		buf += (to_string(fontID / 10) + to_string(fontID % 10));
	}
	else
		buf = textFileNames[fontID - 10];
	buf += fntExtensions[fileExtension];

	inputFileName = buf;
	outputFileName = buf;
}

int randomiseFont() {

	fstream originalFile;
	fstream editedFile;

	// If the file doesn't exist or cant be opened, skip it
	originalFile.open(miscStrings[0] + inputFileName, ios::in);
	if (!originalFile.is_open())
		return -1;
	editedFile.open(miscStrings[1] + outputFileName, ios::out);
	if (!editedFile.is_open())
		return -2;

	vector<string> charID;
	vector<string> charStats;
	vector<bool> available;

	int IDs = 0;
	string readStr, stringCpy;

	// Scan the file line by line, extract and store character IDs and Character Stats
	while (getline(originalFile, readStr)) {
		stringCpy = readStr;
		stringCpy.erase(8, stringCpy.size());

		// If a line start with "char id=" split it into 2 parts, ending at the first 'x'
		if (stringCpy == "char id=") {
			charID.push_back("");
			charStats.push_back(readStr);
			available.push_back(true);

			while (charStats[IDs][0] != 'x') {
				charID[IDs] += charStats[IDs][0];
				charStats[IDs].erase(0, 1);
			}
			IDs++;
		}
	}

	// Close and reopen file to start reading from the start
	originalFile.close();
	originalFile.open(miscStrings[0] + inputFileName, ios::in);
	if (!originalFile.is_open())
		return -1;

	int random, randomisedItems = 0;

	// Start writing the new file
	while (getline(originalFile, readStr)) {
		stringCpy = readStr;
		stringCpy.erase(8, stringCpy.size());
		if (stringCpy == "char id=") {

			// Randomise where the stored stings go in the file
			do {
				random = rand() % IDs;
			} while (!available[random]);

			// Start wiriting the file
			if (available[random]) {
				editedFile << charID[randomisedItems] << charStats[random] << "\n";
				if (!menuElement[0][2].isEnabled) // Text Duplicates
					available[random] = false;
				randomisedItems++;
			}
		}
		else {
			editedFile << readStr << "\n";
		}
	}

	// Clear allocated memory
	vector<string>().swap(charID);
	vector<string>().swap(charStats);
	vector<bool>().swap(available);

	originalFile.close();
	editedFile.close();
	return 0;
}

int brokenText() {

	fstream originalFile;
	fstream editedFile;

	// If the file doesn't exist or cant be opened, skip it
	originalFile.open(miscStrings[0] + inputFileName, ios::in);
	if (!originalFile.is_open())
		return -1;
	editedFile.open(miscStrings[1] + outputFileName, ios::out);
	if (!editedFile.is_open())
		return -2;

	string readStr, stringCpy;
	int brokenCharProbability = rand() % 5 + 3;

	// Scan the file line by line, extract and store character IDs and Character Stats
	while (getline(originalFile, readStr)) {
		stringCpy = readStr;
		stringCpy.erase(8, stringCpy.size());

		// If a line start with "char id=" split it into 2 parts, ending at the first 'x'
		if (stringCpy == "char id=") {
			if ((rand() % brokenCharProbability) < brokenCharProbability - 1)
				editedFile << readStr << "\n";
		}
		else editedFile << readStr << "\n";
	}
	return 0;
}

//----------------------------------------------------------------------

int textureRandomisation(fstream* missingFiles, int* successful, bool menuRand, bool iconRand, bool blockRand) {

	if (!menuRand && !iconRand && !blockRand)
		return 0;

	int retval, missingFileCount = 0;
	int textureQualityCount = 3, totalFileTypes = sizeof(gameSheetNames) / sizeof(gameSheetNames[0]);

	bool lowQuality = (menuElement[primaryMenuElementCount - 2][2].switchState >> 2) % 2;
	bool mediumQuality = (menuElement[primaryMenuElementCount - 2][2].switchState >> 1) % 2;
	bool highQuality = (menuElement[primaryMenuElementCount - 2][2].switchState >> 0) % 2;

	for (int quality = 0; quality < textureQualityCount; quality++)
	{
		if (!lowQuality && quality == 0) quality++;
		if (!mediumQuality && quality == 1) quality++;
		if (!highQuality && quality == 2) break;

		for (int gameSheetID = 0; gameSheetID < totalFileTypes; gameSheetID++)
		{
			if (!menuRand && gameSheetID < 2)
				gameSheetID = 2;
			if (!iconRand && gameSheetID >= 2 && gameSheetID < 3)
				gameSheetID = 3;
			if (!blockRand && gameSheetID >= 3 && gameSheetID < 6)
				break;

			textureFileNameFill(gameSheetID, quality);

			if (gameSheetID < 2)
				retval = readTextureData(menuElement[1][1].isEnabled);
			else if (gameSheetID < 3)
				retval = readTextureData(menuElement[2][1].isEnabled);
			else if (gameSheetID < 6)
				retval = readTextureData(menuElement[3][1].isEnabled);

			if (retval != 0) {
				*missingFiles << inputFileName + "\n";
				missingFileCount++;
			}
			else {
				*successful += 1;
			}
		}
	}

	gotoxy(printCursor.X, printCursor.Y);
	printNTimes(100, ' ');
	gotoxy(printCursor.X, printCursor.Y);
	cout << " - - Texture Randomisation complete!";
	printCursor.Y += 2;

	return missingFileCount;
}

void textureFileNameFill(int TextureFileID, int fileExtension) {
	inputFileName = gameSheetNames[TextureFileID] + plistExtensions[fileExtension];
	outputFileName = gameSheetNames[TextureFileID] + plistExtensions[fileExtension];
}

int readTextureData(bool duplicatesEnabled) {

	string buf, randText = " - - Randomising ";

	gotoxy(printCursor.X, printCursor.Y);
	printNTimes(100, ' ');
	gotoxy(printCursor.X, printCursor.Y);
	cout << randText + inputFileName;

	fstream originalFile;
	fstream editedFile;

	originalFile.open(miscStrings[0] + inputFileName, ios::in);
	if (!originalFile.is_open())
		return -1;

	editedFile.open(miscStrings[1] + outputFileName, ios::out);
	if (!originalFile.is_open())
		return -2;

	string startReferenceString = ".png</key>";
	string endReferenceString = "</dict>";
	vector<string> textureData;
	vector<bool> available;

	string readStr, stringCpy;

	// Extracting relevant data from file
	while (getline(originalFile, readStr)) {
		stringCpy = readStr;
		stringCpy.erase(0, stringCpy.size() - startReferenceString.size());

		if (stringCpy == startReferenceString) {
			available.push_back(true);
			textureData.push_back("");

			while (getline(originalFile, readStr))
			{
				string stringCpy = readStr;
				stringCpy.erase(0, stringCpy.size() - endReferenceString.size());

				if (stringCpy == endReferenceString) {
					textureData[textureData.size() - 1] += readStr + "\n";
					break;
				}
				textureData[textureData.size() - 1] += readStr + "\n";
			}
		}
	}

	originalFile.close();
	originalFile.open(miscStrings[0] + inputFileName, ios::in);
	if (!originalFile.is_open())
		return -1;

	int totalObjects = textureData.size();
	int randomisedTextures = 0, random;

	while (getline(originalFile, readStr)) {

		if (rand() % 16 == 0) {
			gotoxy(printCursor.X, printCursor.Y);
			printNTimes(randText.size() + inputFileName.size() + buf.size() - 3);
			gotoxy(printCursor.X, printCursor.Y);
			buf = " | Progress: " + to_string(randomisedTextures) + " / " + to_string(totalObjects);
			cout << randText + inputFileName + buf;
		}

		string stringCpy = readStr;
		stringCpy.erase(0, stringCpy.size() - startReferenceString.size());

		if (stringCpy != startReferenceString) {
			editedFile << readStr << "\n";
		}
		else {
			do {
				random = rand() % textureData.size();
			} while (!available[random]);
			editedFile << readStr << "\n";
			editedFile << textureData[random];

			if (!duplicatesEnabled)
				available[random] = false;

			while (getline(originalFile, readStr)) {
				removeWhitespace(readStr);
				if (readStr == endReferenceString)
					break;
			}
		}
		randomisedTextures++;
	}

	vector<string>().swap(textureData);
	vector<bool>().swap(available);

	originalFile.close();
	editedFile.close();

	return 0;
}

//----------------------------------------------------------------------

int readConfigFile() {

	fstream configFile;
	int settingType;
	unsigned int size;
	string readStr;

	configFile.open(configFileName, ios::in);
	if (!configFile.is_open()) {
		writeConfigFile();
		return -1;
	}

	while (getline(configFile, readStr)) {
		settingType = -1;
		removeWhitespace(readStr);
		removeSpecialChars(readStr);

		for (int i = 0; i < sizeof(configText) / sizeof(configText[0]); i++)
		{
			if (configText[i] == readStr) {
				settingType = i;
				break;
			}
		}

		switch (settingType)
		{
		case 0:
			size = sizeof(textRandSettings) / sizeof(textRandSettings[0]);
			readRandConfig(&configFile, settingType, textRandSettings, size);
			break;
		case 1:
			size = sizeof(menuRandSettings) / sizeof(menuRandSettings[0]);
			readRandConfig(&configFile, settingType, menuRandSettings, size);
			break;
		case 2:
			size = sizeof(iconRandSettings) / sizeof(iconRandSettings[0]);
			readRandConfig(&configFile, settingType, iconRandSettings, size);
			break;
		case 3:
			size = sizeof(blockRandSettings) / sizeof(blockRandSettings[0]);
			readRandConfig(&configFile, settingType, blockRandSettings, size);
			break;
		case 4:
			size = sizeof(miscSettings) / sizeof(miscSettings[0]);
			readRandConfig(&configFile, settingType, miscSettings, size);
			break;
		default:
			break;
		}
	}

	configFile.close();
	return 0;
}

void readRandConfig(fstream* configFile, int settingType, string* settingArray, unsigned int size) {

	string readStr, stringlet;
	readValue val;

	while (getline(*configFile, readStr)) {
		removeWhitespace(readStr);

		if (readStr == "}" || readStr == "},")
			break;

		splitStringAt(readStr, stringlet, ':');
		removeSpecialChars(readStr);
		removeSpecialChars(stringlet);

		int j = 0;
		for (; j < size; j++)
			if (readStr == settingArray[j])
				break;

		if (menuElement[settingType][j].type == OnOffSwitch) {
			val = readTrueOrFalse(stringlet);
			if (val == True)
				menuElement[settingType][j].isEnabled = true;
			else if (val == False)
				menuElement[settingType][j].isEnabled = false;
		}
		else if (menuElement[settingType][j].type == NumberBox && stringlet.size() < 19) {
			if (stoull(stringlet) > UINT_MAX || stoull(stringlet) > menuElement[settingType][j].dataLimit)
				menuElement[settingType][j].data = 0;
			else
				menuElement[settingType][j].data = stoul(stringlet);
		}
		else if (menuElement[settingType][j].type == Switch) {
			removeNonDigits(stringlet);
			if (stringlet.size() != 0 && stoull(stringlet) <= menuElement[settingType][j].maxSwitchState)
				menuElement[settingType][j].switchState = stoul(stringlet);
		}
	}
}

readValue readTrueOrFalse(string str) {

	if (str == " true")
		return True;
	else if (str == " false")
		return False;
	else
		return Default;
}

int writeConfigFile() {

	fstream configFile;
	vector<string> writeStr;

	configFile.open(configFileName, ios::out);
	if (!configFile.is_open())
		return -1;

	configFile << "{\n\t\"Randomisation Settings\":,\n\t\"App Version\": \"" + appVersion + "\"\n\t{\n";

	for (int i = 1; i < (sizeof(configText) / sizeof(configText[0])) + 1; i++)
	{
		configFile << "\t\t\"" + configText[i - 1] + "\":\n\t\t{\n";

		switch (i)
		{
		case 1:
			for (int j = 0; j < sizeof(textRandSettings) / sizeof(textRandSettings[0]); j++)
				writeStr.push_back(textRandSettings[j]);
			break;
		case 2:
			for (int j = 0; j < sizeof(menuRandSettings) / sizeof(menuRandSettings[0]); j++)
				writeStr.push_back(menuRandSettings[j]);
			break;
		case 3:
			for (int j = 0; j < sizeof(iconRandSettings) / sizeof(iconRandSettings[0]); j++)
				writeStr.push_back(iconRandSettings[j]);
			break;
		case 4:
			for (int j = 0; j < sizeof(blockRandSettings) / sizeof(blockRandSettings[0]); j++)
				writeStr.push_back(blockRandSettings[j]);
			break;
		case 5:
			for (int j = 0; j < sizeof(miscSettings) / sizeof(miscSettings[0]); j++)
				writeStr.push_back(miscSettings[j]);
			break;
		default:
			break;
		}
		for (int j = 0; j < writeStr.size(); j++)
		{
			if (menuElement[i - 1][j].type != Nothing) {
				configFile << "\t\t\t\"" + writeStr[j] + "\": ";
				if (menuElement[i - 1][j].type == OnOffSwitch)
					configFile << std::boolalpha << menuElement[i - 1][j].isEnabled;
				else if (menuElement[i - 1][j].type == NumberBox)
					configFile << menuElement[i - 1][j].data;
				else if (menuElement[i - 1][j].type == Switch)
					configFile << menuElement[i - 1][j].switchState;

				if (j + 1 < writeStr.size())
					configFile << ",";
				configFile << "\n";
			}
		}
		vector<string>().swap(writeStr);
		configFile << "\t\t}";

		if (i < sizeof(configText) / sizeof(configText[0]))
			configFile << ",";
		configFile << "\n";
	}
	configFile << "\t}\n}";

	configFile.close();
	return 0;
}

void readDescriptions() {

	fstream descriptionFile;
	string readStr;

	descriptionFile.open(descFileName, ios::in);
	if (!descriptionFile.is_open())
		return;

	int indents = 0, row = 0, column = 0;

	while (getline(descriptionFile, readStr)) {
		removeWhitespace(readStr);
		if (readStr[readStr.size() - 1] == ',')
			readStr.erase(readStr.size() - 1, readStr.size());

		if (readStr == "{")
			indents++;

		else if (readStr == "}") {
			indents--;
			if (indents == 1) {
				row++;
				column = 0;
			} else if (indents == 2)
				column++;
		}
		else
		{
			if (indents > 1) {
				readStr.erase(0, 1);
				readStr.erase(readStr.size() - 1, readStr.size());
				description[row][column].push_back(readStr);
			}
			if (indents == 2)
				column++;
		}
	}

	descriptionFile.close();
}

void makeDirectories() {
	int retval;

	retval = _mkdir("Data");
	retval = _mkdir("Original files");
	retval = _mkdir("Randomised files");
}

void updateRecentSeedsFile() {

	int maxStoredSeeds = 20;
	fstream recentSeeds;
	string readStr, seedStr;

	recentSeeds.open(recentSeedsFileName, ios::in);

	vector<string> previousData;

	while (getline(recentSeeds, readStr)) {
		previousData.push_back(readStr);
	}

	recentSeeds.close();
	recentSeeds.open(recentSeedsFileName, ios::out);
	if (!recentSeeds.is_open())
		return;

	time_t t = time(0);
	tm time;
	localtime_s(&time, &t);

	seedStr = "Seed: ";
	numToStrWithPadding(seedStr, seed, 10, ' ');
	seedStr += " - ";
	numToStrWithPadding(seedStr, time.tm_year + 1900, 4);
	seedStr += "/";
	numToStrWithPadding(seedStr, time.tm_mon, 2);
	seedStr += "/";
	numToStrWithPadding(seedStr, time.tm_mday, 2);
	seedStr += " ";
	numToStrWithPadding(seedStr, time.tm_hour, 2);
	seedStr += "-";
	numToStrWithPadding(seedStr, time.tm_min, 2);
	seedStr += "-";
	numToStrWithPadding(seedStr, time.tm_sec, 2);

	recentSeeds << seedStr;

	if (previousData.size() != 0) recentSeeds << "\n";

	for (int i = 0; i < previousData.size(); i++) {
		recentSeeds << previousData[i];
		if (i == previousData.size() - 1 || i > maxStoredSeeds - 3)
			break;
		recentSeeds << "\n";
	}

	vector<string>().swap(previousData);

	recentSeeds.close();
}

void deleteOldFiles() {

	int totalTextFileTypes = 14, totalTextureFileTypes = 6, textureQualities = 3;
	string str;
	char fileName[100];

	for (int quality = 0; quality < textureQualities; quality++)
	{
		for (int fontID = 0; fontID < totalTextFileTypes; fontID++) {
			str = miscStrings[1];

			if (fontID == 0)
				str += textFileNames[0];
			else if (fontID < 12) {
				str += textFileNames[1];
				str += (to_string(fontID / 10) + to_string(fontID % 10));
			}
			else
				str += textFileNames[fontID - 10];
			str += fntExtensions[quality];

			strcpy_s(fileName, str.c_str());
			std::remove(fileName);
		}
	}

	for (int quality = 0; quality < textureQualities; quality++)
	{
		for (int textureID = 0; textureID < totalTextureFileTypes; textureID++) {
			str = miscStrings[1] + gameSheetNames[textureID] + plistExtensions[quality];

			strcpy_s(fileName, str.c_str());
			std::remove(fileName);
		}
	}
}

//----------------------------------------------------------------------

void configureInputLogic() {

	for (int i = 0; i < totalPossibleInputs; i++)
	{
		switch (i) {
		case 119:
		case 87:
		case 72:
			userInputRefTable[i] = Up;
			break;
		case 115:
		case 83:
		case 80:
			userInputRefTable[i] = Down;
			break;
		case 27:
			userInputRefTable[i] = Esc;
			break;
		case 97:
		case 65:
		case 75:
			userInputRefTable[i] = Left;
			break;
		case 100:
		case 68:
		case 77:
			userInputRefTable[i] = Right;
			break;
		case 9:
			userInputRefTable[i] = Tab;
			break;
		case 13:
			userInputRefTable[i] = Enter;
			break;
		default:
			userInputRefTable[i] = Undefined;
			break;
		}
	}
}

void configureValues() { // Incomplete, Type is not set up yet

	for (int x = 0; x < primaryMenuElementCount; x++)
	{
		for (int y = 0; y < secondaryMenuMaxElementCount + 1; y++)
		{
			if (y > secondaryMenuElementCount[x])
				break;

			if (y == 0) {
				menuElement[x][y].location.X = leftMenu.textOffset;
				menuElement[x][y].location.Y = leftMenu.location.Y + (2 * x) + 1;
			}
			else {
				menuElement[x][y].location.X = rightMenu.textOffset + rightMenu.location.X;
				menuElement[x][y].location.Y = rightMenu.location.Y + (2 * y) + 2;
			}

			menuElement[x][y].type = OnOffSwitch;
		}
	}

	// Text Randomisation Modes
	menuElement[0][1].type = Switch;
	menuElement[0][1].minSwitchState = 1;
	menuElement[0][1].maxSwitchState = 2;
	menuElement[0][1].dynamicDescription = true;

	// Other settings menu
	menuElement[primaryMenuElementCount - 2][0].location.Y = rightMenu.location.Y + rightMenu.height - 4;
	menuElement[primaryMenuElementCount - 2][0].type = Nothing;
	// Seed input
	menuElement[primaryMenuElementCount - 2][1].type = NumberBox;
	menuElement[primaryMenuElementCount - 2][1].dataLimit = UINT_MAX;
	// Texture Quality Settings
	menuElement[primaryMenuElementCount - 2][2].type = Switch;
	menuElement[primaryMenuElementCount - 2][2].minSwitchState = 1;
	menuElement[primaryMenuElementCount - 2][2].maxSwitchState = 7; // Low, Medium, High and combinations

	// Set up the "Randomise" button
	menuElement[primaryMenuElementCount - 1][0].location.Y = rightMenu.location.Y + rightMenu.height - 2;
	menuElement[primaryMenuElementCount - 1][0].type = Button;
	menuElement[primaryMenuElementCount - 1][0].isEnabled = true;
}

void setUpDefaultValues() {

	menuElement[0][0].isEnabled = true;
	menuElement[1][0].isEnabled = true;

	menuElement[0][1].switchState = 1;

	menuElement[primaryMenuElementCount - 2][2].switchState = 1;
	menuElement[primaryMenuElementCount - 2][3].isEnabled = true;
}

//----------------------------------------------------------------------

void clrLeftSide() {

	for (int i = leftMenu.location.Y; i <= windowHeight; i++)
	{
		gotoxy(leftMenu.location.X, i);
		printNTimes(leftMenu.width);
	}
}

void clrRightSide() {

	for (int i = rightMenu.location.Y; i <= windowHeight; i++)
	{
		gotoxy(rightMenu.location.X, i);
		printNTimes(rightMenu.width);
	}
}

void clrBottomHalf() {

	for (int i = leftMenu.location.Y; i <= windowHeight; i++)
	{
		gotoxy(leftMenu.location.X, i);
		printNTimes(windowWidth);
	}
}

void drawRightScrollBar() {
	Vector2 cursorGoTo = { rightMenu.location.X + rightMenu.width - 2, rightMenu.location.Y };

	for (int i = 1; i < rightMenu.height - 1; i++) {
		gotoxy(cursorGoTo.X, cursorGoTo.Y + i);
		cout << char(219); // 176, 219
	}
	gotoxy(cursorGoTo.X, cursorGoTo.Y);
	cout << char(30);
	gotoxy(cursorGoTo.X, cursorGoTo.Y + rightMenu.height);
	cout << char(31);
}

void drawLeftScrollBar() {
	Vector2 cursorGoTo = { leftMenu.location.X + leftMenu.width - 1, leftMenu.location.Y };

	for (int i = 1; i < leftMenu.height - 1; i++) {
		gotoxy(cursorGoTo.X, cursorGoTo.Y + i);
		cout << char(219); // 176, 219
	}
	gotoxy(cursorGoTo.X, cursorGoTo.Y);
	cout << char(30);
	gotoxy(cursorGoTo.X, cursorGoTo.Y + leftMenu.height);
	cout << char(31);
}

//----------------------------------------------------------------------

void strRandom() {

	for (int i = 0; i < sizeof(MainSectionPrint) / sizeof(MainSectionPrint[0]); i++)
		randomiseString(MainSectionPrint[i], MainSectionPrint[i].size());

	for (int i = 0; i < sizeof(mainSettings) / sizeof(mainSettings[0]); i++)
		randomiseString(mainSettings[i], mainSettings[i].size());

	for (int i = 0; i < primaryMenuElementCount; i++)
		for (int j = 0; j < secondaryMenuMaxElementCount; j++)
			randomiseString(secondarySettings[i][j], secondarySettings[i][j].size());

	for (int i = 0; i < sizeof(missingFilesPrint) / sizeof(missingFilesPrint[0]); i++)
		randomiseString(missingFilesPrint[i], missingFilesPrint[i].size());

	for (int i = 0; i < sizeof(randomisationComplete) / sizeof(randomisationComplete[0]); i++)
		randomiseString(randomisationComplete[i], randomisationComplete[i].size());

	for (int i = 0; i < primaryMenuElementCount - 1; i++)
		for (int j = 0; j < secondaryMenuMaxElementCount - 1; j++)
			for (int k = 0; k < (int)description[i][j].size(); k++)
				randomiseString(description[i][j][k], description[i][j][k].size());

	randomiseString(enabledStr, enabledStr.size());
	randomiseString(disabledStr, disabledStr.size());
	randomiseString(cancelledStr, cancelledStr.size());

	for (int i = 0; i < sizeof(qualitySwitchPrint) / sizeof(qualitySwitchPrint[0]); i++)
		randomiseString(qualitySwitchPrint[i], qualitySwitchPrint[i].size());

	for (int i = 0; i < sizeof(textRandSwitchPrint) / sizeof(textRandSwitchPrint[0]); i++)
		randomiseString(textRandSwitchPrint[i], textRandSwitchPrint[i].size());
}

void randomiseString(string& str, int size) {
	if (size == 0) return;
	int randChance = 8, i = 0;
	while (str[i] != '\n' && str[i] != '\0')
	{
		if (str[i] == '\x1B') {
			while (str[i] != 'm')
				i++;
			i++;
			if (str[i] == '\n' || str[i] == '\0')
				break;
		}
		if (rand() % randChance == 0)
			str[i] = newRandChar();
		i++;
	}
}

int newRandChar() {
	int random, min = 21, max = 126;
	do {
		random = (rand() % (max - min)) + min;
	} while (random == 27);
	return random;
}