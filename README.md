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
  <img width="592" height="240" alt="image" src="https://github.com/user-attachments/assets/5711c210-0d7e-4173-9b02-97ae9b0902dc" />

- Product: Stores product-related information, such as product name and code.
- Order: Contains order details, including the selected product and current order status.
- Audit: Prevents duplicate request handling and ensures no two orders are processed simultaneously.
- Queue: Stores queued actions along with their execution state (e.g., pending, processing, completed, failed, etc.).

Detailed Workflow
-
Checkout:
- 
Api:
<img width="752" height="479" alt="image" src="https://github.com/user-attachments/assets/e46d93f1-7850-4dd1-a206-5a199013d747" />
-
Worker:
<img width="704" height="496" alt="image" src="https://github.com/user-attachments/assets/1306b635-fef7-4e64-bf60-26c7a99603fc" />
-



