# Video Management - Video-On-Demand (VOD) Processing Service
## Introduction
This repository serves as a reference project to demonstrate gained knowledge and skills.
It's a picked service (and specifically adjsuted for reference project purposes) from private system called `PowerTrainer` that I develop in my free time. 

Purpose of this service is to fulfill requirements that system has to manage its own video content, following requiremetns are:
- Video conent has to be private and exclusive for the platform (not accessible by outside  world), public platform like YouTube cannot be used
- Video content uploading and encoding for VOD formats
- Video on demand streaming (playable anytime)
- Video downloading

This service brings in following benefits to the rest of the system:

## Technologies Used
List all the technologies, frameworks, and tools used in this project. For example:
- AWS Services (S3, Elemental MediaConvert, EventBridge, SQS, CloudFront)
- RabbitMQ
- MassTransit
- [Other technologies]

## Architecture Overview
High-level architecture description of the project. Optionally, include an architectural diagram.

## Business Use Cases

In this section, you can elaborate on various business scenarios and use cases that your VOD Processing Service addresses. For each use case, describe the problem, how your service provides a solution, and the benefits it offers.

### Use Case 1: [Title]
- **Description**: Briefly describe the business scenario.
- **Solution**: Explain how your service addresses this scenario.
- **Benefits**: Highlight the advantages or improvements your service offers, such as increased efficiency, cost savings, improved user experience, etc.

### Use Case 2: [Title]
- [Repeat the format above]

### Use Case 3: [Title]
- [Repeat the format above]

...

Feel free to add as many use cases as are relevant to demonstrate the versatility and applicability of your service in various business contexts.

## Project Structure

This section provides an overview of the key components of the project, detailing the organization and purpose of different directories and files. This helps in navigating and understanding the project's codebase more efficiently.
...
```
├───Common
│   ├───Abstractions
│   ├───Auth
│   │   ├───Handlers
│   │   ├───TestHelpers
│   │   └───TokenServices
│   ├───Middleware
│   ├───Repositories
│   ├───ResultTypes
│   ├───Swagger
│   └───Validations
├───VideoManagement
│   ├───Database
│   ├───Features
│   │   ├───BlobStorage
│   │   ├───Media
│   │   ├───Resource
│   │   └───Videos
│   │       ├───DomainEvents
│   │       ├───Download
│   │       ├───Encode
│   │       │   ├───EncodingCompleted
│   │       │   ├───EncodingFailed
│   │       │   └───MediaConvertEvents
│   │       ├───Entity
│   │       ├───Repository
│   │       ├───Stream
│   │       └───Upload
│   ├───Migrations
│   ├───Options
│   ├───Outbox
│   └───Properties
└───VideoManagement.Contracts
    ├───Api
    │   └───V1
    └───IntegrationEvents
```

Remember to customize the directory and file names based on your actual project structure. The goal here is to provide a clear map of your project for easier navigation and understanding.

