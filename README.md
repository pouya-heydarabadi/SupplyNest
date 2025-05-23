# 🏪 SupplyNest

<div align="center">

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![MongoDB](https://img.shields.io/badge/MongoDB-6.0-47A248?style=flat-square&logo=mongodb)](https://www.mongodb.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg?style=flat-square)](LICENSE)
[![Architecture](https://img.shields.io/badge/Architecture-Microservices-2496ED?style=flat-square&logo=docker)]()

</div>

## 📝 Description

SupplyNest is a modern, scalable e-commerce platform built with .NET 9.0 and MongoDB. It provides a robust foundation for managing products, inventory, and orders in a distributed system architecture. The project is evolving into a comprehensive Enterprise Resource Planning (ERP) system based on microservices architecture.

## 🎯 Vision

SupplyNest is being developed as a full-featured ERP system with the following planned modules:

- 🏭 **Manufacturing Management**
  - Production planning
  - Bill of materials
  - Work order management
  - Quality control

- 📦 **Inventory Management**
  - Real-time stock tracking
  - Warehouse management
  - Inventory optimization
  - Barcode integration

- 💰 **Financial Management**
  - Accounting
  - Billing
  - Cost tracking
  - Financial reporting

- 👥 **Human Resources**
  - Employee management
  - Payroll
  - Time tracking
  - Performance management

- 📊 **Business Intelligence**
  - Real-time analytics
  - Custom reports
  - Data visualization
  - Predictive analytics

## ✨ Current Features

- 🛍️ **Product Management**
  - Create, read, update, and delete products
  - Rich product attributes (SKU, category, brand, etc.)
  - Flexible product updates with partial modifications

- 🏗️ **Clean Architecture**
  - Domain-driven design principles
  - Separation of concerns
  - Modular and maintainable codebase

- 🚀 **Modern Tech Stack**
  - .NET 9.0
  - MongoDB for data storage
  - CQRS pattern with DispatchR
  - RESTful API design
  - Microservices architecture

## 🛠️ Technology Stack

- **Backend Framework**: .NET 9.0
- **Database**: MongoDB
- **API Documentation**: Scalar
- **Mediator Pattern**: DispatchR
- **Containerization**: Docker
- **Service Communication**: gRPC/REST
- **Message Broker**: RabbitMQ (planned)
- **Service Discovery**: Consul (planned)
- **API Gateway**: Ocelot (planned)

## 🏗️ Architecture

The system is being built using a microservices architecture with the following components:

```
SupplyNest/
├── Services/
│   ├── Catalog.API         # Product catalog service
│   ├── Inventory.API       # Inventory management service
│   ├── Order.API          # Order processing service
│   ├── Manufacturing.API  # Manufacturing management service
│   ├── Finance.API        # Financial management service
│   └── HR.API            # Human resources service
├── BuildingBlocks/        # Shared components
│   ├── EventBus
│   ├── Common
│   └── Infrastructure
└── Web/                  # Web applications
    ├── Web.Shopping
    └── Web.Admin
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MongoDB](https://www.mongodb.com/try/download/community)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/SupplyNest.git
   cd SupplyNest
   ```

2. Configure MongoDB:
   - Ensure MongoDB is running locally on port 27017
   - Or update the connection string in `appsettings.json`

3. Run the application:
   ```bash
   cd Src/Catalog/SupplyNest.Api
   dotnet run
   ```

### Docker Support

Build and run using Docker Compose:
```bash
docker-compose up -d
```

## 🔧 Configuration

The application can be configured through `appsettings.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "Catalog"
  },
  "ServiceSettings": {
    "ServiceName": "Catalog.API",
    "ServiceId": "Catalog.API-1"
  }
}
```

## 📚 API Documentation

Once the application is running, you can access the API documentation at:
- Swagger UI: `https://localhost:5001/swagger`
- Scalar: `https://localhost:5001/scalar`

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Authors

- Pouya Heydarabadi - Initial work

## 🙏 Acknowledgments

- Thanks to all contributors
- Inspired by clean architecture principles
- Built with modern .NET technologies
- Following microservices best practices 
