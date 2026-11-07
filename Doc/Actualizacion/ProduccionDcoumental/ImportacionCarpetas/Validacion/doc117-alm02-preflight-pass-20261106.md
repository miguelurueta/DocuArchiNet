# DOC-117 — ALM-02 preflight SELECT, 2026-11-06

**PASS de lectura real autorizada: 8/8 consultas SELECT.** No acredita las ventanas internas de fallo ALM-02 ni habilita efectos.

El usuario autorizó expresamente la propuesta de ocho consultas en `https://localhost/GestionDocumental-Docuarchi.net/`, ámbito de datos actor 141/empresa 2/nivel 7/versión 38/gabinete 10. Se usaron los DSN RA/DA del perfil runtime existente y una cuenta MySQL de lectura capturada por consola enmascarada. No hubo login de aplicación. No se guardaron cuentas, contraseñas, conexiones ni filas SQL.

| Control | Filas | Resultado |
| --- | --- | --- |
| Publicación vigente | 1 | Versión 38, gabinete 10/DA, COMPLETO_OBLIGATORIO/LOTE. |
| Reglas publicadas | 5 | CODIGO y NOMBRE usan la carpeta completa; los nombres propuestos coinciden con estos campos. |
| Expedientes existentes con los nombres/códigos propuestos | 0 | Ausencia observada. |
| Planes anteriores que referencien esos nombres | 0 | Ausencia observada. |
| Raíces anteriores que referencien esos nombres | 0 | Ausencia observada. |
| Gabinete | 1 | Disponible. |
| Índices visibles | 21 | Referencias de la receta presentes. |
| Intento protegido 41/lote 18 | 1 | Observado EN_CURSO/CREAR_EXPEDIENTE; sin mutación, reconciliación ni replay. |

Nombres inspeccionados: `DOC117-ALM02-A-20261106` y `DOC117-ALM02-B-20261106`. La ausencia es una observación puntual; deberá revalidarse dentro de un alcance autorizado antes de futuros efectos.

Huella de configuración y reglas inspeccionadas: `0feabba5f1d357215c458d064a84769a56884aa11642caf864cabac22cdd17c6`. Procedencia completa, autorización acotada, tiempos y hashes de soporte en `doc117-alm02-preflight-pass-20261106.json`. La propuesta histórica se conserva sin modificar su hash.

La DLL del sitio mantiene SHA256 `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`, igual a la observada antes del preflight y a la base de la compilación desactivada. Cero preparaciones, confirmaciones, recepciones, expedientes/documentos nuevos o cambios del runtime. El recurso protegido se leyó una vez: no se infiere una comparación antes/después de su contenido.

Runner: `tools/e2e/scripts/run-doc117-alm02-preflight.cjs`. Pruebas locales: `node --test tests/doc117-alm02-preflight.test.cjs`, 10/10 PASS con dobles y sin conexión. El runner consume un artefacto exclusivo de autorización ligado al hash de propuesta y código, detiene ante discrepancias o resultado incierto y no reintenta consultas. Solo persiste conteos, etapas, códigos cerrados, booleanos y la huella de publicación.

Límites: no se acreditaron permisos de aplicación, tipología, valores finales, escritor, checkpoint real, activación/restauración de DLL ni solapamiento de escritores. Esos controles no estaban en las ocho consultas autorizadas. ALM-02/03 reales siguen NO_EJECUTADO; las tareas 16.1, 17.1 y 18.1 continúan pendientes. No repetir esta corrida ni usar su autorización para cargas.
