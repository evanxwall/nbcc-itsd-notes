*Date: September 27th, 2026
Class: C++ Programming
Slides: C++ Textbook

*Refer to slides (or textbook) for any missing examples / diagrams, all should have an alternate text saying what page or slide they are from. Everything should be pasted as they are seen within the slides. A note will be made if the image is later or earlier in the slide*
### 2.1 What You'll Be Able To Do
- Explain what the preprocessor does, and inspect its output with clang++ -E.
- Choose between \#define, const, and constexpr, and say why one of them is the default.
- Write include guards and explain the problem they solve.
- Explain why the size of int and long is implementation-defined, and why that matters in
embedded work.
- Use fixed-width types from \<cstdint> and justify each choice.
- Set, clear, toggle, and test an individual bit with &, |, ~, ^, <<, and >>.
- Explain why changing one bit of a hardware register requires a read-modify-write.

### 2.2 The Preprocessor: A Program That Edits Your Program

- Before the complier sees your code, a separate programs edits it.
- The Preprocessor does not understand C++, it does not know what a type is, or a function, or a scope.
- It does text substitution and hands the result on.
- Everything it does is driven by the lines beginning with \# (#include \<iostream>)
- C# has no real equivalent. 

```C++
#include <iostream> 
// The Preprocessor will paste the entire contents of iostream here

#define MAX_ENTRIES 3 
// Will repleace the text MAX_ENTRIES with the text 3 everywheres its used
```


### 2.3 Seeing it Instead of Believing It

```Terminal
clang++ -E main.cpp -o main.i
```

- Shows that the complier actually receives
- If you search or a macro by name, it won't be there because its been replaced by its value. They are gone before compilation even begins. 

### 2.4 \#define VS. constexpr

Three ways you can name a constant, Only one is a good default.

```C++
#define BUFFER_SIZE 256 // Preprocessor: pure text, no type, no scope
const int bufferSize = 256; // A Real variable, typed and scoped
constexpr int kBufferSize = 256; // A Real variable, typed and scoped, known a complie time
```

**Differences Between \#define and constexpr**
![[Pasted image 20260929174231.png | Diagram from Page 24]]

- You should use constexpr for constants
- You use \#define only for include guards and conditional compilation
- constexpr is also the first construct that hangs directly on [[Chapter 1 - The Machine and the Build |Chapter 1’s ]] compile-time / run-time
boundary. It means compute this during the build. The value is finished before the program starts.

### 2.5 The Macro Pitfall

```C++
#define SQ(x) x*x

int main() {
	std::cout << SQ(2 + 3) << "\n";
	
	return 0;
}
```

- Because the text substitution of \#define, you would expect this to equal 25 but it is actually 11. Why? since the substitution is textual, x becomes (2 + 3) so when SQ is called x\*x becomes: 2+3 \* 2+3 = 11. 
- You can add defensives parentheses and make `#define SQ(x) ((x)*(x))` but there is a better way of doing this with constexpr

```C++
#include <iostream>

constexpr int sq(int x) { return x * x; }

int main() {
	int result = sq(2 + 3)
	std::cout << result << "\n";
	
	return 0;
}
```

- This is the better answer, the function has no bug to fix.
- 2 + 3 is evaluated *before* the call happens. 
- A Macro pastes text; a function receives values.

### 2.6 Include Guards
- If two files both have `#include "sensor.h"` the preprocessor will paste it twice and a type can only be defined once per translation unit. This will cause redefinition errors.
- Using `#pragma once` in your code. Tells the complier: only include this header file once per compilation. So if it sees it again, it will ignore it. 
- Before `#pragma`, common practice was to use `#ifndef SENSOR_H` and `#define SENSOR_H`.
	 - `#ifndef` - saying "If SENSOR_H has NOT been defined..." it continues to the `#define` statement. 
	 - `#pragma` is pretty much the same thing rolled into one

- Headers must stand on their own. It should include everything it needs. 
	- if you need `#include <cstdint>` for your sensor object, it should be included with its header file and not your main file. 
	- If you don't, depending on your order of `#include` statements, you sensor might get processed before `<cstdint>` even though you needs its contents in your sensor. 

```C++
#include <cstdint> 
#include "sensor.h"

// If you weren't to have #include <cstdint> in your header file, but had it like this in your main file. it may still work if there isn't any other issues since its declared before your sensor. 

// ----------------

#include "sensor.h"
#include <cstdint>

// However this won't work because your sensor needs the contents of <cstdint> to work. 
```

### 2.7 `int` Is Not a Promise
- in C#, int is always 32 bits.
- In C++ int is "at least 16 bits, whatever the implementation chose." (same goes for long)

![[Pasted image 20260929182011.png | Diagram from page 25]]

- In practice int is 4 bytes essentially everywhere you will meet it, and claiming otherwise would be overstating the danger. 
- The honest statement is narrower and more useful: the standard does not promise you a size. As seen in the diagram, you can see it in action with the `long` type.

### 2.8 Fixed-Width Types Say What They Mean. 

```C++
#include <cstdint>

uint8_t statusRegister = 0; // exactly 8 bits, unsigned — a register
int32_t temperatureMilliC; // exactly 32 bits, signed
uintptr_t address; // an unsigned integer big enough to hold a pointer
```

Naming Pattern:
- u means *unsigned*
- the number of the count of *bits*
- *_t* means type

When a value maps to hardware, wire protocol, or a file format, the width is part of the specification. Say it in the type. 

### 2.9 The Promotion Trap

```C++
uint8_t a = 200;
uint8_t b = 100;
std::cout << a + b << "\n";
std::cout << sizeof(a + b) << "\n";

uint8_t c = a + b;
std::cout << static_cast<int>(c) << "\n";
```

What Happens?
- Line 1: because C++ has no 8-bit arithmetic, both operands are promoted to int before the + causing the output to equal 300. 
- Line 2: Proves line 1 with the sizeOf(a + b) being 4.
- Line 3: The 32-bit integer is then turned back into a 8-bit causing it to equal 44

**C++ Widens, Computes, and Narrows ONLY when you assign**

That static_cast \<int> on the last line is not decoration. Without it, std::cout treats a uint8_t
as a character and prints the glyph for character code 44 — a comma. You will hit this the first
time you try to print a register value.

### Bit Manipulation: Eight Independent Switches
- A uint8_t register is eight independent switches. these six operators are how you work one switch without disturbing the other seven. 
```C++
#include <cstdint>

/* 
	&  - AND:   Test a bit, clear bits with ~
	\| - OR:    set a bit
	^  - xOR:   toggle a bit
	~  - NOT:   invert every bit - builds a clear mask
	<< - Shift  Left: build a mask, 1 << 3 is bit 3
	>> - Shift  Right: move a bit down to read it. 
*/

uint8_t reg = 0;

reg |= (1 << 3);                  // SET bit 3
reg &= ~(1 << 3);                 // CLEAR bit 3 <-- note the ~
reg ^= (1 << 3);                  // TOGGLE bit 3
bool on = (reg & (1 << 3)) != 0;  // TEST bit 3
```

Build each one by asking a single question: what do I want to happen to the other seven bits? The
answer is always “nothing”, and that requirement is what forces the mask form.

Write 1 << 3, never 8. The shift form says bit three — which is the thing you actually mean —
and it keeps working when the bit number is a variable or a named constant.

### 2.11 Read-Modify-Write

```C++
reg = (1 << 3);    // WRONG: sets bit 3 and clears all seven others
reg |= (1 << 3);   // RIGHT: reads reg, ORs in bit 3, writes it back
```

`|=` is a Read-Modify-Write operator. It gets the current value, change one bit, store it back. It is a three bus operation which is why writing it to a register is never simply a store. 

### 2.12 Two Errors Everyone Makes

**1: Clearing the Wrong Mask**
```C++
reg &= (1 << 3); // WRONG: keeps ONLY bit 3, clears everything else
reg &= ~(1 << 3); // RIGHT: clears bit 3, keeps everything else
```

**2: Precedence - Complier will catch this one**
```C++
if (reg & 0x02 == 0) // WRONG
if ((reg & 0x02) == 0) // RIGHT
```

& binds looser than \==, so the first line means reg & (0x02 == 0), which is reg & 0, which is
always 0. Compiled with -Wall:

```Terminal
warning: & has lower precedence than ==; == will be evaluated first [-Wparentheses]
	6 | std::cout << (reg & 0x02 == 0) << "\n";
	  |                    ^~~~~~~~~~~
```

Noticed that code has no error. It compiles, it runs, and it is silently wrong. the condition can never be true. The compiler found a real bug that would otherwise have cost you an afternoon. This is what -Wall -Wextra is for, and it is why this course treats warnings as errors.

### 2.13 Worked Example
Problem: Write a small interface for an 8-bit status register — set, clear, toggle, and test any bit
— then drive it.

```C++
#include <cstdint>
#include <iostream>

constexpr uint8_t kMotorEnable = 0;  // Bit 0
constexpr uint8_t kFaultFlag = 2;    // Bit 2

void setBit(uint8_t& reg, uint8_t bit) { reg |= (1 << bit); }
void clearBit(uint8_t& reg, uint8_t bit) { reg &= ~(1 << bit); }
void toggleBit(uint8_t& reg, uint8_t bit) { reg ^= (1 << bit); }
bool testBit(uint8_t reg, uint8_t bit) { return (reg & (1 << bit)) != 0; }

int main() {
		uint8_t status = 0;
		
		setBit(status, kMotorEnable);
		setBit(status, kFaultFlag);
		clearBit(status, kFaultFlag);
		
		std::cout << std::boolalpha 
			<< "motor: " << testBit(status, kMotorEnable) << "\n"
			<< "fault: " << testBit(status, kFaultFlag) << "\n";
}
```

The first three functions take uint8_t& reg and the fourth takes plain uint8_t reg. The & means
the function changes the caller’s variable rather than a copy of it. testBit modifies nothing, so it
does not need one. What that & actually is comes apart in [[Chapter 4 - Pointers, References, and const |Chapter 4]]; for now, that sentence is
enough.

The bit numbers are constexpr, from the first half of this chapter, put to work in the second. Named constants are the difference between setBit(status, 2) and setBit(status, kFaultFlag) — and six months later only one of those is readable.

### 2.14 Common Errors

![[Pasted image 20260929233642.png | Diagram from page 29]]