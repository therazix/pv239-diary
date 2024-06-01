# PV239 Project - Personal Diary Management Application

## Authors:
- Filip Vágner (541699)
- Jana Stopková (540463)

## Overview
The Personal Diary Management Application is designed to help users maintain a digital diary, enabling them to create, organize, and manage their personal entries efficiently. This application supports adding multimedia, categorization, and various other functionalities to enrich the user experience.

## Features

### Introduction Screen
- Overview of entries: Provides a comprehensive view of all diary entries.
- Filtering and sorting: Filter entries by creation date, mood and labels. Sort entries for easier navigation.
- New entry creation: Add new entries using a form. Users can select a template to pre-fill the entry details, which can then be customized.
- Calendar view: Display entries in a calendar format. Users can select a specific date to show only the entries that were added on that day.

### Entry Detail
- Display entry information: Shows all details related to an entry, including creation date, modification date, location, labels, mood and attached media.
- Entry management: Edit, delete, or mark entries as favorites for quick access. Users can also assign labels to entries for better organization and filtering. Additionally, users can attach images and videos to enrich their entries.

### Labels
- Labels overview: Manage and view all labels.
- Label management: Add, edit (name and color), or delete labels.

### Templates
- Templates overview: Manage and view all templates.
- Template management: Add, edit, or delete templates to simplify the creation of future entries.

### Time Machine
- Anniversary view: Show all entries that were created on this day in previous years. This feature allows users to reflect on their past by displaying alerts such as "Last year on this same day you added this entry..."
- Push notifications: Receive push notifications when an anniversary arrives, reminding users of entries from previous years.

### Gallery
- Media overview: View all images and videos associated with entries by the date they were created.

### Map
- Google Maps integration: View entries on a map with points representing their locations.

### Mood Tracking
- Mood graph: Visualize mood development over time with multiple graphs.

### Export/Import
- Data management: Export and import all data for backup and transfer purposes.

## Installation
To install and run the latest version of the Personal Diary Management Application, follow one of the methods below:

### Method 1: Install Pre-Built Application
Visit the [release page](https://github.com/therazix/pv239-diary/releases/latest) on GitHub to download the latest `.apk` and `.aab` files for your Android device.

### Method 2: Build from source
If you prefer to build the application directly from the source code, follow these steps:

1. Install .NET
Download and install the .NET SDK. Follow the instructions on [this page]((https://learn.microsoft.com/en-us/dotnet/core/install/windows)).


2. Install the MAUI Workload
```sh
dotnet workload install maui
```

3. Clone the repository
```sh
git clone https://github.com/therazix/pv239-diary.git
```

4. Change directory
```sh
cd pv239-diary
```

5. Restore project dependencies
```sh
dotnet restore Diary/Diary/Diary.csproj
```

6. Build application
```sh
dotnet build Diary/Diary/Diary.csproj -f net8.0-android -c Release
```
- The build files will be located in the `Diary/Diary/bin/Release/net8.0-android` directory.
- If you want to sign the application, follow the steps provided [here](https://learn.microsoft.com/en-us/dotnet/maui/android/deployment/publish-cli?view=net-maui-8.0).
