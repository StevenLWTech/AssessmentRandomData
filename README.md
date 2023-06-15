# Assessment Random Data for StageWood Consortium, Inc.

This project is an ASP.NET Core application designed to handle data downloading, decompressing, and processing tasks automatically. The processed data is then exposed through an API endpoint.

## Features

- Automated downloading of gzipped JSON data file.
- Decompression of gzipped data file.
- Processing of data to provide insights such as the total number of records, the number of records from Florida, the person with the highest income in Florida, and the average income in Florida.
- Exposure of processed data through a REST API endpoint.

## Prerequisites

Before you begin, ensure you have met the following requirements:

- You have installed the latest version of [.NET Core SDK](https://dotnet.microsoft.com/download).
- You have a Windows/Linux/Mac machine.

## Installing Assessment Random Data

To install Assessment Random Data, follow these steps:

1. Clone the repo:

    git clone https://github.com/StevenLWTech/AssessmentRandomData.git

2. Navigate to the cloned directory:

    cd AssessmentRandomData

3. Restore the necessary packages:

    dotnet add package Newtonsoft.Json

## Running Assessment Random Data

1. Run the application:

    dotnet run

2. Once the application is running, navigate to the following URL in your browser to access the data:

    ```
    http://localhost:5000/data
    ```

