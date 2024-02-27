# Video Management - Video-On-Demand (VOD) Processing Service
## Introduction
This repository serves as a reference project to demonstrate gained knowledge and skills.
It's a picked service (and specifically adjsuted for reference project purposes) from a private system/platform called `PowerTrainer` that I develop in my free time. 

Purpose of this service is to fulfill requirements that the system has to manage its own video content, following requiremetns are:
- Video conent has to be private and exclusive for the platform (not accessible by outside  world), public platform like YouTube cannot be used
- Video content uploading and encoding for VOD formats
- Video on demand streaming (videos playable anytime)
- Video downloading

There are a few different services in the system that need to work with the video content, so that is why I decided to create a separate service managing directly those needs. It provides an API client that can be used by other services to start video uploading and encoding workflows. When those processes finish (by publishing integration events), those services can request necessary links for playing VOD and downloading the content. This content is distributted via CDN to make it private, secured and globally available with reasonable response time.

AWS was chosen because it provides all necessary components for VOD functionalities (MediaConvert, CloudFront, S3 bucket) to implement this service in optimal and scalable way.

The primary purpose of creating this service is to establish a clear separation between the PowerTrainer system and various AWS components (such as MediaConvert, CloudFront, S3 bucket, Amazon SQS, etc.) that are crucial to managing video content. This service plays a crucial role in handling AWS events through Amazon SQS. These events might include notifications of new video uploads needing encoding or alerts indicating the completion of video encoding.

The service processes these AWS events to trigger appropriate workflows. Additionally, it converts these AWS events into internal integration events that are tailored for the PowerTrainer platform. This conversion is significant because it allows other services within the platform to detect and respond to these events. For example, if a service has requested a video upload for future playback, the video management component will issue an integration event once the video is successfully encoded. This event informs the service that the video is now ready for use, ensuring smooth delivery and playback as per the original request.

The service additionally plays a key role in controlling access to the video content. It ensures that only authorized services have the capability to interact with the video content. This is crucial for maintaining the security and privacy of the content. To achieve this, the service utilizes CloudFront signed URLs and cookies, as detailed in the AWS documentation ([CloudFront signed URLs and cookies](https://docs.aws.amazon.com/AmazonCloudFront/latest/DeveloperGuide/PrivateContent.html)). This method ensures that access to the video content is not only restricted to permitted entities but also securely managed, maintaining the integrity and confidentiality of the video data.

## Technologies Used
- **.NET 8/C# 12**
- **ASP.NET Core**
- **EF Core 8** - ORM
- **[Mediator](https://github.com/martinothamar/Mediator)** - Implements Mediator pattern
- **Docker** - Running services in containers
- **Testcontainers** - Used for running real database instance as docker container in integration tests
- AWS Services
    - **Amazon S3** - Storage for uploading videos to be encoded + storing and serving encoded content via CLoudFront
    - **MediaConvert** - Tool for encoding videos to VOD format
    - **EventBridge and Amazon SQS** - S3 and MediaConvert crete events that are published to SQS for processing by this service 
    - **CloudFront** - CDN used for distributing the video content globally, secured and private
- **RabbitMQ** - Message broker used for event driven design, async communication between services
- **MassTransit** - Abstraction over message broker (RMQ) providing features and integrations that are beneficial for building scalable and maintainable distributed systems 
- **Outbox pattern** - Outbox implementation for storing domain events in database that are later on published and handled by mediator notifications handler. So database changes and message publishing/side-effects are in one transaction.
- **Quartz.NET** - scheduler used for outbox implementation

## Architecture Overview
High-level architecture description of the project. Optionally, include an architectural diagram.

## Business Use Cases

In this section, you can elaborate on various business scenarios and use cases that your VOD Processing Service addresses. For each use case, describe the problem, how your service provides a solution, and the benefits it offers.

### Use Case 1: Video upload
- **Description**: Briefly describe the business scenario.
- **Solution**: Explain how your service addresses this scenario.
- **Benefits**: Highlight the advantages or improvements your service offers, such as increased efficiency, cost savings, improved user experience, etc.

### Use Case 2: Video encoding for VOD formats
- [Repeat the format above]

### Use Case 3: Playing a video on demand
- [Repeat the format above]

### Use Case 4: Video download
- [Repeat the format above]
...

Feel free to add as many use cases as are relevant to demonstrate the versatility and applicability of your service in various business contexts.

## Project Structure

This section provides an overview of the key components of the project, detailing the organization and purpose of different directories and files. This helps in navigating and understanding the project's codebase more efficiently.
...
```
├───Common  (Reusable functionalities, can be exported as nuget)
│   ├───Abstractions
│   ├───Auth (Authentication and authorization features)
│   │   ├───Handlers (Http message handler for adding access tokens to request)
│   │   ├───TestHelpers
│   │   └───TokenServices (Services for retrieving and caching access token)
│   ├───Middleware
│   ├───Repositories (Repositories abstractions)
│   ├───ResultTypes (Implementation of result pattern with OneOf)
│   ├───Swagger
│   └───Validations (Validation pipeline behavior for mediator)
├───VideoManagement (Service implementation)
│   ├───Database (DB Entities configuration, DbContext, ...)
│   ├───Features
│   │   ├───BlobStorage (Storage services)
│   │   ├───Media (Media encoding services)
│   │   ├───Resource (Resource signing services)
│   │   └───Videos (Video related features)
│   │       ├───DomainEvents (Video domain events with specific handlers)
│   │       ├───Download (Video download related features)
│   │       ├───Encode (Video encoding related features)
│   │       │   ├───EncodingCompleted
│   │       │   ├───EncodingFailed
│   │       │   └───MediaConvertEvents
│   │       ├───Entity
│   │       ├───Repository
│   │       ├───Stream
│   │       └───Upload
│   ├───Migrations (EF Core migrations)
│   ├───Options (Option pattern used for configuration)
│   ├───Outbox (Outbox pattern job implementation)
│   └───Properties
└───VideoManagement.Contracts (Contracts published as nuget for other services)
    ├───Api (Video management client)
    │   └───V1
    └───IntegrationEvents (Integration events published by video management)
```

Remember to customize the directory and file names based on your actual project structure. The goal here is to provide a clear map of your project for easier navigation and understanding.
## Configuration
