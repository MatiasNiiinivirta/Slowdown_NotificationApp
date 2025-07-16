Video: https://github.com/user-attachments/assets/1ddb5e04-5f2a-45e9-b2d1-1264f9911d95

Slowdown - Real-Time Notification App

Slowdown is a real-time notification app developed using ASP.NET Core and SignalR. It was originally created as a school project to explore solutions for challenges in remote education. The core idea is to provide students with a simple way to notify the teacher when the pace of teaching feels too fast. If the counter goes up significantly, it's a clear signal for the teacher to slow down.

**How It Works**

On the first screen, users can either create a room or join an existing one using a room code.

![MainWindow](https://github.com/user-attachments/assets/3cb4cdd8-33fe-48fe-8884-8f4e431bf622)


The second screen (room view) includes the main functionalities: Send a notification with a single button press, See how many users are currently in the room, Leave and rejoin the session at any time.

![MainWindow2](https://github.com/user-attachments/assets/366f1394-a6be-4716-8469-d47e6ec4bdce)

The communication between users is handled via SignalR, enabling real-time updates across all connected clients.

**Features**

-Realtime notification counter visible to all participants
-Join/leave room functionality
-Room number display for easier access
-Custom sound notification with volume slider
-Participant counter
-Hosted on Azure Web App with a custom domain

NuGet packages
-Microsoft.AspNetCore.SignalR 
-NAudio
-Microsoft.AspNetCore.OpenApi
-Swashbuckle.AspNetCore

**My Contributions**

-Designed and implemented the SignalR-based connection logic
-Created the UI/UX design for both views
-Composed and produced the sound notification in FL Studio
-Drew the custom icons (e.g., volume icon)
-Extended the base app with new functionalities like user counter, volume control, and room ID visibility
-Set up the application hosting and domain via Azure Web App

**Team Roles and Contributions**

Larissa Karjalainen (SCRUM Master)
-Led the team and ensured SCRUM practices were followed
-Organized meetings, maintained schedules, and clarified task responsibilities

Otto Tirronen (Product Owner)
-Maintained the backlog and communicated with the client
-Created documentation and recorded the product demo
-Proposed and managed technical improvements (e.g., domain move to Azure)

Paula Timonen
-Implemented the backend logic for the slowdown button, the app's core functionality
-Delivered reliable, on-time work with minimal supervision

Simo Kontio
-Focused on testing and bug fixing
-Played a crucial support role by identifying and resolving issues across the app

Aleksi Kärkkäinen
-Developed supporting features such as room join validation and navigation logic
-Wrote the technical documentation and contributed through independent, focused work

**This application was originally developed in a private Azure DevOps repository rather than on my personal GitHub.**

**Note: This project was developed in a private Azure DevOps environment, so the commit history shown here does not reflect the full development process.**

**How to run:**

Download the Project
-Click the green Code button at the top right of the GitHub repo.
-Select Download ZIP and extract it to a folder on your computer.

Open in Visual Studio
-Open Visual Studio 2022.
-Choose Open a project or solution and select the .sln file from the extracted folder.

Run the App
-Click the Start button or press F5 to run the app.
