# Flujo, rollback y seguridad — DOC-14

El recorrido es: apertura → bootstrap del gate → preview → confirmación → ejecución → auditoría. Preview y ejecución revalidan el gate antes de repositorios, guard de concurrencia o ejecutor legacy.

## Rollback autorizado

No ejecutar este procedimiento sin aprobación del ambiente y de los responsables designados.

1. Registrar responsable, motivo y correlación de cambio.
2. Ejecutar `tools/validation/Invoke-Doc14Rollback.ps1` con `-WhatIf` para revisar el objetivo y después sin `-WhatIf` solo con autorización.
3. El script crea respaldo recuperable, deja `Active=false` y `OfficialMode=false`, vacía usuarios, grupos y metadatos de piloto, y guarda evidencia con hora UTC, responsable, motivo y correlación.
4. Verificar que una apertura posterior sirve legacy y que una llamada moderna recibe un bloqueo funcional. No ejecutar E2E autenticada sin el runbook y autorización correspondiente.
5. Ejecutar el reporte agregado y adjuntar la evidencia en el sistema de cambios autorizado.

El rollback no invoca SQL, JavaScript, `Cambia_Estado` ni una segunda terminación. Una transición ya confirmada no se revierte con esta herramienta; cualquier reversión de negocio usa exclusivamente el procedimiento legacy autorizado.

Si se recibió una llamada moderna después del rollback, la auditoría registra el intento bloqueado con código funcional y advertencia segura si su persistencia falla. La evidencia del rollback conserva la correlación de operación.
