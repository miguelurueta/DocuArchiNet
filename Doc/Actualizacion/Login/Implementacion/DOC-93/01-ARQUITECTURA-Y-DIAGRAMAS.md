# Arquitectura y diagramas verificados

## Alcance

| Repositorio | Módulos incluidos | Verificación |
| --- | --- | --- |
| `DocuArchiNet` | `Modelo/Login/SegundoFactor`, `Infrastructure/Repositories/Login/SegundoFactor`, `Infrastructure/Login/SegundoFactor/Smtp`, `Infrastructure/Shared/Data`, `Domain/Shared` | Código compilado, pruebas con dobles y símbolos validados estructuralmente. |
| `DocuArchiCore` | No incluido | La coexistencia de tablas fue explorada antes de DOC-93, pero sus fuentes no forman parte de esta implementación. |

No hay Controller, Service de aplicación, endpoint, DTO HTTP, UI ni composición productiva. Dibujarlos implicaría documentar comportamiento inexistente.

## Convención

- `CODE:` símbolo local que debe resolverse en código.
- `EXT:` actor o dependencia externa no resoluble localmente.
- `CONCEPT:` tabla, estado o regla sin declaración .NET.

Solo `EXT:` y `CONCEPT:` se excluyen de la resolución estructural.

## Diagramas obligatorios

| Archivo | Contenido | Fuentes |
| --- | --- | --- |
| `Diagramas/01-componentes-clases.mmd` | Puertos, implementaciones, modelos y relaciones reales. | Modelos, interfaces, repositorio, transporte, fachada y datos compartidos. |
| `Diagramas/02-secuencia-resolver-configuracion.mmd` | Lectura, cardinalidad y validación de configuración. | Repositorio SMTP e interfaces de datos. |
| `Diagramas/03-secuencia-enviar-otp.mmd` | Construcción, envío, disposición y error. | Transporte y adaptador framework. |
| `Diagramas/04-secuencia-fachada.mmd` | Mapeo cerrado al contrato DOC-91. | Fachada, repositorio y transporte. |

Cada diagrama declara fuentes, referencias `CODE:` y firmas exactas. `diagram-contract.json` es el inventario autoritativo.

## Decisiones implementadas

- `MySqlSecondFactorSmtpConfigurationRepository` recibe un snapshot defensivo de `ContextoModulo`; nunca consulta `Session` ni `HttpContext`.
- El SQL enumera diez columnas y usa `@enabled=1`. No usa `SELECT *` ni `LIMIT 1`.
- Cero filas es `Disabled`; una fila se valida; una segunda fila produce `AmbiguousConfiguration`.
- `USUARIO_SMTP` es tanto remitente como usuario de credencial explícita, conforme al recorrido legacy observado. `DOMINIO_SMTP` se conserva en el modelo y no se aplica a `NetworkCredential`.
- `SMTP_TIEMPO` debe ser positivo, no desbordar `Int32` al multiplicarse por `100000` y queda limitado a `120000` ms.
- `MailMessage` y el cliente abstraído son descartados aun ante excepción. El adaptador real descarta internamente `SmtpClient`.
- Toda excepción de repositorio, construcción o transporte se reduce a un estado/código público fijo.

## Límites

Las pruebas sin red acreditan estructura, reglas y disposición, no entrega real ni fidelidad del servidor SMTP. La entrega no activa 2FA, no crea challenges y no toma decisiones de login.
