thinkgear-csharp-starter
========================
Barebones Windows form application for connect with ThinkGear EEG device (e.g. MindWave headset).
The program opens a connection with a single headset on a given COM port, and prints out the
attention, meditation, and signal quality data points.  More headset data can be printed out by making
more calls to `ReadPacket`.

Install
=======
The project is a VS 2010 solution using .NET 4.0. The program should not depend on any of the ThinkGear software,
as all interaction with the headset is done using the `thinkgear.dll`.

ThinkGear API
=============
The API is contained in the `thinkgear.dll` file. The interface with C# is done via the `ThinkGear` class.
I copied this class out of the `ThinkGear Communications Driver\win32` example code provided with the 
developer kit. I had to make small modifications to the `DllImport` statements, but the rest is their code.

Resources
=========
http://developer.neurosky.com/ - Neurosky developer tools (including API docs)

Author
======
Kristofer D. Kusano
2 June 2014