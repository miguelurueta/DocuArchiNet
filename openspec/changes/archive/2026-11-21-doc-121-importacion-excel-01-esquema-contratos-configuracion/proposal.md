## Why

DOC-121 formaliza la fase 01 de las siete fases de importación Excel. Las tablas `imp_02_` ya alimentan índices de carpetas; falta el contrato de escritura web, la identidad recuperable de cada carga y el permiso exclusivo. Las plantillas históricas también tienen consumidores SII.

El usuario eligió los prompts actuales: archivo completo, validación integral, revisión y confirmación transaccional; recuperación por ID. No se agrega pausa ni reanudación por fila. El SQL se entrega para que el usuario lo aplique manualmente.

Delimitación vigente: EGRESOS es una fuente de pruebas, según aclaración del usuario. Adecuar esa tabla no es requisito para entregar DOC-121. La clave única se exige cuando una tabla sea elegida como destino del cargador o creada por él. La provisión efectiva de la cuenta dedicada y los recorridos de aplicación corresponden a las fases posteriores; esta fase entrega su contrato y la plantilla SQL.

## What Changes

- Inventario y contratos de las fases posteriores reutilizando los tres catálogos `imp01_*`.
- SQL separado de preflight, permiso RA, extensión de plantilla DA, una tabla operativa DA, adecuación de claves, concesiones y postflight.
- Configuración de 10000 filas/10 MB decimales, vinculada por servidor a empresa y DA autorizada.
- Contratos de autorización, recepción, preparación, confirmación y resultado para crear/agregar/reemplazar.
- Estado de instalación `PENDIENTE_EJECUCION_MANUAL`; SELECT posterior después de la confirmación del usuario.

## Capabilities

### New Capabilities
- `importacion-excel-01-esquema-contratos-configuracion`: base contractual y SQL revisable; endpoints y UI pertenecen a las fases posteriores.

### Modified Capabilities
- Ninguna capacidad de carpetas o SII cambia su comportamiento en esta fase.

## Impact

Solo OpenSpec, documentación y SQL nuevos. Sin cambios compilables, proyecto, configuración efectiva, datos o despliegue. E2E no aplica a una entrega sin recorrido ejecutable. Los metadatos del 11 de noviembre son antecedentes, no lectura actual; debe preservarse la ampliación de BENEFICIARIO informada después por el usuario.

Fuente Jira conservada en `specs/importacion-excel-01-esquema-contratos-configuracion/jira-context.md`. Prompt fuente en `Doc/Actualizacion/Importacion/Exploracion/01-esquema-contratos-y-configuracion.md`. Las fases 02–07 reutilizan estos contratos; este ticket no las implementa.
