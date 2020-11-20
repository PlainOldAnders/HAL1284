#include "Adafruit_Keypad.h"

const byte ROWS = 5; // rows
const byte COLS = 10; // columns

int shiftKey = 18;
byte colPins[COLS] = {7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
byte rowPins[ROWS] = {2, 3, 4, 5, 6};

char ESC = 27;
char ENTER = 13;
char BCKSPC = 8;
char DEL = 127;
char upKey = 128;
char downKey = 129;
char leftKey = 130;
char rightKey = 131;
char upKeyShift = 11;
char downKeyShift = 10;
char leftKeyShift = 8; //Same as backspace
char rightKeyShift = 21;


char keys[ROWS][COLS] = {
  {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0'},
  {'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p'},
  {'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', ENTER},
  {'z', 'x', 'c', 'v', 'b', 'n', 'm', ',', '.', upKey},
  {ESC, '?', '\'', '<', ' ', '-', BCKSPC, leftKey, downKey, rightKey}
};

char shiftKeys[ROWS][COLS] = {
  {'!', '@', '#', '$', '%', '&', '/', '(', ')', '='},
  {'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P'},
  {'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', ENTER},
  {'Z', 'X', 'C', 'V', 'B', 'N', 'M', ';', ':', upKeyShift},
  {ESC, '+', '"', '>', ' ', '*', DEL, leftKeyShift, downKeyShift, rightKeyShift}
};

/*
  left arrow: 37
  up arrow: 38
  right arrow: 39
  down arrow: 40
  space: 32
*/

Adafruit_Keypad customKeypad = Adafruit_Keypad( makeKeymap(keys), rowPins, colPins, ROWS, COLS);
Adafruit_Keypad customKeypad2 = Adafruit_Keypad( makeKeymap(shiftKeys), rowPins, colPins, ROWS, COLS);

void setup() {
  Serial.begin(9600);
  customKeypad.begin();
  customKeypad2.begin();
  pinMode(shiftKey, INPUT);

}

void loop() {

  if (digitalRead(shiftKey) == HIGH) {
    customKeypad2.tick();
    while (customKeypad2.available()) {
      keypadEvent e2 = customKeypad2.read();
      if (e2.bit.EVENT == KEY_JUST_PRESSED) {
        if (e2.bit.EVENT == KEY_JUST_PRESSED) {
          Serial.print((char)e2.bit.KEY);
        }
      }
    }
  } else {
    customKeypad.tick();
    while (customKeypad.available()) {
      keypadEvent e = customKeypad.read();
      if (e.bit.EVENT == KEY_JUST_PRESSED) {
        Serial.print((char)e.bit.KEY);
      }
    }
  }

  delay(10);
}

char getShiftKey(char key) {
  char shiftVal = ' ';
  for (int r = 0; r < sizeof(keys); r++) {
    for (int c = 0; c < sizeof(keys[r]); c++) {
      shiftVal = shiftKeys[r][c];
    }
  }
  return shiftVal;
}

/*
  customKeypad.tick();

  while (customKeypad.available()) {
    keypadEvent e = customKeypad.read();
    if (e.bit.EVENT == KEY_JUST_PRESSED) {
      char currentC = (char)e.bit.KEY;
      if (digitalRead(shiftKey) == LOW) {
        Serial.print(currentC);
      } else {
        //Serial.print((char)e.bit.KEY);
        Serial.print(getShiftKey(currentC));
      }
    }
  }

  delay(10);
*/
