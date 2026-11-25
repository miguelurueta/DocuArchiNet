# DOC-127 — Verificación integral, capacidad y cierre

- Ticket: DOC-127
- Cambio OpenSpec: doc-127-importacion-excel-07-verificacion-integral-cierre
- Clasificación: cross_cutting; perfil legacy-webforms-vb.

## Superficies UI

Preservar ImportacionExcelVista/Api, semáforo de acciones, análisis del archivo para la plantilla, diagnóstico por campo/regla y recuperación por ID. Cerrar/Escape permanece disponible durante consultas. La aceptación física previa usa positivos v5 y lectura v6; v7 conserva cobertura local y hash HTTP. No se anuncia una nueva lectura real v7. Una corrección futura afecta sólo el comportamiento reproducido y sus pruebas existentes.

## Validacion visual

Reutilizar las pruebas de navegador de DOC126 para semáforo, diagnóstico, cierre y foco. La ejecución del refinamiento sólo revisa esos contratos: no toma capturas ni ejecuta UI real. La comprobación del runtime vigente tendrá su ventana de lectura expresamente autorizada; conservar la declaración manual anterior del usuario con su alcance original.
