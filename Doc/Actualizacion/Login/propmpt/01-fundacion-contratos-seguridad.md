# 01 — Fundación de contratos y seguridad 2FA

## ROL ESPERADO

Actúa como arquitecto de seguridad y desarrollador senior VB.NET/.NET Framework 4.6.1.

## OBJETIVO

Implementar la fundación interna de Login 2FA sin persistencia, SMTP, ASMX ni cambios visibles en el login.

## CONTEXTO

Antes de editar, crea o continúa el OpenSpec de la Jira actual y lee completa `Doc/Actualizacion/Login/Exploracion/exploracion-doble-factor-autenticacion.md`. Contrasta sus decisiones con el código vigente; si existe una contradicción material, detente y documéntala, no inventes otra arquitectura.

## PRECONDICIONES DE RUTAS

Antes de implementar, inventaría en `design.md` nombres, namespaces y archivos exactos. Esta tarea solo puede crear o modificar:

- `Modelo/Login/SegundoFactor/` para estados, value objects, resultados e interfaces puras.
- `DTOs/Login/SegundoFactor/` para respuestas públicas compartidas previstas por la frontera; ningún DTO puede contener secretos.
- `Infrastructure/Login/SegundoFactor/Security/` para RNG, HMAC y lectura segura de llaves.
- `Infrastructure/Repositories/Login/SegundoFactor/` es la ruta reservada para implementaciones MySQL posteriores; esta tarea solo define sus puertos y no crea archivos allí.
- `webservice/Login/SegundoFactor/` exclusivamente para el adaptador de contexto pendiente en Session; ningún modelo de dominio se define allí.
- `tests/` o `tools/validation/` para pruebas/validadores, y `GestionDocumental-Docuarchi.net.vbproj` para registrar cada archivo.

No crear todavía archivos en `Services/Login/SegundoFactor/`, `Infrastructure/Repositories/Login/SegundoFactor/` ni `webservice/WebServiceLoginSegundoFactor.asmx(.vb)`. No mover clases existentes. Una desviación requiere evidencia del árbol actual y decisión explícita `D-XX`.

## REQUISITOS POSITIVOS

1. Crear modelos y resultados tipados para identidad canónica, configuración 2FA, challenge, estados, contexto pendiente y respuestas públicas.
2. Crear interfaces pequeñas para reloj UTC, generador OTP, protector HMAC, repositorio de configuración, repositorio de challenge, resolvedor de destinatario, correo y finalizador de login. Las interfaces no deben depender de WebForms, SMTP o SQL.
3. Implementar OTP de seis dígitos con RNG criptográfico y muestreo sin sesgo compatible con .NET 4.6.1.
4. Implementar HMAC-SHA256 con formato `v1:<keyId>:<base64mac>`, clave Base64 mínima de 32 bytes, entrada que incluya propósito, challenge, identidad canónica, vínculo de sesión y OTP, y comparación XOR de tiempo constante.
5. Leer llaves mediante configuración inyectable con nombres `LoginSecondFactorHmacActiveKeyId` y `LoginSecondFactorHmacKey.<keyId>`; soportar llave activa y llaves anteriores durante la vida de challenges. No incluir secretos reales en `Web.config` ni en pruebas.
6. Implementar un almacén de contexto pendiente en Session en la frontera de presentación. Debe contener solo IDs ya resueltos, login normalizado, destino enmascarado, challenge, nonce y tiempo; nunca contraseña, OTP, correo completo, credenciales técnicas ni objetos de conexión.
7. Registrar los nuevos `.vb` explícitamente en `GestionDocumental-Docuarchi.net.vbproj` usando el mismo `Compile`/`Content` de archivos equivalentes. No crear `src/`, otra aplicación ni otro framework.

## Contratos vinculantes

- Propósito inicial único: `LOGIN`.
- Estados: `CREATED`, `SENT`, `FINALIZING`, `COMPLETED`, `DELIVERY_FAILED`, `BLOCKED`, `EXPIRED`, `REVOKED`, `FINALIZATION_FAILED`.
- Máximo 5 intentos, cooldown 60 segundos, un envío inicial y hasta 2 reenvíos.
- `RequiereSegundoFactor`: `1` activo; `0`/`NULL` desactivado; otro valor inválido.
- Proveedor activo permitido: `EMAIL = 1`; TOTP y demás valores no soportados.
- Expiración válida: 1 a 10 minutos.
- Los mensajes públicos no deben revelar existencia del usuario, correo completo, SQL, excepción ni estado interno.

## RESTRICCIONES CRITICAS Y REGLAS DE ANTIRREGRESION

- No modificar `gestor.aspx`, `ClassGestorSesion`, `ClassCorreo`, base de datos, configuración SMTP, recuperación de contraseña ni Forms Authentication.
- Preservar el login existente: estas clases no deben ser llamadas por producción hasta una integración posterior explícita.
- No copiar JWT, `UserAuthContext`, `EmailSenderStub` ni payloads del Core.
- No generar logs con OTP, HMAC, llave, contraseña, correo completo o conexión.
- La fundación debe permanecer inactiva y sin alterar el login hasta integrarse deliberadamente en tareas posteriores.

## PRUEBAS OBLIGATORIAS

Reutiliza la infraestructura del repositorio. Cubre generación y formato OTP, ausencia de sesgo por contrato determinista, HMAC y separación por contexto, llave ausente/inválida/desconocida, rotación, comparación de longitudes diferentes, expiración con reloj controlado, normalización de identidad, enmascarado y rechazo de secretos en el contexto pendiente. Agrega validación estructural Roslyn/CJS solo si aporta una garantía que una prueba de comportamiento no cubre.

Ejecuta pruebas focales y:

```text
msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug
```

Si MSBuild no está disponible, registra el bloqueo y una comprobación reproducible; no afirmes compilación exitosa.

## CRITERIOS DE ACEPTACION

- Los contratos compilan en .NET Framework 4.6.1 y no están acoplados a infraestructura.
- La criptografía cumple exactamente el formato y controles definidos.
- Ningún secreto real fue creado, versionado, impreso o persistido.
- El login y correo legacy permanecen sin cambios.
- El OpenSpec de esta Jira contiene decisiones `D-XX`, requisitos `RQ-XX`, tareas atómicas y trazabilidad a pruebas.

## DOCUMENTACION TECNICA

Actualiza la documentación existente afectada y conserva la exploración como fuente arquitectónica. Registra decisiones y evidencia sanitizada en `Doc/Actualizacion/Login/Implementacion/<JIRA>/`; si una ruta esperada no existe, documenta la ruta creada y la razón.

## ENTREGABLE FINAL

Entrega archivos, contratos exactos, pruebas unitarias con ruta y caso cubierto, comandos, códigos de salida, resultados, riesgos y evidencia sanitizada.
