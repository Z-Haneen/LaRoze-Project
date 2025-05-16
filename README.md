# La Rozeî – Online Clothing Brand Platform

Welcome to the official repository for **La Rozeî**, a modern, stylish, and customer-centric e-commerce platform designed to revolutionize online clothing retail. Developed using ASP.NET MVC, C#, Bootstrap, and a suite of powerful frontend and backend technologies, La Rozeî is tailored for brands aiming to deliver a premium shopping experience.

---

## 🌟 Why La Rozeî?

- **Zero Development Time**: Quickly customizable for any clothing brand or product line.
- **Professional UX/UI**: Designed with modern aesthetics and usability in mind using Figma.
- **Tailored Experience**: Built specifically for emerging and established clothing businesses.

---

## 🎯 Who Is This Website For?

This platform is ideal for:
- Fashion entrepreneurs launching an online brand.
- Clothing businesses looking to scale with a reliable digital storefront.
- Developers or startups building MVPs for the fashion e-commerce domain.

---
## 🔍 Competitive Positioning

La Rozeî bridges the gap between boutique fashion and scalable technology:
- Offers a plug-and-play model with stylish interfaces.
- Competitive UX without the enterprise software overhead.
- Built to scale as the brand grows.

---


## 🌟 Features

- **Seamless Shopping Experience**: Intuitive navigation and responsive design ensure customers can browse and purchase with ease.
- **Admin Dashboard**: Manage products, categories, orders, and users through a comprehensive administrative interface.
- **User Authentication**: Secure login and registration system with role-based access control.
- **Shopping Cart & Checkout**: Efficient cart management and streamlined checkout process.
- **Order Management**: Track and manage customer orders with real-time updates.
- **Product Catalog**: Dynamic product listings with search and filter capabilities.

---

## 🧠 Tech Stack

### Backend
- **ASP.NET MVC 5**
- **C#**
- **Microsoft SQL Server**
- **ASP.NET Identity**
- **Entity Framework**

### Frontend
- **HTML5 / CSS3**
- **Bootstrap**
- **JavaScript / jQuery**

### Tools & Version Control
- **Git** & **GitHub**
- **Figma** for UI/UX design

---

## 🚀 Getting Started

### Prerequisites

- [Visual Studio](https://visualstudio.microsoft.com/) (2019 or later)
- [.NET Framework 4.7.2](https://dotnet.microsoft.com/en-us/download/dotnet-framework/net472)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

### Installation

1. **Clone the Repository**

   ```bash
   git clone https://github.com/EmanMS/LaRoze-Project.git
   ```

2. **Open the Solution**

   Navigate to the cloned directory and open `Graduation Project.sln` in Visual Studio.

3. **Configure the Database**

   Open the `Web.config` file and update the `DefaultConnection` string with your SQL Server instance details.

   ```xml
   <connectionStrings>
     <add name="DefaultConnection" connectionString="Data Source=YOUR_SERVER_NAME;Initial Catalog=LaRozeDB;Integrated Security=True" providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Apply Migrations and Seed the Database**

   Open the Package Manager Console in Visual Studio and run:

   ```powershell
   Update-Database
   ```

   Alternatively, run `SeedDatabase.sql` to insert default data manually.

5. **Run the Application**

   Press `F5` or click the **Start** button in Visual Studio.

---

## 📁 Project Structure

- `Controllers/` – MVC controllers
- `Models/` – Entity models
- `Views/` – Razor views (UI)
- `Scripts/` – JavaScript files
- `Content/` – CSS and images
- `App_Start/` – App configuration
- `SeedDatabase.sql` – Initial DB seed script

---

## 🧪 Testing

1. Open the **Test Explorer** in Visual Studio.
2. Build the solution to discover tests.
3. Run all or selected tests.

---

## 🤝 Contributing

We welcome contributions!

1. Fork the repository.
2. Create a new branch:

   ```bash
   git checkout -b feature/YourFeatureName
   ```

3. Commit your changes:

   ```bash
   git commit -m "Your message"
   ```

4. Push to your branch:

   ```bash
   git push origin feature/YourFeatureName
   ```

5. Open a Pull Request.

---
## 💼 What You Get

- Clean, scalable ASP.NET MVC source code
- Responsive, professional UI
- Ready-to-use shopping features
- Easily adaptable to any clothing niche

---
