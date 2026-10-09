# DOC-91 — Documentación técnica verificada

## Alcance revisado

Repositorio revisado: `DocuArchiNet` (`D:/imagenesda/DocuachiNet/DocuArchiNet`). No se inspecciona ni se declara como implementado código de `DocuArchiCore`; ese repositorio fue únicamente antecedente de la exploración.

Módulos incluidos:

- `Modelo/Login/SegundoFactor/`: dominio y puertos.
- `DTOs/Login/SegundoFactor/`: contratos públicos previstos.
- `Infrastructure/Login/SegundoFactor/Security/`: reloj, OTP, HMAC y configuración de llaves.
- `webservice/Login/SegundoFactor/`: adaptador de contexto pendiente en Session.
- `tests/`, `tools/validation/` y `.github/workflows/`: verificación automatizada.

No existen dentro de DOC-91 endpoints, controllers, services de orquestación, implementaciones de repositorio MySQL, transporte SMTP ni UI. Las interfaces para esas responsabilidades son puertos sin implementación y así se identifican en el inventario.

## Documentos

- [Arquitectura y diagramas](01-ARQUITECTURA-Y-DIAGRAMAS.md)
- [Casos de uso implementados](02-CASOS-DE-USO.md)
- [Inventario técnico](03-INVENTARIO-TECNICO.md)
- [Validación, resultados y pendientes](04-VALIDACION-Y-PENDIENTES.md)
- [Resumen original de implementación](implementacion-fundacion-2fa.md)

## Convención de diagramas

- `CODE:` identifica una clase, interfaz, método, función o DTO que debe resolverse contra el código.
- `EXT:` identifica un actor o dependencia externa/BCL y es la única clase de participante técnico excluida de resolución contra el repositorio.
- `CONCEPT:` identifica una regla, decisión o estado conceptual; también se excluye de resolución.
- Las flechas continuas representan llamadas; las discontinuas, retornos; `alt` y rombos representan condiciones observadas en el código.

El inventario obligatorio de diagramas y símbolos está en `diagram-contract.json`. Los archivos `.mmd` son las fuentes autoritativas; los Markdown explican su interpretación y trazabilidad.
