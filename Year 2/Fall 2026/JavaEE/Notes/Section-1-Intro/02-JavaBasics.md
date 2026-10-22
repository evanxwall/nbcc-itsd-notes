*Date: September 15th, 2026
Class: Enterprise Java
Slides: Java - Class 02 - Java Basics.pptx*

*Refer to slides (or textbook) for any missing examples / diagrams, all should have an alternate text saying what page or slide they are from. Everything should be pasted as they are seen within the slides. A note will be made if the image is later or earlier in the slide*

### Primitive Data Types
- In Java, the sizes of various data types are fixed.
- Integers in Java are always 4 bytes, no matter the hardware or code. This isn't true for C# or C++

![[javaDataTypes.png| Diagram from slide 4]]

### Java Classes
- For SE apps, the main class must be public and have a main method.
- Class name must begin with a capital letter (Use Camel Casing)
- Class name must be a noun, or noun preceded by an adjective. Ex: BigCar

```java
public class ExampleApplication {

	public static void main(String[] args) {
		// This is where you can also declare some variables
		
		int myNumber;
		myNumber = 10;
		
		String myName = "Evan Wall";
	}
}
```
*Example of Java with a main function*

### Control Statement's
- If, Switch statements and Loops (For, While, do-while) all work the same in Java as they do in C#
- Foreach loops are a little different.

```java
for (String s : list) {
	System.out.println(s);
}
```
*For-each Loop in Java*

```java
public class ExampleApplication {

	public static void main(String[] args) {
		
		int myNumber;
		myNumber = 10;
		
		if (myNumber != 10) {  
		    System.out.println(myNumber + "Does not Equal 10");  
		}  
		else {  
		    System.out.println("Your Number is Equal to 10");  
		}
	}
}
```
*Basic If Statement*

```java
public class ExampleApplication {

	public static void main(String[] args) {
		
		int myNumber;
		myNumber = 10;
		
		while (true) {  
		    System.out.println(myNumber);  
			if (--myNumber < 5) {  
		        break;  
		    }  
		}
	}
}
```
*Basic While Loop, pretty much the exact same to C#*

### Functions
- Also very similar to C#, still uses a lot of the same key words and structure as C# Functions

*Simple Math function, functions can also be overloaded*
```java
public class ExampleApplication {

	public static void main(String[] args) {
		
		int myNumber;
		myNumber = 10;
		
		System.out.println("\nYour number times 10 equals: " + timesTen(myNumber));
		
	}
	
	public static int timesTen(int n) {
		return n * 10;
	}
}
```

### Prompting for Input
- You can prompt a user for input using the Scanner object
- You need to include java.util.Scanner at the top of your code

*Basic User Input Example*
```java
import java.util.Scanner; // Needed for the scanner object

public class ExampleApplication {

	public static void main(String[] args) {
	
	// Scanning for String
	System.out.println("\nPlease Enter your First Name:");  
	Scanner idScan = new Scanner(System.in);  
	String scannedName = idScan.next();  
	System.out.println("\nYour First Name is: " + scannedName);  
	  
	// Scanning for Int
	System.out.println("\nPlease Enter your ID:");  
	int scannedID = idScan.nextInt(); // Same Scanner object
	System.out.println("\nYour ID is: " + scannedID);
	}
}
```

### Java Classes Continued
- You can create more than one class, just like C#
- Unlike C#, Java doesn't have property syntax so you have to create your own getter / setter methods instead of just doing { get; set; } for a property

*Example Class Creation with it being called in main*
```java
// Product Class
public class Product {  
    private int productId;  
    private String productName;  
    private double productPrice;  
  
    public Product (int ProductId, String ProductName, double ProductPrice)  
    {  
        this.productId = ProductId;  
        this.productName = ProductName;  
        this.productPrice = ProductPrice;  
    }  
  
    public int getProductId() {  
        return productId;  
    }  
  
    public String getProductName() {  
        return productName;  
    }  
  
    public double getProductPrice() {  
        return productPrice;  
    }  
  
  
}
```

```java
// Creating an Object in our Example App
public class ExampleApplication {

	public static void main(String[] args) {
	
		Product Car1 = new Product(1, "Corolla", 9999.99);  
	  
		System.out.println(Car1.getProductId());  
		System.out.println(Car1.getProductName());  
		System.out.println(Car1.getProductPrice());
	

	}
}
```

#### Regular and Static Class Members
- Classes are a user defined type
- Classes have properties and methods
- An object is an *instance* of a class
- By default, independent copies of variable and methods are created for each object
- If a variable or method is marked static, it belongs to the class itself rather than to individual objects. This means there is only one shared copy that all instances of the class can access.

#### Public and Private Class Members
- public fields & methods are accessible to users of a class
- private fields & methods are only accessible to the class itself
- In C#, protected means "available to any class that inherits from this class"
- In Java, protected means "available any class in the same package"

#### Static Methods and Variables
- A static method can be called without any instance of the class
- For static fields, there is only one ‘copy’, shared by all instances
- Static variables are often constants – e.g. Integer.MAX_INT
- Inside a class, a static method cannot access any non-static fields and cannot call a non-static method
- Regular methods can also access static methods & fields

### Arrays In Java
- Similar to C#
- When declared, the size must be known and doesn't change
- Can contain simple types (int, double, String, etc...)
- Can contain many instances of a class
- Can contain many instances of different compatible classes

*Examples of Different Arrays*
```java
// Simple Numbers Array

public class ExampleApplication {

	public static void main(String[] args) {
	
		int[] numbersArray = new int[10];  

		for (int i = 0; i < 6; i++) {  
		    numbersArray[i] = i;  
		    System.out.println(numbersArray[i]);
		}
	}
}
```

```java
// Simple String Array
public class ExampleApplication {

	public static void main(String[] args) {
	
		String[] stringArray = new String[3];  
		stringArray[0] = "Arrays Are Cool";  
		stringArray[1] = "Arrays Are Awesome";  
		stringArray[2] = "Arrays Are Amazing";  
		  
		for (String s : stringArray) {  
		    System.out.println(s);  
		}
	}
}
```

```java
// Simple Object Array
public class ExampleApplication {

	public static void main(String[] args) {
	
	// Using the same Product class from earlier
		Product[] productsArray = new Product[3];  
  
		productsArray[0] = new Product(2, "Apple", 0.99);  
		productsArray[1] = new Product(3, "Book", 24.99);  
		productsArray[2] = new Product(4, "Headset", 89.99);  
		  
		for (Product p : productsArray) {  
		    System.out.println(p.getProductName());  
		}
	}
}
```

```java
// Passing Arrays Into a Function
public class ExampleApplication {

	public static void main(String[] args) {
		
			int[] intArray1 = new int[3];  
			int[] intArray2 = new int[3];  
			  
			compareLength(intArray1, intArray2);
	}
	
	public static void compareLength(int[] array1, int[] array2) {  
	    if (array1.length != array2.length) {  
	        System.out.println("Array Lengths do not Match");  
	    }  
	    else {  
	        System.out.println("Array Lengths do Match");  
	    }  
	}
	
}
```

### ArrayLists in Java
- ArrayList is a class that manages a Dynamic Array
- Has it's own properties and methods (add, sort, remove, clear, contains, indexOf, etc...)
- Need to import java.util.ArrayList; at the top of code

```java
import java.util.ArrayList

public class ExampleApplication {

	public static void main(String[] args) {
		
		ArrayList<Integer> intArrayList = new ArrayList();  
		intArrayList.add(1);  
		intArrayList.add(100);  
		intArrayList.add(43543); 
		  
		ArrayList<String> stringArrayList = new ArrayList();  
		stringArrayList.add("Chad");  
		stringArrayList.add("Brad");  
		stringArrayList.add("Thad");
		
		System.out.println(intArrayList.get(2));  
		System.out.println(stringArrayList.get(0)); 
		// Can be replaced with .getFirst();
		
	}
}
```

#### Arrays Vs. ArrayLists

| Array                                  | ArrayList                                      |
| -------------------------------------- | ---------------------------------------------- |
| - Arrays have a .length element        | - ArrayList has size() function                |
| - Arrays length can not change         | - ArrayList can grow or shrink as needed       |
| - Array elements are accessed with [i] | - ArrayList elements are accessed with .get(i) |
