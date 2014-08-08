orleans-conference-demo
=======================

This is my sample code for the Orleans demo at [That Conference 2014](https://www.thatconference.com/sessions/session/2451).

To run the code, you will first need to [install Orleans](aka.ms/orleans).

Key Project List:

* **EventTypes** - NOT part of this project, which makes running the other projects impossible at this time.
* **Ingestor** - Ingests data from an EventHub consumer group and sends it to Orleans grains.
* **LocalRunner** - Runs an Orleans silo and some really basic grain code.
* **OrleansWeb** - A WebAPI project that interfaces with Orleans and exposes some simple functions to read/write data.

# License

Microsoft Developer & Platform Evangelism

Copyright (c) Microsoft Corporation. All rights reserved.

THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.

The example companies, organizations, products, domain names, e-mail addresses, logos, people, places, and events depicted herein are fictitious. No association with any real company, organization, product, domain name, email address, logo, person, places, or events is intended or should be inferred.