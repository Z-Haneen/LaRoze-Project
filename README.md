# LaRoze-Project
LaRoze is a feature-rich e-commerce platform designed for seamless online shopping.
# ✨ Larozee E-Commerce Platform

_Larozee_ is a single-provider **e-commerce web application** specializing in clothing 🧥👗.  
It is developed using **C# ASP.NET MVC** and powered by a **SQL Server** database 🛢️.  
The platform supports **client** and **admin** roles with advanced features like filtering, secure registration, and login 🔒.

---


## 📖 Project Overview
Larozee delivers a stylish and responsive shopping experience for clients 👥 while offering admins full control over products and users 🛍️👩‍💻.

---

## 🛠️ Tech Stack
- **Backend:** ⚙️ C# ASP.NET MVC (.NET Framework)
- **Frontend:** 🎨 HTML, CSS, JavaScript, Razor Views
- **Database:** 🛢️ Microsoft SQL Server
- **Authentication:** 🔒 Identity Framework / Custom SQL
- **IDE:** 🧩 Visual Studio

---

## 🚀 Features
- 🔐 **User Authentication:**
  - Register/Login as **Client** 👤
  - Register/Login as **Admin** 👨‍💼

- 🛒 **Product Management:**
  - Admins can add ➕, edit ✏️, and delete ❌ products

- 🛍️ **Product Browsing:**
  - Clients view detailed product pages (name, price, description, image)

- 🧩 **Product Filtering:**
  - Filter clothes by categories, sizes, and price ranges

- 🛍️ **Shopping Experience:**
  - (Optional) Add items to cart 🛒 and checkout 🛍️

- 💾 **Database-Driven System:**
  - Full data storage via **SQL Server**

---

## ⚙️ Installation and Setup

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/yourusername/larozee.git
   ```

2. **Open the Project:**
   - Open the `.sln` file in **Visual Studio** 🎯

3. **Configure Database Connection:**
   - Update `web.config` or `appsettings.json`:
   ```xml
   <connectionStrings>
     <add name="DefaultConnection" 
          connectionString="Server=YOUR_SERVER;Database=LarozeeDB;Trusted_Connection=True;" 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Set Up the Database:**
   - Use EF Migrations or run the provided SQL scripts 📜

5. **Run the Application:**
   - Press `F5` ▶️ to launch the website locally.

---

## 🗂️ Database Structure

**Core Tables:**
- **Users** 👥
  - `Id`, `Username`, `PasswordHash`, `Role` (Admin/Client)
- **Products** 👗
  - `Id`, `Name`, `Description`, `Price`, `Category`, `Size`, `ImagePath`
- **Categories** 🗃️
  - `Id`, `CategoryName`
- _(Optional)_ **Orders**, **CartItems** 🛒

---

## 🎯 Usage

**Client Flow:**
1. 🔑 Register an account.
2. 🔓 Login to access products.
3. 🛒 Browse and filter products.
4. (Optional) 🛍️ Add to cart and purchase.

**Admin Flow:**
1. 🔐 Login as Admin.
2. 🛠️ Create/Update/Delete products.
3. 👥 Manage users and view orders.

---
