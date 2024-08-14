
# Overview
This application was developed to solve a problem where multiple machines need to get the same UDP packets. It functions by listening for 
packets on a host machine using a raw socket to not disturb the packet's intended destination from receiving it. It then sends the payload of 
that packet out to a taget endpoint using UDP Unicast, Multicast, or Broadcast. It was built as a Windows Service backend that interacts 
with a GUI frontend for user interaction and system monitoring. Included in this repo is the installer for the application. 

<p align="center">
  <img src="./Images_For_README/UDP_Packet_Repeater_Flowchart.png" alt="Flowchart">
  <em>System Task Flow</em>
</p>


## Service/Backend
This Windows Service/Backend works by opening a raw socket with an endpoint that is specified by the user in the configuration JSON file under "Receive From". The 
socket used is raw so that it won't prevent the intened recepient of the packet from receiving it. Two things happen whenever a new packet is received: A copy of 
the packet's payload is sent out to the target "Send To" endpoint, and information about the packet is sent to the GUI/Frontend. At the same time, a task is listening 
continually for communication from the GUI/Frontend. This communication is  made by sending packets over the loopback card. The Backend/Service receiving a packet 
from the Fronted/GUI means the user has input new configuration settings. The service then updates the configuration JSON file and restarts the thread that is listening 
for packets. The service is configured to begin running on system bootup. This service reports its' memory used, total packets handled, and packet handling time to a 
user configured Prometheus endpoint.

## GUI/Frontend
The GUI/Frontend is a Windows Forms application that displays statistical data about the system running, aggregates and displays the system's logs, and provides 
a way for the user to update the system's settings. The main form of the frontend is the form that displays statistical data, and holds the buttons that navigate 
to the other children forms of the application. Among those children are forms for settings reconfiguration or system log display. The GUI runs as a startup 
application when the user logs in. It is intended to run in the background and as such, only runs out of the system tray.

## Installer
The installer for this application is a 2 part wix toolset bootstrapper. It first installs .NET Framework 4.7.2 and then runs the main installer for the actual UDP 
Repeater Service. This installer downloads all the needed files for operation, registers the Service/Backend with the Windows Service Controller, and sets up 
the GUI to run on startup. Running [Packet_Repeater_Installer.exe](Packet_Repeater_Installer.exe) will do everything needed to get this system up and running. 

## Monitoring
This system implements a Prometheus/Loki/Grafana monitoring stack for simple and powerful observability. System endpoints for Prometheus metrics and 
Loki logging are user configurable through the GUI. This data is visualized using a grafana dashboard. Dashboard setup information is incluced in this repo
in [Dashboard_Config.json](Dashboard_Config.json). 

I run the Prometheus server with the following command and parameters:
```shell
PS C:\Users> .\prometheus.exe --enable-feature=otlp-write-receiver
```
I run the Loki server with the following command and parameters:
```shell
PS C:\Users> .\loki-windows-amd64.exe --config.file=loki-local-config.yaml
```

<p align="center">
  <img src="./Images_For_README/Dashboard_Screenshot.png" alt="Grafana Dashboard Screenshot">
  <em> Running Grafana Dashboard </em>
</p>
