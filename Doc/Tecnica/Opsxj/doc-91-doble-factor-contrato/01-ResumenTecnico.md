# DOBLE-FACTOR-CONTRATO

- Ticket: DOC-91
- Cambio OpenSpec: doc-91-doble-factor-contrato
- Clasificacion: cross_cutting

## Objetivo

Crear la fundación interna de Login 2FA para WebForms VB.NET/net461: contratos tipados, OTP criptográfico, HMAC versionado, llaves externas y contexto pendiente mínimo. Esta entrega no activa el segundo factor ni modifica un flujo productivo.

## Alcance y compatibilidad

El cambio queda limitado a nuevas rutas de Modelo, DTO, seguridad y adaptador Session, más pruebas y el `.vbproj`. Persistencia MySQL, SMTP, servicios, ASMX e interfaz quedan fuera. `gestor.aspx`, `ClassGestorSesion`, `ClassCorreo`, Forms Authentication y recuperación de contraseña deben conservarse sin cambios.

La documentación exhaustiva verificada contra código se encuentra en `Doc/Actualizacion/Login/Implementacion/DOC-91/README.md`; incluye cuatro diagramas Mermaid, casos de uso, inventario y pendientes.
