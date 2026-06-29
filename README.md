# 🔐 SecurityApi

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-OpenAPI-85EA2D?style=flat-square&logo=swagger&logoColor=black)
![Blog](https://img.shields.io/badge/blog-rsrsystem-FF5722?style=flat-square&logo=blogger&logoColor=white)

> **Web API en ASP.NET Core (.NET 10)** con servicios de **cifrado, descifrado y tokens** para asegurar tus apps PowerBuilder.

## 📋 ¿Qué es esto?

Una API REST que centraliza la **criptografía** (AES) y la generación/validación de claves para que tus
clientes (PowerBuilder u otros) no tengan que implementarla. Entrada/salida en **Base64Url** para que
viaje cómoda por la URL.

## ✨ Endpoints

| Método | Ruta | Qué hace |
|--------|------|----------|
| `POST` | `/api/Security/encrypt` | Cifra `source` con `key` + `iv` |
| `POST` | `/api/Security/decrypt` | Descifra `source` con `key` + `iv` |
| `POST` | `/api/Security/token`   | Genera `key`/`iv` a partir de un token y la clave maestra |
| `POST` | `/api/KeyGenerator/validate` | Valida una clave |

## 🧩 Dependencias

| Paquete | Versión |
|---------|---------|
| [Microsoft.AspNetCore.Mvc.NewtonsoftJson](https://www.nuget.org/packages/Microsoft.AspNetCore.Mvc.NewtonsoftJson) | `10.0.9` |
| [Swashbuckle.AspNetCore](https://www.nuget.org/packages/Swashbuckle.AspNetCore) | `10.2.3` |

## 🛠️ Requisitos

- **.NET SDK 10.0** o superior

## 🚀 Ejecutar

```bat
dotnet run --project SecurityApi
```

Levanta con **Swagger** para probar los endpoints desde el navegador.

---

📨 **Blog:** <https://rsrsystem.blogspot.com/>

> ¡Nos vemos en el próximo artículo! Y recuerda: en PowerBuilder, los límites solo están en nuestra imaginación. 🚀
