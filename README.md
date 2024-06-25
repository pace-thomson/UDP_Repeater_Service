## Overview

This application was developed to solve a problem where multiple machines needed to get the same UDP Multicast messages. 
It was built as a Windows Service backend that ineracts with unser interface frontend. Included in this repo is the installer
for the application. 


<p align="center">
  <img src="./UDP Packet Repeater Flowchart.png" alt="Flowchart">
</p>

## Backend

This Windows Service works by starting a listening connection on a port that is specified by the configuration json file. Whenever 
a new packet is a recieved, two things happen: A copy of the packet is sent out to the target "Send To" machine and information about
the packet is sent the front end. At the same time, a task is listening continually for communication from the frond end, which means
the user has input new configuration settings. The service then updates the configuration json file and restarts the thread that is 
listening for packets. The backend is configured to begin running on bootup.

## Frontend
The GUI is a windows forms application that diplays statistical data about the system running, displays the system's logs, and provides 
a way for the user to update the system settings. The main form of the frontend is the form that displays statistical data. From there, 
the user can open forms for settings reconfiguration or the system's logs. The GUI runs as a startup application when the user logs in. 
It starts minimized and can only be opened from the system tray.

## Installer
The installer for this application does all the work of installing both sides of the system and registering them. Running the .msi file
will do everything needed to get this system up and running. 
