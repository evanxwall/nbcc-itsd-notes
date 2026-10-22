*Date: October 2nd, 2026
Class: C++ Programming
Slides: C++ Textbook

*Refer to slides (or textbook) for any missing examples / diagrams, all should have an alternate text saying what page or slide they are from. Everything should be pasted as they are seen within the slides. A note will be made if the image is later or earlier in the slide*
### 4.1 What You'll Be Able To Do
- Use & (address-of) and * (dereference), and print addresses to see memory.
- Do pointer arithmetic, and explain how a C-style array decays to a pointer.
- Distinguish pass-by-value, pass-by-pointer, and pass-by-reference, and choose correctly.
- Apply const to pointers and references, and read which form protects what.
- Read and act on the compiler error you get from modifying through a const reference.
- Reach a fixed hardware address through a pointer and drive a register with [[Chapter 2 - Types, Bits, and the Preprocessor |Chapter 2]]’s masks.
- Explain what volatile promises, and what it does not.

### 4.2 A Pointer is a Variable That Holds and Address
- That is the whole definition, and almost every pointer bug is a failure to keep in mind

```C++ 
int value = 42;
int* p = &value; // & means "address of" so p = address of value

std::cout << value << "\n";  // Output 42    - the Object
std::cout << &value << "\n"; // Output 0x..  - Where it lives in memory
std::cout << p << "\n";      // Output 0x..  - Same address, stored in the pointer
std::cout << *p << "\n";     // Output 42    - * means "go into that address"
```

![[Pasted image 20261002153051.png | Diagram from page 46]]

- Two Boxes and an Arrow, with Addresses underneath them. 
- When stuck on a pointer question later on, ask yourself: *which box an i talking about?*'
- Need to be careful of the & operator as it has two different jobs within C++
	- Right now with &value, it means "address of"
	- Using it in a declaration like `int& r` it means "reference"

### 4.3 Pointer Arithmetic Is Not Integer Arithmetic

```C++
int array[4] = {10, 20, 30, 40};
int* p = array; // No & needed as the Array converts into a Pointer

*p;            // Output 10 - The First Index of th
*(p + 2);      // Output 30 - NOT the address plus 2 bytes
p[2];          // Output 30 - Same as *(p + 2) just with [] notation
(p + 2) - p;   // Output 2  - The difference in ELEMENTS, not bytes
*p* + 1        // Output 11 - Get and Access Element 0 then add 1
```

- Adding 1 to an int* moves forward by sizeOf(int) bytes, not by one byte. 
- The Complier scales for you - which is why p\[2] and \*(p + 2) are the same.
- Indexing is apart of pointer arithmetic wearing a friendlier notation

```C++
int array[4] = {10, 20, 30, 40};
int* p = array; 

*(p + 1)   // The NEXT Element
*p + 1     // THIS element plus one 
```

- Keep the binding straight. One pair of parentheses separates two different answers
- Dereference binds tighter than +

### 4.4 Array Decay

```C++
int array[4] = {10, 20, 30, 40};
int* p = array; 

std::cout << sizeof(array) << "\n"   // 16 - Four Ints
std::cout << sizeof(p) << "\n"       // 8  - One Address
```

- Passing an Array through a function causes it to decay. 
- The function receives the a pointer, and the length is gone, not hidden, gone.
- However, every C-Style array parameter comes with a separate count. 

```C++
void process(int* data, int count); // The count is not optional
```

- A C# array knows its own Length. A C-style array does not. That information is genuinely lost at the call.
- std::array and std::vector fix this by carrying their size with them (Later Topic / Chapters.)

### 4.5 Three Ways to Pass an Argument
- Three ways to pass an Argument into a Function. 

```C++
void byValue(int x) { x = 99; }    // A Copy of x, caller unaffected
void byPointer(int* x) { *x = 99}  // An Address of x, caller value changes
void byRef(int& x) { x = 99}       // An Alias of x, caller value changes

int myInt = 1;
byValue(myInt)     // myInt is still 1
byPointer(myInt)   // myInt is now 99
byRef(myInt)       // myInt is now 99

/*
	Output:
	After byValue:   1
	After byPointer: 99
	After byRef:     99
*/
```

![[Pasted image 20261002160421.png | Diagram / Table from page 47]]

- A Reference is an Alias - another name for an existing object.
- They aren't copies and they are not pointers you dereference. 
- The "Can be Null" row decides most real choices. Use a pointer when it could be, use a reference when it's not.
- At the call site at `byValue(myInt) and byRef(myInt)` look completely identical by we know one passes a copy and one actually changes the callers value. 
- you must read the signature to know whether your variable can change.
- C# announces this at the call with ref and out; C++ does not, and expecting it to is a habit worth breaking now.

### 4.6 Const, and Reading it Correctly

```C++
int a = 1;
int b = 2;

const int* p = &a;    // Pointer to a const int
*p = 5;               // a is being stored in p, you can't modify it.
p = &b;               // Point elsewhere

int* const q = &a;    // const pointer to int
*q = 5;               // a can be modified but q can't point elsewhere
q = &b;               // This no longer works
```

- q is like saying "q is a pointer to const int"
	- The Pointer itself is not a const
	- You can make it point elsewhere
	- You CANNOT modify the integer it points to

- p is like saying "p is a const pointer to an int"
	- The pointer is now a const
	- You CAN modify the value
	- You CANNOT change where it points too

So Which lines of the four will fail?
```Terminal
error: read-only variable is not assignable
    4 | *p = 5;
error: cannot assign to variable 'q' with const-qualified type 'int *const'
	8 | q = &b;
```

- p* = 5 fails since you CANNOT change the value of the object it is pointing to.
- q = &b fails since you CANNOT change what object a const pointer is pointing to.

### 4.7 const T& - The One You Will Actually Write

```C++
void logReading(const Reading& r); // No Copy and Cannot Modify
```

his is the default for passing anything bigger than a machine word. You avoid the copy a by-value
parameter would make, and the const promises the caller you will not touch their object.

Break that promise and clang will not be happy.
```C++
void logIt(const Reading& r) {
	r.value = 99; // Error
}
```

```Terminal
error: cannot assign to variable 'r' with const-qualified type 'const Reading &'
	3 | r.value = 99;
	  | ~~~~~~~ ^
note: variable 'r' declared const here
	2 | void logIt(const Reading& r) {
```

- The second line shows what you did wrong and points out where the promise was made. 
- Build a habit of reading the whole diagnostic and not just the first line, it helps (Thanks Dave!)

### 4.8 Memory-Mapped I/O

- On a microcontroller a hardware register is not reached through an API. It lives at a fixed address, and you talk to it by posting at that address

```C++
#include <cstdint>

// On real hardware: a number from the datasheet. 
// Here: a simulated register we can actually run
uint*_t simulatedRegister = 0;
volatile uint8_t* const GPIO_PORT = &simulatedRegister;

*GPIO_PORT |= (1 << 3);    // Set bit 3 - Chapter 2: Mask through a pointer
*GPIO_PORT &= ~(1 << 3);   // clear it

// On Real device, the declaration would be more like:
volatile uint8_t* const GPIO_PORT = reinterpret_cast<volatile uint8_t*>(0x400...);
```

- Notice: [[Chapter 2 - Types, Bits, and the Preprocessor |Chapter 2]]'s bit masks reached all the way to Chapter 4's pointer indirection.
- This is the first point in the course where the embedded material stops being an analogy
- Be careful of the type: a const pointer to a volatile uint8_t. The address never changes - the register is bolted down at the location the chip designer picked - but the value can change without warning, because hardware rights it. 

### 4.9 volatile - What It Actually Promises
- volatile tells the compiler one thing: **This location can change without your knowledge, so do not optimize the access way.**

```C++
while (*STATUS_REG == 0) { } // Wait for Hardware to set a bit
```

- Without volatile, the complier sees a loop reading a value that nothing in your program modifies, correctly concludes the value cannot change, reads it once, and rewrites the loop as `while (true) { }`. 
- **volatile is not a threading tool.** The internet will says otherwise but it doesn't make anything atomic and it doesn't order operations between threads. 
- It means one thing: Do not cache this access. ( [[Chapter 12 - Concurrency and Performance |Chapter 12]] will come back to this. )

### 4.10 Worked Example
Problem. Write a register interface that takes the register by pointer, and a logging function that
takes a reading by const reference. Then break both on purpose.

```C++
#include <cstdint>
#include <bitset>
#include <iostream>

struct Reading { int milliVolts; };

void setBit(volatile uint8_t* reg, uint8_t bit) { 
	*reg |= (1 << bit); 
}

void clearBit(volatile uint8_t* reg, uint8_t bit) { 
	*reg &= ~(1 << bit); 
}

bool testBit(const volatile uint8_t* reg, uint8_t bit) { 
	return (*reg & (1 << bit)) != 0; 
}

void logReading(const Reading& r) {
	std::cout << "reading: " << r.milliVolts << " mV\n";
}

int main() {
	uint8_t simulated = 0;
	volatile uint8_t* const PORT = &simulated;
	
	setBit(PORT, 3);
	setBit(PORT, 0);
	clearBit(PORT, 3);
	
	std::cout << std::bitset<8>(simulated) << "\n";
	std::cout << std::boolalpha << testBit(PORT, 0) << "\n";
	
	Reading r{3300};
	logReading(r);
}
```

```Terminal
Output:

00000001
true
reading: 3300 mV
```

Now break it twice:

Add r.milliVolts = 0; inside logReading, and you get the const-reference error from the pre-
vious section. Add PORT = &other; in main, and you get cannot assign to variable ‘PORT’ with
const-qualified type — because the pointer itself is const.

Two different consts. Two different errors. On the same page. That is why this chapter teaches
const T* and T* const together rather than separately: each one protects a different thing, and
the only way to know which you need is to know which thing you are protecting.

### 4.12 Common Errors

![[Pasted image 20261002170625.png]]
