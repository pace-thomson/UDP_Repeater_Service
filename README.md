## http://localhost:3000/public-dashboards/1a985cc2a46a4c17b21d86dee94e17c2

## Overview

This application was developed to solve a problem where multiple machines needed to get the same UDP Unicast messages. It functions by listening for 
packets on a host machine, and then sending the payload of that packet out to a taget machine(s). It was built as a Windows Service backend that interacts 
with a GUI frontend for user interaction and system monitoring. Included in this repo is the installer for the application. 


<p align="center">
  <img src="./UDP Packet Repeater Flowchart.png" alt="Flowchart">
</p>

## Backend
This Windows Service works by starting a listening connection on a port that is specified by the configuration JSON file. Two things happen whenever
a new packet is received: A copy of the packet is sent out to the target "Send To" machine, and information about
the packet is sent to the frontend. At the same time, a task is listening continually for communication from the frond end, which means
the user has input new configuration settings. The service then updates the configuration JSON file and restarts the thread that is 
listening for packets. The backend is configured to begin running on bootup.

## Frontend
The GUI is a Windows Forms application that displays statistical data about the system running, collects and shows the system's logs, and provides 
a way for the user to update the system's settings. The main form of the frontend is the form that displays statistical data. From there, 
the user can open forms for settings reconfiguration or to see the system's logs. The GUI runs as a startup application when the user logs in. 
It is intended to run in the background and can only be found in the system tray.

## Installer
The installer for this application does all the work of installing both sides of the system and registering them. Running the Packet_Repeater_Installer.msi file
will do everything needed to get this system up and running. 
