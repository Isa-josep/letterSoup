using System;
using System.Collections.Generic;
using System.Collections;
using static Debug; 
class Program{
    static public void menu(){
        Console.WriteLine("Choose a difficulty: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("1. Easy");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("2. Medium");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("3. Hard");
        Console.ResetColor();   
    }
    static public void easy(){
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Easy mode selected");
        Console.ResetColor();
    }
    static public void medium(){
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Medium mode selected");
        Console.ResetColor();
    }
    static public void hard(){
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Hard mode selected");
        Console.ResetColor();
    }
    static void Main(string[] args){
        Hashtable hashtable = new Hashtable();
        menu();
        int choice = Convert.ToInt32(Console.ReadLine());
        if(choice == 1){
            easy();
        }
        else if(choice == 2){
            medium();
        }
        else if(choice == 3){
            hard();
        }
        else{
            Console.WriteLine("Invalid choice");
        }
    }
}

