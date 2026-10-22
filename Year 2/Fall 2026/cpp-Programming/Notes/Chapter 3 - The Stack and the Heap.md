*Date: September 30th, 2026
Class: C++ Programming
Slides: C++ Textbook

*Refer to slides (or textbook) for any missing examples / diagrams, all should have an alternate text saying what page or slide they are from. Everything should be pasted as they are seen within the slides. A note will be made if the image is later or earlier in the slide*
### 3.1 What You'll Be Able To Do
- Contrast stack and heap on allocation cost, lifetime, size limit, and fragmentation.
- Explain deterministic destruction, and trace when locals are created and destroyed.
- Predict construction and destruction order for nested scopes and function calls.
- Allocate on the heap with new/delete and new \[]/delete\[], and free correctly.
- Write a memory leak deliberately, explain why it is one, and say what would have prevented it.
- Use LLDB to watch stack frames and inspect values while a program runs.

### 3.2 Two Places a Value Can Live
- Every value in you program lives in one of two places, and which one decides almost everything about how you must treat it. 

```C++
void readSensor() {
	int local = 42;                // the STACK — automatic storage
	int* heapValue = new int(42);  // the HEAP — dynamic storage
	delete heapValue;              // and you must say when it ends
}
```

**Differences Between Stack and Heap**
![[Pasted image 20260930000003.png | Diagram from page 34]]

- In C# the garbage collector would've freed up both the stack and the heap. 

### 3.3 Deterministic Destruction
- This is the idea the whole memory arc of this course hangs on.
- A Stack variable is **destroyed at the closing brace of the scope it was declared in.** 
- Makes it precise, predictable, and in reverse order of construction.

```C++

struct Tracer {
	std::string name;
	Tracer(std::string n) : name(std::move(n)) { std::cout << " + " << name << " constructed\n" }
	~Tracer() { std::cout << " - " << name << " destroyed\n"; }
};
	
void inner() {
	Tracer c{"c (inner)"};
	std::cout << "inner body\n";
} // C is Destroyed here (After the method is finished calling)
	
int main() {
	Tracer a{"a (main)"};
	{
		Tracer b{"b (block)"};
		std::cout << "inside block\n";
	} // B Is destroyed here
	inner();
	std::cout << "end of main\n";
} // A is destroyed here
```

*Output*
``` Terminal
+ a (main) constructed
+ b (block) constructed
inside block
- b (block) destroyed
+ c (inner) constructed
inner body
- c (inner) destroyed
end of main
- a (main) destroyed
```

A is constructed first and destroyed last. Destruction is the Mirror Image of Construction - Not stylish convention but is what makes it safer for a later object to depend on an earlier one since the thing you always depend on is still alive. 

If this feels familiar from C#, it is worth being precise about the difference. IDisposable plus
using gives you deterministic disposal if you remember to write using. C++ gives it to you whether
you remember or not, because it is the language rule rather than a pattern you opt into.

### 3.4 Stack Frames
- Each Function gets a Frame: A slice of the stack holding that call's parameters, locals, and the address to return to it. Calls push frames, returns pop them.

![[Pasted image 20260930001556.png | Diagram from page 36]]

“Reverse order of construction” is just this stack coming apart from the top. A stack overflow is
this picture growing past its limit.

### 3.5 The Debugger as a Way of Looking
- From now on you compile with `-g`. That flags bakes in the line numbers and variable names LLDB needs to show your source instead of machine code. 

```Terminal
clang++ -std=c++20 -Wall -Wextra -g frames.cpp -o frames
lldb .\frames.exe
```

![[combined.png | Diagram from pages 36-37]]

- The command that makes the care for the debugger is `bt`. you drew the frame diagram on paper a page ago. 
- `bt` prints that same diagram

```Terminal
(lldb) bt
* thread #1, stop reason = breakpoint 1.1
	* frame #0: frames.exe`square(n=5) at frames.cpp:4:16
		frame #1: frames.exe`compute(a=5, b=3) at frames.cpp:9:9
		frame #2: frames.exe`main at frames.cpp:14:16
```

- frame #0 is always where execution currently is; the numbers count outward toward the caller. Addresses and process IDs will differ on your machine. The ordering will not.
- Keep in mind the debugger is not a tool of last resort for when something breaks. 
- `print` is how you check whether the thing you believe is happening, actually is.
- If you have used GDB, most names carry over: `break, run, next, step, print, bt, and continue` all work. Where they differ, LLDB’s own form is longer and more explicit
### 3.6 The Heap: New and Delete
You reach for the heap when:
- The Object must outlive its scope
- It's size is not known until runtime
- It's too big for a ~1 MB stack

```C++
int* one = new int(42); // single object
int* samples = new int[100]; // array

delete one; // matching form
delete[] samples; // matching form — note the []
```

Three Rules:
- Every new has exactly one matching delete
- new\[] pairs with delete\[]. Mixing them is undefined behavior
- After delete, the pointer is not null - it still holds the same address which now points somewhere you no longer own (Memory Leak)

Rule 3 produces the worst bugs of the three if not followed. Printing it out before and
after a delete shows the same value both times.

You may be tempted to make p = nullptr; your discipline. It helps — it converts a silent wrong
answer into a reliable crash — but it is not the real fix. The real fix is not owning raw pointers
at all, which arrives in [[Chapter 6 - Smart Pointers, Copy Semantics, and Custom Allocation |Chapter 6]] as unique_ptr. Knowing the destination is worth more than
perfecting the manual hygiene you are about to stop needing.

### 3.7 A Leak, On Purpose

```C++
void readBatch() {
	int* readings = new int[4]{10, 20, 30, 40};
	std::cout << readings[2] << "\n";
	return; // <- readings goes out of scope. The memory does not.
}
```

- When readBatch() returns, the pointer in destroyed, it was local and on the stack so its gone.
- The 400 bytes it pointed to are not destroyed. Nothing on the heap is scope-bound.
There is now no way to reach that memory and no way to free it.
- If this was C# the garbage collector would eventually reclaim it. 
- In C++ It's lost and never reclaimed until the process exits. 
-  On a device that runs for months without restarting, that column is the entire problem.

### 3.8 Why Embedded Work Avoids The Heap
- In later electives you will meets a blunt rule: on a microcontroller, avoid dynamic allocation. Here is why:
	- **No Room:** A microcontroller may have kilobytes of RAM, not gigabytes
	- **Fragmentation:** Repeatedly allocating and freeing memory chops the free space into small pieces. A later request for a large block fails even thought the total free space is ample. It will fail after weeks of uptime, not during testing.
	- **Unpredictable Timing:** mew searches for a block, and how long that takes depends on the current state of the heap. Code that must respond within a fixed time cannot afford and operation with no fixed cost.
	- **Nowhere to report failure:** there is no user, no dialog box, and often no screen.

With Stack Objects however:
- Fixed-size Buffers
- Pools Decided at compile time

### 3.9 Worked Example
Problem. A function returns a pointer to a buffer it allocated. Find the bug, then fix it two ways.

```C++
int* makeReadings(int count) {
	int* data = new int[count];
	for (int i = 0; i < count; ++i) { data[i] = i * 10; }
	return data; // caller now owns this. Does the caller know?
}

int main() {
	int* readings = makeReadings(4);
	std::cout << readings[2] << "\n";
	return 0; // BUG: leaked
}
```

*Fix 1 - Honor the Contract*

```C++
int* makeReadings(int count) {
	int* data = new int[count];
	for (int i = 0; i < count; ++i) { data[i] = i * 10; }
	return data; // caller now owns this. Does the caller know?
}

int main() {
	int* readings = makeReadings(4);
	std::cout << readings[2] << "\n";
	delete[] readings;
}
```

*Fix 2 - Remove the Question*

```C++
std::vector<int> makeReadings(int count) {
	std::vector<int> data(count);
	for (int i = 0; i < count; ++i) { data[i] = i * 10; }
	return data; // ownership is not in doubt; nothing to leak
}
```

std::vector is a container that owns its memory and frees it in its own destructor. You will meet
it properly in [[Chapter 9 - The STL: Containers, Iterators, and Algorithms |Chapter 9]]; today only one thing about it matters.

Both fixes are correct. They are not equally good. Fix 1 requires every future reader to notice
an obligation and honor it. Fix 2 makes the bug unwritable — there is no delete to forget,
because there is no delete. That difference is the argument the whole course is building toward.

### 3.11 Common Errors

![[Pasted image 20260930004611.png | Diagram from Page 40]]