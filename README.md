Overview:
-
- The shopping cart system includes the following functionalities: order searching, payment processing, and sending notification emails to customers.

High-Level Architecture:
-
<img width="1160" height="655" alt="image" src="https://github.com/user-attachments/assets/31acfb5a-2145-469b-9f2c-2acc50117d07" />

Components:
- 
- Load Balancer: Distributes incoming traffic evenly across gateway pods.
- API Gateway: Handles routing, rate limiting, and authorization. Options include Ocelot or NGINX.
- Read API: Provides query capabilities such as searching for orders by product name.
- Checkout API: Manages the payment workflow of the system.
- Queue: Acts as a task buffer for asynchronous jobs. Kafka can be used, but in this case, a database-based queue is applied.
- Worker: A scheduled job that dequeues and processes asynchronous tasks from the queue.

Database Design:
  -
  <img width="1023" height="370" alt="image" src="https://github.com/user-attachments/assets/353171f4-0363-4de7-86d3-4ccc79314d67" />

- Product: Stores product-related information, such as product name and code.
- Order: Contains order details, including the selected product and current order status.
- Audit: Prevents duplicate request handling and ensures no two orders are processed simultaneously.
- Queue: Stores queued actions along with their execution state (e.g., pending, processing, completed, failed, etc.).

Detailed Workflow
-
Checkout:
- 
Api:
-
<img width="678" height="502" alt="image" src="https://github.com/user-attachments/assets/d28dd47d-ed78-4b8d-8412-f76f0d8f9883" />

- Users select and check out their orders via the UI, which triggers an HTTP call to the backend server to initiate the checkout process.
- The first step is auditing: for every client-side action, the system attaches a unique audit GUID to each order and stores it in Redis (in this case, the audit status is saved to the database instead) to mark the order as being processed. Therefore, if a user double-clicks or the order is already under processing, the system returns Conflict to the client.
- Then, the payment step is executed.
- After a successful payment, the system classifies the orders into groups and handles each group in separate pipelines based on the Type field of the order — allowing easy extension when more product types are added in the future.
- Once these steps are completed, the system responds to the client with a success message stating: "Payment successful, order is being processed", while a background worker continues the remaining tasks.
- The system is stateless, so it can easily scale horizontally by adding more pods.

Worker:
-
<img width="588" height="596" alt="image" src="https://github.com/user-attachments/assets/5e9c96d5-5aee-4580-855b-481ba2fc1fa3" />

- Workers run on a continuous interval, consuming data from a queue table.
- Tasks with potentially high latency, such as calling the email service, product service, or invoice service, are offloaded to these workers to reduce load on the API and improve data consistency.
- Worker pipelines operate independently and maintain state, enabling them to resume processing in case of failure.

Search Api:
- 
<img width="458" height="376" alt="image" src="https://github.com/user-attachments/assets/6cac1144-1cb5-4565-9f27-87b7c6179ebb" />

- A view is created to wrap multiple related tables into a unified dataset.
- A GIN index is created on the NameVector column to optimize full-text phrase search.

Summary:
-
Scale: 
-
- The use of MediatR and the CQRS pattern clearly separates commands and queries, making the system easier to manage and extend.
- The flexible order processing design improves maintainability and supports future product types.
- The API is stateless and supports horizontal scalability.
- Backend services are designed as task-specific workers to isolate responsibilities.

Integrity of Data:
- 
- The system follows the Outbox Pattern.
- The queue table is responsible for managing the steps of each transaction. Each processing step appends an order state, making tracking straightforward.
- Individual tasks are handled by backend services, increasing API availability.
- Improvement: After the API finishes processing and saving transaction steps to the queue table, it pushes a notification to a message queue like Kafka. Backend services act as consumers, reducing message processing latency and allowing scalable backend architecture.

Performance:
- 
- Components are easily horizontally scalable.
- Order transactions are processed in parallel.
- Backend processes tasks in batches.
- Two parallel columns are added for name search optimization: one for full-text search vector, and one for accent-insensitive (unaccented) name.
- Data structures are designed with paging in mind, improving response time for reloads and searches.
- Improvement: Use a master-slave (primary-replica) database architecture to optimize read/write throughput. When the volume of Order or Queue data increases, the system can migrate to PostgreSQL’s [TimescaleDB] for better time-series performance.

Structure:
- 
- Về cấu trúc: hệ thống gồm các thành phần chính như sau:
<img width="162" height="240" alt="image" src="https://github.com/user-attachments/assets/f7e8966e-09ba-4b1f-86cb-77c66e653e29" />

- Structure: Analyze and design the system architecture, select technologies, and define code structure. (1 lead)
- Application: Analyze business requirements, plan UI/UX flow, define page count, UI components, and overall client-side structure. (2–3 engineers)
- Backend: Choose appropriate service patterns (e.g., timed service, worker, or scheduler), and design task-specific pipelines. (1–2 engineers)
- API: Define APIs for the application layer, handle user permissions, and choose appropriate design patterns. (2–3 engineers)
- Domain: Design the database schema, create suitable DTOs based on requirements, and optimize EF Core queries. (1 engineer + 1 DBA)
- Shared: Implement constants, enums, extensions, and helpers to support seamless interaction between components. (1 engineer)
- DevOps: Handle CI/CD pipelines, Splunk integration, OpenTelemetry, and infrastructure deployment. (1 engineer)

TimeLine:
- 
- Development timeline: July 11–13 (Git-based tracking)
- Planning & analysis: July 11, 6:00 PM – 10:00 PM
- Project initialization & README file setup: July 11, 10:00 PM – 10:51 PM
- API/Backend setup: July 12, 10:00 AM – 7:40 PM
- WebApp (Frontend) setup: July 13, 8:00 AM – 2:10 PM
- Finalizing and compiling SQL scripts: July 13, 5:00 PM – 7:00 PM
- Completed README documentation: July 13, 8:00 PM – 9:30 PM

