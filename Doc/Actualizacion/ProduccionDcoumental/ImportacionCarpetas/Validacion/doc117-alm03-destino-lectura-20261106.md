# DOC-117 — Lectura ALM-03 y diagnóstico del destino

El usuario autorizó continuar ALM-03. Se acotó esta ejecución a controles SELECT necesarios, con cuenta MySQL de lectura en consola enmascarada. Resultado: **12 SELECT acumulados, cero login de aplicación, cargas, creación de recursos o cambios de DLL**.

El preflight inicial `33a33a3671b9e27a0dc92aeece3e1f22` se detuvo en su consulta 8 con `DOC117_ALM03_PREFLIGHT_CONTROL_NOT_UNIQUE`. Las primeras siete verificaron publicación 38/configuración/reglas, receta de los nombres propuestos, ausencia de esos expedientes/planes/raíces y gabinete/índices disponibles. El control 8 exigía una relación directa del expediente 467 con nivel 7. No se ejecutaron los cuatro controles siguientes y no se concede PASS del preflight completo. La cardinalidad de la consulta fallida no quedó persistida; no se deduce de ese registro que hubiera cero o varias filas.

Se preparó y probó un diagnóstico diferente, limitado a los cuatro controles restantes del mismo techo de doce. Corrida `d8901d156c1fcfc315e1867b505c8d8d`, finalizada `2026-11-06T13:50:41.756Z`, estado DIAGNOSTIC_COMPLETE:

- El expediente 467 existe en empresa 2 y su clasificación tiene huella `083005fcf663febf27982bef6ce9b2f17c03b278ea99d70267da3741cfcdf5ff`.
- Tiene relación activa **1072 con nivel 8**, padre 0. La importación propuesta usa nivel 7; son destinos diferentes y no se cambia ninguna relación para hacerlos coincidir.
- La tipología 50 está activa y es compatible con la clasificación observada.
- Los intentos protegidos conservan sus estados: 41/lote 18 EN_CURSO, 52/lote 21 CONFIRMADO con documento 16222/inventario 23320, y 53/lote 21 EN_CURSO sin checkpoint documental final. Ninguno se repitió, reconcilió o reparó.

La DLL instalada conserva SHA-256 `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`. La publicación observada conserva `0feabba5f1d357215c458d064a84769a56884aa11642caf864cabac22cdd17c6`.

Este diagnóstico no acredita todavía permisos actuales de carga en ambos niveles, coincidencia entre el gabinete del expediente individual y gabinete 10, ausencia del nuevo nombre individual ni acceso por la sesión/UI oficial. Esos controles siguen siendo obligatorios antes de efectos. No se interpreta el diagnóstico como aceptación de ALM-03 ni como autorización de usar el nivel 8 para una carga real.

La corrección de alcance propuesta conserva importación en nivel 7 y expediente individual 467/relación 1072 en su nivel 8. El soporte de preparación siguiente verifica ambas autoridades, clasificación, relación, gabinete, tipología y ausencia del nombre individual antes de crear recursos, y vuelve a verificar el destino al final. Ver `doc117-alm03-setup-listo-20261106.md`.

Evidencia estructurada y SHAs: `doc117-alm03-destino-lectura-20261106.json`. Las referencias de ejecución son saneadas; no se guardan credenciales, cookies, conexiones ni filas de negocio. Progreso 16/18; ALM-03 real y consolidación final permanecen pendientes.
