## Sample Kafka apps running on .NET 5

Following loosely along with the Pluralsight course here (using C# instead of Java): 

https://app.pluralsight.com/library/courses/designing-event-driven-applications-apache-kafka-ecosystem/table-of-contents

The SimpleHostedService sample was following the CodeOpinion guide here:
https://www.youtube.com/watch?v=n_IQq3pze0s

kafka is running in Docker using the docker-conpoise found here:

https://docs.confluent.io/platform/current/quickstart/ce-docker-quickstart.html#ce-docker-quickstart

### Get Started

Run the following command in the root dir to stand up kafka

```
docker-compose up -d
```

You can then run each sample application using

```
dotnet run
```