# DOC127: revisión y cierre de hallazgos demostrados

**7.1 completada.** El hallazgo conocido EXCEL_FORM1_NATIVE_VALIDATION queda cerrado con evidencia clasificada. Las ventanas actuales de autoridad y asistente no demostraron nuevos defectos de producto. No se declara aceptación final ni QA humana.

## Hallazgo y prueba de cierre

Los controles Excel ocultos pertenecían a form1 y sus restricciones nativas impedían el envío del gestor. La corrección17bdf09e asignó un formulario auxiliar, conservando la validación del gestor y el ciclo de vida/foco. Los hashes UTF8/LF actuales de Vista y WebForm coinciden con la corrección.

| Evidencia | Resultado | Vigencia y límite |
| --- | --- | --- |
| Reproducción y corrección local | Antes2 fallos; después81 casos PASS | Historia con SHA original; no se anuncia otra ejecución |
| Integración local posterior |141 casos PASS históricos |12/15 fuentes coinciden; los tres archivos de pruebas cambiados se clasifican abajo |
| Cobertura de componentes cambiados |55 pruebas PASS reutilizadas; ocho hashes coincidentes | Incluye las dos suites de soporte y la suite E2E modificadas; no se suman a141 como casos nuevos independientes |
| Autoridad/lectura real vigente |1 E2E y seis rechazos PASS; aislamiento DOM antes/después | No envió un postback de negocio del gestor |
| Asistente real vigente XLS/XLSX |2/2 PASS con Vista actual | Altas/cargas autorizadas y huellas conservadas; no se modificó producto para superar la prueba |

La prueba focal original cambió sólo en su archivo de test; esa versión coincide con la integración posterior. Se verifica por separado la conservación de los dos archivos de producto. No se atribuye a las141 pruebas una ejecución actual ni se reejecutan cargas consumidas para cerrar este registro.

## Brechas restantes

Actualización posterior: [plan manual revisado](doc127-plan-consulta-20261126.md),2.4 completada. Acceso ALL sin índice elegido, rows estimadas25538; advertencia sin medición de latencia. Los cambios concurrentes de proveedor/editor en8d1f2159 tienen29 pruebas locales PASS y build aislado0 errores/308 advertencias, sin despliegue. Los hashes y resultados de este informe mantienen su corte original. Sólo7.5 conserva QA visual humana actual y revisión/gates finales. Revocación física, política COLUMNA real y cortes físicos COMMIT/red mantienen sus límites de evidencia.

El JSON adjunto fija las fuentes, hashes, diferencias y cobertura de cada componente. Esta revisión es documental y de lectura local: cero SQL, sesiones o efectos nuevos. La entrega conserva abiertos archivo y cierre Jira.
