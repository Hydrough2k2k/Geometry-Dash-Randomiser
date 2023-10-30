#include <iostream>
#include <sstream>
#include <fstream>
#include <cstring>
#include <string>
#include <vector>
#include <chrono>
#include <cstdio>

using std::cout; std::string;

#include <direct.h>
#include <windows.h>

#include "betterConsole.h"
#include "stringMod.h"

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
"Duplicates Allowed",
"Text Anarchy"
};

string menuRandSettings[] = {
"Enabled",
"Duplicates Allowed"
};

string iconRandSettings[] = {
"Enabled",
"Duplicates Allowed"
};

string blockRandSettings[] = {
"Enabled",
"Duplicates Allowed"
};

string miscSettings[] = {
"",
"Seed"
};

//----------------------------------------------------------------------

const string appVersion = "1.0.2";

#define primaryMenuElementCount 6
#define secondaryMenuMaxElementCount 9
#define totalPossibleInputs 255

//----------------------------------------------------------------------

typedef enum readValue { Invalid, True, False, Default };
typedef enum menuElementType { Null, OnOffSwitch, Switch, Button, Textbox, NumberBox, Slider };

typedef enum inputType { Undefined, Up, Down, Left, Right, Tab, Enter, Esc };
inputType userInputRefTable[totalPossibleInputs];

//----------------------------------------------------------------------

struct Vector2 {
	int X;
	int Y;
};

struct locations {
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
	menuElementType type = Null;
	bool isEnabled = false;
	Vector2 location = {0, 0};
	string description = "";
	size_t data = 0;
	size_t dataLimit = UINT_MAX;
};

menuData leftMenu = { 53, 20, 9, {0, 11} };
menuData rightMenu = { 66, 20, 8, {55, 11} };
menuData bottomMenu = { 120, 20, 0, {0, 11} };

menuElementData menuElement[primaryMenuElementCount][secondaryMenuMaxElementCount + 1];
int secondaryMenuElementCount[primaryMenuElementCount] = { 2, 1, 1, 1, 1, 0 };

locations cursor = { 0, 0 };
Vector2 printCursor = { leftMenu.location.X, leftMenu.location.Y + 1 };
int cursorPrintOffset = 3;
int activeColumn = 0;

int windowWidth = 120, windowHeight = 30;

string inputFileName = "";
string outputFileName = "";

string readStr = "";

const string descFileName = "Data\\descriptions.json";
const string configFileName = "Data\\config.json";
const string missingFilesFileName = "MissingFiles.txt";
const string debugFileName = "Debug.txt";

unsigned int seed;
unsigned int lastSeed;

//----------------------------------------------------------------------

const string textFileNames[] = {
	"gjFont",
	"bigFont",
	"goldFont",
	"chatFont"
};

const string fntExtensions[] = {
	".fnt",
	"-hd.fnt",
	"-uhd.fnt"
};

const string plistExtensions[] = {
	".plist",
	"-hd.plist",
	"-uhd.plist"
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
	"GJ_GameSheetGlow"
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
		"Duplicates Enabled:",
		"Text Anarchy:"
	},
	{
		CYAN "Menu Texture Randomisation Settings" RESET,
		"Duplicates Enabled: " BRBLACK "(Temporarily not available)" RESET
	},
	{
		CYAN "Icon Texture Randomisation Settings" RESET,
		"Duplicates Enabled: " BRBLACK "(Temporarily not available)" RESET
	},
	{
		CYAN "Block Texture Randomisation Settings" RESET,
		"Duplicates Enabled: " BRBLACK "(Temporarily not available)" RESET
	},
	{
		CYAN "Other Settings Menu" RESET,
		"Seed:"
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
	"Open the " CYAN "Resources" RESET " folder and paste the files in there, then click \"" YELLOW "Replace all" RESET,
	"",
	"If you want to revert the game files to the original ones copy the files from \"" CYAN "Original files" RESET "\"" ,
	"",
	"Anyways, enjoy the chaos :)"
};

string cancelledStr = YELLOW "Randomisation cancelled" RESET;
string enabledStr = GREEN " Enabled" RESET;
string disabledStr = RED " Disabled" RESET;

//----------------------------------------------------------------------

void headPrint();
void mainSettingsPrint();
void secondarySettingsPrint();

int UIControl();
void moveCursorUp();
void moveCursorDown();
void switchColumn();
void changeSettings();
bool numBox(char);
void printDescription();

void randomiseStuff();
int fontRansomisation(fstream*, int*, bool);
void fontFileNameFill(int, int, int);
int ransomiseFont();

int textureRandomisation(fstream*, int*, bool, bool, bool);
void textureFileNameFill(int);
int readTextureData(int);

void readDescriptions();
void makeDirectories();
int readConfigFile();
int writeConfigFile();
void readRandConfig(fstream*, int, string *settingArray, unsigned int);
readValue readTrueOrFalse(string);

void configureInputLogic();
void configureValues();
void setUpDefaultValues();

void clrLeftSide();
void clrRightSide();
void clrBottomHalf();

void strRandom();
int newRandChar();
void randomiseString(string&, int);

//----------------------------------------------------------------------

void main(void) {

	consoleSetup(false);

	readDescriptions();

	makeDirectories();
	configureInputLogic();
	configureValues();
	setUpDefaultValues();
	
	if (readConfigFile() == 0)
		if (rand() % 24 == 0)
			strRandom();
	
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
	randomiseStuff();
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
				if (menuElement[cursor.primaryColumn][i].data > 9)
					printNTimes(10 - log10(menuElement[cursor.primaryColumn][i].data));
				else
					printNTimes(9);
				cout << BRBLUE " " << menuElement[cursor.primaryColumn][i].data << RESET;
			}
		} else {
			gotoxy(rightMenu.location.X, rightMenu.location.Y + 2 + (i * 2));
			printCenter(secondarySettings[cursor.primaryColumn][i], rightMenu.width);
		}
	}
}

int UIControl() {

	inputType userInput;

	while (true) {

		if (activeColumn == 0) {
			clrLeftSide();
			mainSettingsPrint();
		}
		clrRightSide();
		secondarySettingsPrint();
		printDescription();

		if (activeColumn == 0) {
			gotoxy(menuElement[cursor.primaryColumn][0].location.X - cursorPrintOffset, menuElement[cursor.primaryColumn][0].location.Y);
			cout << ">>";
		} else {
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
	} else {
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
	} else {
		cursor.secondaryColumn++;
		if (cursor.secondaryColumn > secondaryMenuElementCount[cursor.primaryColumn])
			cursor.secondaryColumn = 1;
	}
}

void switchColumn() {
	if (cursor.primaryColumn == primaryMenuElementCount - 1)
		return;

	if (activeColumn == 0) {
		activeColumn = 1;
		cursor.secondaryColumn = 1;
	} else {
		activeColumn = 0;
		cursor.secondaryColumn = 0;
	}
}

void changeSettings() {
	if (menuElement[cursor.primaryColumn][cursor.secondaryColumn].type == OnOffSwitch)
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].isEnabled = !menuElement[cursor.primaryColumn][cursor.secondaryColumn].isEnabled;
	writeConfigFile();
}

bool numBox(char ch) {
	if (ch == 8) {
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].data /= 10;
		return true;
	}
	else if (ch >= 48 && ch < 58) {
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].data *= 10;
		menuElement[cursor.primaryColumn][cursor.secondaryColumn].data += (ch - 48);
		if (menuElement[cursor.primaryColumn][cursor.secondaryColumn].data > menuElement[cursor.primaryColumn][cursor.secondaryColumn].dataLimit)
			menuElement[cursor.primaryColumn][cursor.secondaryColumn].data = menuElement[cursor.primaryColumn][cursor.secondaryColumn].dataLimit;
		return true;
	}
	else if (ch == -32) {
		if (_getch() == 83) {
			menuElement[cursor.primaryColumn][cursor.secondaryColumn].data = 0;
			return true;
		}
	}
	return false;
}

void printDescription() {

	int areaWidth = 64, stringletCount = 0, erasedChars;
	vector<string> stringlets;
	string stringCopy = menuElement[cursor.primaryColumn][cursor.secondaryColumn].description;

	if (stringCopy.size() == 0)
		return;

	while (stringCopy.size() > areaWidth) {
		stringlets.push_back(stringCopy);
		stringlets[stringletCount].erase(areaWidth, stringlets[stringletCount].size());

		erasedChars = areaWidth;
		while (true) {
			if (stringlets[stringletCount][stringlets[stringletCount].size() - 1] == ' ')
				break;
			stringlets[stringletCount].erase(stringlets[stringletCount].size() - 1, stringlets[stringletCount].size());
			erasedChars--;
			if (erasedChars == 0) {
				erasedChars = areaWidth;
				stringlets[stringletCount] = stringCopy;
				stringlets[stringletCount].erase(areaWidth, stringlets[stringletCount].size());
				break;
			}
		}
		stringCopy.erase(0, erasedChars);
		stringletCount++;
	}
	stringlets.push_back(stringCopy);
	stringletCount++;

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
	char ch = _getch();
}

//----------------------------------------------------------------------

int fontRansomisation(fstream *missingFiles, int* successful, bool textRand) {

	if (!textRand)
		return 0;

	int totalTextFiles = 42, randomisedTextFiles = 0, missingTexturesFiles = 0;

	for (int i = 0; i < totalTextFiles; i++)
	{
		gotoxy(printCursor.X, printCursor.Y);
		cout << " - - Text Randomisation Progress: " << randomisedTextFiles << " / " << totalTextFiles << " files complete";

		int nameString = (i >= 33) * (i - 30) / 3;
		int fontID = (i < 33) * ((i / 3) + 1);
		bool print = false;

		fontFileNameFill(nameString, fontID, i);
		if (ransomiseFont() == -1) {
			*missingFiles << inputFileName + "\n";
			missingTexturesFiles++;
		}
		else {
			*successful += 1;
			randomisedTextFiles++;
		}
	}

	gotoxy(printCursor.X, printCursor.Y);
	printNTimes(120, ' ');
	gotoxy(printCursor.X, printCursor.Y);
	cout << " - - Text Randomisation complete!";
	printCursor.Y += 2;

	return missingTexturesFiles;
}

void fontFileNameFill(int nameString, int fontID, int fontNumber) {

	string buf = textFileNames[nameString];

	if (fontNumber < 33)
		buf += to_string(fontID / 10) + to_string(fontID % 10);

	buf += fntExtensions[fontNumber % 3];

	inputFileName = buf;
	outputFileName = buf;
}

int ransomiseFont() {

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
	string stringCpy;

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
			} while (!available[random] && !menuElement[0][2].isEnabled); // Text Anarchy

			// Start wiriting the file
			if (available[random]) {
				editedFile << charID[randomisedItems] << charStats[random] << "\n";
				if (!menuElement[0][1].isEnabled) // Text Duplicates
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

//----------------------------------------------------------------------

int textureRandomisation(fstream *missingFiles, int *successful, bool menuRand, bool iconRand, bool blockRand) {

	int error, missingFileCount = 0;

	for (int i = 0; i < 15; i++)
	{
		if (!menuRand && i < 6)
			i = 6;
		if (!iconRand && i >= 6 && i < 9)
			i = 9;
		if (!blockRand && i >= 9 && i < 15)
			return 0;

		textureFileNameFill(i);

		if (error = readTextureData(i) != 0) {
			*missingFiles << inputFileName + "\n";
			missingFileCount++;
		}
		else {
			*successful += 1;
		}
	}

	gotoxy(printCursor.X, printCursor.Y);
	printNTimes(100, ' ');
	gotoxy(printCursor.X, printCursor.Y);
	cout << " - - Texture Randomisation complete!";
	printCursor.Y += 2;

	return missingFileCount;
}

void textureFileNameFill(int i) {

	string buf = gameSheetNames[i / 3] + plistExtensions[i % 3];

	inputFileName = buf;
	outputFileName = buf;
}

int readTextureData(int texture) {

	string buf, randText = " - - Randomising ";
	int totalObjects;

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

	string refString = ".png</key>";
	vector<string> textureNames;
	vector<int> lineCount;
	vector<bool> isRandomised;
	int line = 1;

	// Extracting relevant data from file
	while (getline(originalFile, readStr)) {
		string stringCpy = readStr;
		removeWhitespace(readStr);
		stringCpy.erase(0, stringCpy.size() - refString.size());

		if (stringCpy == refString) {
			textureNames.push_back(readStr);
			lineCount.push_back(line);
			isRandomised.push_back(false);
		}
		line++;
	}

	// Adding filler data to avoid "out of array index" issues
	textureNames.push_back("");
	lineCount.push_back(0);
	isRandomised.push_back(true);

	originalFile.close();
	originalFile.open(miscStrings[0] + inputFileName, ios::in);
	if (!originalFile.is_open())
		return -1;

	int currentLine = 1;
	int randomisedTextures = 0;
	int random;

	totalObjects = textureNames.size();

	// Writing the randomised file
	while (getline(originalFile, readStr)) {

		if (rand() % 12 == 0) {
			gotoxy(printCursor.X, printCursor.Y);
			printNTimes(randText.size() + inputFileName.size() + buf.size() - 3);
			gotoxy(printCursor.X, printCursor.Y);
			buf = " | Progress: " + to_string(randomisedTextures) + " / " + to_string(totalObjects);
			cout << randText + inputFileName + buf;
		}

		if (currentLine != lineCount[randomisedTextures])
			editedFile << readStr << "\n";
		else {
			editedFile << "\t\t\t";
			while (true) {

				int random = rand() % textureNames.size();
				if (isRandomised[random] == false) {
					isRandomised[random] = true;
					editedFile << textureNames[random];
					break;
				}
			}
			editedFile << "\n";
			randomisedTextures++;
		}
		currentLine++;
	}

	gotoxy(printCursor.X, printCursor.Y);
	printNTimes(randText.size() + inputFileName.size() + buf.size() - 3);
	return 0;
}

//----------------------------------------------------------------------

int readConfigFile() {

	fstream configFile;
	int settingType;
	unsigned int size;

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

void readRandConfig(fstream* configFile, int settingType, string *settingArray, unsigned int size) {

	string readStr, stringlet;
	readValue val;

	while (getline(*configFile, readStr)) {
		removeWhitespace(readStr);

		if (readStr == "}" || readStr == "},")
			break;

		splitStringAt(readStr, stringlet, ':');
		removeSpecialChars(readStr);
		removeSpecialChars(stringlet);

		val = readTrueOrFalse(stringlet);

		int j = 0;
		for (; j < size; j++)
			if (readStr == settingArray[j])
				break;

		if (menuElement[settingType][j].type == OnOffSwitch) {
			if (val == True)
				menuElement[settingType][j].isEnabled = true;
			else if (val == False)
				menuElement[settingType][j].isEnabled = false;
		}
		else if (menuElement[settingType][j].type == NumberBox) {
			menuElement[settingType][j].data = stoul(stringlet);
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
	if (!configFile.is_open()) {
		return -1;
	}

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
			if (menuElement[i - 1][j].type != Null) {
				configFile << "\t\t\t\"" + writeStr[j] + "\": ";
				if (menuElement[i - 1][j].type == OnOffSwitch)
					configFile << std::boolalpha << menuElement[i - 1][j].isEnabled;
				else if (menuElement[i - 1][j].type == NumberBox)
					configFile << menuElement[i - 1][j].data;

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

	int r = 0, c = 0;

	while (getline(descriptionFile, readStr)) {
		removeWhitespace(readStr);
		if (readStr[readStr.size() - 1] == ',')
			readStr.erase(readStr.size() - 1, readStr.size());

		if (readStr == "}") {
			r = 0;
			c++;
		}
		else if (readStr != "{") {
			readStr.erase(0, 1);
			readStr.erase(readStr.size() - 1, readStr.size());

			if (readStr[readStr.size() - 1] != '"') {
				menuElement[c][r].description = readStr;
				r++;
			}
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
	
	// Temporarily disabled settings
	menuElement[1][1].type = Null;
	menuElement[2][1].type = Null;
	menuElement[3][1].type = Null;

	// Other settings menu
	menuElement[primaryMenuElementCount - 2][0].location.Y = rightMenu.location.Y + rightMenu.height - 4;
	menuElement[primaryMenuElementCount - 2][0].type = Null;
	// Seed input
	menuElement[primaryMenuElementCount - 2][1].type = NumberBox;
	menuElement[primaryMenuElementCount - 2][1].dataLimit = UINT_MAX;

	// Set up the "Randomise" button
	menuElement[primaryMenuElementCount - 1][0].location.Y = rightMenu.location.Y + rightMenu.height - 2;
	menuElement[primaryMenuElementCount - 1][0].type = Button;
	menuElement[primaryMenuElementCount - 1][0].isEnabled = true;
}

void setUpDefaultValues() {

	menuElement[0][0].isEnabled = true;
	menuElement[1][0].isEnabled = true;
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

//----------------------------------------------------------------------

void strRandom() {

	for (int i = 0; i < sizeof(MainSectionPrint) / sizeof(MainSectionPrint[0]); i++) {
		randomiseString(MainSectionPrint[i], MainSectionPrint[i].size());
	}
	for (int i = 0; i < sizeof(mainSettings) / sizeof(mainSettings[0]); i++) {
		randomiseString(mainSettings[i], mainSettings[i].size());
	}
	for (int i = 0; i < primaryMenuElementCount; i++)
	{
		for (int j = 0; j < secondaryMenuMaxElementCount; j++)
		{
			randomiseString(secondarySettings[i][j], secondarySettings[i][j].size());
		}
	}
	for (int i = 0; i < sizeof(missingFilesPrint) / sizeof(missingFilesPrint[0]); i++) {
		randomiseString(missingFilesPrint[i], missingFilesPrint[i].size());
	}
	for (int i = 0; i < sizeof(randomisationComplete) / sizeof(randomisationComplete[0]); i++) {
		randomiseString(randomisationComplete[i], randomisationComplete[i].size());
	}
	for (int i = 0; i < primaryMenuElementCount - 1; i++)
	{
		for (int j = 0; j < secondaryMenuMaxElementCount - 1; j++)
		{
			randomiseString(menuElement[i][j].description, menuElement[i][j].description.size());
		}
	}
	randomiseString(enabledStr, enabledStr.size());
	randomiseString(disabledStr, disabledStr.size());
	randomiseString(cancelledStr, cancelledStr.size());
}

void randomiseString(string &str, int size) {
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