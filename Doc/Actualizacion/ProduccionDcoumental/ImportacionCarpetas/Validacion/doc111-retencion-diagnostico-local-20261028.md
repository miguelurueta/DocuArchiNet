# DOC-111: diagnóstico de retención, 2026-10-28

HEAD `b2e93b726f221868cf2b82d43200a8e568871117`, cambios locales sin commit. Avance **95/130 tareas completas**. Se completa 11.6; 11.7 y las dependencias documentales/aceptación permanecen pendientes. La función de almacenamiento existente queda intacta.

El servicio de consulta añade DiagnosticarRetencion con paginación/filtros cerrados, retención opcional de composición servidor y reloj UTC inyectable. Reutiliza autorización actual y el mismo puerto/repositorio de lectura. No añade endpoint, tablas, configuración, acceso al disco, borrado ni escritura de eventos. El resultado conserva diagnosticoEnSeco=True y eliminacionHabilitada=False como propiedades de solo lectura.

Sin retención positiva se conservan todos los recursos. También se conservan ante incertidumbre/actividad global o individual, cambio de revisión entre consultas, fecha inválida/futura, vencimiento no calculable, retención vigente, lote/archivo incompletos o evidencia registrada insuficiente. Los filtros no ocultan los indicadores globales. Solo referencias registradas de lote/archivo completos pueden señalarse para revisión futura al vencer la retención; nunca se autorizan a borrar por este resultado.

El diagnóstico usa la última actividad registrada del lote, mantiene totales originales/filtrados y limita los candidatos contados a la página devuelta. La proyección contiene IDs, indicador de referencia registrada y motivos cerrados; excluye stagingRef/hash/rutas/evidencia libre. No enumera partes/copias huérfanas del disco ni verifica existencia física o relaciones legacy.

Verificación local PASS:

- `node --test tests/doc110-perfil-importacion-behavior.test.cjs`: nuevas pruebas en el arnés Framework existente con doble explícito de solo lectura y registros sintéticos; conserva las familias DOC-110/DOC-111 previas.
- Política ausente/cero/negativa, vencimiento exacto/no vencido, fecha inválida/futura, desbordamiento temporal y reloj no UTC; páginas y totales originales preservados.
- Incertidumbre, intentos activos, trabajos pendientes, recepción/efecto incompletos, referencias/hash/bytes/IDs incoherentes y revisión cambiante: sin candidatos habilitantes.
- Autorización revocada, gabinete denegado y página/tipo inválidos rechazados; evidencia y recursos del doble permanecen iguales tras las consultas. Respuesta sin secretos privados y flags de diagnóstico no modificables.
- Build Debug completo sin despliegue en `.tmp/doc111-retencion-build/`; log `.tmp/doc111-retencion-build.log`, código 0. OpenSpec estricto y `git diff --check` PASS.

**NO_EJECUTADO:** consultas SQL/HTTP del sistema, autenticación real, lectura/borrado de staging o copias, limpieza de historial, verificación física de documentos, creación/almacenamiento/incorporación o despliegue. Los resultados sintéticos completados se usan exclusivamente para probar el clasificador; no representan efectos documentales ejecutados. No se infiere autorización de mantenimiento por tener acceso a esta lectura.
