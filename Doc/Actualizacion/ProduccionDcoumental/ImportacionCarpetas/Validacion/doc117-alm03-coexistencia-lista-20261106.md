# DOC-117 — ALM-03 de coexistencia listo para autorización

Preparación local terminada. **ALM-03 real NO_EJECUTADO; candidata NO_INSTALADA.** El setup autorizado ya produjo plan 38/lote 22; esta propuesta utiliza sus mismos recursos. No se repite el setup. Las tareas 17.1 y 18.1 siguen pendientes: 16/18 completas.

## Alcance exacto

Ambiente `https://localhost/GestionDocumental-Docuarchi.net/`, actor 141/empresa 2, publicación 38/gabinete 10. Dos sesiones independientes de esa misma cuenta; la selección individual conserva su nivel 8 y la importación su nivel 7. Cuenta MySQL exclusivamente SELECT por los DSN existentes, capturada junto con la cuenta de aplicación en consola enmascarada. Sin archivos de secretos, cookies, capturas ni trazas.

| Llamador | Recurso existente | PDF de 677 bytes | Valores esperados |
| --- | --- | --- | --- |
| Importación A | Lote 22, raíz 35, archivo 36, expediente 666, relación 1091, nivel 7 | `DOC117-ALM03-IMPORT-A-20261106.pdf` | Tipología 79; índice 14 TEXTO `EEE` |
| Importación B | Lote 22, raíz 36, archivo 37, expediente 667, relación 1092, nivel 7 | `DOC117-ALM03-IMPORT-B-20261106.pdf` | Tipología 79; índice 14 TEXTO `EEE` |
| Carga individual oficial | Expediente 467, relación 1072, nivel 8 | `DOC117-ALM03-IND-20261106.pdf` | Tipología 50, `Comprobante de Egreso`; índices heredados del mapeo existente |

Los tres contenidos tienen SHA-256 `a5f1924afbae4bd631de893c49a56d9f09219075113e6da2ec983602cff84c6c`. El PDF individual se preparó como copia local del fixture existente; todavía no se ha enviado. Se verifica su original local; el temporal del servidor se consume según el flujo oficial. En el gabinete individual, NOMBRE conserva la descripción tipológica que utiliza el escritor existente, según su visibilidad; inventario e incorporación deben conservar el nombre original del PDF.

**Presupuesto nuevo de coexistencia:** tres solicitudes de almacenamiento, hasta tres documentos/incorporaciones, cero planes/lotes/recepciones/expedientes nuevos, máximo **320 SELECT adicionales** y variante temporal durante un máximo de **600 segundos**. Los 80 SELECT del setup y los doce de lectura anteriores son históricos y no se reinician. Ventana exclusiva y conservación de todos los recursos, incluso ante incertidumbre o serialización.

## Recorrido cerrado

1. Con la DLL original, verificar el lote, sus cuatro intentos de recepción/creación confirmados, revisiones y ausencia documental, originales de staging y huellas de los dos comandos con el calculador .NET contrastado contra el coordinador. Verificar autoridad de ambos niveles, clasificación/relación/tipología individual y ausencia del nombre nuevo. Retener en memoria los valores de sus índices y comprobar preservación de documentos previos.
2. Cerrar sesiones, preparar dos respaldos byte a byte, transferir/verificar ACL y armar un guardián independiente. Solo entonces instalar la candidata revisada. El guardián restaura por salida del propietario o límite de 600 segundos; `finally` también restaura y verifica ambas copias.
3. Abrir dos sesiones nuevas con la candidata. Revalidar los comandos/destinos antes de enviar. Preparar la UI individual mediante el helper oficial existente; su coordinación ocurre en el cliente antes de Guardar. Despachar A y la carga individual una sola vez, sin esperas o barreras dentro del escritor. Esperar la terminación de ambos envíos aunque falle uno.
4. Correlacionar respuestas, IDs conocidos, intentos fijados y las cuatro marcas internas de A/individual. Verificar documento/inventario/incorporación, nombres, índices y clasificación; dos incrementos de inventario y ambos originales conservados. Solo después enviar B una vez.
5. Exigir seis marcas internas y resumen completo sin fallos, tres identidades distintas, dos sesiones distintas y B posterior a ambos intervalos. Restaurar la DLL original antes de los controles finales; reabrir sesión con ella. Verificar dos checkpoints confirmados, lote COMPLETADO, tres incrementos documentales, tres incorporaciones únicas, índices/originales y preservación de recursos previos.

Una respuesta incierta, marca incompleta, alteración, fallo de proyección o discrepancia detiene el recorrido. Se retienen IDs conocidos antes de verificar la proyección y también se leen las salidas disponibles tras una detención. **Sin retry, replay, reconciliación, reparación o limpieza.** Se preservan particularmente el intento 41 y B/intento 53 de ALM-02; no se invocan ni se normalizan.

## Observación y límites de aceptación

La excepción editorial aprobada de D-06/D-08/RQ-06 se limita a una copia del método compartido fuera del proyecto. Las operaciones originales del cuerpo son reversibles al texto original exacto; las dos rutas reciben guardas de laboratorio que vinculan sesión, llamador, archivo real, destino y huellas. La fijación de intentos usa creación exclusiva antes del escritor. La identidad ASP.NET se compara solo en RAM y nunca se escribe.

El intervalo comienza dentro de `AlmacenamientoDocumentoProduccionDocumental`, después de las guardas, y termina en su `finally`. Se exige mismo proceso/AppDomain/reloj y pares de hilo administrado/nativo consistentes. Los intervalos HTTP o del llamador no sustituyen esta evidencia. El resultado combina observación interna, controles de negocio y restauración; los tiempos solos no conceden PASS.

Si A y la carga individual se serializan naturalmente, se completan los controles y se informa **SERIALIZED_METHODS / NO_ACREDITADO**, conservando los documentos. No se repite la corrida para buscar solapamiento. No se atribuyen commits simultáneos, rendimiento de la DLL ordinaria ni aceptación XML/SII. Un resultado real serializado no completa por inferencia la concurrencia de 17.1.

## Verificación local y huellas

**238/238 pruebas locales PASS** en la regresión final conjunta. Incluye comandos inmutables reales en .NET, guardas de ambos llamadores, sesiones independientes y rechazo de doble entrada, sincronización sintética, serialización, errores/ByRef, fallos de observación, transporte incierto, proyección, IDs discordantes, reloj alterado, índices con ceros, preservación y concesión/loader. Las barreras existen únicamente en dobles sintéticos. Compilación completa sin errores; OpenSpec estricto PASS.

Ensayo con los mismos bytes de candidata en sitio temporal PASS: guardián independiente restauró tras salida del propietario sin `finally`, ambas copias idénticas y ACL verificadas. Ninguna autenticación, consulta real, efecto documental o activación de la DLL real durante este avance. Se verificaron intactas las 912 entradas originales de compilación/recursos y la DLL instalada.

| Elemento | SHA-256 |
| --- | --- |
| Propuesta `doc117-alm03-coexistencia-propuesto-20261106.json` | `1dd4c5c7a38c73f051f80f5ebfdc2c5ccaefb1e1d7e5038db796f73ba9775d1b` |
| Código revisado del ejecutor y dependencias | `3d35ef187201bfcd4d125844a8b240963266b2606cd5d74e238a24504241f01e` |
| Alcance real de recursos | `ce0223fa7cda43dd727507a0b45a6384c71143e1f8035473ee9a09d15cf00cf9` |
| Candidata cerrada, no instalada | `142e0d0bba0b6bcfd66933158ac163623784a2d0556c1a0f244ec92396b32e12` |
| DLL original, todavía instalada | `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5` |
| 912 entradas originales | `412a115d0a4e7d8b6b839aa5ba17082337fce14bb129332645db82509909d1d3` |

Detalle verificable: `doc117-alm03-coexistencia-local-20261106.json`; compilación `.tmp/doc117-alm03-coexistencia-final-20261106/build-result.json`; ensayo `.tmp/doc117-alm03-lease-5f05800beeaa4949822e147de4ecb6e2/rehearsal-result.json`. Los dos comandos con claves se mantienen en el archivo privado del setup y no se publican.

El ejecutor `tools/e2e/scripts/run-doc117-alm03-coexistence.cjs --check` valida los archivos, la candidata, las 912 entradas y la DLL original sin efectos. La ejecución exige una concesión ALM-03 nueva vinculada a las cuatro huellas de propuesta/código/candidata/recursos. La de setup y las de ALM-02 son rechazadas; **no se ha creado ni concedido la de coexistencia**.

## Autorización pendiente

Se solicita autorizar específicamente este ambiente, las cuentas de prueba de actor 141/empresa 2 y MySQL solo SELECT, las dos sesiones concurrentes, los tres almacenamientos descritos, los 320 SELECT nuevos y la instalación temporal de esta candidata con doble respaldo/guardián durante un máximo de diez minutos. Todos los documentos y recursos se conservarán.

La confirmación adicional se requiere porque [AGENTS.md](../../../../../AGENTS.md) establece: «No ejecutar E2E real ni carga sin autorización explícita para el ambiente y las cuentas de prueba». Además, la autorización previa de setup limitó expresamente documentos, ALMACENAR y cambios de DLL a cero. Esta revisión prepara la decisión concreta; no amplía ese permiso anterior.
