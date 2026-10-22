*Date: September 16th, 2026
Class: C++ Programming
Slides: C++ Textbook*

*Refer to the textbook for any missing images. Should be pasted as they are seen within the slides. A note will be made if the image is later or earlier in the slide*
### Intro
*You already know how to program. You have written C# for a year — classes, methods, loops, collections, exceptions. This book does not teach you to program. It teaches you what happens when you take away the runtime that has been managing memory for you, and hand you the machine instead.*

*That trade is the whole course. In C#, a garbage collector decides when your objects die. In C++, you decide, and the language gives you exact, unforgiving control over when memory is acquired and when it is released. Everything that makes C++ suitable for a microcontroller with 16 KB of RAM comes from that control, and so does everything that makes it dangerous.*

*- This is straight from the textbook, I just thought it was a good intro piece*

### 1 - The Toolchain and the Build

#### Chapter Objectives
- Explain what a C++ compiler produces, and how that differs from what C# produces.
- Write, compile, and run a C++ program from the terminal, with a plain text editor.
- Tell compile time from run time, and sort a question into the side that answers it.
- Name the four build stages — preprocess, compile, assemble, link — and say what each one
consumes and produces.
- Produce object files with clang++ -c and link them by hand.
- Use the flags -std=c++20, -Wall, -Wextra, -c, -o, and -E.
- Write a minimal CMakeLists.txt and build a project with two commands.

#### 1.2 No Runtime, No Collector

**Biggest Difference Between C# and C++**
- C# complies to IL (an Intermediate language,) which runs on the CLR - a managed runtime that complies IL to machine code as the program runs. That then runs a garbage collector to free memory on your behalf
- C++ complies directly to native machine code for one specific kind of CPU. No runtime layer, no garbage collector. The executable is the program. If you allocate memory, you free it. 

That is the trade this whole course is about. You lose the safety net; you gain a program small enough and predictable enough to run on a chip with a few kilobytes of RAM.

#### 1.3 Your First Program

```C++
#include <iostream> 
// Brings in std::cout - Like C#'s using System

int main() { // The Entry point / Method

	std::cout << "Hello, Embedded World!\n";
	
	return 0; // 0 means exited it without issue
}
```

- `#include <iostream>` makes `std::cout` available. It is like `using System;` in C#. However it uses an entirely different mechanism.
- `int main()` is the entry point / method. It returns an int unlike C# returning void. If the program runs without issue, it returns 0, any other value signals a problem has occurred. 
- `std::cout` prints. The `std::` prefix says the name lives in the standard library's namespace, std. It doesn't add a new line. `std::cout` is the same as `Console.Write();` not `Console.WriteLine();` you need to use the escape characters like `\n` to start a new line

#### 1.4 Compiling and Running from the Terminal

- No Run Button, Two Commands
```Terminal
clang++ -std=c++20 -Wall -Wextra hello_world.cpp -o hello_world
./hello_world     # Windows: .\hello_world.exe
```

The Flags:
- `std=c++20` - compile against the C++20 standard. This course is modern C++ though-out
- `-Wall -Wextra` - turn on warnings. Warnings are free bug reports, treat them as errors.
- `-o hello_world` - name the output file. Without -o, the name will default to a.

Your compiler is `clang++`, from the LLVM project, and your debugger ( [[Chapter 3 - The Stack and the Heap |Chapter 3]] ) is LLDB. There is no `g++` or `gcc` on a correctly configured machine. If a textbook or website says use `g++ or gcc` use `clang++` instead.

There is many C and C++ compilers. Two of the most popular are the LLVM compliers (`clang, clang++`) which we use and the GNU compliers (`gcc, g++`.)

#### 1.5 Reading a Complier Error

```Terminal
hello_world.cpp:4:44: error: expected ';' after expression
	4 | std::cout << "Hello, Embedded World!\n"
	  |                                        ^
	  |                                        ;
1 error generated.
```

Error Reading:
1. hello_world.cpp - the file
2. 4:44 - line 4, column 44 
3. expected ' ; ' after the expression - what it wanted
4. the caret ( ^ ) pointing at the exact column, with what character is missing underneath.

The complier is able to locate the error fast and tell you want needs to be fixed / changed. Read the first error in full before changing anything as it could be the root problem to any other errors that may have occurred. 

#### 1.6 Compile Time Vs. Runtime

```Terminal
clang++ -std=c++20 -Wall -Wextra hello_world.cpp -o hello_world # compile time
./hello_world                                                   # run time
```

- Compile Time - Everything that happens while your program is being built, while the compiler reads your source code and produces and executable. 
- Runtime - is when your finish executable actually executes. 

in C++ the boundary is a file. to the left of it: source code and a complier reading it. To the right: a native executable that no longer needs the compiler, source code or you. 

#### 1.7 One Command, Four Stages

`clang++` runs four steps:
- **Preprocess** — expands `#include` directives and macros. Pure text substitution. The output
is a single translation unit.
- **Compile** — translates that C++ text into assembly for your CPU. Most syntax and type
errors surface here.
- **Assemble** — turns assembly into machine code inside an object file (.o). Not yet runnable.
- **Link** — stitches object files and libraries into one executable. undefined symbol errors
live here: the compiler was satisfied, but the linker could not find a definition for something
you called.

All four stages are compile time. Every error from any step is considered a compile-time error as the application is never created. 

#### 1.8 Doing it By Hand

You can split compiling and linking. This allows you to see the actual object file(s). using `-c` in your command allows for compiling and assembling but no linking, creating object files.

`clang++ -std=c++20 -c hello_world.cpp -o hello_world.o    # stop after assemble`
`clang++ hello_world.o -o hello_world    # link the .o into an executable`

#### 1.9 Why CMake

Tying `clang++` by hand is fine for one file but not fifty. [[C++ Glossary |CMake]] is the build system the embedded electives use, so you start with it now. You get to describe what to build and `CMake` generates the commands. 
```C++
cmake_minimum_required(VERSION 4.4)
project(FirstApp CXX)

set(CMAKE_CXX_STANDARD 20)

set(CMAKE_CXX_STANDARD_REQUIRED True)
add_executable(hello hello_world.cpp)
```

Build it out-of-source, so everything generated stays in build/

``` Terminal
cmake -G "MinGW Makefiles" -B build 
# configure: read CMakeLists.txt, generate build files

cmake --build build 
# compile + link

.\build\hello.exe 
# macOS/Linux: ./build/hello
```

- Pass -G "MinGW Makefiles" every time on Windows. The name is a historical wart: it selects the
make program, not the compiler. Your setup script already pointed `CMake` at clang. 
- Never edit anything inside of the build directory. It is generated by the `CMakeLists.txt` and thrown away on the next build. If somethings wrong, change `CMakeLists.txt`

##### 1.9.1 Why that First Line says 4.4

- `cmake_minimum_required(VERSION 4.4)` says your project needs CMake 4.4 or newer. Older makes will refuse to compile. 
- It also changes how `CMake` behaves. Writing Version 3.10 doesn't just permit old CMake, it asked your modern CMake to behave like the old one, switching on years of backward compatibility rules to protect projects written before those rules changed. 

#### 1.10 Worked Examples

**Problem:** Split a one file program into two source files then combine them - By hand and with CMake

**By Hand:**

*Greet.cpp File*
```C++
#include <iostream>

void greet() {
	std::cout << "Hello, Embedded World!\n";
}
```

*Main.cpp File*
```C++
#include <iostream>

void greet(); // Declaration - Greet() exists somewheres

int main() {
	greet();
	
	return 0;
}
```

*Terminal Commands*
```Terminal
clang++ -std=c++20 -Wall -Wextra -c greet.cpp -o greet.o
clang++ -std=c++20 -Wall -Wextra -c main.cpp -o main.o
clang++ greet.o main.o -o app
./app
```

**With CMake**

*CMakeLists.txt File*
```CMakeList.txt
cmake_minimum_required(VERSION 4.4)
project(App CXX)
set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED True)
add_executable(app main.cpp greet.cpp)
```

*Terminal Commands*
```Terminal
cmake -G "MinGW Makefiles" -B build
cmake --build build
.\build\app.exe
```
#### 1.12 Common Errors

![[Pasted image 20260929172149.png | Diagram from Page 18]]