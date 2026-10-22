*Date: October 5th, 2026
Class: C++ Programming
Slides: C++ Textbook

*Refer to slides (or textbook) for any missing examples / diagrams, all should have an alternate text saying what page or slide they are from. Everything should be pasted as they are seen within the slides. A note will be made if the image is later or earlier in the slide*

### 5.1 What You'll Be Able to Do
- State the only difference between struct and class in C++, and map both onto the OOP you already know.
- Write constructors and destructors, and use member initialization lists.
- Predict construction and destruction order for members and for nested scopes. 
- Use this and static members correctly.
- Explain RAII, and build a class that acquires a resource in its constructor and releases it in its destructor.
- Explain alignment and padding, predict a structs sizeOf(), and say when packing is justified.

### 5.2 struct and class are the Same Thing
- In C++ there is exactly one difference between them: **default access** 
```C++
struct Point { int x; int y; };   // Members are public by default
struct Point { int x; int y; };   // Members are private by default
```

- Constructors, Destructors, Inheritance, and Methods all work Identically. 
- The Conventions this course follows, and the one you will need in embedded code:
	- `struct` for plain data with no invariant to protect
	- `class` when the object owns something or has to keep itself consistent

This is where knowledge in C# can become misleading. In C#, a `struct` and `class` are genuinely different from each other. One is a reference type, one is a value type which decides where it lives in memory. That distinction doesn't carry over to C++. Where objects live in memory is decided by how you declare it, not by its defined type.  A `class` can be a stack local; a `struct` can be newed onto the heap.

### 5.3 Constructors, Destructors, and Member Init Lists
```C++
class Sensor {
	int id_;
	double scale_;
	
public:
	Sensor(int id, double scale) : id_(id), scale_(scale) { // Member Init List
		std::cout << "Sensor " << id_ << " ready\n";
	}
	
	~Sensor() {
		std::cout << "Sensor " << id_ << " shutdown\n";
	}
};

// Initialize the Members in the body

Sensor(int id, double scale) { id_ = id; scale_ = scale; }
// This works but the default-constructs them first then overwrites. 
// Wasted work for variables like doubles and ints and impossible for any member that is a const

```

- The ` : id_(id), scale_(scale) {...` part is the **Member initialization list**. You can still assign them in the body
- Members are initialized in **Declaration Order**, not in the order you write them in the list. Write them out of order and clang++ will say something. 
- Initialization Lists are better than assigning in body becasue:
	- Assigning in the body **default-constructs then overwrites**, which is wasted work.
	- And it is **impossible** for `const` members and reference members, which can only be initialized.

### 5.4 Order: Members First, Object Last
```C++
class GpioPin {
	int pin_;
	Member m1_{"m1"};
	Member m2_{"m2"};
	
public:
	explicit GpioPin(int pin) : pin_(pin) {
		std::cout << " GpioPin Aquried\";
	}
	
	~GpioPin() {
		std::cout << " GpioPin released\n"
	}
};
```

- Construction: Members in declaration order, then the constructor body.
- Destruction: The destructor body, then members in reverse order. 

Predict before you run: Six Lines, Write them down
```Terminal
member m1 ctor
member m2 ctor
GpioPin 13 ACQUIRED
GpioPin 13 released
member m2 dtor
member m1 dtor
```

You might think `std::cout << " GpioPin Aquried\";` would be first but you need `m1_` to be constructed first before the body can run. 

That ordering is not a detail to memorize - it is what makes both bodies safe to write. By the time your constructor body runs, every member already exists and can be used.  Your destructor body runs while the members are still alive, so you may use them there too. 

### 5.5 this and Static Members
```C++
class Counter {
	int value_ = 0;
	static int instances_;    // Declared Here
	
public:
	Counter() { ++instances_; }
	Counter& add(int n) {value_ += n; return *this; } // Return the object itself
	static int instances() { return instances_; }
};

int Counter::instances_ = 0;
```

- `this` is a Pointer to the current object; \*this is the current object
- A `static` member belongs to the `class`, not to any instance. There is exactly one, shared by every object. 
- A `static` data member must be defined once outside the class

That last point is worth trying deliberately. Omit the definition line and the class complies perfectly then the build fails with:

```Terminal
ld.lld: error: undefined symbol: Counter::instances_
```

A good old linker error. The complier was completely happy with the class but the linker wasn't. This goes back to [[Chapter 1 - The Machine and the Build]]: The declaration was a promise, and nothing kept it. 

### 5.6 RAII
**Resource Acquisition Is Initialization**. The name isn't helpful but the idea is simple: 
	Acquire the resource in the constructor. Release it in the destructor. Then let the scope do the work. 

Because destruction is deterministic - the guarantee from [[Chapter 3 - The Stack and the Heap]] - releasing is not something you remember todo. It's something the language does. 

```C++
class GpioPin(int pin) : pin_(pin) {
	std << "pin " << pin_ << " configured as output\n";
}

	~GpioPin() {
		std::cout << "pin " << pin_ << " released\n"
	}
	
	void write(bool high) { /* ..... */ }
};

void blink() {
	GpioPin led{13};
	led.write(true);
	// No clean up code here. Nothing to forget
}
```

in C# you would write:
```C#
using (var led = new GpioPin(13)) { led.Write(true); } // Works if you write 'using'
```

C# gives you deterministic disposal when you remember the keyword. C++ gives it to you because it is the language rule. Forgetting is not an available option.

**The argument that should actually convince you is the early return**
```C++
void blink() {
	GpioPin led{13};
	if (!selfTest()) { return; } // The Pin is still released
	led.Write(true);
};
```

Ask yourself where a hand-written `releasePin(13);` would have gone. At the bottom of the function - where return never reaches. Every early return, and every error path someone adds later, is a place manual cleanup gets missed and RAII does not. 

### 5.7 Alignment and Padding
```C++
struct Bad { uint8_t flag; uint32_t value; uint8_t mode; };
struct Good { uint32_t value; uint8_t flag; uint8_t mode; };
```

Predict before you run. What is the `sizeOf()` each one? Write both numbers down first.

```Terminal
sizeOf(Bad)   = 12
sizeOf(Good)  = 8
sum of fields = 6
alignOf(Bad)  = 4
offset flag   = 0
offset value  = 4 <--- 3 bytes of padding sits between them
offset mode   = 8
```

- Padding is invisible in the source, which is exactly why it is worth predicting rather than being told. 

a `uint32_t` must sit at an address divisible by 4. In bad, the one-byte flag takes byte 0, so value cannot start until byte 4 - three bytes are wasted - and the struct is then rounded up to a multiple of its alignment so the next element of an array is also aligned. 

Same three fields, same three types, one third less memory - purely because of declaration order. In an array of a few hundred, on a device with kilobytes of RAM, it is a real difference. 

### 5.8 \#pragma pack, and When It Is Justified
```C++
#pragma pack(push, i)
struct Packed { uint8_t flag; uint32_t value; uint8_t mode; };
#pragma pack(pop)
```

- sizeOf(Packed) is equal to 6 - exactly the sum of it's fields.

This matters the moment your struct has to match something outside your program: a hardware register block, a binary file format, or bytes on a wire. The device does not know about your complier's padding rules. If you `memcpy` a bad onto a 6-byte protocol frame, you send twelve bytes and everything after it is garbage. 

But packing is not free, and there rule is not "always pack": unaligned access is slower on x86 and on some architectures it faults. **Pack only where an external format requires it, and reorder your fields everywhere else.** This judgment will come back in [[Chapter 11 - File I/O and Streams |Chapter 11]].

### 5.9 Worked Example

Problem. Build an RAII wrapper for a simulated peripheral, and prove it releases on every exit
path.

```C++
#include <iostream>
#include <cstdint>

class GpioPin {
	uint8_t pin_;
	volatile uint8_t* port_;
	
	public:
		GpioPin(uint8_t pin, volatile uint8_t port) : pin_(pin), port_(port) {
			*port_ |= (1 << pin_)   // Acquire: Drive the pin high
			std::cout << "pin" << int(pin_) << " acquired\n";
		}
		
		~GpioPin() {
			*port_ &= ~(1 << pin_); // Release: Always
			std::cout << "pin" << int(pin_) << " released\n";
		}
		
		void write(bool high) {
			if (high) { *port_ |= ( 1 << pin_ ); }
			else      { *port_ &= ~(1 << pin_ ); }
		}
};

bool selftTest() { return false; } // Pretend it fails

void blink(volatile uint8_t* port) {
	GpioPin led{3, port};
	if(!selfTest()) {
		std::cout << "self-test failed, bailing out\n";
		return;  // early exit
	}
	led.write(true)
}

int main() {
	uint8_t simulated = 0;
	blink(&simulated);
	std::cout << "register now: " << int(simulated) << "\n";
}
```

Output:
```Terminal
pin 3 acquired
self-test failed, bailing out
pin 3 released
register now: 0
```

The 0 is the proof. The register returned to it's clean state even though the function exited from the middle, and nothing in blink says "clean up." The closing brace / end of scope does it for you. 

Look at what is in this one class. [[Chapter 2 - Types, Bits, and the Preprocessor |Chapter 2]]'s bit masks, [[Chapter 4 - Pointers, References, and const |Chapter 4]]'s volatile pointer, [[Chapter 3 - The Stack and the Heap |Chapter 3]]'s deterministic destruction. This current chapters constructors and destructors. This is the first point in the course where everything you have learned assembles into one thing. 

### 5.11 Common Errors

![[cppChap5CE.png | Common Errors from page 63]]
















































