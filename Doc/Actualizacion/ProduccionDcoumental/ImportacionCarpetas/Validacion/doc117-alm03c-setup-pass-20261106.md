# DOC-117 ALM-03C: preparación real aprobada y completada

La preparación terminó **PASS**, salida 0, el 2026-11-06 a las **22:27:58 UTC / 17:27:58 Bogotá**. Se preparó un caso nuevo, con la descripción canónica exacta fijada en su alcance V2. **Cero documentos almacenados, cero ALMACENAR, cero carga individual y ningún cambio de DLL.** Esta fase no acredita coexistencia ni concurrencia real.

## Recursos preparados

| Recurso | A nuevo | B nuevo |
| --- | --- | --- |
| Plan / lote | 39 / 23 | 39 / 23 |
| Raíz / archivo | 37 / 38 | 38 / 39 |
| Expediente | 668 | 669 |
| Relación de nivel | 1093 | 1094 |
| Intento de creación confirmado | 61 | 62 |
| Nombre recibido | `DOC117-ALM03C-IMPORT-A-20261106.pdf` | `DOC117-ALM03C-IMPORT-B-20261106.pdf` |
| Tamaño / contenido | 677 bytes / SHA aprobado | 677 bytes / SHA aprobado |

Los IDs son los devueltos por el servidor; no se predijeron. Hubo un plan, un lote, dos recepciones, dos creaciones de expediente y dos materializaciones. Los valores efectivos de importación son tipología 79 e índice 14 TEXTO `EEE`, con originales verificados y ausencia documental del lote comprobada. Los dos comandos futuros ALMACENAR y sus huellas se sellaron privadamente, **sin enviarlos**.

El destino individual sigue siendo expediente 467/nivel 8/relación 1072/tipología 50. Se verificaron autoridad, clasificación y ausencia del nombre nuevo `DOC117-ALM03C-IND-20261106.pdf`, sin subirlo. La descripción exacta del catálogo se leyó y revalidó al final; su valor permanece en el alcance privado y no se imprime en este informe.

## Detención inicial y corrección del control

El usuario aprobó expresamente la preparación y la ampliación **320 → 410**, con máximo 160 SELECT adicionales desde 250. La primera corrida `b83e24a81417007ea8f92c54d3c624b1` se detuvo tras tres SELECT, antes de solicitar plan, lote o recepciones: `DOC117_ALM03C_PRIOR_INVENTORY_CHANGED`.

El defecto estaba en el control de pruebas: comparaba un COUNT sin filtro de toda la tabla con **8814**, que era el conteo limitado a **usuario 141/empresa 2**. No se capturó el total sin filtro y no se afirma un cambio de datos por ese rechazo. La continuación corrigió la consulta para usar exactamente los mismos filtros y parámetros del control histórico. Las referencias anteriores a «inventario global 8814» deben leerse con ese ámbito de actor/empresa; los artefactos anteriores no se reescriben.

La corrección pasó **336/336 pruebas**. La continuación `30d7b62134015710dbb8c455c8723de6` conservó el mismo alcance, efectos y techo aprobados, verificó por SHA que el intento anterior solo consumió lecturas y cargó esas tres consultas al contador. No repitió ninguna solicitud de creación, recepción o materialización. El permiso original no se modificó y sus tres consultas no se borraron del presupuesto.

## Consumo y preservación verificados

- SELECT del intento inicial: **3**. SELECT nuevos de la continuación: **86**. Total de la fase: **89/160**.
- Acumulado: **250 + 3 + 86 = 339/410**. Restan 71 hasta ese techo, sin habilitar una corrida documental distinta.
- Inventario de usuario 141/empresa 2: **8814 → 8814**. Documentos individuales observados: **11 → 11**, con huella de registros igual antes/después.
- El ejecutor comparó la preservación de planes, lotes y recursos anteriores, incluidos los del lote 22, y verificó clasificación, originales nuevos y ausencia documental antes de publicar el alcance. No hubo envío o replay de A/B anteriores, reconciliación, reparación o limpieza.
- La comprobación local independiente verificó por tamaño y SHA los cuatro originales de staging: lote 22/archivos 36–37 y lote 23/archivos 38–39. Todos conservan 677 bytes y el hash aprobado. No existe un temporal individual ALM-03C en el servidor.
- DLL original, cuatro respaldos byte a byte y las 912 entradas originales siguen intactos. Las candidatas y evidencias históricas se conservaron. No se activó una variante de laboratorio durante la preparación.

## Alcance sellado y trazabilidad

Protocolo: `ALM03_CANONICAL_CLOSED_RESOURCES_BEFORE_WRITER_V2`. Run de alcance: `71a43d1962ba3285b3801c4f2080934c`. SHA del alcance compacto: `9221008ec1566b27b7ee765e7ed8b770a2ba1b12bf42778eb5b40281d7d2ea19`. SHA de la descripción canónica exacta: `e2f8d3f24d68e56190ab8a643eecc4be8fd7e2e294aae8008fbc56fa22641456`.

Fuentes: `tools/e2e/artifacts/doc117-alm03c-setup-b83e24a81417007ea8f92c54d3c624b1.json` y `doc117-alm03c-setup-30d7b62134015710dbb8c455c8723de6.json`. La concesión derivada, el alcance y los comandos se conservan en `.tmp/doc117-alm03c-setup-30d7b62134015710dbb8c455c8723de6-*` y `.tmp/doc117-alm03c-setup-continuation-authorization.json`, sin imprimir claves operativas o secretos. Certificado independiente: `doc117-alm03c-setup-pass-20261106.json`.

## Estado siguiente

La autorización aprobada cubrió solamente esta preparación. Falta adaptar y revisar la candidata activa y el ejecutor de coexistencia para este alcance V2, comprobar restauración con sus bytes y definir otro presupuesto/permiso específico para documentos y runtime. El saldo de 71 SELECT no se trata como permiso para esas acciones. No repetir esta preparación ni reutilizar los comandos antiguos.

**DOC-117 permanece 16/18, con 17.1 y 18.1 abiertas**, sin PASS de coexistencia/serialización, cierre, archivo o aceptación XML/SII. El acumulado inicial de cualquier fase futura será **339**.
