# DOC-111: compatibilidad de creación, consultas SELECT

Inspección real finalizada el 2026-10-28 a las 19:31:46 UTC, reutilizando `tools/e2e/scripts/inspect-doc110-final-prerequisites.ps1` con modo `Doc111Creation`. Se reutilizaron la resolución privada de configuración existente, el hash de destino fijado por `run-doc111-persistence.ps1` y el perfil 24/versión 38, sin ejecutar el runner de persistencia ni activar el perfil. No se imprimieron ni guardaron credenciales, cadenas de conexión o rutas documentales.

Ejecución, con la convención de política de proceso ya usada por `tools/e2e/package.json`:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File tools/e2e/scripts/inspect-doc110-final-prerequisites.ps1 -Mode Doc111Creation
```

Resultado `INSPECTED`; reporte local saneado en `tools/e2e/artifacts/doc111-creation-compatibility-select.json`. Todas las consultas fueron SELECT. El modo DOC110 original se conserva como predeterminado; el modo nuevo exige el destino fijado y el perfil existente, enlaza los parámetros de clasificación y falla sin exponer mensajes de conexión.

| Comprobación | Resultado observado |
|---|---|
| Motor | MySQL 5.1.37-community |
| Modo SQL de sesión | STRICT_TRANS_TABLES, NO_AUTO_CREATE_USER, NO_ENGINE_SUBSTITUTION |
| VOLUMEN_EXPEDIENTE | int unsigned, NOT NULL, DEFAULT 1 |
| Ruta global | Una fila; no se consultaron valores de disco/ruta |
| Clasificación del perfil | Una coincidencia: empresa 2, instrumento 3, organigrama 1, área 49 |
| Nombres de organigrama en empresa 2 | Cero grupos duplicados; sin índice único nombre/empresa |
| Índices únicos | Fondo por nombre, Ciclo por nombre y Área por organigrama/nombre |

El DEFAULT actual corrige la sospecha derivada del esquema histórico sin DEFAULT. No demuestra que Conservación respete un volumen solicitado distinto de 1: su INSERT sigue omitiendo ese parámetro. Una clasificación compatible en este instante tampoco demuestra toda la clasificación, autorización del actor, contexto RA de sesión, estabilidad concurrente, correlación o resultado de una creación real.

**95/130 tareas completas, sin incremento.** 9.3/9.4 siguen abiertas. Próximo trabajo: completar la composición del adaptador y la verificación de clasificación/correlación reutilizando los contratos existentes. No se ejecutaron HTTP/E2E, creación, almacenamiento, escrituras SQL, limpieza, cambios de configuración o despliegue. Las funciones legacy y SII permanecen intactas.
