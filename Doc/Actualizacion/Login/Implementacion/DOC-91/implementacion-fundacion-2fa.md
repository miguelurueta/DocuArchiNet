# DOC-91 — Fundación de contratos y seguridad 2FA

## Resultado

Se implementó una fundación interna e inactiva para Login 2FA compatible con ASP.NET WebForms, VB.NET y .NET Framework 4.6.1. Ningún flujo productivo invoca todavía estos componentes.

## Componentes

| Área | Archivos | Contrato implementado |
| --- | --- | --- |
| Dominio | `Modelo/Login/SegundoFactor/SegundoFactorModels.vb` | Identidad canónica, configuración, challenge, estados, contexto de protección, correo, entrega, finalización y contexto pendiente. |
| Puertos | `Modelo/Login/SegundoFactor/SegundoFactorInterfaces.vb` | Reloj, OTP, HMAC, llaves, configuración, challenge, destinatario, correo, finalizador legacy y Session. |
| DTO | `DTOs/Login/SegundoFactor/SegundoFactorDtos.vb` | Estado y resultado público sin identidad interna ni secretos. |
| Seguridad | `Infrastructure/Login/SegundoFactor/Security/` | UTC, OTP de seis dígitos con rechazo, HMAC-SHA256 v1 y llaves externas rotables. |
| WebForms | `webservice/Login/SegundoFactor/SessionPendingSecondFactorContextStore.vb` | Contexto mínimo pendiente sobre `HttpSessionStateBase`. |

## Reglas verificadas

- Propósito único `LOGIN` y nueve estados vinculantes.
- `RequiereSegundoFactor`: 1 activo; 0 o `NULL` desactivado; otro valor inválido.
- Proveedor único EMAIL=1 y expiración de 1 a 10 minutos.
- Cinco intentos, cooldown de 60 segundos y hasta dos reenvíos como constantes de dominio.
- OTP uniforme de seis dígitos mediante `RandomNumberGenerator` y muestreo por rechazo.
- HMAC `v1:<keyId>:<base64mac>` ligado a propósito, challenge, identidad, sesión y OTP.
- Material Base64 mínimo de 32 bytes; protección con llave activa y verificación con llave identificada anterior.
- Comparación XOR para MAC de igual longitud y rechazo de formato, versión, Base64, llave o longitud inválidos.
- Contexto Session sin contraseña, OTP, correo completo, HMAC, llave, credenciales ni conexiones.
- Enmascarado determinista del correo y DTO públicos sanitizados.

## Evidencia del 2026-10-09

| Comando | Código | Resultado |
| --- | ---: | --- |
| `msbuild .\GestionDocumental-Docuarchi.net.vbproj /t:Build /p:Configuration=Debug /m:1 /v:minimal` | 0 | Ensamblado net461 generado. El proyecto conserva advertencias legacy preexistentes; no hubo error de compilación. |
| `node --test tests/login-second-factor-foundation.test.cjs` | 0 | 3 pruebas pasaron: comportamiento, contrato estructural y aislamiento productivo. Duración final: 2,64 s. |
| `npm.cmd --prefix tools/opsxj run opsxj:refine -- DOC-91 --sync` | 0 | Refinamiento aprobado y trazabilidad D-01..D-08 completa. |
| `openspec.cmd validate doc-91-doble-factor-contrato --strict` | 0 | Cambio OpenSpec válido antes de la implementación; se repite en el cierre. |

Las llaves, identidades, correos y nonces usados por las pruebas son sintéticos. No se accedió a base de datos, SMTP, navegador, credenciales ni ambiente autenticado.

## Compatibilidad y límites

No se modificaron `gestor.aspx`, `ClassGestorSesion`, `ClassCorreo`, Forms Authentication, recuperación de contraseña, `Web.config`, esquema MySQL o configuración SMTP. No existen repositorios MySQL, servicios de orquestación, ASMX o UI de 2FA en DOC-91. Por ello estas pruebas validan la fundación local, pero no acreditan todavía entrega de correo, concurrencia de challenges ni login completo.

## Documentación estructural

La documentación verificada, los diagramas Mermaid, casos de uso, inventario y contrato automático están indexados en [README.md](README.md). La prueba `tests/doc91-technical-documentation.test.cjs` valida los diagramas con el parser oficial; `Doc72SourceValidator` valida declaraciones, enums, firmas y DTO mediante Roslyn.

## Reversa

Retirar los ocho elementos `Compile`, los archivos nuevos, el arnés y la prueba. No se requiere reversa de datos, configuración o comportamiento productivo porque la fundación no está conectada.
